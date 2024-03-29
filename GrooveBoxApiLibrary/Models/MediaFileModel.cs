﻿namespace GrooveBoxApiLibrary.Models;
public class MediaFileModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailPath { get; set; }
    public string FilePath { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public GenreModel Genre { get; set; }
    public BasicUserModel Author { get; set; }
    public HashSet<string> UserVotes { get; set; } = new();
    public bool Archived { get; set; } = false;
}