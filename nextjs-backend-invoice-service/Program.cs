using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nextjs_backend.Grpc;
using nextjs_backend.Models;
using nextjs_backend.Services;
using Steeltoe.Discovery.Client;

// net6.0's HttpClient refuses cleartext (non-TLS) HTTP/2 by default; cust-service's
// gRPC endpoint is plain http:// inside the docker/k8s network, so opt in explicitly.
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    //options.AddPolicy("ReleaseCorsPolicy", builder =>
    //{
    //    builder.WithOrigins("https://kapcimix.com", "https://*.kapcimix.com")
    //           .SetIsOriginAllowedToAllowWildcardSubdomains()
    //           .AllowCredentials()
    //           .AllowAnyHeader()
    //           .AllowAnyMethod();
    //});

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
builder.Services.AddGrpcClient<CustomerLookup.CustomerLookupClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcServices:CustomerService"]!);
});
builder.Services.AddSingleton<RabbitMqPublisher>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCorsPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseDiscoveryClient();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
