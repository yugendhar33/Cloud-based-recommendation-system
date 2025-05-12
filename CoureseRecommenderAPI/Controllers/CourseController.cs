using Microsoft.AspNetCore.Mvc;
using CourseRecommenderAPI.Models;
using CourseRecommenderAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseRecommenderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;
        private readonly DynamoDbService _dynamoDbService;
        private readonly CourseraApiService _courseraApiService;

        public CourseController(RecommendationService recommendationService, DynamoDbService dynamoDbService)
        {
            _recommendationService = recommendationService;
            _dynamoDbService = dynamoDbService;
            _courseraApiService = new CourseraApiService(); // Direct instantiation
        }

        // GET: api/course
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _dynamoDbService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // POST: api/course
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] Course course)
        {
            await _dynamoDbService.SaveCourseAsync(course);
            return Ok();
        }

        // POST: api/course/recommend
        [HttpPost("recommend")]
        public async Task<IActionResult> Recommend([FromBody] UserPreference preference)
        {
            var allCourses = await _dynamoDbService.GetAllCoursesAsync();
            var recommendations = _recommendationService.RecommendCourses(preference, allCourses);
            return Ok(recommendations);
        }

        // POST: api/course/fetch-coursera
        [HttpPost("fetch-coursera")]
        public async Task<IActionResult> FetchAndStoreCourseraCourses()
        {
            var courses = await _courseraApiService.FetchCoursesAsync();

            foreach (var course in courses)
            {
                await _dynamoDbService.SaveCourseAsync(course);
            }

            return Ok(new { Message = $"{courses.Count} Coursera courses added to DynamoDB." });
        }

        // POST: api/course/fetch-and-store
        [HttpPost("fetch-and-store")]
        public async Task<IActionResult> FetchAndStoreCoursesFromCoursera()
        {
            var externalCourses = await _recommendationService.FetchCoursesFromCourseraAsync();
            foreach (var course in externalCourses)
            {
                await _dynamoDbService.SaveCourseAsync(course);
            }
            return Ok("Courses fetched and saved.");
        }

        // POST: api/course/recommend-from-external
        [HttpPost("recommend-from-external")]
        public async Task<IActionResult> RecommendFromExternal([FromBody] UserPreference preference)
        {
            var externalCourses = await _recommendationService.FetchCoursesFromCourseraAsync();
            var recommendations = _recommendationService.RecommendCourses(preference, externalCourses);
            return Ok(recommendations);
        }
    }
}
