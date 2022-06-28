using EFTClient;
using EFTServer;
using Newtonsoft.Json;

namespace PointOfSale
{
    public delegate string CallBack(string Message);
    
    public class Program
    {
        int RetryCount = 0;
        string PurchaseID = string.Empty;
        public PurchaseStatusDetails StatusDetails;        

        public static void Main()
        {
            Program program = new Program();
            Console.WriteLine("====================== **EFTPOS** ======================");
            Console.WriteLine("");           

            while (true)
            {
                if (program.RetryCount <= 2)
                {
                    program.RunAsync().GetAwaiter().GetResult(); ;
                }
                else
                {
                    Console.WriteLine("Server is disconnected");
                    break;
                }
            }
            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

        async Task RunAsync()
        {
            try
            {
                StatusDetails = JsonConvert.DeserializeObject<PurchaseStatusDetails>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "appsettings.json"));
                EFTClientLib Client = new EFTClientLib();
                Console.WriteLine("Create New Purchase Request (Y/N) ");
                ConsoleKeyInfo result = Console.ReadKey();
                Console.WriteLine("");
                CallBack myCallBack = new CallBack(DisplayStatus);
               
                if ((result.KeyChar == 'Y') || (result.KeyChar == 'y'))
                {
                    Console.WriteLine("Enter PurchaseID :");
                    Guid ID = Guid.NewGuid();
                    Console.WriteLine(ID.ToString());
                    Console.WriteLine("Enter MerchantID :");
                    int MerchantID = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Amount :");
                    float Amount = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("");

                    // Create a new Purchase
                    Purchase purchase = new Purchase
                    {
                        ID = ID.ToString(),
                        MerchantID = MerchantID,
                        Amount = Amount,
                        Status = StatusDetails.RequestReceived
                    };

                     Client.ConnectToServer();
                     PurchaseID = await Client.CreatePurchaseAsync(purchase, DisplayStatus);
                    
                }
                else if ((result.KeyChar == 'N') || (result.KeyChar == 'n'))
                {
                    Console.WriteLine("Please Try again");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("Please Enter Valid Input (Y/N)");
                    Console.WriteLine("");
                }
            }
            catch (Exception e)
            {
                RetryCount++;
                Console.WriteLine("Trying to connect to server\r\n");
                //Console.WriteLine(e.Message);                
            }
        }

        private string DisplayStatus(string Message)
        {            
            Console.WriteLine(Message);
            return Message;
        }

    }
}
