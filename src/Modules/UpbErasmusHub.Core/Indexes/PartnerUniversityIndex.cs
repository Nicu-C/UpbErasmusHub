using OrchardCore.ContentManagement;
using OrchardCore.Data;
using UpbErasmusHub.Core.Models;
using YesSql.Indexes;

namespace UpbErasmusHub.Core.Indexes;

public class PartnerUniversityIndex : MapIndex
{
    public string ContentItemId { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ErasmusCode { get; set; } = string.Empty;
    public string RequiredLanguageLevel { get; set; } = string.Empty;
    public int AvailableSpots { get; set; }
    public bool Published { get; set; }
}

public class PartnerUniversityIndexProvider : IndexProvider<ContentItem>, IScopedIndexProvider
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<PartnerUniversityIndex>()
            .When(contentItem => contentItem.Has<PartnerUniversityPart>())
            .Map(contentItem =>
            {
                var part = contentItem.As<PartnerUniversityPart>();
                if (part == null) return null!;

                return new PartnerUniversityIndex
                {
                    ContentItemId = contentItem.ContentItemId,
                    UniversityName = contentItem.DisplayText ?? string.Empty,
                    Country = part.Country ?? string.Empty,
                    City = part.City ?? string.Empty,
                    ErasmusCode = part.ErasmusCode ?? string.Empty,
                    RequiredLanguageLevel = part.RequiredLanguageLevel ?? "B2",
                    AvailableSpots = part.AvailableSpots,
                    Published = contentItem.Published
                };
            });
    }
}