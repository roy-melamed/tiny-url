using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models
{
    public class UrlMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? LongUrl { get; set; }

        public string? ShortUrl { get; set; }
    }
}