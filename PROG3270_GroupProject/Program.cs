using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Application.Interfaces;
using PROG3270_GroupProject.Application.Services;
using PROG3270_GroupProject.Infrastructure.Data;
using PROG3270_GroupProject.Infrastructure.Interfaces;
using PROG3270_GroupProject.Infrastructure.Repositories;
using PROG3270_GroupProject.Interfaces;
using PROG3270_GroupProject.Repositories;
using PROG3270_GroupProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register DbContext first
builder.Services.AddDbContext<ProjectContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectContext")));

// Register HTTP Client
builder.Services.AddHttpClient();

// Register repositories
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<PROG3270_GroupProject.Infrastructure.Interfaces.IMemberRepository, 
                           PROG3270_GroupProject.Infrastructure.Repositories.MemberRepository>();

// Configure HttpClient for ProductRepository
builder.Services.AddHttpClient<IProductRepository, ProductRepository>(client => {
    // Configure client if needed
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register services
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Add API controllers
builder.Services.AddControllers();

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
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