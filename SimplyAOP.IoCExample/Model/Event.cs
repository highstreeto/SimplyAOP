using System;

namespace SimplyAOP.IoCExample.Model
{
    public class Event
    {
        public Event(string what)
            : this(DateTime.Now, what, Environment.UserName) { }

        public Event(DateTime when, string what, string who) {
            When = when;
            What = what;
            Who = who;
        }

        public DateTime When { get; }
        public string What { get; }
        public string Who { get; }

        public override string ToString() {
            return $"[{When}]: <{Who}> {What}";
        }
    }
}
