using System.ComponentModel.DataAnnotations;

namespace TechDesk.Models;

public class Device
{
    public int Id { get; set; }

    [Display(Name = "Asset tag")]
    [Required(ErrorMessage = "Enter the asset tag")]
    [StringLength(10, ErrorMessage = "The asset tag can be at most 10 characters")]
    public string AssetTag { get; set; } = string.Empty;

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Enter the device name")]
    [StringLength(50, ErrorMessage = "The name can be at most 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Enter the category, such as Laptop or Tablet")]
    [StringLength(20, ErrorMessage = "The category can be at most 20 characters")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Year bought")]
    [Range(2015, 2026, ErrorMessage = "The year bought must be from 2015 to 2026")]
    public int PurchaseYear { get; set; }

    [Display(Name = "On loan?")]
    public bool IsOnLoan { get; set; }
}
