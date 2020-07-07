using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Rolodex.ConsoleClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            DiscoveryDocumentResponse disco;
            TokenResponse tokenResponse;

            using (var client = new HttpClient())
            {
                // Get Discovery Endpoint data
                disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            }

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadLine();
                return;
            }

            using (var client = new HttpClient())
            {
                // Get a Token using the TokenEndpoint, ClientId & Secret
                tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret"
                });
            }

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadLine();
                return;
            }

            // Write token to console
            Console.WriteLine("Access Token \n\n");
            Console.WriteLine(tokenResponse.AccessToken);

            // Create an address
            using (var client = new HttpClient())
            {
                // Set Bearer Token
                client.SetBearerToken(tokenResponse.AccessToken);

                // Create Json Object
                var address = new StringContent(
                    JsonConvert.SerializeObject(
                        new
                        {
                            Name = Guid.NewGuid(), 
                            Street = Guid.NewGuid(), 
                            Town = Guid.NewGuid(), 
                            Postcode = Guid.NewGuid()
                        }), 
                    Encoding.UTF8, "application/json");

                var createAddressResponse = await client.PostAsync(
                    "http://localhost:5001/api/addresses", address);
                
                if (!createAddressResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(createAddressResponse.StatusCode);
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Address Created");
            }

            // Get all Addresses
            using (var client = new HttpClient())
            {
                // Set Bearer Token
                client.SetBearerToken(tokenResponse.AccessToken);

                var getAddressesResponse = await client.GetAsync("http://localhost:5001/api/addresses");

                if (!getAddressesResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(getAddressesResponse.StatusCode);
                    Console.ReadLine();
                    return;
                }

                var content = await getAddressesResponse.Content.ReadAsStringAsync();

                Console.WriteLine(content);

            }

            Console.ReadLine();
        }

    }
}

