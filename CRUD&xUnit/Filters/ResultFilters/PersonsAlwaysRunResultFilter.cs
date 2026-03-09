using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_xUnit.Filters.ResultFilters
{
    public class PersonsAlwaysRunResultFilter : IAsyncAlwaysRunResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                await next();
                return;
            }

            await next();
        }
    }
}
