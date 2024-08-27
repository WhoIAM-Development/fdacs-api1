using FluentValidation;

namespace IntermediateAPI.Models.Validators
{
    public class HomeRealmSettingsValidator : AbstractValidator<HomeRealmSettings>
    {
        public HomeRealmSettingsValidator()
        {
            RuleFor(x => x.StorageAccount).NotEmpty();
            RuleFor(x => x.Table).NotEmpty();          
        }
    }
}
