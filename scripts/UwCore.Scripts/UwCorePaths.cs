using System.IO;

namespace UwCore.Scripts
{
    public static class UwCorePaths
    {
        public static string SlnDirectory
        {
            get
            {
                var currentFolder = new DirectoryInfo(Path.GetDirectoryName(typeof(Program).Assembly.Location));
                return currentFolder.Parent.Parent.Parent.Parent.Parent.FullName;
            }
        }
        public static string ArtifactsDirectory => Path.Combine(SlnDirectory, "artifacts");
        public static string DotNetToolsDirectory => Path.Combine(SlnDirectory, "dotnettools");
        public static string Nbgv => Path.Combine(DotNetToolsDirectory, "nbgv");
        public static string DirectoryBuildProps => Path.Combine(SlnDirectory, "Directory.Build.props");

        public static class UwCore
        {
            public static string CsProj => Path.Combine(SlnDirectory, "src", "UwCore", "UwCore.csproj");
            public static string BinDirectory => Path.Combine(SlnDirectory, "src", "UwCore", "bin");
            public static string ObjDirectory => Path.Combine(SlnDirectory, "src", "UwCore", "obj");
        }
    }
}
