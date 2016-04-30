Function Pack($root, $assemblyName)
{
    $assemblyPath = "$root\src\$assemblyName\bin\Release\$assemblyName.dll";

	$version = [System.Reflection.Assembly]::LoadFile($assemblyPath).GetName().Version;
	$versionAsString = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build);

	$nuspecPath = $root + "\nuget\" + $assemblyName + ".nuspec";				

	[xml]$content = Get-Content $nuspecPath; 
	$content.package.metadata.version = $versionAsString;
	$content.Save($nuspecPath);

	& nuget pack $nuspecPath;
}

$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..';

Pack -root $root -assemblyName "UwCore";