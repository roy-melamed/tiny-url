using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Cache;
using TinyUrl.Models;
using TinyUrl.Repositories;

namespace TinyUrlSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly UrlRepository _urlRepository;
        private readonly LRUCache<string, string> _cache;

        public UrlController(UrlRepository urlRepository, LRUCache<string, string> cache)
        {
            _urlRepository = urlRepository;
            _cache = cache;
        }

        [HttpPost("shorten")]
        public async Task<ActionResult<string>> ShortenUrl([FromBody] string longUrl)
        {
            // Check cache
            if (_cache.TryGetValue(longUrl, out string shortUrl))
            {
                return shortUrl;
            }

            // Check if long URL already exists in database
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
                    // Handle the case where ShortUrl is null
                    return BadRequest("ShortUrl is null");
                }
            }

            // Generate short URL
            shortUrl = GenerateShortUrl(longUrl);

            if (shortUrl != null)
            {
                // Save mapping to database
                var mapping = new UrlMapping { LongUrl = longUrl, ShortUrl = shortUrl };
                await _urlRepository.CreateAsync(mapping);

                // Add to cache
                _cache.Add(longUrl, shortUrl);

                return shortUrl;
            }
            else
            {
                // Handle the case where shortUrl is null
                return BadRequest("ShortUrl generation failed");
            }
        }

        [HttpGet("{shortUrl}")]
        public async Task<IActionResult> RedirectUrl([FromRoute] string shortUrl)
        {
            // Check cache
            if (_cache.TryGetValue(shortUrl, out string longUrl))
            {
                return RedirectPermanent(longUrl);
            }

            var mapping = await _urlRepository.GetByShortUrlAsync(shortUrl);

            if (mapping == null)
            {
                return NotFound();
            }

            if (mapping.LongUrl != null)
            {
                // Add to cache
                _cache.Add(shortUrl, mapping.LongUrl);

                return RedirectPermanent(mapping.LongUrl);
            }
            else
            {
                // Handle the case where LongUrl is null
                return BadRequest("LongUrl is null");
            }
        }

        private string GenerateShortUrl(string longUrl)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(longUrl));
                var hashStringBuilder = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashStringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                // Take the first 7 characters of the hash as the short URL
                return hashStringBuilder.ToString().Substring(0, 7);
            }
        }
    }
}
