using AutoFixture;
using Entities;
using Entities.DTO;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Services;
using Xunit;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly PersonsDbContext _db;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<PersonsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new PersonsDbContext(options);
            _countriesService = new CountriesService(_db);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act
            Func<Task> action = async () => await _countriesService.AddCountry(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddCountry_NullCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            // Act
            Func<Task> action = async () => await _countriesService.AddCountry(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_DuplicateName_ThrowsArgumentException()
        {
            // Arrange
            string duplicateName = "Ukraine";

            Country existing = new Country { CountryID = Guid.NewGuid(), CountryName = duplicateName };
            _db.Countries.Add(existing);
            await _db.SaveChangesAsync();

            CountryAddRequest request = new CountryAddRequest { CountryName = duplicateName };

            // Act
            Func<Task> action = async () => await _countriesService.AddCountry(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_ValidRequest_ReturnsCountryResponseWithNewID()
        {
            // Arrange
            CountryAddRequest request = new CountryAddRequest { CountryName = "Poland" };

            // Act
            CountryResponse result = await _countriesService.AddCountry(request);

            // Assert
            result.CountryID.Should().NotBe(Guid.Empty);
            result.CountryName.Should().Be("Poland");
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_EmptyDb_ReturnsEmptyList()
        {
            // Act
            List<CountryResponse> result = await _countriesService.GetAllCountries();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_WithCountries_ReturnsAllCountries()
        {
            // Arrange
            List<Country> countries = new List<Country>
            {
                new Country { CountryID = Guid.NewGuid(), CountryName = "Ukraine" },
                new Country { CountryID = Guid.NewGuid(), CountryName = "Germany" },
            };

            _db.Countries.AddRange(countries);
            await _db.SaveChangesAsync();

            List<CountryResponse> expected = countries
                .Select(c => c.ToCountryResponse())
                .ToList();

            // Act
            List<CountryResponse> result = await _countriesService.GetAllCountries();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region GetCountryByID

        [Fact]
        public async Task GetCountryByID_NullID_ReturnsNull()
        {
            // Act
            CountryResponse? result = await _countriesService.GetCountryByID(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCountryByID_NonExistentID_ReturnsNull()
        {
            // Act
            CountryResponse? result = await _countriesService.GetCountryByID(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCountryByID_ValidID_ReturnsMatchingCountry()
        {
            // Arrange
            Country country = new Country { CountryID = Guid.NewGuid(), CountryName = "France" };
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            CountryResponse expected = country.ToCountryResponse();

            // Act
            CountryResponse? result = await _countriesService.GetCountryByID(country.CountryID);

            // Assert
            result.Should().Be(expected);
        }

        #endregion
    }
}