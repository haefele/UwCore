#addin "Cake.FileHelpers"

using System.Xml.Linq;

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
	.Does(() => 
{
	var nuspec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"">
  <metadata>
    <id>UwCore</id>
    <version>$version$</version>
    <authors>Daniel Häfele</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>Makes my life with uwp projects easier.</description>
    <dependencies>
		$dependencies$
    </dependencies>
  </metadata>
  <files>
    <file src=""..\src\UwCore\bin\Release\UwCore**"" target=""lib\uap10.0"" />
  </files>
</package>";

	var dependencies = string.Join(Environment.NewLine, XDocument.Load("./../src/UwCore/UwCore.csproj")
		.Descendants()
		.Where(f => f.Name.LocalName == "PackageReference")
		.Select(f => new 
			{
				Name = f.Attribute("Include").Value,
				Version = f.Attribute("Version")?.Value ?? f.Descendants().Where(d => d.Name.LocalName == "Version").FirstOrDefault()?.Value
			})
		.Select(f => $"<dependency id=\"{f.Name}\" version=\"{f.Version}\" />"));

	FileWriteText("./UwCore.nuspec", nuspec.Replace("$version$", versionNumber).Replace("$dependencies$", dependencies));
	
	var settings = new NuGetPackSettings
	{
		BasePath = ".",
		OutputDirectory = "./../artifacts"
	};
	NuGetPack("./UwCore.nuspec", settings);
});

Task("UploadArtifacts")
	.IsDependentOn("CreateNuGetPackage")
	.Does(() => 
{	
	var nugetPackagePath = string.Format("./../artifacts/UwCore.{0}.nupkg", versionNumber);
	BuildSystem.AppVeyor.UploadArtifact("./../artifacts/UwCore.nupkg");
});

Task("Default")
    .IsDependentOn("UploadArtifacts");

RunTarget(target);