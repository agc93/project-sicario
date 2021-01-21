#load "build/helpers.cake"
#addin nuget:?package=Cake.Docker

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
		DotNetCoreRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	var settings = new DotNetCoreBuildSettings {
		Configuration = configuration,
		NoIncremental = true,
		ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}")
	};
	DotNetCoreBuild(solutionPath, settings);
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
    CreateDirectory(testResultsPath);
	if (projects.TestProjects.Any()) {

		var settings = new DotNetCoreTestSettings {
			Configuration = configuration
		};

		foreach(var project in projects.TestProjects) {
			DotNetCoreTest(project.Path.FullPath, settings);
		}
	}
});

Task("Post-Build")
	.IsDependentOn("Build")
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
    DotNetCorePublish("./src/SicarioPatch.App/SicarioPatch.App.csproj", new DotNetCorePublishSettings {
        OutputDirectory = projectDir + "/dotnet-any",
		Configuration = configuration
    });
    var runtimes = new[] { "linux-x64", "win-x64"};
    foreach (var runtime in runtimes) {
		var runtimeDir = $"{projectDir}/{runtime}";
		CreateDirectory(runtimeDir);
		Information("Publishing for {0} runtime", runtime);
		var settings = new DotNetCorePublishSettings {
			Runtime = runtime,
			Configuration = configuration,
			OutputDirectory = runtimeDir,
			PublishSingleFile = true,
			PublishTrimmed = true
		};
		DotNetCorePublish("./src/SicarioPatch.App/SicarioPatch.App.csproj", settings);
		CreateDirectory($"{artifacts}archive");
		Zip(runtimeDir, $"{artifacts}archive/sicario-app-{runtime}.zip");
    }
});

Task("Build-Linux-Packages")
	.IsDependentOn("Publish-Runtime")
	.WithCriteria(IsRunningOnUnix())
	.Does(() => 
{
	Information("Building packages in new container");
	CreateDirectory($"{artifacts}/packages/");
	foreach(var project in projects.SourceProjects.Where(p => p.Name == "ModMetaRelay")) {
        var runtime = "linux-x64";
        var sourceDir = MakeAbsolute(Directory($"{artifacts}publish/server/{runtime}"));
        var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
		foreach (var package in GetPackageFormats()) {
			var runSettings = new DockerContainerRunSettings {
				Name = $"docker-fpm-{(runtime.Replace(".", "-"))}",
				Volume = new[] { 
					$"{sourceDir}:/src:ro", 
					$"{packageDir}:/out:rw",
					$"{MakeAbsolute(Directory("./scripts/"))}:/scripts:ro",
				},
				Workdir = "/out",
				Rm = true,
				//User = "1000"
			};
			var opts = "-s dir -a x86_64 --force -m \"Alistair Chapman <alistair@agchapman.com>\" -n modmeta-relay --after-install /scripts/post-install.sh --before-remove /scripts/pre-remove.sh";
			DockerRun(runSettings, "tenzer/fpm", $"{opts} -v {packageVersion} --iteration {package.Key} {package.Value} /src/=/usr/lib/modmeta-relay/");
		}
	}
});

Task("Build-NuGet-Packages")
	.IsDependentOn("Post-Build")
	.Does(() => 
{
	Information("Building NuGet packages");
	var nupkgDir = $"{artifacts}/nuget/";
	CreateDirectory(nupkgDir);
	var packSettings = new DotNetCorePackSettings
     {
         Configuration = configuration,
         OutputDirectory = nupkgDir,
		 ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}")
     };
	DotNetCorePack(solutionPath, packSettings);
});

Task("Build-Docker-Image")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("Build-Linux-Packages")
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
    .IsDependentOn("Post-Build");

Task("Publish")
	.IsDependentOn("Build-Linux-Packages")
	.IsDependentOn("Build-NuGet-Packages")
	.IsDependentOn("Build-Docker-Image");

RunTarget(target);