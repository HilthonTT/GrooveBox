namespace GrooveBoxLibrary.Models;
public class BasicMediaFileModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }

    public BasicMediaFileModel()
    {
        
    }

    public BasicMediaFileModel(MediaFileModel media)
    {
        Id = media.Id;
        Title = media.Title;
    }
}
