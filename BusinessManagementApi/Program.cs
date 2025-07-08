using BusinessManagementApi.Helpers;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Read JWT values from appsettings.json
/// </summary>
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<AuthService>();

/// <summary>
/// Add CORS Policy
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

///<summary>
///Configure JWT Authentication
///</summary>
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing"))
        )
    };
});

/// <summary>
/// MONGODB CONFIGURATION
/// </summary>

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

///<summary>
///Configure JWT Authentication
///</summary>
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDbSettings section is missing.");

    return new MongoClient(mongoSettings.ConnectionString ?? throw new InvalidOperationException("MongoDB connection string is missing"));
});

///<summary>
///Register mongodb database
///</summary>
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDbSettings section is missing.");

    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName ?? throw new InvalidOperationException("MongoDB database name is missing"));
});

builder.Services.AddSingleton<MongoDbContext>();

/// <summary>
/// REPOSITORIES & SERVICES
/// </summary>

builder.Services.AddScoped(sp => new Repository<User>(sp.GetRequiredService<MongoDbContext>(), "Users"));
builder.Services.AddScoped(sp => new Repository<Business>(sp.GetRequiredService<MongoDbContext>(), "Businesses"));
builder.Services.AddScoped(sp => new Repository<Invoice>(sp.GetRequiredService<MongoDbContext>(), "Invoices"));

builder.Services.AddScoped<InvoiceRepository>(); // InvoiceRepository with custom methods
builder.Services.AddScoped<InvoiceService>();

builder.Services.AddSingleton<BusinessRepository>(); // Singleton for BusinessRepository
builder.Services.AddScoped<BusinessService>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Business Management API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token in format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();