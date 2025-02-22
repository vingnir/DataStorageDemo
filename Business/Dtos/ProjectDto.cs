

namespace Business.Dtos;

public class ProjectDto
{
    public required string ProjectNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public int ServiceId { get; set; }
    public int StaffId { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Description { get; set; }


    public ServiceDto? Service { get; set; }

    public StaffDto? Staff { get; set; }
    public CustomerDto? Customer { get; internal set; }
}
