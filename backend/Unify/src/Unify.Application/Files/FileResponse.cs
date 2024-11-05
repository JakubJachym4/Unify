namespace Unify.Application.Files;

public sealed class FileResponse
{
    public FileResponse(string fileName, string contentType, string data)
    {
        FileName = fileName;
        ContentType = contentType;
        Data = data;
    }

    public string FileName { get; init; }
    public string ContentType { get; init; }
    public string Data { get; init; }


}