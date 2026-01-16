using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Service;
using FluentValidation;
using FluentValidation.AspNetCore;
using Taskify.Api.Controllers;
using Taskify.Api.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Taskify.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateTaskDtoValidator>());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure EF Core
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=TaskifyDb;Trusted_Connection=True;MultipleActiveResultSets=true";
            builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(connectionString));

            // Register services
            builder.Services.AddScoped<ITaskService, TaskService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            // CORS - allow Angular and React dev servers
            var angularDevOrigin = "http://localhost:4200";
            var reactDevOrigin = "http://localhost:5173";

            builder.Services.AddCors(options =>
            {
                // Combined policy for all local dev origins
                options.AddPolicy("AllowLocalDev", policy =>
                {
                    policy.WithOrigins(angularDevOrigin, reactDevOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });

                // fallback permissive policy for other dev origins if needed
                options.AddPolicy("AllowAllLocal", policy =>
                {
                    policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // JWT
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "default_secret_key_please_change";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "taskify";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "taskify_users";
            var key = Encoding.UTF8.GetBytes(jwtKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use CORS before authentication - allows both Angular and React
            app.UseCors("AllowLocalDev");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
