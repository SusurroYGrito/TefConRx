using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Operators
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            var realtimeSeq1 = Observable.Interval(TimeSpan.FromSeconds(3))
                .Select<long, TelephonyEvent>(
                    i =>
                        new Call
                        {
                            Incoming = true,
                            Duration = (int) i,
                            Partner = "John",
                            Timestamp = DateTime.Now

                        });
            var realtimeSeq2 = Observable.Interval(TimeSpan.FromSeconds(4))
                .Select<long, TelephonyEvent>(
                    i =>
                        new Sms
                        {
                            Incoming = true,
                            Text = String.Format("Dude sent you {0} sms", i),
                            Partner = "John",
                            Timestamp = DateTime.Now
                        });
            var realtimeSeq3 = Observable.Interval(TimeSpan.FromSeconds(4))
                .Select<long, TelephonyEvent>(
                    i =>
                        new Sms
                        {
                            Incoming = false,
                            Text = String.Format("Yo, Eric?  {0} sms", i),
                            Partner = "Eric",
                            Timestamp = DateTime.Now
                        });



            var oldEvents = ExistingTelephonyEvents().ToObservable<TelephonyEvent>();
            oldEvents
                .Merge(realtimeSeq1)
                .Merge(realtimeSeq2)
                .Merge(realtimeSeq3)
                .OfType<Sms>()
                .Where(sms => String.Equals("John", sms.Partner)) //Change to Eric for demo
                .ObserveOnDispatcher()
                .Subscribe(sms => this.sms.Items.Add(
                    new TextBlock
                    {
                        Padding = new Thickness(20),
                        FontSize = 20,
                        Text =
                            String.Format("{0} \n{1} at {2} {3}", sms.Text, sms.Incoming ? "Received" : "Sent",
                                sms.Timestamp.ToShortDateString(), sms.Timestamp.ToShortTimeString()),
                        Background =
                            sms.Incoming
                                ? new SolidColorBrush(Colors.DarkOliveGreen)
                                : new SolidColorBrush(Colors.Aquamarine),
                        HorizontalAlignment = sms.Incoming
                            ? HorizontalAlignment.Left
                            : HorizontalAlignment.Right
                    }));
        }


        private IEnumerable<TelephonyEvent> ExistingTelephonyEvents()
        {
            yield return new Call
            {
                Timestamp = DateTime.Now.AddDays(-4),
                Partner = "John",
                Duration = 5,
                Incoming = true
            };
            yield return new Sms
            {
                Timestamp = DateTime.Now.AddDays(-3.5),
                Partner = "John",
                Text = "Going to TefCon?",
                Incoming = true,
            };
            yield return new Call
            {
                Timestamp = DateTime.Now.AddDays(-3),
                Partner = "Eric",
                Duration = 5,
                Incoming = false
            };
            yield return new Sms
            {
                Timestamp = DateTime.Now.AddDays(-2),
                Partner = "Eric",
                Text = "Eric, going to TefCon with John",
                Incoming = false
            };
            yield return new Sms
            {
                Timestamp = DateTime.Now.AddDays(-1),
                Partner = "John",
                Text = "Sure, I'll meet you there",
                Incoming = false
            };
        }
    }
}