using Grpc.Net.Client;

namespace NetworkSpaceClient
{
    class Program
    {
        static void Main()
        {
            var client = new Log.LogClient(GrpcChannel.ForAddress("http://localhost:5211"));

            while (true)
            {
                Console.WriteLine("\nEnter number:");
                var number = int.Parse(Console.ReadLine());

                var logovi = client.GetAllLogs(new EmptyMessage());

            }
        }
    }
}