using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using CommonProtos;
using Grpc.Net.Client;

namespace ZeroDowntimeProxy
{
    public class GreeterService : Greeter.GreeterBase
    {

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                string PrimaryHostname = "localhost";
                string SecondaryHostname = "localhost";

                string PrimaryPort = "8082";
                string SecondaryPort = "8081";

                do
                {
                    var policy = MyPolicy.IsUsingPrimary ? MyPolicy.PrimaryPolicy : MyPolicy.SecondaryPolicy;
                    var hostname = MyPolicy.IsUsingPrimary ? PrimaryHostname : SecondaryHostname;
                    var port = MyPolicy.IsUsingPrimary ? PrimaryPort : SecondaryPort;

                    var channel = GrpcChannel.ForAddress($"https://{hostname}:{port}");
                    var client = new Greeter.GreeterClient(channel);

                    Console.WriteLine($"  Using polly+gRPC to access {hostname}:{port}");

                    var result = policy.ExecuteAndCapture<HelloReply>(() =>
                    {
                        var res = client.SayHello(request);
                        return res;
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
            });
        }
    }
}
