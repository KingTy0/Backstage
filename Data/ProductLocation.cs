using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuppetFestAPP.Web.Data;

public class ProductLocation
{
    public int Id { get; set; }

    // Navigation properties

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
   
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int Quantity { get; set; } = 0;

    [Required]
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}