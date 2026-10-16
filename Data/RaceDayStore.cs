using Assignment2.Models;

namespace Assignment2.Data
{
    public class RaceDayStore
    {
        public List<User> Users { get; } = new();
        public List<Organiser> Organisers { get; } = new();
        public List<Participant> Participants { get; } = new();
        public List<Event> Events { get; } = new();
        public List<Category> Categories { get; } = new();
        public List<EventCategory> EventCategories { get; } = new();
        public List<Assignment2.Models.Route> Routes { get; set; } = new();
        public List<EventEnrollment> EventEnrollments { get; } = new();
        public List<Result> Results { get; } = new();
        public List<EventWeather> EventWeather { get; } = new();

        private int _nextUserId = 1;
        private int _nextOrganiserId = 1;
        private int _nextParticipantId = 1;
        private int _nextEventId = 1;
        private int _nextCategoryId = 1;
        private int _nextRouteId = 1;
        private int _nextEnrollmentId = 1;
        private int _nextResultId = 1;
        private int _nextWeatherId = 1;

        public int GetNextUserId() => _nextUserId++;
        public int GetNextOrganiserId() => _nextOrganiserId++;
        public int GetNextParticipantId() => _nextParticipantId++;
        public int GetNextEventId() => _nextEventId++;
        public int GetNextCategoryId() => _nextCategoryId++;
        public int GetNextRouteId() => _nextRouteId++;
        public int GetNextEnrollmentId() => _nextEnrollmentId++;
        public int GetNextResultId() => _nextResultId++;
        public int GetNextWeatherId() => _nextWeatherId++;
    }
}