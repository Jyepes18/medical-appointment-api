using Amazon.Runtime;
using Amazon.S3;
using MedicalAppointment.Api;
using MedicalAppointment.Api.Helpers;
using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using MedicalAppointment.Api.Services;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RailwayBucketOptions>(
    builder.Configuration.GetSection("RailwayBucket"));

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RailwayBucketOptions>>().Value;

    var credentials = new BasicAWSCredentials(
        options.AccessKey,
        options.SecretKey);

    var config = new AmazonS3Config
    {
        ServiceURL = options.Endpoint,
        ForcePathStyle = true
    };

    return new AmazonS3Client(credentials, config);
});

QuestPDF.Settings.License = LicenseType.Community;

// Dependency Injection
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminValidation, AdminValidation>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserValidate, UserValidate>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPdfService, PdfService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();