using System;
using System.Collections.Generic;
using System.Linq;
using SimplyAOP.IoCExample.Model;

namespace SimplyAOP.IoCExample.Services
{
    public class InMemoryEventRepository : IEventRepository
    {
        private readonly List<Event> events;

        public InMemoryEventRepository() {
            this.events = new List<Event>();
        }

        public IEnumerable<Event> Query(TimeSpan period) {
            var cutoffPoint = DateTime.Now - period;
            return events
                .Where(e => e.When > cutoffPoint)
                .OrderBy(e => e.When);
        }

        public void Add(Event aEvent) {
            events.Add(aEvent);
        }
    }
}
