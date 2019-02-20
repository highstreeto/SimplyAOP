using System;
using System.Collections.Generic;
using System.Text;
using SimplyAOP.IoCExample.Model;

namespace SimplyAOP.IoCExample.Services
{
    public interface IEventRepository
    {
        IEnumerable<Event> Query(TimeSpan period);

        void Add(Event aEvent);
    }
}
