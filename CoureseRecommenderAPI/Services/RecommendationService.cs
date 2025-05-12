using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CourseRecommenderAPI.Models;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

namespace CourseRecommenderAPI.Services
{
    public class RecommendationService
    {
        private readonly DynamoDBContext _context;

        public RecommendationService()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
        }
        // Main logic to recommend courses based on user's skills and goals
        public List<Course> RecommendCourses(UserPreference user, IEnumerable<Course> allCourses)
        {
            // Step 1: Match by skills
            var skillMatchedCourses = allCourses
                .Where(course =>
                    course.Topics != null &&
                    user.Skills.Any(skill =>
                        course.Topics.Any(topic =>
                            topic != null &&
                            topic.Equals(skill, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                )
                .ToList();

            // Step 2: Match by goal
            var goalMatchedCourses = allCourses
                .Where(course =>
                    !string.IsNullOrEmpty(course.CourseType) &&
                    user.Goal != null &&
                    course.CourseType.Equals(user.Goal, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            // Step 3: Combine results and ensure there are no duplicates
            var combinedMatchedCourses = skillMatchedCourses
                .Union(goalMatchedCourses)
                .Distinct()
                .ToList();

            // Step 4: If no matches found, return top N default courses
            if (combinedMatchedCourses.Count == 0)
            {
                return allCourses.Take(5).ToList();
            }

            // Return the matched courses
            return combinedMatchedCourses;
        }
        // Loads all courses from DynamoDB and returns recommendations
        public async Task<List<Course>> GetRecommendationsAsync(UserPreference user)
        {
            var allCourses = await _context.ScanAsync<Course>(default).GetRemainingAsync();
            return RecommendCourses(user, allCourses);
        }

        // Optional utility to fetch a few courses directly from Coursera API
        public async Task<List<Course>> FetchCoursesFromCourseraAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.coursera.org/api/courses.v1?limit=10");

            if (!response.IsSuccessStatusCode)
                return new List<Course>();

            var json = await response.Content.ReadAsStringAsync();
            dynamic parsed = JsonConvert.DeserializeObject(json);

            var courses = new List<Course>();

            foreach (var item in parsed.elements)
            {
                courses.Add(new Course
                {
                    course_id = item.id,
                    Title = item.name,
                    Url = item.link != null ? (string)item.link : string.Empty,
                    Slug = item.slug != null ? (string)item.slug : string.Empty,
                    CourseType = item.courseType != null ? (string)item.courseType : string.Empty,
                    Topics = item.topics != null ? item.topics.ToObject<List<string>>() : new List<string>()
                });
            }

            return courses;
        }
    }
}
