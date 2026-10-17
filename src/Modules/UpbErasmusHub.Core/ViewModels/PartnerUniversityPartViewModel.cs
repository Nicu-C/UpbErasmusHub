using System.ComponentModel.DataAnnotations;

namespace UpbErasmusHub.Core.ViewModels;

public class PartnerUniversityPartViewModel
{
    [Required]
    public string ErasmusCode { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string RequiredLanguageLevel { get; set; } = "B2";

    public string WebsiteUrl { get; set; } = string.Empty;

    [Range(1, 100)]
    public int AvailableSpots { get; set; } = 2;
}