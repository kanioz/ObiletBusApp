using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ObiletBusApp.Models;

namespace ObiletBusApp.Controllers;

public class HomeController(
    IHttpContextAccessor httpContextAccessor)
    : BaseObiletController(httpContextAccessor)
{
    public IActionResult Index(string originId, string from, string destinationId, string to, string date)
    {
        ViewBag.OriginId = originId;
        ViewBag.OriginName = from;
        ViewBag.DestinationId = destinationId;
        ViewBag.DestinationName = to;
        ViewBag.Date = date;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
