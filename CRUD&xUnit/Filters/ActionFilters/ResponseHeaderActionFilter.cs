using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;

        public int Order { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key = "My-Key-From-Global", string value = "My-Value-From-Global", int order = 2)
        {
            _logger = logger;
            Key = key;
            Value = value;
            Order = order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} before method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            await next();

            _logger.LogInformation("{FilterName}.{MethodName} after method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
