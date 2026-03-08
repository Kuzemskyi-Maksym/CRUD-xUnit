using CRUD_xUnit.Controllers;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUD_xUnit.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;


        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

                    personsController.ViewBag.Countries = allCountries.Select(country => new SelectListItem()
                    {
                        Text = country.CountryName,
                        Value = country.CountryID.ToString()
                    });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(error => error.Errors).Select(error => error.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personAddRequest"];

                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
