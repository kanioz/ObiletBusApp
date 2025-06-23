using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ObiletBusApp.Models;
using System.Text;
using ObiletBusApp.Common;

namespace ObiletBusApp.Controllers;

public class BaseObiletController(IHttpContextAccessor httpContextAccessor) : Controller
{
    protected async Task<ObiletSession> GetObiletSessionAsync()
    {
        var context = httpContextAccessor.HttpContext!;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var port = context.Connection.RemotePort.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
            
        var (browserName, browserVersion) = ParseBrowserFromUserAgent(userAgent);
            
        var client = new HttpClient();
        var sessionRequest = new ObiletSessionRequest
        {
            Type = 1,
            Connection = new ObiletConnection { IpAddress = ipAddress, Port = port },
            Browser = new ObiletBrowser { Name = browserName, Version = browserVersion }
        };
        var request = new HttpRequestMessage(HttpMethod.Post, Constants.ObiletSessionUrl);
        request.Headers.Add("Authorization", Constants.ObiletAuthHeader);
        request.Content = new StringContent(
            JsonConvert.SerializeObject(sessionRequest),
            Encoding.UTF8, "application/json"
        );
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var sessionResponse = JsonConvert.DeserializeObject<ObiletSessionResponse>(json);
        return new ObiletSession
        {
            SessionId = sessionResponse?.Data?.SessionId,
            DeviceId = sessionResponse?.Data?.DeviceId
        };
    }

    private (string Name, string Version) ParseBrowserFromUserAgent(string userAgent)
    {
        var browserName = "Unknown";
        var browserVersion = "Unknown";

        if (string.IsNullOrEmpty(userAgent))
        {
            return (browserName, browserVersion);
        }
            
        var browserChecks = new[] { "Chrome", "Edg", "Firefox", "Safari", "Opera" };
        foreach (var browser in browserChecks)
        {
            var browserIndex = userAgent.IndexOf(browser + "/", StringComparison.OrdinalIgnoreCase);
            if (browserIndex == -1) continue;
                
            browserName = browser;
            var versionStartIndex = browserIndex + browser.Length + 1;
            var versionEndIndex = userAgent.IndexOf(' ', versionStartIndex);
            if (versionEndIndex == -1)
            {
                versionEndIndex = userAgent.Length;
            }
            browserVersion = userAgent.Substring(versionStartIndex, versionEndIndex - versionStartIndex).Trim();
            break;
        }
        return (browserName, browserVersion);
    }
}