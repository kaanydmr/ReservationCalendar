using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

[Route("api/holidays")]
[ApiController]
public class HolidaysApiController : ControllerBase
{
    private readonly ReservationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public HolidaysApiController(ReservationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetHolidays([FromQuery] string? start, [FromQuery] string? end)
    {
        await FetchAndSaveExternalHolidaysIfNeeded();

        DateOnly? startDate = null, endDate = null;
        if (DateOnly.TryParse(start, out var s)) startDate = s;
        if (DateOnly.TryParse(end, out var e)) endDate = e;

        var query = _context.Holidays.AsQueryable();
        if (startDate.HasValue) query = query.Where(h => h.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(h => h.Date <= endDate.Value);

        var holidays = await query
            .Select(h => new {
                id = "holiday-" + h.Id,
                title = h.Name,
                start = h.Date.ToString("yyyy-MM-dd"),
                end = h.Date.ToString("yyyy-MM-dd"),
                description = h.Description,
                classNames = new[] { "fc-event-holiday" },
                isHoliday = true,
                holidayName = h.Name
            })
            .ToListAsync();

        return Ok(holidays);
    }

    private async Task FetchAndSaveExternalHolidaysIfNeeded()
    {

        var has2025 = await _context.Holidays.AnyAsync(h => h.Date.Year == 2025);
        if (has2025) return;
        
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://date.nager.at/api/v3/PublicHolidays/2025/TR");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var holidays = JsonSerializer.Deserialize<List<NagerHolidayDto>>(json);

        if (holidays == null)
        {
            throw new Exception("Failed to deserialize holidays from external API.");
        }
        Console.WriteLine($"{holidays.Count} holidays fetched from API:");
        foreach (var h in holidays)
        {
            Console.WriteLine($"Date: {h.date}, Local Name: {h.localName}, Name: {h.name}");
            var date = DateOnly.ParseExact(h.date, "yyyy-MM-dd");
            if (!await _context.Holidays.AnyAsync(x => x.Date == date && x.Name == h.localName))
            {
                _context.Holidays.Add(new Holiday
                {
                    Date = date,
                    Name = h.localName,
                    Description = h.name,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        await _context.SaveChangesAsync();
        Console.WriteLine("Holidays saved to database (if not already present).");
    }

    private class NagerHolidayDto
    {
        public string date { get; set; }
        public string localName { get; set; }
        public string name { get; set; }
        // other fields omitted
    }
}