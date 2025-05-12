using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CourseRecommenderAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseRecommenderAPI.Services
{
    public class DynamoDbService
    {

        private readonly DynamoDBContext _context;

        public DynamoDbService()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
        }

        public async Task SaveUserPreferencesAsync(UserPreference userPreference)
        {
            await _context.SaveAsync(userPreference);
        }

        public async Task<UserPreference> GetUserPreferencesAsync(string user_id)
        {
            return await _context.LoadAsync<UserPreference>(user_id);
        }

        // Method to save a course to DynamoDB
        public async Task SaveCourseAsync(Course course)
        {
            await _context.SaveAsync(course);
        }

        // Method to get a course by its ID
        public async Task<Course> GetCourseByIdAsync(string id)
        {
            return await _context.LoadAsync<Course>(id);
        }

        // Method to get all courses
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            var scan = _context.ScanAsync<Course>(default);
            return await scan.GetRemainingAsync();
        }
    }
}