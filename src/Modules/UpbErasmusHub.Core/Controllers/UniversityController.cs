using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using UpbErasmusHub.Core.Indexes;
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

        // 1. Fetch distinct countries directly from the PartnerUniversityIndex SQL table
        var allIndexRows = await _session
            .QueryIndex<PartnerUniversityIndex>(x => x.Published)
            .ListAsync();

        var availableCountries = allIndexRows
            .Select(x => x.Country)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        // 2. Build a SQL-backed YesSql query against PartnerUniversityIndex
        var query = _session.Query<ContentItem, PartnerUniversityIndex>(x => x.Published);

        if (!string.IsNullOrWhiteSpace(country))
        {
            var trimmedCountry = country.Trim();
            query = query.Where(x => x.Country == trimmedCountry);
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            var trimmedLang = language.Trim().ToUpperInvariant();
            query = query.Where(x => x.RequiredLanguageLevel == trimmedLang);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.UniversityName.Contains(term) ||
                x.City.Contains(term) ||
                x.ErasmusCode.Contains(term));
        }

        // 3. Count and paginate at the database level
        int totalCount = await query.CountAsync();
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        if (page > totalPages) page = totalPages;
        if (page < 1) page = 1;

        var pagedItems = (await query
            .OrderBy(x => x.UniversityName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ListAsync())
            .ToList();

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

        return View(model);
    }

    [HttpGet("/about")]
    public IActionResult About()
    {
        return View();
    }
}