using IntermediateAPI.Models;
using IntermediateAPI;
using IntermediateAPI.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Identity.Web;
using Serilog;
using Serilog.Events;
using IL.FluentValidation.Extensions.Options;
using Microsoft.Extensions.Options;
using Serilog.Context;
using IntermediateAPI.Services;
using IntermediateAPI.Models.Validators;

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.AzureApp()
                .WriteTo.File(IntermediateAPI.Constants.AZWEBAPP_LOG_LOCATION,
                    rollingInterval: RollingInterval.Infinite,
                    retainedFileCountLimit: 1,
                    fileSizeLimitBytes: 10485760) //10MB
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();

Log.Information("API starting...");

var builder = WebApplication.CreateBuilder(args);

// fluent validation on appsettings
var azureAdValidator = new AzureAdSettingsValidator();
builder.Services.AddOptions<AzureAdSettings>()
    .Bind(builder.Configuration.GetSection("AzureAd"))
    .Validate(azureAdValidator)
    .ValidateOnStart();

var homeRealmValidator = new HomeRealmSettingsValidator();
builder.Services.AddOptions<HomeRealmSettings>()
    .Bind(builder.Configuration.GetSection("HomeRealmSettings"))
    .Validate(homeRealmValidator)
    .ValidateOnStart();

var dfpValidator = new FraudProtectionSettingsValidator();
builder.Services.AddOptions<FraudProtectionSettings>()
    .Bind(builder.Configuration.GetSection("FraudProtectionSettings"))
    .Validate(dfpValidator)
    .ValidateOnStart();

// Add AAD bearer token authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IAuthProvider, DfpAuthProvider>();
builder.Services.AddSingleton<DfpService>();

// Add data services
builder.Services.AddDataServices();
builder.Services.AddApplicationInsightsTelemetry();
builder.Host.UseSerilog((context, serviceProvider, loggerConfig) =>
{
    loggerConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.AzureApp()
                .WriteTo.Console(outputTemplate: IntermediateAPI.Constants.SERILOG_LOG_FORMAT)
                .WriteTo.ApplicationInsights(serviceProvider.GetService<TelemetryConfiguration>(), TelemetryConverter.Traces)
                .WriteTo.File(IntermediateAPI.Constants.AZWEBAPP_LOG_LOCATION,
                    rollingInterval: RollingInterval.Infinite,
                    retainedFileCountLimit: 1,
                    fileSizeLimitBytes: 10485760); //10MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});

app.UseExceptionHandlingMiddleware();
app.UseDfpBypassing();

//app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

try
{
    app.Run();
}
catch (OptionsValidationException ex)
{
    using (LogContext.PushProperty("SourceContext", ex.OptionsType))
    {
        Log.Fatal(ex.Message);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex.Message, ex);
}
finally
{
    Log.CloseAndFlush();
}
