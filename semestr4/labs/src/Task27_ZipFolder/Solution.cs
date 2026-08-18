using System.IO.Compression;

namespace Task27_ZipFolder;

public static class FolderZipper
{
    public static void Zip(string sourceDirectory, string zipPath)
    {
        if (File.Exists(zipPath)) File.Delete(zipPath);
        ZipFile.CreateFromDirectory(sourceDirectory, zipPath);
    }
}
