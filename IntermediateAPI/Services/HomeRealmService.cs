using Azure.Data.Tables;
using IntermediateAPI.Extensions;
using IntermediateAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace IntermediateAPI.Services
{
    public class HomeRealmService
    {
        private readonly HttpClient httpClient;
        private IMemoryCache _cache;
        private readonly ILogger<HomeRealmService> logger;

        private const string _homeRealmCacheKey = "homeRealmList";
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private TableClient tableClient;
        HomeRealmSettings config;
        public HomeRealmService(
            ILogger<HomeRealmService> logger,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IOptions<HomeRealmSettings> homeRealmConfig)
        {
            config = homeRealmConfig.Value;
            tableClient = new TableClient(config.StorageAccount, config.Table);
            httpClient = httpClientFactory.CreateClient();
            this.logger = logger;
            this._cache = cache;
        }
        public async Task<AllowedLogins> GetDomain(string key)
        {
            logger.LogInformation("Home Realm request received for key {key}", key);
            var tableCreateResult = await tableClient.CreateIfNotExistsAsync();
            if (tableCreateResult != null)
            {
                var te = new TableEntity("", "default")
                {
                    { "LoginType", "local" }
                };
                await tableClient.AddEntityAsync(te);
            }

            var homerealmList = await GetAllHomeRealm();
            var userDomains = homerealmList.FirstOrDefault(x => x.Domain == key) ?? homerealmList.FirstOrDefault(x => x.Domain == Constants.Default);

            if (userDomains == null)
            {
                string msg = $"Table improperly configured: could not find login types for domain {key}";
                logger.LogError(msg);
                throw new Exception(msg);
            }

            if (config.DomainInfoLookupMS && userDomains.HasLogin(Constants.AAD))
            {
                logger.LogInformation("AAD domain found.  Checking validity with Microsoft services...");
                bool validDomain = await IsDomainValidInAzureDomainInfoLookup(key);
                if (!validDomain)
                {
                    logger.LogWarning("Purning invalid domain {key} from home realm results", key);
                    userDomains.RemoveLoginType(Constants.AAD);
                }
                else
                {
                    logger.LogInformation("AAD domain name is valid");
                }
            }

            logger.LogInformation("Home realm success: {@userDomains}", userDomains);

            return userDomains;

        }
        private async Task<bool> IsDomainValidInAzureDomainInfoLookup(string id)
        {
            var response = await httpClient.GetAsync($"https://login.microsoftonline.com/common/userrealm/?user=@{id}&checkForMicrosoftAccount=true&api-version=2.1");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var domainInfo = JsonConvert.DeserializeObject<AzureDomainInfo>(data);

                if (domainInfo != null && (domainInfo.NameSpaceType?.ToLower() == "federated" || domainInfo.NameSpaceType?.ToLower() == "managed"))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<IEnumerable<AllowedLogins>> GetAllHomeRealm()
        {
            logger.Log(LogLevel.Information, "Trying to fetch the list of data from cache.");

            if (_cache.TryGetValue(_homeRealmCacheKey, out IEnumerable<AllowedLogins> homerealmList))
            {
                logger.Log(LogLevel.Information, "Data found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();

                    if (_cache.TryGetValue(_homeRealmCacheKey, out homerealmList))
                    {
                        logger.Log(LogLevel.Information, "Data found in cache.");
                    }
                    else
                    {
                        logger.Log(LogLevel.Information, "Data not found in cache. Fetching from database.");

                        var result = tableClient.Query<TableEntity>();

                        if (result == null)
                        {
                            string msg = "Table improperly configured: could not find table";
                            logger.LogError(msg);
                            throw new Exception(msg);
                        }

                        homerealmList = result.Select(x => x.ToAllowedLogins()).ToList();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(config.CacheTimeoutInSeconds))
                            .SetPriority(CacheItemPriority.Normal);

                        _cache.Set(_homeRealmCacheKey, homerealmList, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return homerealmList;
        }
    }

}
