using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters
{
    public class SkipFilter : Attribute, IFilterMetadata //1) make it as an attribute 2) act as a filter
    {

    }
}
