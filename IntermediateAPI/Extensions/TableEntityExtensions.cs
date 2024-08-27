using Azure.Data.Tables;
using IntermediateAPI.Models;

namespace IntermediateAPI.Extensions
{
    public static class TableEntityExtensions
    {
        public static AllowedLogins ToAllowedLogins(this TableEntity tableEntity)
        {
            var loginTypes = tableEntity.GetString(Constants.LoginTypes).Split(",");
            return new AllowedLogins()
            {
                Domain = tableEntity.RowKey,
                LoginTypes = loginTypes.Select(x => x.Trim())
            };
        }
    }
}
