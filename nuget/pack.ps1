Function Pack($root, $assemblyName)
{
	#Get Assembly Path
    $assemblyPath = "$root\src\$assemblyName\bin\Release\$assemblyName.dll";

    #Get project.json
	$projectJsonPath = "$root\src\$assemblyName\project.json";
	$projectJson = Get-Content $projectJsonPath -Raw | ConvertFrom-Json;

	#Get nuspec
	$nuspecPath = $root + "\nuget\" + $assemblyName + ".nuspec";				
	$nuspec = [xml](Get-Content $nuspecPath); 

	#Update nuspec

    #Update version
	$version = [System.Reflection.Assembly]::LoadFile($assemblyPath).GetName().Version;
	$nuspec.package.metadata.version = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build);
	
    #Update dependencies
    $dependency = $nuspec.package.metadata.dependencies.dependency;

    Write-Host $dependency;

	foreach($element in $projectJson.dependencies.psobject.Properties) 
	{
        $currentDependency = $dependency.Clone();
        
        $currentDependency.id = $element.name;
        $currentDependency.version = $element.value;

        $nuspec.package.metadata.dependencies.AppendChild($currentDependency);
	}

    $nuspec.package.metadata.dependencies.RemoveChild($dependency);

	#Save nuspec
	$nuspec.Save($nuspecPath);

	#Create package
	& nuget pack $nuspecPath;
}

$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..';

Pack -root $root -assemblyName "UwCore";