#tool "nuget:https://api.nuget.org/v3/index.json?package=nuget.commandline&version=5.5.1"

Task("Publish-Docker-Image")
.IsDependentOn("Build-Docker-Image")
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("QUAY_TOKEN")))
//.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v"))
.Does(() => {
    var token = EnvironmentVariable("QUAY_TOKEN");
    DockerLogin(new DockerRegistryLoginSettings{
        Password = token,
        Username = EnvironmentVariable("QUAY_USER") ?? "sicario_build"
    }, "quay.io");
    DockerPush($"quay.io/project-sicario/server:{packageVersion}");
});

Task("Publish-NuGet-Packages")
.IsDependentOn("Build-NuGet-Packages")
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("NUGET_TOKEN")))
//.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v"))
.Does(() => {
    var nupkgDir = $"{artifacts}nuget";
    var nugetToken = EnvironmentVariable("NUGET_TOKEN");
    var pkgFiles = GetFiles($"{nupkgDir}/*.nupkg");
    NuGetPush(pkgFiles, new NuGetPushSettings {
      Source = "https://api.nuget.org/v3/index.json",
      ApiKey = nugetToken
    });
});

Task("Release")
.IsDependentOn("Publish")
.IsDependentOn("Publish-Docker-Image")
.IsDependentOn("Publish-NuGet-Packages");