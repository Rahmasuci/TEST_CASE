using Healthcare.Api.Application.DTOs;
using Healthcare.Api.Helper;
using Healthcare.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Healthcare.Api.Application.Services
{
    public class AvailabilitySlotService
    {
        private readonly AppDbContext _context;

        public AvailabilitySlotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AvailablitySlotDto>> GetAvailablityAsync(
            int doctorID, DateTime from, DateTime to, int slotMinutes
        )
        {
            try
            {
                //1. validasi
                if (slotMinutes % 5 != 0)
                    throw new Exception("Slot must be multiple of 5");

                if (from > to)
                    throw new Exception("Range time wrong");

                //2. get doctor timezone
                var doctor = await _context.Doctors.FindAsync(doctorID)
                    ?? throw new Exception("Doctor not found");
                var tz = TimezoneHelper.GetDoctorTz(doctor.Timezone);

                //3. ubah input ke format utc
                var fromUtc = from.ToUniversalTime();
                var toUtc = to.ToUniversalTime();

                //4. get data schedule
                var schedules = await _context.DoctorSchedules
                    .Where(x => x.DoctorID == doctorID)
                    .ToListAsync();

                //5. get data appointment
                var appointments = await _context.Appointments
                    .Where(x => x.DoctorID == doctorID
                        && x.StartTime < toUtc
                        && x.EndTime > fromUtc
                    )
                    .ToListAsync();

                var result = new List<AvailablitySlotDto>();

                //6. loop per hari 
                for (var dateUtc = fromUtc.Date; dateUtc <= toUtc.Date; dateUtc = dateUtc.AddDays(1))
                {
                    var dateLocal = TimezoneHelper.ToLocal(dateUtc, tz);
                    var day = (int)dateLocal.DayOfWeek;
                    day = day == 0 ? 7 : day;

                    var daySchedules = schedules.Where(s => s.DayOfWeek == day);

                    foreach (var sched in daySchedules)
                    {
                        var startLocal = dateLocal.Date + sched.StartTime;
                        var endLocal = dateLocal.Date + sched.EndTime;

                        var startUtc = TimezoneHelper.ToUtc(startLocal, tz);
                        var endtUtc = TimezoneHelper.ToUtc(endLocal, tz);

                        for (var slotStart = startUtc; slotStart.AddMinutes(slotMinutes) <= endtUtc; slotStart = slotStart.AddMinutes(slotMinutes))
                        {
                            var slotEnd = slotStart.AddMinutes(slotMinutes);

                            //overlap check
                            var overlap = appointments
                                .Any(a => a.StartTime < slotEnd
                                    && slotStart < a.EndTime
                            );

                            if (!overlap)
                            {
                                var slotStartLocal = TimezoneHelper.ToLocal(slotStart, tz);
                                var slotEndLocal = TimezoneHelper.ToLocal(slotEnd, tz);

                                result.Add(new AvailablitySlotDto
                                {
                                    Start = slotStartLocal,
                                    End = slotEndLocal
                                });
                            }
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}