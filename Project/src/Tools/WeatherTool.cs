using System.Text.Json;

namespace ToolForge.Tools;

public class WeatherTool
{
    private readonly HttpClient _http = new();

    public async Task<string> GetWeatherAsync(string city)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            return "OPENWEATHER_API_KEY not found.";

        if (string.IsNullOrWhiteSpace(city))
            return "City name is required.";

        try
        {
            var url =
                $"https://api.openweathermap.org/data/2.5/weather" +
                $"?q={Uri.EscapeDataString(city)}" +
                $"&appid={apiKey}" +
                $"&units=metric";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return $"Could not retrieve weather for {city}.";

            var json =
                JsonDocument.Parse(
                    await response.Content.ReadAsStringAsync()
                );

            var root = json.RootElement;

            var temp =
                root.GetProperty("main")
                    .GetProperty("temp")
                    .GetDecimal();

            var humidity =
                root.GetProperty("main")
                    .GetProperty("humidity")
                    .GetInt32();

            var description =
                root.GetProperty("weather")[0]
                    .GetProperty("description")
                    .GetString();

            return
                $"Weather in {city}: " +
                $"{temp}°C, " +
                $"{humidity}% humidity, " +
                $"{description}";
        }
        catch (Exception ex)
        {
            return $"Weather error: {ex.Message}";
        }
    }
}