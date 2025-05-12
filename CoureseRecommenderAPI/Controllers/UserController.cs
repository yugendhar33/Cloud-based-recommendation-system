using Microsoft.AspNetCore.Mvc;
using CourseRecommenderAPI.Models;
using CourseRecommenderAPI.Services;

namespace CourseRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DynamoDbService _service = new();

        [HttpPost]
        public async Task<IActionResult> SavePreferences([FromBody] UserPreference prefs)
        {
            await _service.SaveUserPreferencesAsync(prefs);
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPreferences(string userId)
        {
            var prefs = await _service.GetUserPreferencesAsync(userId);
            return Ok(prefs);
        }
    }
}
