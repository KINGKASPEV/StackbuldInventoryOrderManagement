using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using StackbuldInventoryOrderManagement.Application.Middlewares;
using StackbuldInventoryOrderManagement.Common.Config;
using StackbuldInventoryOrderManagement.Common.Handlers;
using StackbuldInventoryOrderManagement.Persistence.Context;
using StackbuldInventoryOrderManagement.Persistence.Extensions;
using StackbuldInventoryOrderManagement.Persistence.Repositories;
using System.Reflection;
using System.Text;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application...");

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Configure to use Render's PORT environment variable
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    ConfigurationManager configuration = builder.Configuration;

    // Add services to the container.
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Information);
    builder.Host.UseNLog();

    // Load environment variables from .env file and add configurations
    Env.Load();
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddHealthChecks();
    builder.Services.AddSwaggerGenNewtonsoftSupport();
    builder.Services.AddDatabaseServices(configuration);
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //builder.Services.AddCors();
    builder.Services.AddRequestDecompression();
    builder.Services.AddRepository();
    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version"),
            new MediaTypeApiVersionReader("x-api-version")
        );
    });

    builder.Services.AddDateOnlyTimeOnlyStringConverters();
    builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Stackbuld Inventory & Order Management API",
            Version = "v1",
            Description = "A production-grade API for managing products and orders with concurrency control",
            Contact = new OpenApiContact
            {
                Name = "Kingsley Okafor",
                Email = "kingsleychiboy22@gmail.com"
            }
        });
        c.UseDateOnlyTimeOnlyStringConverters();
        c.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            }
        );
        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            }
        );

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowClient",
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
        );
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressConsumesConstraintForFormFileParameters = true;
    });

    #region JWT Token

    var authSettingsSection = builder.Configuration.GetSection("AuthSettings");
    builder.Services.Configure<AuthSettings>(authSettingsSection);
    var authSettings = authSettingsSection.Get<AuthSettings>();
    var key = Encoding.ASCII.GetBytes(authSettings?.SecretKey!);

    builder
        .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(30),
                ValidateIssuerSigningKey = true,
                ValidIssuer = authSettings?.Issuer,
                ValidAudience = authSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };
        });

    #endregion

    builder
        .Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                .Json
                .ReferenceLoopHandling
                .Ignore;
        });

    builder.Services.AddFluentValidationServices();

    var app = builder.Build();

    AppSettingsHelper.AppSettingsConfigure(app.Services.GetRequiredService<IConfiguration>());

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        //app.UseSwagger();.
        app.UseSwagger(options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stackbuld Inventory & Order Management API v1");
        });
    }

    //app.UseCors(corsPolicyBuilder =>
    //{
    //    corsPolicyBuilder.AllowAnyHeader();
    //    corsPolicyBuilder.AllowAnyMethod();
    //    corsPolicyBuilder.AllowAnyOrigin();
    //});

    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        if (dbContext != null)
            await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.Error(ex, "Database is not available. Application cannot start");
    }

    app.UseLogUtil();

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors("AllowClient");

    app.UseAuthentication();

    app.UseAuthorization();

    app.ConfigureCustomExceptionMiddleware();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks(
            "/health/live",
            new HealthCheckOptions() { Predicate = (_) => false }
        );
        endpoints.MapHealthChecks(
            "/health/ready",
            new HealthCheckOptions { Predicate = (check) => check.Tags.Contains("ready") }
        );
    });
    await app.RunAsync();
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
}
finally
{
    LogManager.Shutdown();
}
