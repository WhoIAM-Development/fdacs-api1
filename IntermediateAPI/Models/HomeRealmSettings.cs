namespace IntermediateAPI.Models
{
    public class HomeRealmSettings
    {
        public string StorageAccount { get; set; }
        public string Table { get; set; }
        public bool DomainInfoLookupMS { get; set; }
        public int CacheTimeoutInSeconds { get; set; } = 15;
    }
}
