namespace IntermediateAPI.Models
{
    public class AzureAdSettings
    {
        public string Instance { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public bool AllowWebApiToBeAuthorizedByACL { get; set; } 
    }
}
