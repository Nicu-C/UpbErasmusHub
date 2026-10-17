using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using UpbErasmusHub.Core.Models;

namespace UpbErasmusHub.Core;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Register the custom Content Part
        services.AddContentPart<PartnerUniversityPart>();

        // Register the Data Migration so Orchard runs CreateAsync on startup
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