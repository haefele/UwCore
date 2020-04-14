using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UwCore.Scripts
{
    public static class RunHelper
    {
        private static readonly string MsBuildPath = GetMsBuildPath();
        private static string GetMsBuildPath()
        {
            var vsWhere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vswhere.exe");
            var path = Read(vsWhere, "-products * -requires Microsoft.Component.MSBuild -property installationPath").Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();

            var msBuildPaths = new string[]
            {
                Path.Combine(path, "MSBuild", "15.0", "Bin", "MSBuild.exe"),
                Path.Combine(path, "MSBuild", "Current", "Bin", "MSBuild.exe"),
            };

            foreach (var msBuildPath in msBuildPaths)
            {
                if (File.Exists(msBuildPath))
                    return msBuildPath;
            }

            throw new Exception("MSBuild.exe not found.");
        }

        public static void RunDotNet(string arguments)
        {
            Run("dotnet", arguments);
        }
        public static void RunDotNetTool(string arguments)
        {
            Run("dotnet", "tool " + arguments);
        }
        public static void RunMsBuild(string arguments)
        {
            Run(MsBuildPath, arguments);
        }
        public static void RunGit(string arguments)
        {
            Run("git", arguments, workingDirectory: UwCorePaths.SlnDirectory);
        }
        public static void RunNbgv(string arguments)
        {
            try
            {
                Run(UwCorePaths.Nbgv, arguments, workingDirectory: UwCorePaths.SlnDirectory);
            }
            catch (Exception) when (arguments == "cloud")
            {
                // No cloud environment detected
            }
            catch
            {
                throw;
            }
        }
        public static string ReadNbgv(string arguments)
        {
            return Read(UwCorePaths.Nbgv, arguments, workingDirectory: UwCorePaths.SlnDirectory);
        }

        public static void Run(string name, string args, string workingDirectory = null)
        {
            Read(name, args, workingDirectory);
        }

        public static string Read(string name, string args, string workingDirectory = null)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = name,
                    Arguments = args,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardOutput = true,
                };

                process.Start();

                var output = process.StandardOutput.ReadToEnd(); //Make sure to read the output before we WaitForExit, or the process might hang forever
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception($"The command \"{name} {args}\" failed!{Environment.NewLine}{output}");

                return output;
            }
        }
    }
}
