namespace Data.Entities;

public class Project
{
    public string ProjectNumber { get; set; } = string.Empty;  // ✅ Default empty string
    public string Name { get; set; } = string.Empty;           // ✅ Default empty string

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // ✅ Nullable CustomerId allows projects to exist without a customer
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }  // ✅ Nullable to prevent warnings

    // ✅ Foreign keys and navigation properties now properly handled
    public int? ServiceId { get; set; }  // ✅ Nullable if service is optional
    public Service? Service { get; set; }  // ✅ Nullable if service is optional

    public int? StaffId { get; set; }  // ✅ Nullable if staff is optional
    public Staff? Staff { get; set; }  // ✅ Nullable to prevent warnings

    public int? StatusId { get; set; }  // ✅ Nullable if status is optional
    public Status? Status { get; set; }  // ✅ Nullable to prevent warnings

    public decimal TotalPrice { get; set; } = 0.0m;  // ✅ Default to 0 to avoid null issues

    public string? Description { get; set; } = "No description provided";  // ✅ Default to a meaningful value
}
