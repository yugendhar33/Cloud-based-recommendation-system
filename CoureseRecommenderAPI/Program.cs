using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CourseRecommenderAPI.Models;
using CourseRecommenderAPI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<DynamoDbService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<CourseraApiService>(); // Add Coursera API service

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course Recommender API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 Get all courses
app.MapGet("/courses", async (IDynamoDBContext db) =>
{
    var scan = db.ScanAsync<Course>(default);
    var results = await scan.GetRemainingAsync();
    return Results.Ok(results);
});

// 🔹 Get course by ID
app.MapGet("/courses/{id}", async (string id, IDynamoDBContext db) =>
{
    var course = await db.LoadAsync<Course>(id);
    return course is not null ? Results.Ok(course) : Results.NotFound("Course not found");
});

// 🔹 Add user preference
app.MapPost("/preferences", async ([FromBody] UserPreference pref, IDynamoDBContext db) =>
{
    await db.SaveAsync(pref);
    return Results.Ok(pref);
});

// 🔹 Get preference by userId
app.MapGet("/preferences/{user_id}", async (string user_id, IDynamoDBContext db) =>
{
    var pref = await db.LoadAsync<UserPreference>(user_id);
    return pref is not null ? Results.Ok(pref) : Results.NotFound("User preference not found");
});

// 🔹 Recommend courses
app.MapPost("/recommend", async ([FromBody] UserPreference pref, RecommendationService recommender) =>
{
    var recommendations = await recommender.GetRecommendationsAsync(pref);
    Console.WriteLine("Recommended Courses:");
    foreach (var course in recommendations)
    {
        Console.WriteLine($"- {course.Title} (Category: {course.CourseType})");
    }
    return Results.Ok(recommendations);
});

// 🔹 Fetch courses from Coursera API and save to DynamoDB
app.MapPost("/courses/fetch", async (CourseraApiService courseraApiService, DynamoDbService dynamoDbService) =>
{
    var courses = await courseraApiService.FetchCoursesAsync();

    foreach (var course in courses)
    {
        await dynamoDbService.SaveCourseAsync(course);
    }

    return Results.Ok(new { message = "Courses fetched and saved successfully." });
});

app.Run();
public partial class Program { }
