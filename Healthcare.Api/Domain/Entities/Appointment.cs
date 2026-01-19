using System.ComponentModel.DataAnnotations;

namespace Healthcare.Api.Domain.Entities
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default;
    }
}