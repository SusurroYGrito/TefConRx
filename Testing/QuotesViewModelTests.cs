using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Testing
{
    public interface IQuoteService
    {
        IObservable<int> GetQuote(string symbol);
    }

    public interface ISchedulers
    {
        IScheduler NewThread { get; set; }
        IScheduler Dispatcher { get; set; }
    }

    public class QuotesViewModel
    {
        private readonly IQuoteService _quoteService;
        private readonly ISchedulers _schedulers;

        public QuotesViewModel(IQuoteService quoteService, ISchedulers schedulers)
        {
            _quoteService = quoteService;
            _schedulers = schedulers;
        }

        public int Quote { get; set; }

        public bool Timedout { get; set; }

        public void StartGetQuote(string symbol)
        {
            Timedout = false;
            _quoteService.GetQuote(symbol)
                .SubscribeOn(_schedulers.NewThread)
                .ObserveOn(_schedulers.Dispatcher)
                .Timeout(TimeSpan.FromSeconds(10), _schedulers.NewThread)
                .Subscribe(quote => Quote = quote,
                ex =>
                {
                    if (ex is TimeoutException)
                        Timedout = true;
                });
        }
    }


    [TestClass]
    public class QuotesViewModelTests
    {
        [TestMethod]
        public void QuotesViewModel_QuoteIsSet_WhenReturnedByService()
        {
            var schedulers = MockRepository.GenerateStub<ISchedulers>();
            var testNewThreadScheduler = new TestScheduler();
            schedulers.NewThread = testNewThreadScheduler;
            var testDispatcherScheduler = new TestScheduler();
            schedulers.Dispatcher = testDispatcherScheduler;
            var service = MockRepository.GenerateStub<IQuoteService>();
            var subject = new Subject<int>();
            service.Stub(s => s.GetQuote(Arg<string>.Is.Anything)).Return(subject);
            var viewmodel = new QuotesViewModel(service, schedulers);

            viewmodel.StartGetQuote("msft");
            viewmodel.Quote.Should().Be(0);

            testNewThreadScheduler.Schedule(() => subject.OnNext(105));
            testNewThreadScheduler.AdvanceBy(1);
            viewmodel.Quote.Should().Be(0); //Still Zero

            testDispatcherScheduler.AdvanceBy(1);
            viewmodel.Quote.Should().Be(105);
        }


        [TestMethod]
        public void QuotesViewModel_TimedoutIsSet_WhenServiceTimesOut()
        {
            var timeoutPeriod = TimeSpan.FromSeconds(10);

            var schedulers = MockRepository.GenerateStub<ISchedulers>();
            var testNewThreadScheduler = new TestScheduler();
            schedulers.NewThread = testNewThreadScheduler;
            var testDispatcherScheduler = new TestScheduler();
            schedulers.Dispatcher = testDispatcherScheduler;
            var service = MockRepository.GenerateStub<IQuoteService>();
            service.Stub(s => s.GetQuote(Arg<string>.Is.Anything)).Return(Observable.Never<int>());
            var viewmodel = new QuotesViewModel(service, schedulers);

            viewmodel.StartGetQuote("msft");
            viewmodel.Timedout.Should().BeFalse();

            testNewThreadScheduler.AdvanceBy(timeoutPeriod.Ticks - 1);
            viewmodel.Timedout.Should().BeFalse();//Still false

            testNewThreadScheduler.AdvanceBy(timeoutPeriod.Ticks);
            viewmodel.Timedout.Should().BeTrue();
        }
    }
}
