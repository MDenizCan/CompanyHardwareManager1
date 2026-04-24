using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using CHM.API.Middlewares;
using CHM.BLL.Interfaces;
using CHM.BLL.Services;
using CHM.INFRASTRUCTURE.Repositories;
using CHM.INFRASTRUCTURE.Seed;
using CHM.MODELS.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CHM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/chm_log_.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                // Add services to the container (Dependency Injection / Konteyner ayarları)
            // SQL Server Veritabanı bağlantı konfigürasyonu
            builder.Services.AddDbContext<CHM.INFRASTRUCTURE.AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            // JWT ile Authentication (Kimlik Doğrulama) yapılandırması.
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        // Token'ı biz mi ürettik? (Issuer aynı mı)
                        ValidateAudience = true,
                        // Token'ı kim için ürettik? (Audience aynı mı)
                        ValidateIssuerSigningKey = true,
                        // Token'ı kim imzaladı? (Key aynı mı)
                        ValidateLifetime = true,
                        // Token'ın süresi dolmuş mu? (Süre kontrolü)
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            builder.Services.AddAuthorization(); // Yetkilendirme (Rol vb.) alt yapısını ekler.

            // FluentValidation Kütüphanesini API'ye otomatik entegre eder (Validator kuralları otomatik çalışır).
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddAutoMapper(typeof(CHM.BLL.Mappings.MappingProfile).Assembly);

            // Bağımlılık Enjeksiyonu (Dependency Injection - DI).
            // Her IUserRepository istendiğinde, o istek bazlı (Scoped) yeni bir UserRepository örneği (instance) ver.
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IAssetRepository, AssetRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<IRequestRepository, RequestRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAssetService, AssetServicev2>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<IRequestService, RequestService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CHM API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT as: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

            var app = builder.Build();

            // Veritabanında gerekli varsayılan rollerin (Admin, IT vb.) olduğundan emin olmak için Seeder çalıştırılır.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CHM.INFRASTRUCTURE.AppDbContext>();
                DatabaseSeeder.SeedAsync(db, CancellationToken.None).GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
