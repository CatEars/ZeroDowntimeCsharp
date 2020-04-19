# ZeroDowntimeCsharp

An example of how to do a zero downtime upgrade with a service written in .NET
Core.

For this example I use the concept of a [Blue Green
Deployment](https://martinfowler.com/bliki/BlueGreenDeployment.html),
popularized by Dave Farley and Jez Humble in their book [Continuous
Delivery](https://www.goodreads.com/book/show/8686650-continuous-delivery).

I show two different ways to achieve a Blue Green Deployment. One with
client-side failover and one with server-side proxying. In both cases I use
[Polly](https://github.com/App-vNext/Polly) to simplify the [circuit
breaking](https://martinfowler.com/bliki/CircuitBreaker.html) process.

The application uses [Cake](https://cakebuild.net/) for process automation.
Don't worry, it bootstraps itself and all the necessary commands will be
available in this README. You just need basic command line skills with
Powershell and [.NET Core](https://dotnet.microsoft.com/download) installed.

# Building

```powershell
> git clone git@github.com:CatEars/ZeroDowntimeCsharp.git
...
> cd ZeroDowntimeCsharp
> .\build.ps1 -t Build
```
