using FluentValidation;

namespace IntermediateAPI.Models.Validators
{
    public class AzureAdSettingsValidator : AbstractValidator<AzureAdSettings>
    {
        public AzureAdSettingsValidator() 
        {
            RuleFor(x => x.Domain).NotEmpty();
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
        }
    }
}
