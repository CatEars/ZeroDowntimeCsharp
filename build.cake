var t = Argument("t", "Hello");
var target = Argument("target", t);
var configuration = Argument("configuration", "Release");

Task("Hello")
    .Does(() => {
        Information("#################################");
        Information("# Zero Downtime C sharp Example #");
        Information("#################################");
    });

Task("Nuget")
    .Does(() => {
        NuGetRestore("./ZeroDowntimeExample.sln");
    });

Task("Build")
    .IsDependentOn("Hello")
    .IsDependentOn("Nuget")
    .Does(() => {
        DotNetCoreBuild("./ZeroDowntimeExample.sln");
    });

Task("Publish")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCorePublish("./ZeroDowntimeExample.sln", new DotNetCorePublishSettings
                          {
                              Configuration = "Release",
                              OutputDirectory = "./artifacts"
                          });
    });

RunTarget(target);
