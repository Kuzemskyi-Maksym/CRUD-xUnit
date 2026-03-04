using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_xUnit.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {

        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        //after execution
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");
        }

        //before execution
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("PersonsListActionFilter.OnActionExecuting method");
        }
    }
}
