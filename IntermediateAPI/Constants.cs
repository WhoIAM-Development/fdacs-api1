namespace IntermediateAPI
{
    public static class Constants
    {
        public const string Default = "default";
        public const string Local = "local";
        public const string AAD = "aad";
        public const string HomeRealmTable = "DomainHomeRealm";
        public const string LoginTypes = "LoginType";

        public static readonly string DEFAULT_DFP_PROD_URI = "api.dfp.dynamics.com";
        public static readonly string DEFAULT_DFP_SANDBOX_URI = "api.dfp.dynamics-int.com";

        public static readonly string Validation_Dfp_MissingSecretOrCert = "Please configure either the client secret or certificate thumbprint";
        public static readonly string Validation_Dfp_CertAndSecretConfigured = "Please only configure certificate or secret authentication (not both) in the appsettings file.";

        public static readonly string SERILOG_LOG_FORMAT = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}";
        public static readonly string AZWEBAPP_LOG_LOCATION = @"D:\home\LogFiles\http\RawLogs\log.txt";
    }
}
