using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoClient _client;

    public MongoDbContext(string connectionUri)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        _client = new MongoClient(settings);
    }

    public IMongoDatabase GetDatabase(string databaseName)
    {
        return _client.GetDatabase(databaseName);
    }
}
