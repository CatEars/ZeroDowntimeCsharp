# Lab 1 - Basic Service

In this lab we will create a basic service with a single client and a single
service and experience the pain points of the most basic style of deployment.

### Used Programs

* 1x GreeterClient
* 1x GreeterService

# Architecture

<p align="center">
<img alt="Architecture sketch" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Lab1-Architecture.PNG" />
</p>

The architecture for this example is extremly simple. We have a client and we
have a server and that's about it.

# Lab

## Start the server

Open a powershell window and go into the `artifacts` folder.


```powershell
> .\GreeterService.exe --urls=https://localhost:8080 --greeting=Hiya
```

This should start a server on the local computer available at port `8080`. The
code for this is available in the `ZeroDowntimeExample`
[project](ZeroDowntimeExample). The most interesting files are `Program.cs` and
`Services\GreeterService.cs`. The `--greeting=Hiya` configures our service to
respond to any request with "Hiya" instead of "Hello".

## Start the client

Open a second powershell window (I recommend using [Windows
Terminal](https://github.com/microsoft/terminal) as it has tabs and can run both
Bash in WSL and PowerShell). Navigate to `artifacts`.

```powershell
> .\GreeterClient.exe
```

This will start the `GreeterClient` which will continuously send requests to the
`GreeterService` every 3 seconds.

The code for the client can be found in the `ZeroDowntimeClient`
[project](ZeroDowntimeClient). The most interesing file for this lab is the
`Program.cs` tab. The only two methods used are `Main()` and `Greet()`.

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

### Note: Deployment

At this point we can see that we have deployed an application and that it is
working fine. As long as absolutely positively nothing happens to the
application, the computer, or the network we can rest assured that nothing bad
will happen. Ask any systems administrator and they will guarantee you that one
(or all) of the things I just listed are subject to change over time.

Software deployments are much more than just the software that is deployed. The
deployment usually lives on a computer of some sort. Usually it has an operating
system, which is subject to change. The deployment relies on hardware, which is
also subject to change. It is also quite often connected to the network, which
changes surprisingly often.

## Redeploy the server with a configuration change

It has been decided that the greeting "Hiya" is not as professional as we want
it to be. We need to redeploy our service with a changed configuration.

To do this, stop the currently running server and rerun it with

```powershell
> .\GreeterService.exe --urls=https://localhost:8080 --greeting=Greetings
```

Can you do it without the original program crashing? What happens if you do not
upgrade it within 30 seconds?

<p align="center">
<img alt="QuestionMark" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Question.PNG" />
</p>

### Note: Real World Impact

The service in this example is quite simple and not something that is essential
to anyone but us. However, imagine that this was software that was crucial to
either a business or a societal function. How would the process, as a whole, be
changed in order to coordinate updating the service? Would you need to contact
someone to notify them of the upgrade? How would you schedule that upgrade? How
are dependent systems affected by an upgrade like this?

If this is a medical system, what could the consequences be if the service takes
longer than expected to get up and running? What if it was a crucial piece of
software for the national defense? Or the police? If something goes wrong, how
can that be effectively communicated to the users of the system?

<p align="center">
<img alt="Warning" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Warning.PNG" />
</p>

### Note: Reverting To The Old Version

Assume we have a much larger application, which takes more time to configure. We
have tried our best configuring the upgraded version of the app, but we know
that we will not be able to complete the upgrade on time. How easy or hard would
it be to revert to the old version? What if we have already started the upgraded
version and it has been running for a day, but found out it was poorly
configured, how would we fix the configuration with minimal disruption? How will
the end users be affected by the change in configuration? Does the change need
to be coordinated?

<p align="center">
<img alt="QuestionMark" src="https://raw.githubusercontent.com/CatEars/ZeroDowntimeCsharp/master/Pictures/Question.PNG" />
</p>

### Note: Testing The New Version

Is there a way to test that the application works like it should before we
deploy it? If we run the application on one port and try it, are we guaranteed
that it will work once we reconfigure it to replace the old version? What if
configuring the "test version" is much harder than changing a single line?

# Summary

In this lab we have seen how the most simple deployment process can cause issues
with dependent services. We have experienced how upgrading a service without
disruption can for some applications be an impossibility. 

We have reflected over the ways a "simple" deployment might cost more overall
time because of the coordination effort involved in managing dependent users and
services. We have also reflected over how to properly test a new version and how
that we might only achieve "best effort testing" if the old version of the
application is running.

This lab should convince you that there are better and more flexible ways to
deploy than using a "simple" deployment strategy.

In the next lab we will use a server side transparent proxy in order to perform
a Blue Green Deployment. This will allow us to upgrade with zero downtime. We
will also read about an example of how we could test our service before it is
brought online.

Go to [Lab2.md here](./Lab2.md)
