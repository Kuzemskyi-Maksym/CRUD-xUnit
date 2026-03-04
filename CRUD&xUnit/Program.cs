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

            builder.Services.AddDbContext<PersonsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30

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
