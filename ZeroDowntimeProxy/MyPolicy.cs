using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroDowntimeProxy
{
    public class MyPolicy
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
