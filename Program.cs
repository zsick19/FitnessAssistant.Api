using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Shared;
using FitnessAssistant.Api.Shared.Authorization;
using FitnessAssistant.Api.Shared.FileUpload;
using FitnessAssistant.Api.Shared.MappingProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();

var connString = builder.Configuration.GetConnectionString("FitnessAssistant");
builder.Services.AddSqlite<FitnessAssistantContext>(connString);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;

    options.CombineLogs = true;
});
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor().AddSingleton<FileUploader>();



builder.AddFitnessAssistantAuthentication();
builder.AddFitnessAssistantAuthorization();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            var allowedOrigin = "http://localhost:5173";
            policy.WithOrigins(allowedOrigin)
                  .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                  .AllowAnyMethod();
        });
});



var app = builder.Build();

app.UseStaticFiles();
app.UseCors();


app.UseAuthentication();
app.UseAuthorization();


app.MapAllRoutes();




app.UseHttpLogging();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); } else { app.UseExceptionHandler(); }

// app.UseMiddleware<RequestTimingMiddleware>();

app.UseStatusCodePages();

await app.InitializeDbAsync();

app.Run();
