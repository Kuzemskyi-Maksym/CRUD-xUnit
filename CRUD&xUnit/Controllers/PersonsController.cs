using CRUD_xUnit.Filters.ActionFilters;
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
        [TypeFilter(typeof(PersonsListActionFilter))]
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
            List<PersonResponse>  sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrderOption);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOption.ToString();


            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            ViewBag.Countries = allCountries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() });

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

                ViewBag.Countries = allCountries.Select(country => new SelectListItem()
                {
                    Text = country.CountryName,
                    Value = country.CountryID.ToString()
                });

                ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(error => error.ErrorMessage).ToList();

                return View();
                 
            }

            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personID);

            if (personResponse == null)
                return RedirectToAction("Index");

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

            ViewBag.Countries = new SelectList(
                allCountries,
                "CountryID",
                "CountryName",
                personUpdateRequest.CountryID
            );

            return View(personUpdateRequest);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid personID, PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personUpdateRequest.PersonID);

            if (personResponse == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

                ViewBag.Countries = allCountries.Select(country => new SelectListItem()
                {
                    Text = country.CountryName,
                    Value = country.CountryID.ToString()
                });

                ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(error => error.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonById(personID);

            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
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
