using System.Reflection;
using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OllamaSharp;
using WorkoutTrackerServices.Entities;
using WorkoutTrackerServices.Repositories;
using WorkoutTrackerServices.Repositories.Interfaces;
using WorkoutTrackerServices.Services;
using WorkoutTrackerServices.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// Ollama config (optional, can be set in appsettings.json)
// builder.Configuration["Ollama:Endpoint"] = "http://localhost:11434";
// builder.Configuration["Ollama:DefaultModel"] = "llama3";

ConfigureServices(builder);
ConfigureAuthentication(builder);

var app = builder.Build();

ConfigureMiddleware(app);

app.Run();
return;

void ConfigureServices(WebApplicationBuilder builder)
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ApplicationException("Database connection string is missing!");
    // Configure DbContext
    builder.Services.AddDbContext<WorkoutContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
    builder.Services.AddScoped<IWorkoutService, WorkoutService>();
    builder.Services.AddControllers(options =>
    {
        // Apply [Authorize] globally
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAutoMapper(typeof(Program));
    // Register OllamaSharp IChatClient for LLM (for chat history support)
    builder.Services.AddSingleton<IChatClient>(sp =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var endpoint = config["Ollama:Endpoint"] ?? "http://localhost:11434";
        var model = config["Ollama:DefaultModel"] ?? "llama3";
        return new OllamaApiClient(new Uri(endpoint), model);
    });
    builder.Services.AddScoped<ILlmService, LlmService>();
    ConfigureSwagger(builder);
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkoutTracker API", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter your JWT token in this field",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        c.AddSecurityDefinition("Bearer", securityScheme);

        var securityRequirement = new OpenApiSecurityRequirement
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
                new string[] {}
            }
        };

        c.AddSecurityRequirement(securityRequirement);
    });
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    string jwtKey = builder.Configuration["JwtSettings:Key"] ?? throw new ApplicationException("JWT_KEY is missing");
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// builder.Services.Configure<JwtSettings>(jwtSettings);
    byte[] key = Encoding.ASCII.GetBytes(jwtKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
        };
    });

    builder.Services.AddAuthorization();
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

/*
    The JWT setup is already configured in the ConfigureAuthentication method above.
    - It reads the JWT key from configuration.
    - Sets up JwtBearer authentication with token validation parameters.
    - Adds authentication and authorization services.
    No further code is needed here for JWT setup.
*/
/*
    The Swagger setup is already configured in the ConfigureSwagger method above.
    - It sets up Swagger documentation for the API.
    - It includes XML comments for better documentation.
    - It adds security definitions for JWT authentication.
    No further code is needed here for Swagger setup.
*/
