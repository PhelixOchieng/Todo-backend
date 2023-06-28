using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Todo.Filters;

public class GlobalResponseFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
			Console.WriteLine("Called");
        if (context.Result is not ObjectResult objectResult)
            return;

        var response = new
        {
            error = false,
            message = "success",
            data = objectResult.Value
        };
        context.Result = new ObjectResult(response)
        {
            StatusCode = objectResult.StatusCode,
            ContentTypes = objectResult.ContentTypes,
        };
    }

		public void OnActionExecuting(ActionExecutingContext context) {}
}
