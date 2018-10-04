private string MakeAbsoluteDir(string directory) 
{
    return MakeAbsolute(Directory(directory)).FullPath + "/";
}
private string MakeAbsoluteFile(string file) 
{
    return MakeAbsolute(File(file)).FullPath;
}