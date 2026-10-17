using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using UpbErasmusHub.Core.Models;
using UpbErasmusHub.Core.ViewModels;
using YesSql;

namespace UpbErasmusHub.Core.Controllers;

public sealed class UniversityController : Controller
{
    private readonly ISession _session;

    public UniversityController(ISession session)
    {
        _session = session;
    }

    [HttpGet("/universities")]
    public async Task<IActionResult> Index(string? search, string? country, string? language, int page = 1)
    {
        const int pageSize = 6;

        // 1. Query published partner universities from YesSql[cite: 10]
        var query = _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "PartnerUniversity" && x.Published);

        var allItems = await query.ListAsync();

        // 2. Extract available countries for the filter dropdown
        var availableCountries = allItems
            .Select(i => i.As<PartnerUniversityPart>()?.Country)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .Cast<string>()
            .ToList();

        // 3. Apply C# Filters (Search keyword, Country, Language Level)[cite: 10]
        var filteredItems = allItems.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var queryText = search.Trim().ToLowerInvariant();
            filteredItems = filteredItems.Where(i =>
                i.DisplayText.ToLowerInvariant().Contains(queryText) ||
                (i.As<PartnerUniversityPart>()?.City?.ToLowerInvariant().Contains(queryText) ?? false) ||
                (i.As<PartnerUniversityPart>()?.ErasmusCode?.ToLowerInvariant().Contains(queryText) ?? false)
            );
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            filteredItems = filteredItems.Where(i =>
                string.Equals(i.As<PartnerUniversityPart>()?.Country, country, System.StringComparison.OrdinalIgnoreCase)
            );
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            filteredItems = filteredItems.Where(i =>
                string.Equals(i.As<PartnerUniversityPart>()?.RequiredLanguageLevel, language, System.StringComparison.OrdinalIgnoreCase)
            );
        }

        var list = filteredItems.OrderBy(i => i.DisplayText).ToList();
        int totalCount = list.Count;
        int totalPages = System.Math.Max(1, (int)System.Math.Ceiling(totalCount / (double)pageSize));

        if (page > totalPages) page = totalPages;
        if (page < 1) page = 1;

        // 4. Paginate with Skip / Take[cite: 10]
        var pagedItems = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var model = new UniversityCatalogViewModel
        {
            Universities = pagedItems,
            AvailableCountries = availableCountries,
            SearchQuery = search,
            SelectedCountry = country,
            SelectedLanguageLevel = language,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        // 5. Renders Views/University/Index.cshtml[cite: 10]
        return View(model);
    }
}