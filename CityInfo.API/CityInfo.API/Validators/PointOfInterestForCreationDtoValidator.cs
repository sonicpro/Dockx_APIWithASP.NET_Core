using FluentValidation;
using CityInfo.API.Models;

namespace CityInfo.API.Validators
{
    public class PointOfInterestForCreationDtoValidator : AbstractValidator<PointOfInterestForCreationDto>
    {
        public PointOfInterestForCreationDtoValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(m => m.Description)
                .MaximumLength(200);

            RuleFor(m => m.Name)
                .NotEqual(m => m.Description);
        }
    }
}
