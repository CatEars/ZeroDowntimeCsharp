# ZeroDowntimeCsharp

An example of how to do a zero downtime upgrade with a service written in .NET
Core.

For this example I use the concept of a [Blue Green
Deployment](https://martinfowler.com/bliki/BlueGreenDeployment.html),
popularized by Dave Farley and Jez Humble in their book [Continuous
Delivery](https://www.goodreads.com/book/show/8686650-continuous-delivery).

I show how to achieve a zero downtime upgrade with Blue Green Deployment. It
uses a server-side transparent proxy. I also use
[Polly](https://github.com/App-vNext/Polly) to simplify the [circuit
breaking](https://martinfowler.com/bliki/CircuitBreaker.html) process.

The application uses [Cake](https://cakebuild.net/) for process automation.
Don't worry, it bootstraps itself and all the necessary commands will be
available in this README. You just need basic command line skills with
Powershell and [.NET Core](https://dotnet.microsoft.com/download) installed.

If you don't wanna build the application itself you can compiled .exe files from
the [Github release page](https://github.com/CatEars/ZeroDowntimeCsharp/releases).

# Building

```powershell
> git clone git@github.com:CatEars/ZeroDowntimeCsharp.git
...
> cd ZeroDowntimeCsharp
> .\build.ps1 -t Publish
```

After running you will find the build artifacts in the `artifacts` directory.

# Running the example

There are three components built as part of this example. All of them use
[Kestrel](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.1),
which will ask you to install a root certificate for localhost which is
necessary for you to be able to run these examples.

### GreeterService

The actual service implementation for the typical `Greeter` service. This is a
service very similar to the one created by `dotnet` when you create a new gRPC
service. I have made a small change from the original template which allows a
user to pass `--greeting=abcd` to the executable, this will change the message
that is returned from `Hello XYZ` to `abcd XYZ`.

Runs like this:

```powershell
> .\GreeterService.exe --urls=https://localhost:8080 --greeting=Hiya
```

### GreeterClient

The client which accesses the service. Will request the service every 3 seconds.
The client can optionally use Polly for a fault tolerant client, however none of
the labs utilize this.

The primary url is `https://localhost:8080` and the secondary url is
`https://localhost:8081`. The normal "dumb" client will go for
`https://localhost:8080`, while the "smart" Polly client will start with
`https://localhost:8080` and then switch to `https://localhost:8081` (and back
and forth).

Runs like this:

```powershell
> .\GreeterClient.exe [--polly]
```

### GreeterProxy

A proxy for the `GreeterService`. It has the exact same interface as
`GreeterService` but forwards everything to `https://localhost:8081` and
`https://localhost:8082`. It does so in the exact same way that the "smart"
client talks.

Runs like this:

```powershell
> .\GreeterProxy --urls=https://localhost:8080
```

# Lab 1 - Basic Service

In this lab we learn what the service looks like and we will see the
consequences of having the simplest and most naive deployment strategy.

[Lab 1](./Lab1.md)

# Lab 2 - Transparent Server Side Proxy

In this lab we learn how we can deploy a transparent proxy that will allow us to
do a Blue Green Deployment. After setting up the environment we will do a Blue
Green Deployment and see the effects from that.

[Lab 2](./Lab2.md)

