namespace AI.Domain.Models.Requests;

public class UploadFileRequest
{

    public string FullFilename { get; set; }

    public string File => Path.GetFileName(FullFilename);

    public string Purpose { get; set; }
}