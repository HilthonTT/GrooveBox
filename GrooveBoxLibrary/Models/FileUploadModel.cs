namespace GrooveBoxLibrary.Models;
public class FileUploadModel
{
    public Stream Stream { get; set; }
    public string FileName { get; set; }

    public FileUploadModel()
    {

    }

    public FileUploadModel(Stream stream, string fileName)
    {
        Stream = new MemoryStream();
        ReadStream(stream, Stream);
        Stream.Position = 0;
        FileName = fileName;
    }

    private static void ReadStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8192]; // Adjust the buffer size as needed
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }

    public void Dispose()
    {
        Stream?.Dispose();
    }
}
