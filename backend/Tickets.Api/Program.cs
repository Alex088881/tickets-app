using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tickets.Application;
using Tickets.Application.Interfaces;
using Tickets.Infrastructure.Data;
using Tickets.Infrastructure.Repositories;
using Tickets.Infrastructure.Services;
using static Tickets.Infrastructure.Services.YandexRaspService;

namespace Tickets.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication. CreateBuilder(args);

            builder.Services.Configure<YandexRaspSettings>(builder.Configuration.GetSection("TICKETS:YandexRasp"));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient<IYandexRaspService, YandexRaspService>();
            builder.Services.AddControllers();

            builder.Services.AddScoped<IStationService, StationService>();
            builder.Services.AddScoped<IStationRepository, StationRepository>();
            builder.Services.AddScoped<IRouteService, RouteService>();
            
            builder.Services.AddScoped<IDataInitializer, DataInitializer>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app =  builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
                try
                {
                    await initializer.InitializeAsync();
                    Console.WriteLine("������������� ������ ��������� �������.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"������ ��� ������������� ������: {ex.Message}");
                }
            }

            app.UseCors("AllowAll");

            //app.UseStaticFiles();
            //app.UseDefaultFiles();
            //app.MapFallbackToFile("index.html");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
