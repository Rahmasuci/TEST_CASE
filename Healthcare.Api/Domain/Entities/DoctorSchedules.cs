namespace Healthcare.Api.Domain.Entities
{
    public class DoctorSchedules
    {
        public int ScheduleID { get; set; }
        public int DoctorID { get; set; }
        public int DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}