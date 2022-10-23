using FluentValidation;
using MeetAndGo.Infrastructure.Utils;

namespace MeetAndGo.Infrastructure.Services
{
    public interface IValidationService
    {
        Result<T> ValidateRules<T>(IValidator<T> validator, T obj);
    }

    public class ValidationService : IValidationService
    {
        public Result<T> ValidateRules<T>(IValidator<T> validator, T obj)
        {
            if (obj == null) return Result.Fail<T>("The sent object cannot be null");
            var results = validator.Validate(obj);

            return !results.IsValid ? Result.Fail<T>(string.Join(";", results.Errors)) : Result.Ok(obj);
        }
    }
}
