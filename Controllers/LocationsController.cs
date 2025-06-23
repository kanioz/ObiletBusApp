using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObiletBusApp.Models;
using System.Text;
using ObiletBusApp.Common;

namespace ObiletBusApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController(IHttpContextAccessor httpContextAccessor) : BaseObiletController(httpContextAccessor)
{
    private const string LocationUrl = Constants.ObiletLocationUrl;

    private async Task<List<ObiletLocation>> SearchLocationsAsync(string? query, ObiletSession session)
    {
        var client = new HttpClient();
        var body = new ObiletLocationSearchRequest
        {
            Data = query,
            DeviceSession = new ObiletDeviceSession
            {
                SessionId = session.SessionId,
                DeviceId = session.DeviceId
            },
            Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            Language = "tr-TR"
        };
        var request = new HttpRequestMessage(HttpMethod.Post, LocationUrl);
        request.Headers.Add("Authorization", Constants.ObiletAuthHeader);
        request.Content = new StringContent(
            JsonConvert.SerializeObject(body),
            Encoding.UTF8, "application/json"
        );
        var response = await client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var locationResponse = JsonConvert.DeserializeObject<ObiletLocationResponse>(json);
        return locationResponse?.Data ?? [];
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        if (!string.IsNullOrEmpty(query) && query.Length < 2)
        {
            return Ok(new List<object>());
        }
        
        var session = await GetObiletSessionAsync();
        var results = await SearchLocationsAsync(query, session);
        var output = new List<object>();
        foreach (var l in results)
        {
            output.Add(new { l.Id, l.Name });
        }
        return Ok(output);
    }
}