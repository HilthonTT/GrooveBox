using Microsoft.Extensions.Logging;
using GrooveBox.Data;
using GrooveBoxLibrary.DataAccess;

namespace GrooveBox;
public static class RegisterServices
{
    public static void ConfigureServices(this MauiAppBuilder builder)
    {
        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        //builder.Services.AddAuthenticationCore();
        //builder.Services.AddAuthorizationCore();
        //builder.Services.AddMemoryCache();
        //builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        //builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        //        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"))
        //        .EnableTokenAcquisitionToCallDownstreamApi()
        //        .AddInMemoryTokenCaches();

        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("Admin", policy =>
        //    {
        //        policy.RequireClaim("jobTitle", "Admin");
        //    });
        //});

        builder.Services.AddSingleton<WeatherForecastService>();

        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<IGenreData, MongoGenreData>();
        builder.Services.AddSingleton<IMediaFileData, MongoMediaFileData>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
    }
}
