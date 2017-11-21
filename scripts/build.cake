#addin "Cake.FileHelpers"

var target = Argument("target", "Default");
var buildInAppveyor = bool.Parse(EnvironmentVariable("APPVEYOR") ?? "False");
var manualBuild = bool.Parse(EnvironmentVariable("APPVEYOR_FORCED_BUILD") ?? "False");
var isNotForPullRequest = string.IsNullOrWhiteSpace(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
var slnPath = "./../UwCore.sln";
var versionNumber = (EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "1.0.0.0").Split('-')[0];

Task("CleanFolders")
    .Does(() => 
{
    var paths = new string[] 
    {
        "./../src/UwCore/",
        "./../src/UwCoreTest/"
    };

    foreach (var path in paths)
    {
        CleanDirectory(path + "bin");
        CleanDirectory(path + "obj");
    }
});

Task("NuGetRestore")
    .IsDependentOn("CleanFolders")
    .Does(() => 
{
	var settings = new MSBuildSettings().WithTarget("restore");
	MSBuild(slnPath, settings);
});

Task("Build")
    .IsDependentOn("NuGetRestore")
    .Does(() => 
{
    MSBuildSettings settings;
    if (buildInAppveyor && manualBuild && isNotForPullRequest)
    {
        settings = new MSBuildSettings 
        {
            Configuration = "Release",
			MSBuildPlatform = MSBuildPlatform.x86,
			ToolVersion = MSBuildToolVersion.VS2017,
        };
    }
    else
    {
        settings = new MSBuildSettings 
        {
            Configuration = "Debug",
			MSBuildPlatform = MSBuildPlatform.x86,
			ToolVersion = MSBuildToolVersion.VS2017,
        };
    }

    MSBuild(slnPath, settings);
});

Task("CreateNuGetPackage")
	.IsDependentOn("Build")
	.WithCriteria(() => buildInAppveyor && manualBuild && isNotForPullRequest)
	.Does(() => 
{
	//TODO: Add versionnumber to UwCore.nuspec
	//TODO: Add dependencies to UwCore.nuspec
	
	var settings = new NuGetPackSettings
	{
		BasePath = ".",
		OutputDirectory = "./../artifacts"
	};
	NuGetPack("./UwCore.nuspec", settings);
});

Task("UploadArtifacts")
	.IsDependentOn("CreateNuGetPackage")	
    .WithCriteria(() => buildInAppveyor && manualBuild && isNotForPullRequest)
	.Does(() => 
{	
	var nugetPackagePath = string.Format("./../artifacts/UwCore.{0}.nupkg", versionNumber);
	BuildSystem.AppVeyor.UploadArtifact("./../artifacts/UwCore.nupkg");
});

Task("Default")
    .IsDependentOn("UploadArtifacts");

RunTarget(target);