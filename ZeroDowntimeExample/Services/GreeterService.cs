using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using CommonProtos;

namespace ZeroDowntimeExample
{
    public class GreeterService : Greeter.GreeterBase
    {
        public static string GreetingPhrase = "Hello";

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = GreetingPhrase + " " + request.Name
            });
        }
    }
}
