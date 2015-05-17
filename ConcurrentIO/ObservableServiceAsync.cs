using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ConcurrentIO
{
    public class ObservableServiceAsync
    {
        public IObservable<int> GetQuote(string symbol)
        {
            return Observable.FromAsync(() => GetQuoteTask(symbol));
        }

        private async Task<int> GetQuoteTask(string symbol)
        {
            var response = await new HttpClient().GetAsync("http://localhost:4106/quotes/" + symbol);
            var readAsStringAsync = await response.Content.ReadAsStringAsync();
            return int.Parse(readAsStringAsync);
        }
    }

    public class ObservableService
    {
        public IObservable<int> GetQuote(string symbol)
        {
            return Observable.Create<int>(
                observer =>
                {
                    var downloadString = new WebClient().DownloadString("http://localhost:4106/quotes/" + symbol);
                    var quote = int.Parse(downloadString);
                    observer.OnNext(quote);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
        }
    }
}
