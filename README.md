# ZeroDowntimeCsharp

An example of how to do a zero downtime upgrade with a service written in .NET
Core.

For this example I use the concept of a [Blue Green
Deployment](https://martinfowler.com/bliki/BlueGreenDeployment.html),
popularized by Dave Farley and Jez Humble in their book [Continuous
Delivery](https://www.goodreads.com/book/show/8686650-continuous-delivery).

I show two different ways to achieve a Blue Green Deployment. One with
client-side failover and one with server-side transparent proxying. In both cases I use
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
> .\build.ps1 -t Publish
```

After running you will find the build artifacts in the `artifacts` directory.

# Running the example

There are three components built as part of this example.

### GreeterService

The actual service implementation for our `Greeter` service. If you create a new
gRPC service you will essentially end up with this one. I have made a small
change in it which allows a user to pass `--greeting=abcd` to the executable,
this will change the message that is returned from `Hello XYZ` to `abcd XYZ`.

Runs like this:

```powershell
> .\GreeterService.exe --urls=https://localhost:8080 --greeting=Hiya
```

### GreeterClient

The client which accesses the service. Will request the service every 3 seconds.
Can be controlled if it should use the "dumb" client or "smart" client. By
adding `--polly` to the command line.

Primary target for gRPC is `https://localhost:8080` and secondary target is
`https://localhost:8081`. The normal "dumb" client will go for
`https://localhost:8080`, while the "smart" Polly client will start with
`https://localhost:8080` and then switch to `https://localhost:8081`.

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
