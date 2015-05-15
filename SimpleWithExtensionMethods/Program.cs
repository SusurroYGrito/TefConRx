using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SimpleWithExtensionMethods
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var numbers = Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                observer.OnCompleted();
                return Disposable.Empty;
            });

            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            numbers.Subscribe(
                value => Console.WriteLine("Received value {0}", value),
                error => Console.WriteLine("Sequence faulted with {0}", error),
                () => Console.WriteLine("Sequence terminated"));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
