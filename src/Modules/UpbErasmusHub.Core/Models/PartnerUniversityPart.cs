using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace UpbErasmusHub.Core.Models
{
    public class PartnerUniversityPart : ContentPart
    {
        // Strongly-typed properties for C# Drivers & Controllers
        public string ErasmusCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string RequiredLanguageLevel { get; set; } = "B2";
        public string WebsiteUrl { get; set; } = string.Empty;
        public int AvailableSpots { get; set; } = 2;

        // Content fields attached to this part
        public TextField FacultyDepartment { get; set; } = new();
    }
}