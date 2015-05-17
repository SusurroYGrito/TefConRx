using System;

namespace Operators
{
    public abstract class TelephonyEvent
    {
        public string Partner { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Incoming { get; set; }
    }

    public class Sms : TelephonyEvent
    {
        public string Text { get; set; }
    }

    public class Call : TelephonyEvent
    {
        public int Duration { get; set; }
    }
}
