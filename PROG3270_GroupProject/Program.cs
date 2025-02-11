using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Enable CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Allows only the React app to make requests
              .AllowAnyMethod()  // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any header
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("ProjectContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
