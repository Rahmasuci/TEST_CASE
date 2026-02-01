using Healthcare.Api.Application.DTOs;
using Healthcare.Api.Domain.Entities;
using Healthcare.Api.Helper;
using Healthcare.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Application.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAsync(CreateAppointmentDto dto)
        {
            // 1. validasi durasi
            if (dto.DurationMinutes % 5 != 0)
                throw new Exception("Slot must be multiple of 5");

            if (dto.DurationMinutes != 15
                && dto.DurationMinutes != 30
                && dto.DurationMinutes != 60)
            {
                throw new InvalidOperationException("Invalid durasi");
            }

            if (dto.StartTime.Minute % 5 != 0)
                throw new Exception("StarTime must be multiple of 5");

            //2. get doctor timezone
            var doctor = await _context.Doctors.FindAsync(dto.DoctorID)
                ?? throw new Exception("Doctor not found");
            var tz = TimezoneHelper.GetDoctorTz(doctor.Timezone);

            //3. Ambil waktu lokal dokter
            DateTime localStart;
            if (dto.StartTime.Offset != TimeSpan.Zero)
            {
                //client kirim date dengan timezone -> convert ke waktu dokter
                localStart = TimeZoneInfo.ConvertTime(dto.StartTime, tz).DateTime;
            }
            else
            {
                //client tidak kirim timezone -> anggap jam dokter
                localStart = DateTime.SpecifyKind(dto.StartTime.DateTime, DateTimeKind.Unspecified);
            }
            DateTime localEnd = localStart.AddMinutes(dto.DurationMinutes);

            //4. validasi diluar jam kerja
            var day = (int)localStart.DayOfWeek;
            day = day == 0 ? 7 : day;

            var schedules = await _context.DoctorSchedules
                .Where(x => x.DoctorID == dto.DoctorID
                    && x.DayOfWeek == day)
                .ToListAsync();

            var isWithinWorkinghours = schedules
                .Any(x => localStart.TimeOfDay >= x.StartTime
                        && localEnd.TimeOfDay <= x.EndTime);

            if (!isWithinWorkinghours)
                throw new InvalidOperationException("Outside doctor working hours");


            // 5, Convert to UTC
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
            var endUtc = startUtc.AddMinutes(dto.DurationMinutes);


            //6. Cek overlap appointment
            var overlap = await _context.Appointments
                .AnyAsync(x => x.DoctorID == dto.DoctorID
                    && x.StartTime < endUtc
                    && startUtc < x.EndTime);

            if (overlap)
                throw new InvalidOperationException("Time slot already booked");

            //5. simpan appointment ke db
            var appointment = new Appointment
            {
                DoctorID = dto.DoctorID,
                PatientID = dto.PatientID,
                StartTime = startUtc,
                EndTime = endUtc
            };

            _context.Appointments.Add(appointment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("Time slot already booked");
            }

            //7. convert ke local untuk tampilan
            appointment.StartTime = localStart;
            appointment.EndTime = localEnd;

            return appointment;
        }

        public async Task CancelAsync(int id)
        {
            //1. cari data dan validasi appointment
            var apointment = await _context.Appointments
                .FirstOrDefaultAsync(x => x.AppointmentID == id);

            if (apointment == null)
                throw new KeyNotFoundException("Appointment not found");

            var now = DateTimeOffset.UtcNow;
            var cuttoff = TimeSpan.FromHours(2);

            //2. cutoof validation
            if (apointment.StartTime - now < cuttoff)
                throw new InvalidOperationException("Cancellation cutoff excedeed");

            //3. delete
            _context.Appointments.Remove(apointment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Appointment sudah diubah/dihapus oleh proses yg lain
                throw new InvalidOperationException("Appointment allready cancelled");
            }

        }
    }
}