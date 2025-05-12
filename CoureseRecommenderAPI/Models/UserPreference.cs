using Amazon.DynamoDBv2.DataModel;

namespace CourseRecommenderAPI.Models
{
    [DynamoDBTable("UserPreference")]
    public class UserPreference
    {
        [DynamoDBHashKey]
        public string? user_id { get; set; }  // Made nullable

        public List<string> Skills { get; set; }
        public string Goal { get; set; }
        public List<string> PreferredPlatforms { get; set; }
        public List<string> History { get; set; }
    }
}
