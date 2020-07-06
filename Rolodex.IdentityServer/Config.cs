using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Rolodex.IdentityServer
{
    public class Config
    {
        public static IEnumerable<ApiScope> GetAllApiResources()
        {
            return new List<ApiScope>
            {
                new ApiScope("rolodexApi", "Address Api for Rolodex")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"rolodexApi"}
                }
            };
        }
    }
}
