using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObiletBusApp.Models;
using System.Globalization;
using System.Text;
using ObiletBusApp.Common;
using System.Linq;

namespace ObiletBusApp.Controllers;

public class JourneyController(IHttpContextAccessor httpContextAccessor)
    : BaseObiletController(httpContextAccessor)
{
    private async Task<List<ObiletJourney>> GetJourneysAsync(int originId, int destinationId, string date, ObiletSession session)
    {
        var client = new HttpClient();
        var body = new ObiletJourneySearchRequest
        {
            DeviceSession = new ObiletDeviceSession
            {
                SessionId = session.SessionId,
                DeviceId = session.DeviceId
            },
            Date = date,
            Language = "tr-TR",
            Data = new ObiletJourneySearchData
            {
                OriginId = originId,
                DestinationId = destinationId,
                DepartureDate = date
            }
        };
        var request = new HttpRequestMessage(HttpMethod.Post, Constants.ObiletJourneyUrl);
        request.Headers.Add("Authorization", Constants.ObiletAuthHeader);
        request.Content = new StringContent(
            JsonConvert.SerializeObject(body),
            Encoding.UTF8, "application/json"
        );
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode(); // Hata varsa exception fırlatır
        var json = await response.Content.ReadAsStringAsync();
        var journeyResponse = JsonConvert.DeserializeObject<ObiletJourneyResponse>(json);
        return journeyResponse?.Data ?? new List<ObiletJourney>();
    }

    public async Task<IActionResult> Index(int originId, string from, int destinationId, string to, string date)
    {
        if (originId == destinationId)
        {
            TempData["ErrorMessage"] = "Kalkış ve varış noktası aynı olamaz.";
            return RedirectToAction("Index", "Home");
        }

        if (!DateTime.TryParse(date, out var departureDate) || departureDate.Date < DateTime.Today)
        {
            TempData["ErrorMessage"] = "Geçerli bir tarih seçilmelidir.";
            return RedirectToAction("Index", "Home");
        }
            
        var session = await GetObiletSessionAsync();

        var journeys = await GetJourneysAsync(originId, destinationId, date, session);
        
        var sortedJourneys = journeys.OrderBy(j => j.Journey.Departure).ToList();
        
        ViewBag.From = from;
        ViewBag.To = to;
        ViewBag.Date = DateTime.Parse(date).ToString("dd MMMM dddd", new CultureInfo("tr-TR"));
        ViewBag.OriginId = originId;
        ViewBag.DestinationId = destinationId;
        ViewBag.QueryDate = date;
        return View(sortedJourneys);
    }
}