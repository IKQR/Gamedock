using System.IO.Compression;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;

namespace GameDock.Server.Application.Helpers;

public static class ArchiveHelper
{
    public static async ValueTask<Stream> ZipToTarAsync(Stream sourceStream, bool dispose = false)
    {
        ArgumentNullException.ThrowIfNull(sourceStream);

        var resultStream = new MemoryStream();
        using var tarWriter = WriterFactory.Open(resultStream, ArchiveType.Tar,
            new TarWriterOptions(CompressionType.None, true));
        
        using var zipArchive = new ZipArchive(sourceStream, ZipArchiveMode.Read);
        
        foreach (var entry in zipArchive.Entries)
        {
            await using var entryStream = entry.Open();
            var tempEntryStream = new MemoryStream();
            await entryStream.CopyToAsync(tempEntryStream);
            tempEntryStream.Position = 0;
            tarWriter.Write(entry.Name, tempEntryStream);
        }

        if (dispose)
        {
            await sourceStream.DisposeAsync();
        }

        resultStream.Position = 0;

        return resultStream;
    }
}