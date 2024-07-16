using EventScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventScheduler.Services
{
    public class EventSchedulerService
    {
        private readonly List<Event> _events;
        private readonly List<LocationDuration> _durations;

        public EventSchedulerService(List<Event> events, List<LocationDuration> durations)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _durations = durations ?? throw new ArgumentNullException(nameof(durations));
        }

        public List<Event> GetOptimalEvents(int maxEvents)
        {
            if (maxEvents <= 0) throw new ArgumentException("Maksimum event sayısı 0'dan büyük olmalıdır.", nameof(maxEvents));

            List<Event> selectedEvents = new List<Event>();
            var sortedEvents = _events.OrderByDescending(e => e.Priority).ToList();

            foreach (var currentEvent in sortedEvents)
            {
                if (selectedEvents.Count == maxEvents)
                    break;

                if (IsEventFeasible(currentEvent, selectedEvents))
                {
                    selectedEvents.Add(currentEvent);
                }
            }

            return selectedEvents;
        }

        private bool IsEventFeasible(Event currentEvent, List<Event> selectedEvents)
        {
            foreach (var selectedEvent in selectedEvents)
            {
                if (!CanAttendEvent(currentEvent, selectedEvent))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanAttendEvent(Event currentEvent, Event selectedEvent)
        {
            var travelTime = GetTravelTime(currentEvent.Location, selectedEvent.Location);
            return currentEvent.StartTime >= selectedEvent.EndTime.Add(TimeSpan.FromMinutes(travelTime)) ||
                   selectedEvent.StartTime >= currentEvent.EndTime.Add(TimeSpan.FromMinutes(travelTime));
        }

        private int GetTravelTime(string from, string to)
        {
            var duration = _durations.FirstOrDefault(d => (d.From == from && d.To == to) || (d.From == to && d.To == from));
            return duration?.DurationMinutes ?? int.MaxValue;
        }
    }
}
