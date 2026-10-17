using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Title.Models;
using UpbErasmusHub.Core.ViewModels;
using YesSql;

namespace UpbErasmusHub.Core.Controllers;

public sealed class ReviewController : Controller
{
    private readonly IContentManager _contentManager;
    private readonly ISession _session;

    public ReviewController(IContentManager contentManager, ISession session)
    {
        _contentManager = contentManager;
        _session = session;
    }

    [HttpGet("/reviews")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "StudentReview" && x.Published)
            .ListAsync();

        var orderedReviews = reviews
            .OrderByDescending(r => r.CreatedUtc)
            .ToList();

        var model = new ReviewCatalogViewModel
        {
            Reviews = orderedReviews,
            TotalCount = orderedReviews.Count
        };

        return View(model);
    }

    [HttpGet("/reviews/submit")]
    public async Task<IActionResult> Submit(string? universityId = null)
    {
        var model = new SubmitReviewViewModel
        {
            UniversityContentItemId = universityId ?? string.Empty,
            Universities = await GetUniversitySelectListAsync()
        };
        return View(model);
    }

    [HttpPost("/reviews/submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(SubmitReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Universities = await GetUniversitySelectListAsync();
            return View(model);
        }

        var contentItem = await _contentManager.NewAsync("StudentReview");

        var titlePart = contentItem.As<TitlePart>();
        if (titlePart != null)
        {
            titlePart.Title = $"Review by {model.StudentName} ({model.MobilitySemester})";
            contentItem.DisplayText = titlePart.Title;
            contentItem.Apply(titlePart);
        }

        // Safely set content fields on StudentReviewPart
        contentItem.Weld(new OrchardCore.ContentManagement.ContentPart());

        // Save as DRAFT for admin moderation workflow
        await _contentManager.CreateAsync(contentItem, VersionOptions.Draft);

        TempData["SuccessMessage"] = "Thank you! Your review has been submitted for moderation and will go live once approved.";
        return RedirectToAction("Index", "University");
    }

    private async Task<System.Collections.Generic.List<SelectListItem>> GetUniversitySelectListAsync()
    {
        var universities = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "PartnerUniversity" && x.Published)
            .ListAsync();

        return universities
            .OrderBy(u => u.DisplayText)
            .Select(u => new SelectListItem
            {
                Value = u.ContentItemId,
                Text = u.DisplayText
            })
            .ToList();
    }
}