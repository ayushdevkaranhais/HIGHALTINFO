using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
 
class Program
{
    private static readonly string apiToken = "d4a8577d2036f84f408aeb35bfface46117d0c47321db72deeede80b5c40bd07";
    private static readonly string baseUrl = "https://project.highaltsolutions.in";
    private static readonly string projectIdentifier = "hais-admin-v2-1";
 
    private static readonly HttpClient httpClient = new HttpClient();
 
    static async Task Main()
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"apikey:{apiToken}")));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
 
        if (!await Authenticate())
        {
            Console.WriteLine("❌ Authentication failed.");
            return;
        }
 
        await FetchAndSaveUserStories();
    }
 
    private static async Task<bool> Authenticate()
    {
        var url = $"{baseUrl}/api/v3/users/me";
 
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
 
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
 
            var name = root.GetProperty("name").GetString();
            var login = root.GetProperty("login").GetString();
 
            Console.WriteLine($"✅ Authenticated as: {name} ({login})");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error authenticating: {ex.Message}");
            return false;
        }
    }
 
    private static async Task FetchAndSaveUserStories()
    {
        var url = $"{baseUrl}/api/v3/projects/{projectIdentifier}/work_packages";
 
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
 
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var workPackages = doc.RootElement
                .GetProperty("_embedded")
                .GetProperty("elements")
                .EnumerateArray();
 
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "user_stories");
            Directory.CreateDirectory(folderPath);
 
            int count = 0;
 
            foreach (var wp in workPackages)
            {
                if (!wp.TryGetProperty("_links", out var links)) continue;
                if (!links.TryGetProperty("type", out var typeLink)) continue;
                if (!typeLink.TryGetProperty("title", out var titleProp)) continue;
 
                string typeTitle = titleProp.GetString()?.ToLowerInvariant() ?? "";
                if (!typeTitle.Contains("user story")) continue;
 
                string subject = wp.GetProperty("subject").GetString() ?? "No Subject";
                string id = wp.GetProperty("id").ToString();
                string description = wp.GetProperty("description").GetProperty("raw").GetString() ?? "No description.";
 
                string safeFileName = Regex.Replace(subject, @"[^\w\s-]", "");
                safeFileName = Regex.Replace(safeFileName, @"\s+", "-");
                string fileName = $"{id}-{safeFileName}.md";
 
                string filePath = Path.Combine(folderPath, fileName);
                string content = $"# {subject}\n\n{description}";
 
                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine($"✅ Saved: {fileName}");
                count++;
            }
 
            if (count == 0)
                Console.WriteLine("⚠️ No user stories found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching work packages: {ex.Message}");
        }
    }
}

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
// // Enable CORS for frontend-backend communication
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();
// app.UseCors();


// app.MapGet("/", () => "Hello World");

// app.Run();
