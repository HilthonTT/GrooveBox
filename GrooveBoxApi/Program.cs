using GrooveBoxApi.Data;
using GrooveBoxApi.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using GrooveBoxLibrary.DataAccess;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("OpenCorsPolicy", opt =>
        opt.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();

builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<ISQLUserData, SQLUserData>();
builder.Services.AddSingleton<IDbConnection, DbConnection>();
builder.Services.AddSingleton<IGenreData, MongoGenreData>();
builder.Services.AddSingleton<IUserData, MongoUserData>();
builder.Services.AddSingleton<IMediaFileData, MongoMediaFileData>();
builder.Services.AddSingleton<IFileStorage, MongoFileStorage>();

builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var connectionString = config.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped(provider =>
{
    var mongoClient = provider.GetRequiredService<IMongoClient>();
    var databaseName = config.GetValue<string>("MongoDB:DatabaseName");
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultAuthenticateScheme = "JwtBearer";
})
    .AddJwtBearer("JwtBearer", jwtBearerOptions =>
    {
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Secrets:SecurityKey"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
        };
    });


builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Groove Box API",
        Version = "v1",
    });
});

builder.Services.Configure<MvcOptions>(options =>
{

});

var app = builder.Build();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    string roleName = "Admin";
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Check if the role already exists
    if (await roleManager.RoleExistsAsync(roleName) is false)
    {
        // Create a new role
        var role = new IdentityRole(roleName);
        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            // Role created successfully
            // Handle any additional logic or return a success response
        }
        else
        {
            // Failed to create the role
            // Handle the error or return an error response
        }
    }
    else
    {
        // Role already exists
        // Handle accordingly or return a response indicating the role already exists
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("OpenCorsPolicy");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Groove Box API v1");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
