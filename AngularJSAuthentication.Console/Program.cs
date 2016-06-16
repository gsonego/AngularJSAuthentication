using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AngularJSAuthentication.ConsoleApp
{
    class Program
    {
        private static string _baseAddress;

        public Program()
        {
            _baseAddress = "http://localhost:26000/";
        }

        static void Main()
        {
            try
            {
                Token token = GetToken();

                ListOrders(token).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }

        static Token GetToken()
        {
            var tokenModel = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", "gsonego"},
                {"password", "123456"},
                {"client_id", "consoleApp"},
                {"client_secret", "abc@123"}
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);

                HttpResponseMessage response = client.PostAsync("token",
                    new FormUrlEncodedContent(tokenModel)).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Get token info and bind into Token object.           
                    var token = response.Content.ReadAsAsync<Token>(new[]
                    {
                        new JsonMediaTypeFormatter()
                    }).Result;

                    return token;
                }

                // Get error info and bind into TokenError object.
                // Doesn't have to be a separate class but shown for simplicity.
                var errorToken = response.Content.ReadAsAsync<TokenError>(new[]
                {
                    new JsonMediaTypeFormatter()
                }).Result;

                throw new Exception("Não foi possível obter o token de acesso. " + errorToken.Error + " - " + errorToken.Message);
            }
        }

        static async Task ListOrders(Token token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                HttpResponseMessage response = await client.GetAsync("api/orders");

                if (response.IsSuccessStatusCode)
                {
                    List<Order> orders = await response.Content.ReadAsAsync<List<Order>>();

                    foreach (var order in orders)
                    {
                        Console.WriteLine("{0}\t${1}\t{2}\t{3}", order.OrderId, order.CustomerName, order.IsShipped,
                            order.ShipperCity);
                    }
                }
                else
                {
                    Console.WriteLine("Status: {0}, Message: {1}", response.StatusCode, response.ReasonPhrase);
                }
            }
        }
    }
}
