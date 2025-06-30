using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Services;
using LibraryManagement.Repositories;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Configure to bind to all interfaces
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    // Docker environment - use HTTP only
    builder.WebHost.UseUrls("http://0.0.0.0:5288");
}
else
{
    // Local development - use both HTTP and HTTPS
    builder.WebHost.UseUrls("http://0.0.0.0:5288", "https://0.0.0.0:7291");
}

builder.Services.AddOpenApi();

// Add Swagger/OpenAPI services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Library Management API", 
        Version = "v1",
        Description = "A comprehensive API for managing library operations including books, members, and loans."
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

// Register Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanService, LoanService>();

// Configure CORS - More explicit configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Also add a named policy
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        
        // Ensure database is created (this will create tables if they don't exist)
        context.Database.EnsureCreated();
        
        Console.WriteLine("Database initialized successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API V1");
        c.RoutePrefix = "swagger";
    });
}

// CORS must be one of the first middleware
app.UseCors();

app.UseHttpsRedirection();

// Add routing and map controllers
app.UseRouting();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
    .WithName("HealthCheck");

app.Run();