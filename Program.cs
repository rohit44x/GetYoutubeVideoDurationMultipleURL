using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    static async Task Main(string[] args)
    {
        // Read URLs from a text file
        var filePath = "C:\\Users\\Rohit\\Desktop\\urls.txt"; // Ensure the file exists in your working directory
        List<string> videoUrls = File.ReadAllLines(filePath).ToList();

        // Get YouTube API key enable Youtube Services for that API key.
        var apiKey = "API-Key";

        // Initialize YouTube API service
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "YouTubeDurationCalculator"
        });

        // Get video IDs from URLs
        List<string> videoIds = GetVideoIdsFromUrls(videoUrls);

        TimeSpan totalDuration = TimeSpan.Zero;

        foreach (var videoId in videoIds)
        {
            var videoDuration = await GetVideoDurationAsync(youtubeService, videoId);
            totalDuration += videoDuration;
        }

        // Output total duration
        Console.WriteLine($"Total Duration: {totalDuration}");
    }

    // Extracts video ID from YouTube URL
    static List<string> GetVideoIdsFromUrls(List<string> urls)
    {
        List<string> videoIds = new List<string>();
        foreach (var url in urls)
        {
            var uri = new Uri(url);
            var query = uri.Query;
            var videoId = System.Web.HttpUtility.ParseQueryString(query).Get("v");
            if (!string.IsNullOrEmpty(videoId))
            {
                videoIds.Add(videoId);
            }
        }
        return videoIds;
    }

    // Fetches the duration of a YouTube video
    static async Task<TimeSpan> GetVideoDurationAsync(YouTubeService youtubeService, string videoId)
    {
        var videoRequest = youtubeService.Videos.List("contentDetails");
        videoRequest.Id = videoId;

        var response = await videoRequest.ExecuteAsync();
        var video = response.Items.FirstOrDefault();
        if (video != null)
        {
            var duration = XmlConvert.ToTimeSpan(video.ContentDetails.Duration);
            return duration;
        }

        return TimeSpan.Zero;
    }
}
