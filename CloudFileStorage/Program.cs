using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amazon.S3;
using Azure.Storage.Blobs;
using CloudFileStorage.Data;
using CloudFileStorage.Extensions;
using CloudFileStorage.Helpers;
using CloudFileStorage.Services.Implementations;
using CloudFileStorage.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../.env");
builder.Configuration
    .AddEnvironmentVariables(); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration["AZURE:BLOB_STORAGE_CONNECTION_STRING"];
    return new BlobServiceClient(connectionString);
});
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["JWT:SECRET"];
        Console.WriteLine(jwtSecret);
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero // Remove delay of token when expire 
        };
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
