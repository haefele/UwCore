using System;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static UwCore.Scripts.FileHelper;
using static UwCore.Scripts.RunHelper;

namespace UwCore.Scripts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Target("clean", () =>
            {
                GitResetFile(UwCorePaths.DirectoryBuildProps);

                DeleteDirectory(UwCorePaths.ArtifactsDirectory);
                DeleteDirectory(UwCorePaths.DotNetToolsDirectory);

                DeleteDirectory(UwCorePaths.UwCore.BinDirectory);
                DeleteDirectory(UwCorePaths.UwCore.ObjDirectory);
            });

            Target("setup-versioning", DependsOn("clean"), () =>
            {
                RunDotNetTool($"install nbgv --tool-path \"{UwCorePaths.DotNetToolsDirectory}\"");
                RunNbgv("install");
                RunNbgv("cloud");
            });

            Target("build-nuget-package", DependsOn("setup-versioning"), () =>
            {
                RunMsBuild($"\"{UwCorePaths.UwCore.CsProj}\" /p:Configuration=Release /restore /t:Pack /p:PackageOutputPath=\"{UwCorePaths.ArtifactsDirectory}\"");
            });

            Target("default", DependsOn("build-nuget-package"));

            await RunTargetsAndExitAsync(args);
        }
    }
}
