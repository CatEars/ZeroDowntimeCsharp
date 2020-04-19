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

## Start the servers

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

### Note: Duplication

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

For some applications it can be hard to duplicate a service like this. An
example of this might be an application that uses [sticky
sessions](https://stackoverflow.com/questions/10494431/sticky-and-non-sticky-sessions).
Different applications have different needs, but generally most of them need to
scale.
