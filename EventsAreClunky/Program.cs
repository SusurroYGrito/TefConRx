using System;

namespace EventsAreClunky
{
    public class EventSource
    {
        //Declaración de un evento
        public event EventHandler<char> AnEvent;

        public void Start()
        {
            while (true)
            {
                var c = Console.ReadKey(true);
                AnEvent(this, c.KeyChar);
            }
        }
    }

    public class EventConsumer
    {
        public EventConsumer(EventSource eventSource)
        {
            //Subscripción a un evento
            eventSource.AnEvent += AnEventHandler;
        }

        //Manejador del evento
        private void AnEventHandler(object sender, char e)
        {
            Console.WriteLine(e);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var source = new EventSource();
            var consumer = new EventConsumer(source);
            source.Start();
        }
    }
}
