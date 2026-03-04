using AutoFixture;
using Entities;
using Entities.DTO;
using Entities.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly PersonsDbContext _db;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper output)
        {
            _output = output;
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<PersonsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new PersonsDbContext(options);
            _countriesServiceMock = new Mock<ICountriesService>();
            _personsService = new PersonsService(_db, _countriesServiceMock.Object);
        }

        // Helper: додає особу в БД і повертає PersonResponse
        private async Task<PersonResponse> AddPersonToDb(
            string? name = "John",
            string email = "test@example.com",
            GenderOptions gender = GenderOptions.Male)
        {
            Person person = new Person
            {
                PersonID = Guid.NewGuid(),
                PersonName = name,
                Email = email,
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = gender,
                CountryID = null,
                Address = "Test Street 1",
                ReceiveNewsLetters = false
            };

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullRequest_ThrowsArgumentNullException()
        {
            // Act
            Func<Task> action = async () => await _personsService.AddPerson(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_NullPersonName_AddsPersonWithNullName()
        {
            // Arrange
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, null as string)
                .With(p => p.Email, "test@example.com")
                .Create();

            // Act
            PersonResponse result = await _personsService.AddPerson(request);

            // Assert
            result.PersonID.Should().NotBe(Guid.Empty);
            result.PersonName.Should().BeNull();
        }

        [Fact]
        public async Task AddPerson_ValidRequest_ReturnsPersonResponseWithNewID()
        {
            // Arrange
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "valid@example.com")
                .Create();

            // Act
            PersonResponse result = await _personsService.AddPerson(request);

            // Assert
            result.PersonID.Should().NotBe(Guid.Empty);
            result.Email.Should().Be("valid@example.com");
        }

        [Fact]
        public async Task AddPerson_ValidRequest_PersonSavedToDatabase()
        {
            // Arrange
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "db@example.com")
                .Create();

            // Act
            PersonResponse result = await _personsService.AddPerson(request);

            // Assert
            Person? savedPerson = await _db.Persons.FindAsync(result.PersonID);
            savedPerson.Should().NotBeNull();
            savedPerson!.Email.Should().Be("db@example.com");
        }

        #endregion

        #region GetPersonById

        [Fact]
        public async Task GetPersonById_NullID_ReturnsNull()
        {
            // Act
            PersonResponse? result = await _personsService.GetPersonById(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonById_NonExistentID_ReturnsNull()
        {
            // Act
            PersonResponse? result = await _personsService.GetPersonById(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonById_ValidID_ReturnsCorrectPerson()
        {
            // Arrange
            PersonResponse expected = await AddPersonToDb("Alice", "alice@example.com");

            // Act
            PersonResponse? result = await _personsService.GetPersonById(expected.PersonID);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_EmptyDb_ReturnsEmptyList()
        {
            // Act
            List<PersonResponse> result = await _personsService.GetAllPersons();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_WithPersons_ReturnsAll()
        {
            // Arrange
            PersonResponse p1 = await AddPersonToDb("Alice", "alice@example.com");
            PersonResponse p2 = await AddPersonToDb("Bob", "bob@example.com");
            PersonResponse p3 = await AddPersonToDb("Charlie", "charlie@example.com");

            // Act
            List<PersonResponse> result = await _personsService.GetAllPersons();

            _output.WriteLine("Result:");
            result.ForEach(p => _output.WriteLine(p.ToString()));

            // Assert
            result.Should().HaveCount(3);
            result.Should().ContainEquivalentOf(p1);
            result.Should().ContainEquivalentOf(p2);
            result.Should().ContainEquivalentOf(p3);
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ReturnsAll()
        {
            // Arrange
            await AddPersonToDb("Alice", "alice@example.com");
            await AddPersonToDb("Bob", "bob@example.com");

            // Act
            List<PersonResponse> result = await _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetFilteredPersons_ByPersonName_ReturnsMatchingPersons()
        {
            // Arrange
            await AddPersonToDb("Alice Smith", "alice@example.com");
            await AddPersonToDb("Bob Jones", "bob@example.com");
            await AddPersonToDb("Alice Brown", "abrown@example.com");

            // Act
            List<PersonResponse> result = await _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "Alice");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.PersonName!.Contains("Alice", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetFilteredPersons_ByEmail_ReturnsMatchingPersons()
        {
            // Arrange
            await AddPersonToDb("Alice", "alice@gmail.com");
            await AddPersonToDb("Bob", "bob@yahoo.com");

            // Act
            List<PersonResponse> result = await _personsService.GetFilteredPersons(nameof(PersonResponse.Email), "gmail");

            // Assert
            result.Should().HaveCount(1);
            result[0].Email.Should().Be("alice@gmail.com");
        }

        [Fact]
        public async Task GetFilteredPersons_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            await AddPersonToDb("Alice", "alice@example.com");

            // Act
            List<PersonResponse> result = await _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "XYZ_NOBODY");

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ByNameAsc_ReturnsSortedList()
        {
            // Arrange
            await AddPersonToDb("Charlie", "c@example.com");
            await AddPersonToDb("Alice", "a@example.com");
            await AddPersonToDb("Bob", "b@example.com");

            List<PersonResponse> all = await _personsService.GetAllPersons();

            // Act
            List<PersonResponse> result = await _personsService.GetSortedPersons(all, nameof(PersonResponse.PersonName), SortOrderOptions.ASC);

            // Assert
            result.Should().BeInAscendingOrder(p => p.PersonName);
        }

        [Fact]
        public async Task GetSortedPersons_ByNameDesc_ReturnsSortedList()
        {
            // Arrange
            await AddPersonToDb("Charlie", "c@example.com");
            await AddPersonToDb("Alice", "a@example.com");
            await AddPersonToDb("Bob", "b@example.com");

            List<PersonResponse> all = await _personsService.GetAllPersons();

            // Act
            List<PersonResponse> result = await _personsService.GetSortedPersons(all, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);

            // Assert
            result.Should().BeInDescendingOrder(p => p.PersonName);
        }

        [Fact]
        public async Task GetSortedPersons_EmptySortBy_ReturnsUnchangedList()
        {
            // Arrange
            await AddPersonToDb("Alice", "a@example.com");
            List<PersonResponse> all = await _personsService.GetAllPersons();

            // Act
            List<PersonResponse> result = await _personsService.GetSortedPersons(all, "", SortOrderOptions.ASC);

            // Assert
            result.Should().BeEquivalentTo(all);
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullRequest_ThrowsArgumentNullException()
        {
            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_NonExistentPersonID_ThrowsArgumentException()
        {
            // Arrange
            PersonUpdateRequest request = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.Email, "test@example.com")
                .Create();

            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_ValidRequest_UpdatesName()
        {
            // Arrange
            PersonResponse added = await AddPersonToDb("OriginalName", "orig@example.com", GenderOptions.Male);

            _countriesServiceMock
                .Setup(s => s.GetCountryByID(It.IsAny<Guid?>()))
                .ReturnsAsync((CountryResponse?)null);

            PersonUpdateRequest request = added.ToPersonUpdateRequest();
            request.PersonName = "UpdatedName";
            request.CountryID = null;

            // Act
            PersonResponse result = await _personsService.UpdatePerson(request);

            // Assert
            result.PersonName.Should().Be("UpdatedName");
        }

        [Fact]
        public async Task UpdatePerson_FutureDateOfBirth_ThrowsArgumentException()
        {
            // Arrange
            PersonResponse added = await AddPersonToDb();
            PersonUpdateRequest request = added.ToPersonUpdateRequest();
            request.DateOfBirth = DateTime.Now.AddYears(1);
            request.CountryID = null;

            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(request);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_NullID_ThrowsArgumentNullException()
        {
            // Act
            Func<Task> action = async () => await _personsService.DeletePerson(null);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeletePerson_NonExistentID_ReturnsFalse()
        {
            // Act
            bool result = await _personsService.DeletePerson(Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePerson_ValidID_ReturnsTrueAndRemovesFromDb()
        {
            // Arrange
            PersonResponse added = await AddPersonToDb("ToDelete", "del@example.com");

            // Act
            bool result = await _personsService.DeletePerson(added.PersonID);

            // Assert
            result.Should().BeTrue();

            Person? deleted = await _db.Persons.FindAsync(added.PersonID);
            deleted.Should().BeNull();
        }

        #endregion
    }
}