using AutoFixture;
using CRUD_xUnit.Controllers;
using Entities;
using Entities.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonsService>();
            _countriesServiceMock = new Mock<ICountriesService>();
        }

        private PersonsController CreateController() =>
            new PersonsController(_personsServiceMock.Object, _countriesServiceMock.Object);

        #region Index

        [Fact]
        public async Task Index_ValidRequest_ReturnsViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons = _fixture.Create<List<PersonResponse>>();

            _personsServiceMock
                .Setup(s => s.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons);

            _personsServiceMock
                .Setup(s => s.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Index(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                nameof(PersonResponse.PersonName),
                SortOrderOptions.ASC);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons);
        }

        #endregion

        #region Create (GET)

        [Fact]
        public async Task CreateGet_ReturnsViewWithCountriesInViewBag()
        {
            // Arrange
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(s => s.GetAllCountries())
                .ReturnsAsync(countries);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Create();

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData["Countries"].Should().NotBeNull();
        }

        #endregion

        #region Create (POST)

        [Fact]
        public async Task CreatePost_InvalidModelState_ReturnsCreateView()
        {
            // Arrange
            PersonAddRequest request = _fixture.Create<PersonAddRequest>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(s => s.GetAllCountries())
                .ReturnsAsync(countries);

            var controller = CreateController();
            controller.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            // Act
            IActionResult result = await controller.Create(request);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeNull(); // Create POST не передає model у View при помилці
        }

        [Fact]
        public async Task CreatePost_ValidRequest_RedirectsToIndex()
        {
            // Arrange
            PersonAddRequest request = _fixture.Create<PersonAddRequest>();
            PersonResponse response = _fixture.Create<PersonResponse>();

            _personsServiceMock
                .Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(response);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Create(request);

            // Assert
            RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
            redirect.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("Persons");
        }

        #endregion

        #region Edit (GET)

        [Fact]
        public async Task EditGet_NonExistentPersonID_RedirectsToIndex()
        {
            // Arrange
            _personsServiceMock
                .Setup(s => s.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync((PersonResponse?)null);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Edit(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
            redirect.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EditGet_ValidPersonID_ReturnsViewWithPersonUpdateRequest()
        {
            // Arrange
            PersonResponse person = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _personsServiceMock
                .Setup(s => s.GetPersonById(person.PersonID))
                .ReturnsAsync(person);

            _countriesServiceMock
                .Setup(s => s.GetAllCountries())
                .ReturnsAsync(countries);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Edit(person.PersonID);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
        }

        #endregion

        #region Delete (GET)

        [Fact]
        public async Task DeleteGet_NonExistentPersonID_RedirectsToIndex()
        {
            // Arrange
            _personsServiceMock
                .Setup(s => s.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync((PersonResponse?)null);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
            redirect.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task DeleteGet_ValidPersonID_ReturnsViewWithPerson()
        {
            // Arrange
            PersonResponse person = _fixture.Create<PersonResponse>();

            _personsServiceMock
                .Setup(s => s.GetPersonById(person.PersonID))
                .ReturnsAsync(person);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(person.PersonID);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().Be(person);
        }

        #endregion

        #region Delete (POST)

        [Fact]
        public async Task DeletePost_ValidPersonID_RedirectsToIndex()
        {
            // Arrange
            PersonResponse person = _fixture.Create<PersonResponse>();
            PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();

            _personsServiceMock
                .Setup(s => s.GetPersonById(person.PersonID))
                .ReturnsAsync(person);

            _personsServiceMock
                .Setup(s => s.DeletePerson(person.PersonID))
                .ReturnsAsync(true);

            var controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(person.PersonID, updateRequest);

            // Assert
            RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
            redirect.ActionName.Should().Be("Index");
        }

        #endregion
    }
}