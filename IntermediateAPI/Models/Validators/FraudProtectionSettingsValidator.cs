using FluentValidation;
using IntermediateAPI;

namespace IntermediateAPI.Models.Validators
{
    public class FraudProtectionSettingsValidator : AbstractValidator<FraudProtectionSettings>
    {
        public FraudProtectionSettingsValidator()
        {
            RuleFor(x => x.EnvironmentId).NotEmpty();
            RuleFor(x => x.AADTenantId).NotEmpty();
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.ClientSecret) || !string.IsNullOrEmpty(x.CertificateThumbprint)).WithMessage(Constants.Validation_Dfp_MissingSecretOrCert);
            RuleFor(x => x).Must(x => string.IsNullOrEmpty(x.CertificateThumbprint) || string.IsNullOrEmpty(x.ClientSecret)).WithMessage(Constants.Validation_Dfp_CertAndSecretConfigured);

            RuleFor(x => x.Overrides).SetValidator(new FraudProtectionOverrideValidator());
        }
    }
}
