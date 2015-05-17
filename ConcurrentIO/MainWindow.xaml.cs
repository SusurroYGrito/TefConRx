using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;

namespace ConcurrentIO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableService _observableService;
        private readonly ObservableServiceAsync _observableServiceAsync;

        public MainWindow()
        {
            InitializeComponent();
            _observableService = new ObservableService();
            _observableServiceAsync = new ObservableServiceAsync();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _observableService.GetQuote("msft")
                .SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(quote => this.quote.Text = quote.ToString());
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            _observableServiceAsync.GetQuote("msft")
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(quote => this.quote2.Text = quote.ToString());
        }
    }
}
