using CRUD_xUnit.Filters.ActionFilters;
using CRUD_xUnit.Filters.ExceptionFilters;
using CRUD_xUnit.Filters.ResultFilters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
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

            builder.Services.AddControllersWithViews(options => {
                //options.Filters.Add<ResponseHeaderActionFilter>(5);

                var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "My-Key-From-Global",
                    Value = "My-Value-From-Global",
                    Order = 2
                });
            });

            builder.Services.AddScoped<ResponseHeaderActionFilter>();



            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();

            builder.Services.AddTransient<PersonCreateAndEditPostActionFilter>();
            builder.Services.AddTransient<PersonsListActionFilter>();
            builder.Services.AddTransient<TokenAuthorizationFilter>();
            builder.Services.AddTransient<FeatureDisabledResourceFilter>();
            builder.Services.AddTransient<PersonsAlwaysRunResultFilter>();
            builder.Services.AddTransient<PersonsListResultFilter>();
            builder.Services.AddTransient<TokenResultFilter>();
            builder.Services.AddTransient<HandleExceptionFilter>();

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