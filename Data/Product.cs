using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PuppetFestAPP.Web.Data;
 
public enum ProductColor
{
    Red = 1, Orange = 2, Yellow = 3, Green = 4,
    Blue = 5, Purple = 6, Black = 7, White = 8,
    Brown = 9, Pink = 10, Multi = 11, Clear = 12
}
 
public enum ProductSize
{
    XS = 1, S = 2, M = 3, L = 4, XL = 5, XXL = 6, XXXL = 7
}
 
public class Product
{
    public int Id { get; set; }
 
    // -------------------------------------------------------
    // PARENT vs VARIANT discriminator
    // Null  → this record IS the parent (the "group" concept)
    // Int   → this record is a variant; points to the parent
    // -------------------------------------------------------
    public int? ParentProductId { get; set; }
 
    /// <summary>
    /// Navigation property to the parent product.
    /// Null when this record IS the parent.
    /// </summary>
    public Product? ParentProduct { get; set; }
 
    /// <summary>
    /// Child variants of this product.
    /// Only populated when this record is a parent (ParentProductId == null).
    /// </summary>
    public ICollection<Product> Variants { get; set; } = new List<Product>();
 
    // -------------------------------------------------------
    // SHARED fields — meaningful on parent; typically null on variants
    // -------------------------------------------------------
 
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
 
    [MaxLength(500)]
    public string? Description { get; set; }
 
    // -------------------------------------------------------
    // VARIANT-LEVEL fields — meaningful on variants; base value on parent
    // -------------------------------------------------------
 
    /// <summary>
    /// Base price on the parent. Variant price may differ (e.g. XXL upcharge).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
 
    public ProductSize? Size { get; set; }
 
    public ProductColor? Color { get; set; }
 
    /// <summary>
    /// Free-text material descriptor (e.g. "100% Cotton", "Polyester Blend").
    /// Preferred over an enum — easier to extend without migrations.
    /// </summary>
    [MaxLength(100)]
    public string? Material { get; set; }
 
    [Required]
    public bool IsActive { get; set; } = true;
 
    /// <summary>
    /// UTC timestamp set automatically when the record is first created.
    /// </summary>
    [Required]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
 
    // -------------------------------------------------------
    // NOTE: Quantity has been removed from Product.
    // Inventory is tracked per-variant, per-location in ProductLocation.
    // -------------------------------------------------------
 
    // -------------------------------------------------------
    // NAVIGATION — Image lives on parent; variants leave ImageId null
    // -------------------------------------------------------
 
    public int? ImageId { get; set; }
    public Image? Image { get; set; }
 
    [Required]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
 
    /// <summary>
    /// For variants: each ProductLocation entry holds Quantity at that location.
    /// For parents: typically empty — query via Variants instead.
    /// </summary>
    public ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();
}