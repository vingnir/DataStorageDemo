

namespace Business.Dtos;

public class ServiceDto
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = "Unknown Service";
    public decimal HourlyPrice { get; set; } = 0;
}

