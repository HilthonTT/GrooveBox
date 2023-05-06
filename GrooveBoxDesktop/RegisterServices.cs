﻿using Blazored.LocalStorage;
using GrooveBoxDesktop.Authentication;
using GrooveBoxDesktop.Data;
using GrooveBoxDesktop.Helpers;
using GrooveBoxLibrary.API;
using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrooveBoxDesktop;

public static class RegisterServices
{
    public static void ConfigureServices(this MauiAppBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<WeatherForecastService>();

        builder.Services.AddMemoryCache();
        builder.Services.AddBlazoredLocalStorage();

        // Configuration JSON File Injection
        builder.Configuration.AddConfiguration(AddConfiguration());

        //Authentication
        builder.Services.AddScoped(sp => new HttpClient { });
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

        //Personal Services
        builder.Services.AddSingleton<IAPIHelper, APIHelper>();
        builder.Services.AddSingleton<IUserEndpoint, UserEndpoint>();
        builder.Services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();
        builder.Services.AddSingleton<IOidGenerator, OidGenerator>();

        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<IUserData, MongoUserData>();
        builder.Services.AddSingleton<IGenreData, MongoGenreData>();
        builder.Services.AddSingleton<IMediaFileData, MongoMediaFileData>();
    }
    private static IConfiguration AddConfiguration()
    {
        string configFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);


        return builder.Build();
    }
}
