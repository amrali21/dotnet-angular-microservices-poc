using nextjs_backend.Models;

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
builder.Services.AddDbContext<nextjstestContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCorsPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
