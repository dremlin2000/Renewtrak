using Glossary.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Glossary.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x =>
                        new ValidationErrorVm
                        {
                            PropertyName = x.Key,
                            Messages = x.Value.Errors.Select(y => y.ErrorMessage)
                        });

                var error = new ErrorMessageVm
                {
                    ErrorCode = ((int)HttpStatusCode.BadRequest).ToString(),
                    ErrorType = HttpStatusCode.BadRequest.ToString(),
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(error);
                return;
            }

            await next();
        }
    }
}
