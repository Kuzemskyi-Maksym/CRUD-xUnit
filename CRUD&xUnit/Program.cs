using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

namespace CRUD_xUnit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews().AddXmlSerializerFormatters();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();
            builder.Services.AddDbContext<PersonsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            Rotativa.AspNetCore.RotativaConfiguration.Setup(
                builder.Environment.WebRootPath,
                "Rotativa"
            );

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}