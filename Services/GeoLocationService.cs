using System.Text.Json;
using Devdiscourse.Models.ViewModel;
using Microsoft.Extensions.Caching.Distributed;

namespace Devdiscourse.Services;

public interface IGeoLocationService
{
    Task<GeoLocationViewModel> GetGeoLocationAsync(string? visitorIp, CancellationToken cancellationToken = default);
}

public sealed class GeoLocationService : IGeoLocationService
{
    private const string DefaultApiKey = "DUmVyqfXtgPOLyL";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeoLocationService> _logger;

    public GeoLocationService(
        HttpClient httpClient,
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<GeoLocationService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GeoLocationViewModel> GetGeoLocationAsync(string? visitorIp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(visitorIp))
        {
            return new GeoLocationViewModel();
        }

        string cacheKey = $"geo:{visitorIp.Trim()}";
        string? cachedPayload = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedPayload))
        {
            var cachedLocation = JsonSerializer.Deserialize<GeoLocationViewModel>(cachedPayload);
            if (cachedLocation != null)
            {
                return cachedLocation;
            }
        }

        string apiKey = _configuration["IpApi:ApiKey"] ?? DefaultApiKey;
        string requestUri = $"https://pro.ip-api.com/json/{Uri.EscapeDataString(visitorIp)}?fields=query,country,city&key={apiKey}";

        try
        {
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new GeoLocationViewModel();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<IpApiResponse>(stream, cancellationToken: cancellationToken);
            var location = new GeoLocationViewModel
            {
                IPv4 = payload?.query,
                country_name = payload?.country,
                city_name = payload?.city
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(location),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                },
                cancellationToken);

            return location;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to resolve geolocation for {VisitorIp}.", visitorIp);
            return new GeoLocationViewModel();
        }
    }

    private sealed class IpApiResponse
    {
        public string? query { get; set; }
        public string? country { get; set; }
        public string? city { get; set; }
    }
}