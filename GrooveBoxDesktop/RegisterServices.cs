﻿using Blazored.LocalStorage;
using GrooveBoxDesktop.Authentication;
using GrooveBoxDesktop.Helpers;
using GrooveBoxLibrary.API;
using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
using MatBlazor;
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

        builder.Services.AddMemoryCache();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddMatBlazor();

        // Configuration JSON File Injection
        builder.Configuration.AddConfiguration(AddConfiguration());

        //Authentication
        builder.Services.AddScoped(sp => new HttpClient { });
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        builder.Services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();
        builder.Services.AddSingleton<IAPIHelper, APIHelper>();

        //Personal Services
        builder.Services.AddTransient<IUserEndpoint, UserEndpoint>();
        builder.Services.AddTransient<IFileStorage, MongoFileStorage>();
        builder.Services.AddTransient<IGenreEndpoint, GenreEndpoint>();
        builder.Services.AddTransient<IMediaFileEndpoint, MediaFileEndpoint>();
        builder.Services.AddTransient<IVideoEndpoint, VideoEndpoint>();
        builder.Services.AddTransient<IConnectionStringEndpoint, ConnectionStringEndpoint>();
        builder.Services.AddTransient<IAPIDbConnection, APIDbConnection>();
        builder.Services.AddTransient<IRoleEndpoint, RoleEndpoint>();

        builder.Services.AddSingleton<ISecureStorageWrapper, SecureStorageWrapper>();
        builder.Services.AddSingleton<IFileProcessor, FileProcessor>();
    }
    private static IConfiguration AddConfiguration()
    {
        string configFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);

        return builder.Build();
    }
}
