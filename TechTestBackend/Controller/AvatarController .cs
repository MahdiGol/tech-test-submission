using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TechTestBackend.Models;

namespace TechTestBackend.Controller
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Text.RegularExpressions;
    using TechTestBackend.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly HttpClient _httpClient = new HttpClient();

        public AvatarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("avatar")]
        public async Task<IActionResult> GetAvatar(string userIdentifier)
        {
            if (string.IsNullOrEmpty(userIdentifier))
            {
                return BadRequest("User identifier cannot be empty.");
            }

            string imageUrl = await GetImageUrl(userIdentifier);
            return Ok(new { url = imageUrl });
        }

        private async Task<string> GetImageUrl(string userIdentifier)
        {
            // Rule 1: Last character is [6, 7, 8, 9]
            char lastChar = userIdentifier[^1];
            if ("6789".Contains(lastChar))
            {
                string url = $"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{lastChar}";
                var response = await _httpClient.GetStringAsync(url);
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                return data.url;
            }

            // Rule 2: Last character is [1, 2, 3, 4, 5]
            if ("12345".Contains(lastChar))
            {
                int lastDigit = int.Parse(lastChar.ToString());
                var image = await _context.Images.FirstOrDefaultAsync(i => i.Id == lastDigit);
                if (image != null)
                {
                    return image.Url;
                }
            }

            // Rule 3: Contains a vowel (aeiou)
            if (Regex.IsMatch(userIdentifier, "[aeiouAEIOU]"))
            {
                return "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150";
            }

            // Rule 4: Contains a non-alphanumeric character
            if (Regex.IsMatch(userIdentifier, @"[^a-zA-Z0-9]"))
            {
                int randomNumber = new Random().Next(1, 6); // Random number between 1 and 5
                return $"https://api.dicebear.com/8.x/pixel-art/png?seed={randomNumber}&size=150";
            }

            // Default case
            return "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150";
        }
    }


}
