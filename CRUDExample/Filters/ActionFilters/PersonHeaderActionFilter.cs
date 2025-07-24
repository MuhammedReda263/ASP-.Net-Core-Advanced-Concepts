using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{

    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private string? Key { get; set; }
        private string? Value { get; set; }
        private int Order { get; set; }
        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }


        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<PersonHeaderActionFilter>(); // we do DI in p.cs to make this line work 
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;
            return filter;
        }

    }

    public class PersonHeaderActionFilter : IAsyncActionFilter , IOrderedFilter
    {    
        private readonly ILogger<PersonHeaderActionFilter> _logger;
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; } // it's for global filter Specifically in program.cs

        public PersonHeaderActionFilter(ILogger<PersonHeaderActionFilter> logger)
        {
            _logger = logger;
        
        }
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonHeaderActionFilter), nameof(OnActionExecuting));
            
        //}

        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonHeaderActionFilter), nameof(OnActionExecuted));
        //    context.HttpContext.Request.Headers[Key] = Value;

        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonHeaderActionFilter), nameof(OnActionExecutionAsync));
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Request.Headers[Key] = Value;

        }
    }


}
