using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using CourseRecommenderAPI.Models;

namespace CourseRecommenderAPI.Services
{
    public class CourseraApiService
    {
        private readonly HttpClient _httpClient;

        public CourseraApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Course>> FetchCoursesAsync()
        {
            var url = "https://api.coursera.org/api/courses.v1?limit=100";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(stream);
            var root = jsonDoc.RootElement;

            var results = root.GetProperty("elements");
            var courses = new List<Course>();

            foreach (var item in results.EnumerateArray())
            {
                var course = new Course
                {
                    course_id = item.GetProperty("id").GetString() ?? Guid.NewGuid().ToString(),
                    Title = item.GetProperty("name").GetString() ?? "No Title",
                    Url = $"https://www.coursera.org/learn/{item.GetProperty("slug").GetString()}", // Generate URL
                    Slug = item.GetProperty("slug").GetString() ?? string.Empty, // Set Slug
                    CourseType = item.GetProperty("courseType").GetString() ?? string.Empty, // Set CourseType
                    Topics = new List<string> { "Software Engineering" } // Static fallback Topics
                };

                courses.Add(course);
            }

            return courses;
        }
    }
}
