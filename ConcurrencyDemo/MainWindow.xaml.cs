using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Controls;

namespace ConcurrencyDemo
{

    //1.    Start with all the lines commented out. All process is synchronous, hence the UI is blocked
    //      Invoked on threadId:9
    //      Received 1 on threadId:9
    //      Received 2 on threadId:9
    //      Received 3 on threadId:9
    //      Finished on threadId:9
    //      OnComplete on threadId:9
    //2.    Uncomment .SubscribeOn(System.Reactive.Concurrency.NewThreadScheduler.Default) 
    //      Now we have a crash coz the Observable delegate runs on the ThreadPool. In order to fix that uncomment 
    //      the Dispatcher Invoke on OutputText 
    //      Starting on threadId:9
    //      Invoked on threadId:10
    //      Received 1 on threadId:10
    //      Received 2 on threadId:10
    //      Received 3 on threadId:10
    //      Finished on threadId:10
    //      OnComplete on threadId:10
    //3.    This is really ugly, so comment it out again and uncomment the .ObserveOnDispatcher() line
    //      Starting on threadId:8
    //      Invoked on threadId:9
    //      Received 1 on threadId:8
    //      Received 2 on threadId:8
    //      Received 3 on threadId:8
    //      Finished on threadId:9
    //      OnComplete on threadId:8

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                OutputText(String.Format("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId));
                var observable = Observable.Create<string>(o =>
                {
                    Debug.WriteLine("Invoked on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    o.OnNext("1");
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    o.OnNext("2");
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    o.OnNext("3");
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Debug.WriteLine("Finished on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    o.OnCompleted();
                    return Disposable.Empty;

                });
                observable
                    //.SubscribeOn(System.Reactive.Concurrency.NewThreadScheduler.Default)
                    //.ObserveOnDispatcher()
                    .Subscribe(
                    e1 => OutputText(String.Format("Received {0} on threadId:{1}", e1, Thread.CurrentThread.ManagedThreadId)),
                    () => OutputText(String.Format("OnComplete on threadId:{0}", Thread.CurrentThread.ManagedThreadId)));
            };

        }

        private void OutputText(string text)
        {
            Debug.WriteLine(text);
            //Dispatcher.Invoke(() =>
            //{
                ListBox.Items.Add(new TextBlock { Text = text });
            //});
        }
    }
}