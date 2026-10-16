using Assignment2.Data;
using Assignment2.DTOs;
using Assignment2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignment2.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayStore _store;
        public EventsController(RaceDayStore store) => _store = store;
        private int CurrentUserID => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetEvents()
        {
            var events = _store.Events.Select(e => new
            {
                e.EventID,
                e.OrganiserID,
                e.EventName,
                e.Description,
                e.EventDate,
                e.StartTime,
                e.Location,
                e.Province,
                e.EntryFee,
                e.MaxParticipants,
                e.Status,
                Categories = _store.EventCategories
                    .Where(ec => ec.EventID == e.EventID)
                    .Select(ec => _store.Categories.First(c => c.CategoryID == ec.CategoryID).CategoryName)
                    .ToList()
            });
            return Ok(events);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult GetEvent(int id)
        {
            var e = _store.Events.FirstOrDefault(x => x.EventID == id);
            if (e == null) return NotFound(new { message = "Event does not exist." });
            return Ok(new
            {
                e.EventID,
                e.OrganiserID,
                e.EventName,
                e.Description,
                e.EventDate,
                e.StartTime,
                e.Location,
                e.Province,
                e.EntryFee,
                e.MaxParticipants,
                e.Status,
                Categories = _store.EventCategories.Where(x => x.EventID == id)
                    .Select(x => _store.Categories.First(c => c.CategoryID == x.CategoryID)),
                Route = _store.Routes.FirstOrDefault(r => r.EventID == id),
                Weather = _store.EventWeather.Where(w => w.EventID == id).ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateEvent(CreateEventDto dto)
        {
            var organiser = _store.Organisers.FirstOrDefault(o => o.UserID == CurrentUserID);
            if (organiser == null) return BadRequest(new { message = "Organiser profile not found." });
            if (!_store.Categories.Any(c => c.CategoryID == dto.CategoryID)) return BadRequest(new { message = "Category does not exist." });

            var e = new Event
            {
                EventID = _store.GetNextEventId(),
                OrganiserID = organiser.OrganiserID,
                EventName = dto.EventName.Trim(),
                Description = dto.Description.Trim(),
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                Location = dto.Location.Trim(),
                Province = dto.Province.Trim(),
                EntryFee = dto.EntryFee,
                MaxParticipants = dto.MaxParticipants,
                Status = dto.Status.Trim()
            };
            _store.Events.Add(e);
            _store.EventCategories.Add(new EventCategory { EventID = e.EventID, CategoryID = dto.CategoryID });
            return CreatedAtAction(nameof(GetEvent), new { id = e.EventID }, e);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateEvent(int id, CreateEventDto dto)
        {
            var e = _store.Events.FirstOrDefault(x => x.EventID == id);
            if (e == null) return NotFound(new { message = "Event does not exist." });
            var organiser = _store.Organisers.FirstOrDefault(o => o.UserID == CurrentUserID);
            if (organiser == null || e.OrganiserID != organiser.OrganiserID) return Forbid();
            if (!_store.Categories.Any(c => c.CategoryID == dto.CategoryID)) return BadRequest(new { message = "Category does not exist." });

            e.EventName = dto.EventName.Trim(); e.Description = dto.Description.Trim(); e.EventDate = dto.EventDate;
            e.StartTime = dto.StartTime; e.Location = dto.Location.Trim(); e.Province = dto.Province.Trim();
            e.EntryFee = dto.EntryFee; e.MaxParticipants = dto.MaxParticipants; e.Status = dto.Status.Trim();

            var existing = _store.EventCategories.FirstOrDefault(x => x.EventID == id);
            if (existing == null) _store.EventCategories.Add(new EventCategory { EventID = id, CategoryID = dto.CategoryID });
            else existing.CategoryID = dto.CategoryID;
            return Ok(e);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult DeleteEvent(int id)
        {
            var e = _store.Events.FirstOrDefault(x => x.EventID == id);
            if (e == null) return NotFound(new { message = "Event does not exist." });
            var organiser = _store.Organisers.FirstOrDefault(o => o.UserID == CurrentUserID);
            if (organiser == null || e.OrganiserID != organiser.OrganiserID) return Forbid();
            if (_store.EventEnrollments.Any(x => x.EventID == id)) return BadRequest(new { message = "Event has enrolments and cannot be deleted." });
            _store.EventCategories.RemoveAll(x => x.EventID == id);
            _store.Routes.RemoveAll(x => x.EventID == id);
            _store.EventWeather.RemoveAll(x => x.EventID == id);
            _store.Events.Remove(e);
            return NoContent();
        }
    }
}