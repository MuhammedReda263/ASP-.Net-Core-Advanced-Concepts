using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonHeaderActionFilter : IAsyncActionFilter , IOrderedFilter
    {    
        private readonly ILogger<PersonHeaderActionFilter> _logger;
        private readonly string Key; 
        private readonly string Value;

        public int Order { get; set; } // it's for global filter Specifically in program.cs

        public PersonHeaderActionFilter(ILogger<PersonHeaderActionFilter> logger, string key, string value,int order)
        {
            _logger = logger;
             Key = key;
             Value = value;
             Order = order;
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
