using Google.Protobuf.WellKnownTypes;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroDowntimeClient
{
    public static class MyPolicy
    {
        public static bool IsUsingPrimary = true;
        public static Policy PrimaryPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    3,
                    TimeSpan.FromSeconds(10),
                    (_, __) => IsUsingPrimary = !IsUsingPrimary,
                    () => { });

        public static Policy SecondaryPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    3,
                    TimeSpan.FromSeconds(10),
                    (_, __) => IsUsingPrimary = !IsUsingPrimary,
                    () => { });
    }
}
