using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Backend.Filters.ActionFilters
{
    public class Category_ValidateCategoryIdFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var categoryId = context.ActionArguments["categoryId"] as int?;

            if (categoryId.HasValue)
            {
                if (categoryId.Value <= 0)
                {
                    context.ModelState.AddModelError("CategoryId", "CategoryId is invalid");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }
            }
        }
    }
}