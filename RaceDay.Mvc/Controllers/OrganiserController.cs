using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.DTOs;
using RaceDay.Mvc.Attributes;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Organiser")]
public class OrganiserController : Controller
{
    private readonly RaceDayApiClient _api;
    public OrganiserController(RaceDayApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var events = await _api.GetEventsAsync();
        return View(events);
    }

    public async Task<IActionResult> ManageEvents()
    {
        var events = await _api.GetEventsAsync();
        return View(events);
    }

    [HttpGet]
    public IActionResult CreateEvent() => View();

    [HttpPost]
    public async Task<IActionResult> CreateEvent(EventDto dto, IFormFile bannerFile)
    {
        if (!ModelState.IsValid) return View(dto);

        // 1. Create the event in the API
        var createdEvent = await _api.CreateEventAsync(dto);

        if (createdEvent == null)
        {
            ModelState.AddModelError("", "Failed to create event.");
            return View(dto);
        }

        // 2. If a banner was uploaded, send it to the API
        if (bannerFile != null && bannerFile.Length > 0)
        {
            await _api.UploadBannerAsync(createdEvent.Id, bannerFile);
        }

        TempData["Success"] = "Event created successfully!";
        return RedirectToAction(nameof(ManageEvents));
    }

    public async Task<IActionResult> ViewEnrolments(int id)
    {
        var enrolments = await _api.GetEventEnrolmentsAsync(id);
        ViewBag.EventId = id;
        return View(enrolments);
    }
}