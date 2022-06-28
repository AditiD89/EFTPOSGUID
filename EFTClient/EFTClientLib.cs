using EFTServer;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EFTClient
{
    public class EFTClientLib
    {

        HttpClient client = new HttpClient();
        public ServerClientTimerDetails TimerDetails;
        public PurchaseStatusDetails StatusDetails;
        public void ConnectToServer()
        {
            try
            {
                StatusDetails = JsonConvert.DeserializeObject<PurchaseStatusDetails>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json"));
                TimerDetails = JsonConvert.DeserializeObject<ServerClientTimerDetails>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json"));

                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
               
            }catch (Exception ex)
            {
               //Error
            }
            
        }
         
        //Function to POST Purchase Request to Server
        public async Task<string> CreatePurchaseAsync(Purchase purchase, Func<string, string> Callback)
        {
            
            string LocationURL = string.Empty;
            var response = await client.PostAsJsonAsync(
                "Purchase", purchase);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            LocationURL = response.Headers.Location.ToString();

            while (true)
            {
                purchase = await GetPurchaseAsync(LocationURL);
                if (purchase.Status != StatusDetails.Success)
                {
                    Callback(purchase.Status + "\r\n" + "Please Wait...Processing\r\n");
                    Thread.Sleep(TimerDetails.ClientTimer);
                }
                else
                {
                    Callback("Success\r\nTransaction Completed\r\n");
                    break;
                }
            }
            return purchase.ID;

        }

        //Function to call GET method to read Request status
        public async Task<Purchase> GetPurchaseAsync(string path)
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