using System.Threading.Tasks;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using UpbErasmusHub.Core.Models;
using UpbErasmusHub.Core.ViewModels;

namespace UpbErasmusHub.Core.Drivers;

public sealed class PartnerUniversityPartDisplayDriver : ContentPartDisplayDriver<PartnerUniversityPart>
{
    // 1. Shows the fields in the Admin Editor
    public override IDisplayResult Edit(PartnerUniversityPart part, BuildPartEditorContext context)
    {
        return Initialize<PartnerUniversityPartViewModel>(
            GetEditorShapeType(context),
            model =>
            {
                model.ErasmusCode = part.ErasmusCode;
                model.Country = part.Country;
                model.City = part.City;
                model.RequiredLanguageLevel = part.RequiredLanguageLevel;
                model.WebsiteUrl = part.WebsiteUrl;
                model.AvailableSpots = part.AvailableSpots;
            })
            .Location("Parts:5");
    }

    // 2. Saves the fields when you click Publish in Admin
    public override async Task<IDisplayResult> UpdateAsync(PartnerUniversityPart part, UpdatePartEditorContext context)
    {
        var viewModel = new PartnerUniversityPartViewModel();

        await context.Updater.TryUpdateModelAsync(
            viewModel,
            Prefix,
            m => m.ErasmusCode,
            m => m.Country,
            m => m.City,
            m => m.RequiredLanguageLevel,
            m => m.WebsiteUrl,
            m => m.AvailableSpots);

        part.ErasmusCode = viewModel.ErasmusCode;
        part.Country = viewModel.Country;
        part.City = viewModel.City;
        part.RequiredLanguageLevel = viewModel.RequiredLanguageLevel;
        part.WebsiteUrl = viewModel.WebsiteUrl;
        part.AvailableSpots = viewModel.AvailableSpots;

        return Edit(part, context);
    }
}