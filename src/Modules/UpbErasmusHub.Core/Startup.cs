using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay; // <-- Added for UseDisplayDriver
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using System;
using UpbErasmusHub.Core.Drivers; // <-- Added to find PartnerUniversityPartDisplayDriver
using UpbErasmusHub.Core.Handlers;
using UpbErasmusHub.Core.Indexes;
using UpbErasmusHub.Core.Models;

namespace UpbErasmusHub.Core;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Register the custom Content Part and attach its Display Driver
        services.AddContentPart<PartnerUniversityPart>()
            .UseDisplayDriver<PartnerUniversityPartDisplayDriver>();
        services.AddScoped<IContentHandler, PartnerUniversityPartHandler>();        // Register the Data Migration so Orchard runs CreateAsync on startup
        services.AddScopedIndexProvider<PartnerUniversityIndexProvider>();
        services.AddDataMigration<Migrations>();
    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapAreaControllerRoute(
            name: "Home",
            areaName: "UpbErasmusHub.Core",
            pattern: "Home/Index",
            defaults: new { controller = "Home", action = "Index" }
        );
    }
}