using CRUD_xUnit.Filters;
using CRUD_xUnit.Filters.ActionFilters;
using CRUD_xUnit.Filters.ExceptionFilters;
using CRUD_xUnit.Filters.ResultFilters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_xUnit.Controllers
{
    [Route("[controller]")]
    [ResponseHeaderFilterFactory("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [ServiceFilter(typeof(HandleExceptionFilter))]
    [ServiceFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("/")]
        [Route("[action]")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
        [ServiceFilter(typeof(PersonsListResultFilter))]
        [ResponseHeaderFilterFactory("MyKey-FromAction", "MyValue-From-Action", 1)]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrderOption = SortOrderOptions.ASC)
        {
            //Searching
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Age), "Age" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Gender), "Gender" },
            };

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //sorting
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrderOption);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOption.ToString();


            return View(sortedPersons);
        }

        [HttpGet]
        [Route("[action]")]
        [ResponseHeaderFilterFactory( "my-key", "my-value", 4 )]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            ViewBag.Countries = allCountries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() });

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [ServiceFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [ServiceFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [ServiceFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [ServiceFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonResponse updatedPerson = await _personsService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personID);

            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID, PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personUpdateRequest.PersonID);

            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                FileName = "Persons.pdf",
                PageMargins = new Margins()
                {
                    Top = 20,
                    Right = 20,
                    Left = 20,
                    Bottom = 20,
                },
                PageOrientation = Orientation.Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream stream = await _personsService.GetPersonsCSV();
            return File(stream, "application/octet-stream", "Persons.csv");
        }
    }
}