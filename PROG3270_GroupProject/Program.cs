using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Infrastructure.Data;
using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Repositories;
using PROG3270_GroupProject.Services;
using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Application.Services;
using PROG3270_GroupProject.Infrastructure.Repositories;
using PROG3270_GroupProject.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Allows only the React app to make requests
              .AllowAnyMethod()  // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any header
    });
});

// Register services and repositories (Dependency Injection)
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("ProjectContext")));

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();