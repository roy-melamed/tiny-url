using MongoDB.Driver;
using System.Threading.Tasks;
using TinyUrl.Models;

namespace TinyUrl.Repositories
{
    public class UrlRepository
    {
        private readonly IMongoCollection<UrlMapping> _urlMappings;

        public UrlRepository(IMongoDatabase database)
        {
            _urlMappings = database.GetCollection<UrlMapping>("UrlMappings");
        }

        public async Task<UrlMapping> GetByLongUrlAsync(string longUrl)
        {
            return await _urlMappings.Find(mapping => mapping.LongUrl == longUrl).FirstOrDefaultAsync();
        }

        public async Task<UrlMapping> GetByShortUrlAsync(string shortUrl)
        {
            return await _urlMappings.Find(mapping => mapping.ShortUrl == shortUrl).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(UrlMapping urlMapping)
        {
            await _urlMappings.InsertOneAsync(urlMapping);
        }
    }
}
