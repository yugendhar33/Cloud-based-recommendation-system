using CourseRecommenderAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseRecommenderAPI.Services
{
    public class CourseService
    {
        private readonly CourseraApiService _courseraApiService;
        private readonly DynamoDbService _dynamoDbService;

        public CourseService(CourseraApiService courseraApiService, DynamoDbService dynamoDbService)
        {
            _courseraApiService = courseraApiService;
            _dynamoDbService = dynamoDbService;
        }

        // Method to Fetch courses from Coursera and save them to DynamoDB
        public async Task FetchAndSaveCoursesAsync()
        {
            var courses = await _courseraApiService.FetchCoursesAsync();
            foreach (var course in courses)
            {
                await _dynamoDbService.SaveCourseAsync(course);
            }
        }
    }
}
