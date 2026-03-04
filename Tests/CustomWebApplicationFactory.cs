using CRUD_xUnit;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureTestServices(services =>
            {
                // Видаляємо всі дескриптори що стосуються PersonsDbContext і EF Core опцій
                var toRemove = services
                    .Where(d =>
                        d.ServiceType == typeof(PersonsDbContext) ||
                        d.ServiceType == typeof(DbContextOptions<PersonsDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        (d.ServiceType.Namespace != null &&
                         d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore")))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                // Додаємо свіжий InMemory контекст без жодних залишків від SqlServer
                services.AddDbContext<PersonsDbContext>((sp, options) =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting");
                });
            });
        }
    }
}