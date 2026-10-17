using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using YesSql;

namespace UpbErasmusHub.Core.Controllers;

public sealed class ApplicationController : Controller
{
    private readonly ISession _session;

    public ApplicationController(ISession session)
    {
        _session = session;
    }

    [HttpGet("/application-steps")]
    public async Task<IActionResult> Index()
    {
        // Query published ApplicationStep items from YesSql
        var steps = await _session
            .Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "ApplicationStep" && x.Published)
            .ListAsync();

        // Order them by the numeric StepNumber field
        var orderedSteps = steps
            .OrderBy(s => (double?)s.Content?.ApplicationStepPart?.StepNumber?.Value ?? 0)
            .ToList();

        return View(orderedSteps);
    }
}