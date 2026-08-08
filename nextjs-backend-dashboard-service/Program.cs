using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nextjs_backend_dashboard_service.Models;
using nextjs_backend_dashboard_service.Services;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://localhost:4200", "http://localhost:3001", "http://127.0.0.1:5500")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<nextjstestContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddScoped<KpiService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCorsPolicy");
    app.MapOpenApi();
}

//app.UseDiscoveryClient();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
