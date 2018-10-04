#load "utilities.cake";

var target = Argument("target", "Default");

var slnDir = MakeAbsoluteDir("./../");
var slnPath = MakeAbsoluteFile(slnDir + "UwCore.sln");
var projectDir = MakeAbsoluteDir(slnDir + "src/UwCore/");
var projectPath = MakeAbsoluteFile(projectDir + "UwCore.csproj");
var artifactsDir = MakeAbsoluteDir("./artifacts/");
var dotnetToolsDir = MakeAbsoluteDir("./dotnettools/");

Task("SetupVersioning")
  .Does(() =>
{
    StartProcess("dotnet", "tool install nbgv --tool-path \"" + dotnetToolsDir + "\"");
    StartProcess(dotnetToolsDir + "nbgv", new ProcessSettings().WithArguments(f => f.Append("install")).UseWorkingDirectory(slnDir));
    StartProcess(dotnetToolsDir + "nbgv", new ProcessSettings().WithArguments(f => f.Append("cloud")).UseWorkingDirectory(slnDir));
});

Task("Build")
  .IsDependentOn("SetupVersioning")
  .Does(() => {
      var settings = new MSBuildSettings()
        .SetConfiguration("Release")
        .WithRestore()
        .WithTarget("Pack")
        .WithProperty("PackageOutputPath", artifactsDir);

      MSBuild(projectPath, settings);
  });

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);