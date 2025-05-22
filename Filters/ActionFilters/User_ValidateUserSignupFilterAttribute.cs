using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Backend.Dto;

namespace Backend.Filters.ActionFilters
{
    public class User_ValidateUserSignupFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var userDto = context.ActionArguments["userCreate"] as UserDto;

            if (userDto != null)
            {
                if (!Validations.IsValidEmail(userDto.Email))
                {
                    context.ModelState.AddModelError("User Email", "Email format is invalid");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                string isPasswordStrong = Validations.IsValidPassword(userDto.Password);

                if (isPasswordStrong != "ok")
                {
                    context.ModelState.AddModelError("User Password", isPasswordStrong);
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