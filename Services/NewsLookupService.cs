using Devdiscourse.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Devdiscourse.Services;

public sealed record SectorLookup(int Id, string Title, string Slug);

public interface INewsLookupService
{
    Task<string?> GetRegionByCountryAsync(string country, CancellationToken cancellationToken = default);
    Task<SectorLookup?> GetSectorBySlugAsync(string slug, CancellationToken cancellationToken = default);
}

public sealed class NewsLookupService : INewsLookupService
{
    private static readonly TimeSpan MetadataCacheDuration = TimeSpan.FromMinutes(30);

    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _memoryCache;

    public NewsLookupService(ApplicationDbContext db, IMemoryCache memoryCache)
    {
        _db = db;
        _memoryCache = memoryCache;
    }

    public Task<string?> GetRegionByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        string normalizedCountry = country.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCountry))
        {
            return Task.FromResult<string?>(null);
        }

        return _memoryCache.GetOrCreateAsync(
            $"country-region:{normalizedCountry.ToLowerInvariant()}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = MetadataCacheDuration;

                return await _db.Countries
                    .AsNoTracking()
                    .Where(c => c.Title != null && c.Title.Contains(normalizedCountry))
                    .Join(
                        _db.Regions.AsNoTracking(),
                        countryRecord => countryRecord.RegionId,
                        region => region.Id,
                        (_, region) => region.Title)
                    .FirstOrDefaultAsync(cancellationToken);
            });
    }

    public Task<SectorLookup?> GetSectorBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        string normalizedSlug = slug.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
        {
            return Task.FromResult<SectorLookup?>(null);
        }

        return _memoryCache.GetOrCreateAsync(
            $"sector:{normalizedSlug.ToLowerInvariant()}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = MetadataCacheDuration;

                return await _db.DevSectors
                    .AsNoTracking()
                    .Where(sector => sector.Slug == normalizedSlug)
                    .Select(sector => new SectorLookup(sector.Id, sector.Title ?? string.Empty, sector.Slug ?? normalizedSlug))
                    .FirstOrDefaultAsync(cancellationToken);
            });
    }
}