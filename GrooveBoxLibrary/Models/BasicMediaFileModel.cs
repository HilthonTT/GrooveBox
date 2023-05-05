namespace GrooveBoxLibrary.Models;
public class BasicMediaFileModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FileName { get; set; }

    public BasicMediaFileModel()
    {
        
    }

    public BasicMediaFileModel(MediaFileModel file)
    {
        Id = file.Id;
        FileName = file.FileName;
    }
}
