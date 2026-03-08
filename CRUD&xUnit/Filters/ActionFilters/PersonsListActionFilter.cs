using CRUD_xUnit.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUD_xUnit.Filters.ActionFilters
{
    public class PersonsListActionFilter : IAsyncActionFilter
    {

        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("PersonsListActionFilter.OnActionExecuting method");

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Age),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                    };

                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("Actual searchBy value is: {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("Updated searchBy value is: {searchBy}", searchBy);
                    }
                }
            }

            await next();

            _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["searchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["searchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["searchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["sortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
            }
        }
    }
}
