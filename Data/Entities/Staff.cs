
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Staff
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
        
    [Required]  
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; } = new();
}
