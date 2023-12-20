using Microsoft.Extensions.DependencyInjection;

namespace BarcodeParserBuilder;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddBarcodeParserBuilder(this IServiceCollection services)
    {
        //add to the collection
        services.AddScoped<IBarcodeParserBuilder, BarcodeParserBuilder>();
        services.AddSingleton<IAimParser, AimParser>();

        //generate our list so its not executed when it needs to.
        AimParser.ParserBuilders = AimParser.CompileParserBuildersList();

        return services;
    }
}
