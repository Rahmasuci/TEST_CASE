namespace Healthcare.Api.Application.DTOs
{
    public class AvailablitySlotDto
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}