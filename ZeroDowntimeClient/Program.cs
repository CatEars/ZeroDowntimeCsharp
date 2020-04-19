using System;
using System.Data;
using System.Linq;
using System.Threading;
using CommonProtos;
using Grpc;
using Grpc.Core;
using Grpc.Net.Client;
using Polly;

namespace ZeroDowntimeClient
{
    class Program
    {

        private static readonly string Hostname = "localhost";
        private static readonly string Port = "8080";

        static string GreetWithPolly()
        {
            string PrimaryHostname = Hostname;
            string SecondaryHostname = Hostname;

            string PrimaryPort = Port;
            string SecondaryPort = "8081";

            do
            {
                var policy = MyPolicy.IsUsingPrimary ? MyPolicy.PrimaryPolicy : MyPolicy.SecondaryPolicy;
                var hostname = MyPolicy.IsUsingPrimary ? PrimaryHostname : SecondaryHostname;
                var port = MyPolicy.IsUsingPrimary ? PrimaryPort : SecondaryPort;

                var channel = GrpcChannel.ForAddress($"https://{hostname}:{port}");
                var client = new Greeter.GreeterClient(channel);

                Console.WriteLine($"  Using polly+gRPC to access {hostname}:{port}");

                var result = policy.ExecuteAndCapture<string>(() =>
                {
                    var res = client.SayHello(new HelloRequest()
                    {
                        Name = "Polly"
                    });
                    return res.Message;
                });

                if (result.FinalException == null)
                {
                    return result.Result;
                }
                else
                {
                    Console.WriteLine($"  Tried to contact {hostname}:{port} but got an error");
                }
            } while (true);
        }

        static string Greet()
        {
            Console.WriteLine($"  Using gRPC to access {Hostname}:{Port}");
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress($"https://{Hostname}:{Port}");
            var client = new Greeter.GreeterClient(channel);
            var result = client.SayHello(new HelloRequest()
            {
                Name = "Mr. Cuddlesworth"
            });
            return result.Message;
        }

        static void Main(string[] args)
        {
            var usePolly = args.Contains("--polly");

            while (true)
            {
                Console.WriteLine($"Starting gRPC call to greeter service");
                var result = usePolly ? GreetWithPolly() : Greet();
                Console.WriteLine("  Got response: " + result);
                Console.WriteLine("  Sleeping for 3s");
                Thread.Sleep(3000);
            }
        }
    }
}
