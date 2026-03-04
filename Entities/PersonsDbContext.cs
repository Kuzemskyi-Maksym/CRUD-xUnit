using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {

        public PersonsDbContext(DbContextOptions options) : base (options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //seed data

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            string basePath = AppContext.BaseDirectory;

            string countriesJson = File.ReadAllText(
                Path.Combine(basePath, "countries.json"));

            string personsJson = File.ReadAllText(
                Path.Combine(basePath, "persons.json")); 
            
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson, options);

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            foreach (Person person in persons)
            {   
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent api
            modelBuilder.Entity<Person>().Property(person => person.Address).HasColumnName("ActualAddress").HasColumnType("varchar(60)").HasDefaultValue("Academian street");

            //table relations(
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(country => country.Country).WithMany(person => person.Persons).HasForeignKey(person => person.CountryID);
            //});
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public void sp_AddPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            };

            Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters); 
        }
    }
}
