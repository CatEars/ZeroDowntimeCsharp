# Lab 2 - Server Side Transparent Proxy

In this lab we will use the proxy to make the process of deploying a new version
transparent, performing a Blue Green Deployment. We will also think about how a
proxy might be useful if we want to test our application before "going live".

### Used Programs

* 1x GreeterClient
* 2x GreeterService
* 1x GreeterProxy

# Architecture

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab2-Architecture.PNG" />
</p>

The architecture for this example is a bit more complex, but still quite simple.
We have a client, that one is working in the exact same way as before. No
changes what so ever.

On the server side, however, we have duplicated the service and added a proxy in
front of the services. The proxy contains a circuit breaker. If the current
service is unavailable the circuit breaker will switch over to the other one. It
will try three times on the current server until the circuit breaks and the
switch is made.

# Lab

## Start The Servers

Open two powershell windows and go into the `artifacts` folder.

```powershell
> .\GreeterService.exe --urls=https://localhost:8081 --greeting=Greetings
```

In the other

```powershell
> .\GreeterService.exe --urls=https://localhost:8082 --greeting=Greetings
```

Now we have started two identical services on port `8081` and `8082`. At this
point we think about them as both being Blue deployments in their current state.

The two services we run are identical to the one we ended up with in Lab 1.

### Note: Duplication

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

For some applications it can be hard to duplicate a service like this. An
example of this might be an application that uses [sticky
sessions](https://stackoverflow.com/questions/10494431/sticky-and-non-sticky-sessions).
Another service might rely on in-memory data, which cannot easily be pushed to a
database. Different applications have different needs, but generally most of
them need to scale. If you want to deploy an application with zero downtime,
then that application generally needs to be able to run at least two instances at the same time.

## Start The Proxy

Open a powershell window and go into the `artifacts` folder.

```powershell
> .\GreeterProxy.exe
```

The proxy will transparently forward a request to one of the GreeterServices.
Unless that service is unreachable three times in a row it will use the same
service over and over. Once a service has failed three times, the proxy will
switch over to the other service.

The code for the proxy is available in the `ZeroDowntimeProxy`
[project](ZeroDowntimeProxy). The service fully mimics the interface of the
`GreeterService`. However, it always forwards the requests to either
`https://localhost:8081` or `https://localhost:8082`. This code is found in the
`Services\GreeterService.cs`. The project uses
[Polly](https://github.com/App-vNext/Polly) to create a circuit breaker policy,
which simplifies the switching between the other servers. This policy is found
in `MyPolicy.cs`.

<p align="center">
<img alt="QuestionMark" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Question.PNG" />
</p>

### Note: To Be Transparent Or Not To Be Transparent

In the current implementation the proxy will not reveal any form of exception to
the client. What are the advantages of keeping the exceptions hidden from the
client? Would it be possible to somehow make the client aware of the errors? If
the errors were available at the client, how would this change the client code?
What possibilities are opened up? From an architecture standpoint, is it best to
be transparent or not? Is there a clear answer?

## Start The Client

...


