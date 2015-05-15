using System;
using System.Reactive.Disposables;

namespace SimplestEver
{

    public class MyConsoleObserver<T> : IObserver<T>
    {
        public void OnNext(T value)
        {
            Console.WriteLine("Received value {0}", value);
        }
        public void OnError(Exception error)
        {
            Console.WriteLine("Sequence faulted with {0}", error);
        }
        public void OnCompleted()
        {
            Console.WriteLine("Sequence terminated");
        }
    }

    public class MySequenceOfNumbers : IObservable<int>
    {
        public IDisposable Subscribe(IObserver<int> observer)
        {
            observer.OnNext(1);
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            var numbers = new MySequenceOfNumbers();
            var observer = new MyConsoleObserver<int>();
            Console.WriteLine("Press any key to subscribe");
            Console.ReadKey();
            numbers.Subscribe(observer);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
