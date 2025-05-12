using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace CourseRecommenderAPI.Models
{
    [DynamoDBTable("Course")]
    public class Course
    {
        [DynamoDBHashKey] // Partition key for DynamoDB
        public string course_id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public List<String> Topics { get; set; } = new List<string>();
    }
}
