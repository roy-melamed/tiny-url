using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Cache;
using TinyUrl.Models;
using TinyUrl.Repositories;

namespace TinyUrl.Services
{
    public class UrlService
    {
        private readonly UrlRepository _urlRepository;
        private readonly LRUCache<string, string> _cache;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortUrlLength = 6;

        public UrlService(UrlRepository urlRepository, LRUCache<string, string> cache)
        {
            _urlRepository = urlRepository;
            _cache = cache;
        }

        public async Task<string> GetOrCreateShortUrlAsync(string longUrl)
        {
            if (_cache.TryGetValue(longUrl, out var cachedShortUrl))
            {
                return cachedShortUrl ?? throw new Exception("Cached short URL is unexpectedly null.");
            }

            var existingMapping = await _urlRepository.GetByLongUrlAsync(longUrl);
            if (existingMapping != null)
            {
                if (existingMapping.ShortUrl != null)
                {
                    _cache.Add(longUrl, existingMapping.ShortUrl);
                    return existingMapping.ShortUrl;
                }
                else
                {
                    throw new Exception("Existing short URL is unexpectedly null.");
                }
            }

            var shortUrl = GenerateShortUrl(longUrl);
            var newMapping = new UrlMapping { LongUrl = longUrl, ShortUrl = shortUrl };
            await _urlRepository.CreateAsync(newMapping);
            _cache.Add(longUrl, shortUrl);

            return shortUrl ?? throw new Exception("Short URL generation failed unexpectedly.");
        }

        public async Task<string?> GetLongUrlAsync(string shortUrl)
        {
            if (_cache.TryGetValue(shortUrl, out var cachedLongUrl))
            {
                return cachedLongUrl;
            }

            var mapping = await _urlRepository.GetByShortUrlAsync(shortUrl);
            if (mapping != null)
            {
                if (mapping.LongUrl != null)
                {
                    _cache.Add(shortUrl, mapping.LongUrl);
                    return mapping.LongUrl;
                }
                else
                {
                    throw new Exception("Mapping long URL is unexpectedly null.");
                }
            }

            return null; // No matching long URL found for the given short URL
        }

        private string GenerateShortUrl(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var shortUrl = new StringBuilder();
                for (int i = 0; i < ShortUrlLength; i++)
                {
                    shortUrl.Append(Alphabet[hash[i] % Alphabet.Length]);
                }
                return shortUrl.ToString();
            }
        }
    }
}
