using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.DTOs;
using RaceDay.Mvc.Attributes;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Participant")] 
public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _api;
    public ParticipantController(RaceDayApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var events = await _api.GetEventsAsync();
        return View(events);
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        var success = await _api.EnrolAsync(new EnrolmentDto { EventId = eventId, CategoryId = categoryId });
        if (!success) TempData["Error"] = "Failed to enrol.";

        return RedirectToAction("MyEnrolments");
    }

    public async Task<IActionResult> MyEnrolments()
    {
        var enrolments = await _api.GetMyEnrolmentsAsync();
        return View(enrolments);
    }

    public async Task<IActionResult> MyResults()
    {
        var results = await _api.GetMyResultsAsync();
        return View(results);
    }
}