using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!disco.IsError)
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api2"
                });
                if (!tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Json);
                    Console.WriteLine("\n\n");

                    var apiClient = new HttpClient();
                    apiClient.SetBearerToken(tokenResponse.AccessToken);
                    var response = await apiClient.GetAsync("https://localhost:6001/identity");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(JArray.Parse(content));
                    }
                }
            }
        }
        static private async Task<DiscoveryDocumentResponse> GetDiscoveryDoc(HttpClient client)
        {
            return await client.GetDiscoveryDocumentAsync("https://localhost:5001");
        }
        static private async Task GetToken(HttpClient client, DiscoveryDocumentResponse disco)
        {
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
        }
    }
}
