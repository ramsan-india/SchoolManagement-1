using Microsoft.AspNetCore.Authorization;
using SchoolManagement.API.Authorization;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Services;
using SchoolManagement.Application.Students.Commands;
using SchoolManagement.Application.Validators;
using SchoolManagement.Domain.Services;
using SchoolManagement.Infrastructure.BackgroundServices;
using SchoolManagement.Infrastructure.Configuration;
using SchoolManagement.Infrastructure.Services;
using SchoolManagement.Persistence;
using SchoolManagement.Persistence.Repositories;

namespace SchoolManagement.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Database Configuration
            services.AddDbContext<SchoolManagementDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SchoolManagement.API")));

            // Authentication & Authorization
            var jwtSettings = Configuration.GetSection("JwtSettings");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("StaffAccess", policy => policy.RequireRole("Admin", "Principal", "Teacher"));
                options.AddPolicy("ParentAccess", policy => policy.RequireRole("Parent"));
            });

            // Dependency Injection

            // Menu and Permission Services
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleMenuPermissionRepository, RoleMenuPermissionRepository>();
            services.AddScoped<IMenuPermissionService, MenuPermissionService>();
            services.AddScoped<IUserService, UserService>();

            // Authorization Handlers
            services.AddScoped<IAuthorizationHandler, MenuPermissionHandler>();

            

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBiometricVerificationService, BiometricVerificationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAttendanceCalculationService, AttendanceCalculationService>();
            services.AddScoped<ISalaryCalculationService, SalaryCalculationService>();

            // MediatR for CQRS
            services.AddMediatR(typeof(CreateStudentCommand).Assembly);

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(CreateStudentCommandValidator).Assembly);

            // HTTP Client Factory
            services.AddHttpClient("SMS", client =>
            {
                client.BaseAddress = new Uri(Configuration["NotificationSettings:SMS:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("Email", client =>
            {
                client.BaseAddress = new Uri(Configuration["NotificationSettings:Email:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Authorization Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("StudentManagement", policy =>
                    policy.Requirements.Add(new MenuPermissionAttribute("StudentManagement", "view")));

                options.AddPolicy("StudentAdd", policy =>
                    policy.Requirements.Add(new MenuPermissionAttribute("StudentManagement", "add")));

                options.AddPolicy("EmployeeManagement", policy =>
                    policy.Requirements.Add(new MenuPermissionAttribute("HRMSManagement", "view")));

                options.AddPolicy("AttendanceView", policy =>
                    policy.Requirements.Add(new MenuPermissionAttribute("AttendanceManagement", "view")));

                options.AddPolicy("SystemAdmin", policy =>
                    policy.Requirements.Add(new MenuPermissionAttribute("SystemSettings", "view")));
            });

            // Configuration Options
            services.Configure<BiometricSettings>(Configuration.GetSection("BiometricSettings"));
            services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));
            services.Configure<AttendanceSettings>(Configuration.GetSection("AttendanceSettings"));

            // API Documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "School Management System API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new()
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddControllers();
            services.AddHealthChecks()
                .AddDbContextCheck<SchoolManagementDbContext>();

            // Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
            });

            // Background Services
            services.AddHostedService<AttendanceSyncService>();
            services.AddHostedService<NotificationProcessorService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
