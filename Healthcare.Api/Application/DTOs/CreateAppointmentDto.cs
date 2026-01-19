namespace Healthcare.Api.Application.DTOs
{
    public class CreateAppointmentDto
    {
        public int DoctorID { get; set; }
        public int PatientID { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}