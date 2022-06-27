using EFTServer;
using System.Net.Http.Json;

namespace EFTClient
{
    public class HTTPClientLibrary
    {
        static HttpClient client = new HttpClient();
      
        //Function to POST Purchase Request to Server
        public static async Task<int> CreatePurchaseAsync(Purchase purchase, Action<string> Callback)
        {
            string LocationURL = string.Empty;
            var response = await client.PostAsJsonAsync(
                "purchase", purchase);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            LocationURL = response.Headers.Location.ToString();
            while (true)
            {
                purchase = await GetPurchaseAsync(LocationURL);
                if (purchase.Status != "Success")
                {
                    Callback("Please Wait...Processing\n\n" + purchase.Status + "\n");
                    Thread.Sleep(2000);
                }
                else
                {
                    Callback("Transaction Completed");
                    break;
                }
            }
            
            return purchase.ID;

        }

       
        //Function to call GET method to read Request status
        static async Task<Purchase> GetPurchaseAsync(string path)
        {
            Purchase purchase = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                purchase = await response.Content.ReadAsAsync<Purchase>();
            }

            return purchase;
        }
    }
}