namespace AI.Domain.Models;

public class UploadFileRequest
{

    public string FullFilename { get; set; }

    public string File => System.IO.Path.GetFileName(FullFilename);

    public string Purpose { get; set; }
}