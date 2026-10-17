using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UpbErasmusHub.Core.ViewModels;

public class SubmitReviewViewModel
{
    [Required]
    public string UniversityContentItemId { get; set; } = string.Empty;

    [Required]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    public string MobilitySemester { get; set; } = "Winter 2025/2026";

    [Range(1, 5)]
    public int OverallRating { get; set; } = 5;

    [Range(100, 5000)]
    public int MonthlyLivingCost { get; set; } = 650;

    [Required]
    public string HousingAdvice { get; set; } = string.Empty;

    [Required]
    public string ReviewComments { get; set; } = string.Empty;

    public List<SelectListItem> Universities { get; set; } = [];
}