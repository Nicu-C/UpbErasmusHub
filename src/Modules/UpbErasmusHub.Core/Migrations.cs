using System.Threading.Tasks;
using OrchardCore.Autoroute.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using UpbErasmusHub.Core.Models;

namespace UpbErasmusHub.Core
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<int> CreateAsync()
        {
            // 1. Define the PartnerUniversityPart and its attached Fields
            await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(PartnerUniversityPart), part => part
                .Attachable()
                .WithDescription("Provides Erasmus exchange details for a partner university.")
                .WithField(nameof(PartnerUniversityPart.FacultyDepartment), field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Target UPB Faculty / Department")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "e.g., Automatic Control and Computers (ACS), ETTI, FILS"
                    })
                )
            );

            // 2. Define the PartnerUniversity Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("PartnerUniversity", type => type
                .DisplayedAs("Partner University")
                .Creatable()
                .Listable()
                .Draftable()
                .Versionable()
                .Securable()
                .WithPart("TitlePart", part => part
                    .WithPosition("0")
                )
                .WithPart("AutoroutePart", part => part
                    .WithPosition("1")
                    .WithSettings(new AutoroutePartSettings
                    {
                        Pattern = "universities/{{ ContentItem.DisplayText | slugify }}",
                        AllowCustomPath = true
                    })
                )
                .WithPart(nameof(PartnerUniversityPart), part => part
                    .WithPosition("2")
                )
            );

            // 3. Define the StudentReview Part (holds the ContentPickerField & Review fields)
            await _contentDefinitionManager.AlterPartDefinitionAsync("StudentReviewPart", part => part
                .WithField("HostUniversity", field => field
                    .OfType(nameof(ContentPickerField))
                    .WithDisplayName("Host Partner University")
                    .WithPosition("0")
                    .WithSettings(new ContentPickerFieldSettings
                    {
                        Required = true,
                        Multiple = false,
                        DisplayedContentTypes = new[] { "PartnerUniversity" },
                        Hint = "Select the partner university where you completed your Erasmus mobility."
                    })
                )
                .WithField("StudentName", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Student Name")
                    .WithPosition("1")
                    .WithSettings(new TextFieldSettings
                    {
                        Required = true,
                        Hint = "Your full name or initials (e.g., Alex M.)"
                    })
                )
                .WithField("MobilitySemester", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Mobility Semester")
                    .WithPosition("2")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "e.g., Winter 2025 / 2026"
                    })
                )
                .WithField("OverallRating", field => field
                    .OfType(nameof(NumericField))
                    .WithDisplayName("Overall Rating (1 to 5)")
                    .WithPosition("3")
                    .WithSettings(new NumericFieldSettings
                    {
                        Required = true,
                        Scale = 0,
                        Minimum = 1,
                        Maximum = 5,
                        DefaultValue = "5"
                    })
                )
                .WithField("ReviewComments", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Student Feedback & Experience")
                    .WithEditor("TextArea")
                    .WithPosition("4")
                    .WithSettings(new TextFieldSettings
                    {
                        Required = true,
                        Hint = "Share your experience regarding courses, accommodation, living costs, and campus life."
                    })
                )
            );

            // 4. Define the StudentReview Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("StudentReview", type => type
                .DisplayedAs("Student Review")
                .Creatable()
                .Listable()
                .Draftable()
                .Versionable()
                .Securable()
                .WithPart("TitlePart", part => part
                    .WithPosition("0")
                )
                .WithPart("StudentReviewPart", part => part
                    .WithPosition("1")
                )
            );

            return 1;
        }
    }
}