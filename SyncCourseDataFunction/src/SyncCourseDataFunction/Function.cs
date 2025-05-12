using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

// Required by AWS Lambda to use JSON serialization
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SyncCourseDataFunction
{
    public class Function
    {
        private static readonly HttpClient httpClient = new();
        private readonly DynamoDBContext _context;

        public Function()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
        }

        public async Task FunctionHandler(ILambdaContext context)
        {
            var courseraApi = "https://api.coursera.org/api/courses.v1?limit=100";

            try
            {
                var response = await httpClient.GetFromJsonAsync<CourseraResponse>(courseraApi);

                if (response?.Elements != null)
                {
                    foreach (var course in response.Elements)
                    {
                        var newCourse = new Course
                        {
                            CourseId = course.Id,
                            CourseType =course.courseType,
                            Title = course.Name,
                            Slug = course.Slug,
                            Platform = "Coursera",
                            Url = $"https://www.coursera.org/learn/{course.Slug}"
                        };

                        await _context.SaveAsync(newCourse);
                        context.Logger.LogLine($"Saved course: {newCourse.Title}");
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error fetching or saving courses: {ex.Message}");
            }
        }
    }

    // Deserialization models for Coursera API
    public class CourseraResponse
    {
        [JsonPropertyName("elements")]
        public List<CourseraCourse> Elements { get; set; }
    }

    public class CourseraCourse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // DynamoDB model
    [DynamoDBTable("Courses")]
    public class Course
    {
        [DynamoDBHashKey]
        public string CourseId { get; set; }

        public string Title { get; set; }
        public string Platform { get; set; }
        public string Level { get; set; }
        public double Rating { get; set; }
        public double Price { get; set; }
        public List<string> Topics { get; set; }
        public string Url { get; set; }
    }
}
