namespace Business.Dtos;

public class ProjectCreateDetailedDto
{
    public required string ProjectNumber { get; set; }
    public required string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CustomerId { get; set; }


    public ServiceDto? Service { get; set; } // Creates or finds the service
    public StaffDto? Staff { get; set; }     // Creates or finds the staff
    public CustomerDto? Customer { get; set; }

    public int ServiceId { get; set; }
    public int StaffId { get; set; }

    public int StatusId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Description { get; set; }
}

