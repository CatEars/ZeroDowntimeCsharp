# Lab 2 - Transparent Server Side Proxy

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
We have a client. The client is working in the exact same way as in Lab 1. No
changes.

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

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

### Note: Duplication

For some applications it can be hard to duplicate a service like this. An
example of this might be an application that uses [sticky
sessions](https://stackoverflow.com/questions/10494431/sticky-and-non-sticky-sessions).
Another service might rely on in-memory data, which cannot easily be pushed to a
database. Different applications have different needs, but most applications
that have any users need a credible answer to the question "How does it scale?".
If you want to upgrade an application with zero downtime, then that application
NEEDS to be able to run at least two instances at the same time.

## Start The Proxy

Open a powershell window and go into the `artifacts` folder.

```powershell
> .\GreeterProxy.exe --urls=https://localhost:8080
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

Open a powershell window and go into the `artifacts` folder.

```powershell
> .\GreeterClient.exe
```

Notice that the client is started in exactly the same way as in Lab 1. This is a
prerequisite for the proxy to be transparent. The output of the client should
not be distinguishable from its output in Lab 1 in any meaningful way.
Obviously, the same code runs on the client as when it was run in Lab 1.

At this point we have the whole system set up and working. We can start our Blue
Green Deployment.

<p align="center">
<img alt="QuestionMark" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Question.PNG" />
</p>

### Note: Idle Service

Right now one of the services should be completely idle. The proxy will only
send requests to one of the services. This implies that only one of the services
needs to run normally. What are the effects of running two services? Is more
energy consumed than normally? How much, compared to running just one service?
Is it negligible? Assuming we have a proxy similar to ours, is there any issue
with running only one service? Would we profit from a third service?

## Start An Upgraded Version of the Service

You have received word from your boss that you will need to change the greeting
yet again. This time you will use the short and simple greeting "Hi". However,
she has told you that this change cannot disrupt the running business. Luckily,
you have been smart in your deployment and are set for an easy task.

Start with stopping either of the servers. Try and stop the one the proxy is using
and witness how it switches over to another service. Also notice how, at the
client, the only difference is that a single request seems to take a little bit
longer. 

At this point we have the following architecture up and running:

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab2-Architecture-OneOff.PNG" />
</p>

At this point we need to start our new version of the `GreeterService` which
says "Hi" instead of "Greetings". If you shut down the server that was using
port `8081` then run the following:

```powershell
> .\GreeterService.exe --urls=https://localhost:8081 --greeting=Hi
```

else substitue `8081` for `8082`. The server should start without anything
remarkable happening. The client should continue getting "Greetings" from the
proxy, which talks to the "old" service.

At this point you should have an architecture sketch that looks like this:

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab2-Architecture-BlueGreen.PNG" />
</p>

Server 2 is now a different version than Server 1, symbolized by the Green color.

<p align="center">
<img alt="QuestionMark" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Question.PNG" />
</p>

### Note: Extra Testing

Before making the switch from the old application to the new, is it possible to
verify that the new version is working correctly? How could a proxy be built so
that it would take requests sent by you to the new version? Would it be possible
to send "some" requests to the new version? How would those requests be
selected? What if this component was part of a long chain of requests, would it
still be possible to test it before going live? Can we automate the process of
checking that the new service runs correctly?

## Shut Down the Old Service

Now you should shut down the old service, watch both the output from the proxy
and the client. What does the client notice? What does the proxy notice? How has
this changed from before?

At this point you should have an architecture sketch that looks like this:

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab2-Architecture-BlueOff.PNG" />
</p>

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

### Note: Controlled Failover

In this system we forcefully remove the old service and trust that the proxy
will eventually make the switch from sending requests to the old version to
sending them to the new version. Are there ways in which this could be handled
more gracefully? How could we verify that the old service is no longer in use by
anyone? Is the strategy different if requests to our service are long lived, say
2 hours? If we had a method for manually switching which service the proxy
targets, would there be a benefit in keeping the old version running for a
longer time before shutting it off? Is there a benefit to killing the running
process as fast as possible?

## Start Another Instance of the New Version

Even if we have failed over and use the new version we want to start a second
instance of the new version of the service so that we have a resilient service.

Open or reuse a powershell window and navigate to the `artifacts` folder.

```powershell
> .\GreeterService.exe --urls=https://localhost:8082 --greeting=Hi
```

This should start the second service and we should be running the following
architecture:

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab2-Architecture-Finished.PNG" />
</p>

At this point we have managed to migrate from our blue software version into a
green software version. We have successfully managed to update our service
without any visible downtime to the user.

## Summary

In this lab we have seen how a transparent proxy can allow us to reduce any
downtime of a service to zero. We have pondered exactly what the necessary
prerequisites for our software to use this type of deployment is.

We have investigated how a circuit breaking policy allows us to start with one
method and after multiple failures fail over to a secondary method. We have also
thought about the reason why you might want to choose between a transparent
proxy and an opaque proxy.

We have seen that stopping one of the servers is not noticable from the
perspective of the client. We have noted that with this kind of deployment it is
possible to run some tests on the new version before we actually allow users to
access it.

This lab should illustrate to you a credible way of how a deployment can be made
with zero downtime and fully convince you that a "simple" deployment lacks a lot
of valuable properties compared to the deployment done in this lab.

After having done both of these labs you should have a firm grasp of a practical
way to implement a Blue Green Deployment. You should also understand that the
most straightforward way of deploying and upgrading software might come with
several downsides, which a change in architecture could solve. It should also be
clear to you that this change in architecture is not necessarily a huge
undertaking.


