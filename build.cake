#addin nuget:?package=Cake.FileHelpers&version=3.2.1
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
        Information("Cleaning artifact output folder.");
        DeleteFiles("./artifacts/**/*");

        Information("Publishing client");
        DotNetCorePublish("./ZeroDowntimeClient/ZeroDowntimeClient.csproj",
                          new DotNetCorePublishSettings
                          {
                              PublishSingleFile = true,
                              Runtime = "win-x64",
                              Framework = "netcoreapp3.1",
                              Configuration = configuration,
                              OutputDirectory = "./artifacts/Client"
                          });

        Information("Publishing proxy");
        DotNetCorePublish("./ZeroDowntimeProxy/ZeroDowntimeProxy.csproj",
                          new DotNetCorePublishSettings
                          {
                              PublishSingleFile = true,
                              Runtime = "win-x64",
                              Framework = "netcoreapp3.1",
                              Configuration = configuration,
                              OutputDirectory = "./artifacts/Proxy"
                          });

        Information("Publishing Service");
        DotNetCorePublish("./ZeroDowntimeExample/ZeroDowntimeExample.csproj",
                          new DotNetCorePublishSettings
                          {
                              PublishSingleFile = true,
                              Runtime = "win-x64",
                              Framework = "netcoreapp3.1",
                              Configuration = configuration,
                              OutputDirectory = "./artifacts/Service"
                          });

        CopyFile("./artifacts/Client/GreeterClient.exe", "./artifacts/GreeterClient.exe");
        CopyFile("./artifacts/Proxy/GreeterProxy.exe", "./artifacts/GreeterProxy.exe");
        CopyFile("./artifacts/Service/GreeterService.exe", "./artifacts/GreeterService.exe");
    });


public string ReadVersionFromFile(string fpath) {
    if (FileExists(fpath)) {
        return FileReadText(fpath).Trim();
    }
    return null;
}

Task("Zip-Release-Assets")
    .Does(() => {
        var version = ReadVersionFromFile("./version.txt") ??
            EnvironmentVariable("LAB_VERSION") ??
            "0.0.1";
        Information($"Zipping with version: {version}");
        DeleteFiles("./artifacts/**/*.pdb");

        Information("Zipping client");
        Zip("./artifacts/Client", $"./Release-{version}-Client.zip");
        Information("Zipping proxy");
        Zip("./artifacts/Proxy", $"./Release-{version}-Proxy.zip");
        Information("Zipping service");
        Zip("./artifacts/Service", $"./Release-{version}-Service.zip");
    });


Task("Set-Version-CI")
    .Does(() => {
        var version = EnvironmentVariable("VERSION");
        FileWriteText("./version.txt", version);
    });

RunTarget(target);
