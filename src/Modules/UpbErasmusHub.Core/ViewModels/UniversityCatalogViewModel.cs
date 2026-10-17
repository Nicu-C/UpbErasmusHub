using System.Collections.Generic;
using OrchardCore.ContentManagement;

namespace UpbErasmusHub.Core.ViewModels;

public class UniversityCatalogViewModel
{
    public IEnumerable<ContentItem> Universities { get; set; } = [];
    public IEnumerable<string> AvailableCountries { get; set; } = [];

    public string? SearchQuery { get; set; }
    public string? SelectedCountry { get; set; }
    public string? SelectedLanguageLevel { get; set; }

    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
}