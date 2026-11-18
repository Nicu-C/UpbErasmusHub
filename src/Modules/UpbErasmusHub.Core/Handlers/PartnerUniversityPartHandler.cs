using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using UpbErasmusHub.Core.Models;

namespace UpbErasmusHub.Core.Handlers;

public sealed class PartnerUniversityPartHandler : ContentHandlerBase
{
    public override Task InitializingAsync(InitializingContentContext context)
    {
        if (context.ContentItem.Has<PartnerUniversityPart>())
        {
            context.ContentItem.Alter<PartnerUniversityPart>(part =>
            {
                if (part.AvailableSpots <= 0)
                {
                    part.AvailableSpots = 2;
                }

                if (string.IsNullOrWhiteSpace(part.RequiredLanguageLevel))
                {
                    part.RequiredLanguageLevel = "B2";
                }
            });
        }

        return Task.CompletedTask;
    }

    // Runs on the VERY FIRST Publish/Save of a brand-new item
    public override Task CreatingAsync(CreateContentContext context)
    {
        NormalizeErasmusCode(context.ContentItem);
        return Task.CompletedTask;
    }

    // Runs on every SUBSEQUENT Publish/Save when editing an item
    public override Task UpdatedAsync(UpdateContentContext context)
    {
        NormalizeErasmusCode(context.ContentItem);
        return Task.CompletedTask;
    }

    private static void NormalizeErasmusCode(ContentItem contentItem)
    {
        if (contentItem.Has<PartnerUniversityPart>())
        {
            contentItem.Alter<PartnerUniversityPart>(part =>
            {
                if (!string.IsNullOrWhiteSpace(part.ErasmusCode))
                {
                    part.ErasmusCode = part.ErasmusCode.Trim().ToUpperInvariant();
                }
            });
        }
    }
}