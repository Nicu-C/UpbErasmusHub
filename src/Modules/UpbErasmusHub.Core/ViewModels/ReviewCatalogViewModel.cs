using System.Collections.Generic;
using OrchardCore.ContentManagement;

namespace UpbErasmusHub.Core.ViewModels;

public class ReviewCatalogViewModel
{
    public IEnumerable<ContentItem> Reviews { get; set; } = [];
    public int TotalCount { get; set; }
}