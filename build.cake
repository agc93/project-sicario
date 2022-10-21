#load "build/helpers.cake"
#addin nuget:?package=Cake.Docker&version=1.1.2

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
// var framework = Argument("framework", "netcoreapp3.1");

///////////////////////////////////////////////////////////////////////////////
// VERSIONING
///////////////////////////////////////////////////////////////////////////////

var packageVersion = string.Empty;
#load "build/version.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/ProjectWingmanPatcher.sln");
var solution = ParseSolution(solutionPath);
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
var PackagedRuntimes = new List<string> { "centos", "ubuntu", "debian", "fedora", "rhel" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
    packageVersion = BuildVersion(fallbackVersion);
	if (FileExists("./build/.dotnet/dotnet.exe")) {
		Information("Using local install of `dotnet` SDK!");
		Context.Tools.RegisterFile("./build/.dotnet/dotnet.exe");
	}
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	foreach (var project in projects.AllProjectPaths) {
		DotNetRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	var settings = new DotNetBuildSettings {
		Configuration = configuration,
		NoIncremental = true,
		ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}")
	};
	DotNetBuild(solutionPath, settings);
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
    CreateDirectory(testResultsPath);
	if (projects.TestProjects.Any()) {

		var settings = new DotNetTestSettings {
			Configuration = configuration
		};

		foreach(var project in projects.TestProjects) {
			DotNetTest(project.Path.FullPath, settings);
		}
	}
});

Task("Post-Build")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
// 	CopyFiles(GetFiles("./build/Dockerfile*"), artifacts);
});

Task("Publish-Runtime")
	.IsDependentOn("Post-Build")
	.Does(() =>
{
	var projectDir = $"{artifacts}publish";
	CreateDirectory(projectDir);
    DotNetPublish("./src/SicarioPatch.App/SicarioPatch.App.csproj", new DotNetPublishSettings {
        OutputDirectory = projectDir + "/dotnet-any",
		Configuration = configuration
    });
    var runtimes = new[] { "linux-x64", "win-x64"};
    foreach (var runtime in runtimes) {
		var runtimeDir = $"{projectDir}/{runtime}";
		CreateDirectory(runtimeDir);
		Information("Publishing for {0} runtime", runtime);
		var settings = new DotNetPublishSettings {
			Runtime = runtime,
			Configuration = configuration,
			OutputDirectory = runtimeDir,
			PublishSingleFile = true,
			PublishTrimmed = true
		};
		DotNetPublish("./src/SicarioPatch.App/SicarioPatch.App.csproj", settings);
		CreateDirectory($"{artifacts}archive");
		Zip(runtimeDir, $"{artifacts}archive/sicario-app-{runtime}.zip");
    }
});


Task("Publish-Merger")
	.IsDependentOn("Post-Build")
	.Does(() =>
{
	var projectDir = $"{artifacts}merger";
	CreateDirectory(projectDir);
    DotNetPublish("./src/SicarioPatch.Loader/SicarioPatch.Loader.csproj", new DotNetPublishSettings {
        OutputDirectory = projectDir + "/dotnet-any",
		Configuration = configuration
    });
    var runtimes = new[] {"win-x64"};
    foreach (var runtime in runtimes) {
		var runtimeDir = $"{projectDir}/{runtime}";
		CreateDirectory(runtimeDir);
		Information("Publishing for {0} runtime", runtime);
		var settings = new DotNetPublishSettings {
			Runtime = runtime,
			Configuration = configuration,
			OutputDirectory = runtimeDir,
			SelfContained = true,
			PublishSingleFile = true,
			PublishTrimmed = true,
			IncludeNativeLibrariesForSelfExtract = true,
			ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}").Append("/p:TrimMode=Link")
		};
		DotNetPublish("./src/SicarioPatch.Loader/SicarioPatch.Loader.csproj", settings);
		CreateDirectory($"{artifacts}archive");
		Zip(runtimeDir, $"{artifacts}archive/sicario-merger-{runtime}.zip");
    }
});

Task("Build-NuGet-Packages")
	.IsDependentOn("Post-Build")
	.Does(() => 
{
	Information("Building NuGet packages");
	var nupkgDir = $"{artifacts}/nuget/";
	CreateDirectory(nupkgDir);
	var packSettings = new DotNetPackSettings
     {
         Configuration = configuration,
         OutputDirectory = nupkgDir,
		 ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}")
     };
	DotNetPack(solutionPath, packSettings);
});

Task("Build-Docker-Image")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("Publish-Runtime")
	.Does(() =>
{
	Information("Building Docker image...");
	CopyFileToDirectory("./build/Dockerfile.build", artifacts);
	var bSettings = new DockerImageBuildSettings {
        Tag = new[] { $"project-sicario/server:{packageVersion}", $"quay.io/project-sicario/server:{packageVersion}"},
        File = artifacts + "Dockerfile.build"
    };
	DockerBuild(bSettings, artifacts);
	DeleteFile(artifacts + "Dockerfile.build");
});

#load "build/publish.cake"

Task("Default")
    .IsDependentOn("Post-Build")
    .IsDependentOn("Run-Unit-Tests");

Task("Publish")
    .IsDependentOn("Publish-Runtime")
    .IsDependentOn("Publish-Merger")
	.IsDependentOn("Build-NuGet-Packages")
	.IsDependentOn("Build-Docker-Image");

RunTarget(target);