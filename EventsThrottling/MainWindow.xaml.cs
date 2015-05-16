using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsThrottling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDisposable _subscripption;

        public MainWindow()
        {
            InitializeComponent();

            _subscripption = Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>
                (h => textBox.TextChanged += h,
                 h => textBox.TextChanged -= h)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOnDispatcher()
                .Subscribe(_ => textBlockThrottled.Text = textBox.Text);
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            textBlockUnthrottled.Text = textBox.Text;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _subscripption.Dispose();
        }
    }
}