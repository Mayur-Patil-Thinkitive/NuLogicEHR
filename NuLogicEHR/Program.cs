using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NuLogicEHR.Configurations;
using NuLogicEHR.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new NuLogicEHR.Configurations.DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NuLogicEHR.Configurations.NullableDateOnlyJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
           ValidAudience = builder.Configuration["JwtSettings:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
       };
   });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
          .ReplaceService<IModelCacheKeyFactory, DynamicSchemaModelCacheKeyFactory>());

// Services
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<DbContextFactory>();
builder.Services.AddSingleton<IModelCacheKeyFactory, DynamicSchemaModelCacheKeyFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // context.Database.EnsureCreated(); // Commented for migrations
}

app.Run();