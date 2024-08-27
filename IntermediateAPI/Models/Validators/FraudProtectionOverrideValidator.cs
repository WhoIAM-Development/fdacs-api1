using FluentValidation;

namespace IntermediateAPI.Models
{
    public class FraudProtectionOverrideValidator : AbstractValidator<FraudProtectionOverrides>
    {
        public FraudProtectionOverrideValidator()
        {
            RuleFor(x => x.ApiBaseUrl).Must(uri => this.EnsureHttpsUri(uri)).When(x => !string.IsNullOrEmpty(x.ApiBaseUrl));

            RuleFor(x => x.ApiResourceUri).Must(uri => this.EnsureHttpsUri(uri)).When(x => !string.IsNullOrEmpty(x.ApiResourceUri));
        }

        public bool EnsureHttpsUri(string uri)
        {
            bool isUri = Uri.TryCreate(uri, UriKind.Absolute, out Uri configValue);
            bool isHttps = configValue?.ToString()?.StartsWith("https") ?? false;

            return isUri && isHttps;
        }
    }
}
