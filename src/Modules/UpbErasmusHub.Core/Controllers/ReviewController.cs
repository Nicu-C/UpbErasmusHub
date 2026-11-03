using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.ContentFields.Fields;
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

        // 1. Gather all university IDs referenced by the reviews
        var universityIds = orderedReviews
            .Select(r => (string?)r.Content.StudentReviewPart?.HostUniversity?.ContentItemIds?[0])
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        // 2. Fetch the corresponding universities in one query
        var universities = await _contentManager.GetAsync(universityIds);
        var universityDict = universities.ToDictionary(u => u.ContentItemId, u => u.DisplayText);

        // 3. Pass to the view via ViewBag
        ViewBag.UniversityNames = universityDict;

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

        // 1. Initialize a new StudentReview content item
        var contentItem = await _contentManager.NewAsync("StudentReview");

        // 2. Set the TitlePart and DisplayText
        var title = $"Review by {model.StudentName} ({model.MobilitySemester})";
        contentItem.DisplayText = title;

        var titlePart = contentItem.As<TitlePart>();
        if (titlePart != null)
        {
            titlePart.Title = title;
            contentItem.Apply(titlePart);
        }

        // 3. Combine housing advice and comments into ReviewComments text
        var combinedComments = string.IsNullOrWhiteSpace(model.HousingAdvice)
            ? model.ReviewComments
            : $"Housing Advice:\n{model.HousingAdvice}\n\nReview & Experience:\n{model.ReviewComments}";

        // 4. Retrieve or create StudentReviewPart (exact name from Migrations.cs)
        var reviewPart = contentItem.Get<ContentPart>("StudentReviewPart") ?? new ContentPart();

        // HostUniversity (ContentPickerField)
        var pickerField = new ContentPickerField
        {
            ContentItemIds = string.IsNullOrWhiteSpace(model.UniversityContentItemId)
                ? Array.Empty<string>()
                : new[] { model.UniversityContentItemId }
        };
        reviewPart.Apply("HostUniversity", pickerField);

        // StudentName (TextField)
        reviewPart.Apply("StudentName", new TextField { Text = model.StudentName });

        // MobilitySemester (TextField)
        reviewPart.Apply("MobilitySemester", new TextField { Text = model.MobilitySemester });

        // OverallRating (NumericField)
        reviewPart.Apply("OverallRating", new NumericField { Value = (decimal)model.OverallRating });

        // ReviewComments (TextField with TextArea editor)
        reviewPart.Apply("ReviewComments", new TextField { Text = combinedComments });

        // MonthlyLivingCost (NumericField) -> Applied BEFORE contentItem.Apply
        reviewPart.Apply("MonthlyLivingCost", new NumericField { Value = (decimal)model.MonthlyLivingCost });

        // 5. Apply the part back onto the content item
        contentItem.Apply("StudentReviewPart", reviewPart);

        // 6. Save as DRAFT for admin moderation workflow
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