using Data.Entities;

public class Customer
{
    public int CustomerId { get; set; }

    public required string Name { get; set; } = string.Empty; 

    public string? ContactPerson { get; set; }

    public ICollection<Project>? Projects { get; set; } = new List<Project>(); 
}