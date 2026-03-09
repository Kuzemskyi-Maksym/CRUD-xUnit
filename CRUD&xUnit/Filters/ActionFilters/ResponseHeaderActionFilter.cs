using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private string? Key {  get; set; }
        private string? Value {  get; set; }
        private int Order {  get; set; }


        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();

            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;

            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public string Key;
        public string Value;

        public int Order { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
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
