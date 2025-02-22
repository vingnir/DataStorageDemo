using Business.Interfaces;
using Business.Services;
using Business.Services.Implementations;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register OpenAPI
builder.Services.AddOpenApi();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5127")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ✅ JSON Serialization Settings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ✅ Register DbContextOptions<AppDbContext> as Singleton (Fixes the Scoped-to-Singleton Issue)
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Data"));
    return optionsBuilder.Options;
});

// ✅ Register Scoped DbContext (Used by UnitOfWork & Repositories)
builder.Services.AddDbContext<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// ✅ Register DbContextFactory (For Short-Lived Queries)
builder.Services.AddDbContextFactory<AppDbContext>((provider, options) =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    options.UseSqlServer(dbContextOptions.Extensions.OfType<RelationalOptionsExtension>().First().ConnectionString);
});

// ✅ Register UnitOfWork (Scoped - Ensures transactions)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ✅ Register Repositories (Scoped - Uses Scoped DbContext)
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// ✅ Register Services (Using UnitOfWork Instead of Injecting DbContext)
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// ✅ Enable OpenAPI/Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
    app.UseDeveloperExceptionPage();
}

// ✅ Enable CORS
app.UseCors("AllowNextJs");

// ✅ Middleware
app.UseRouting();
app.UseAuthorization();
app.MapControllers();  // 🔥 This requires `AddControllers()` to be registered above
app.Run();
