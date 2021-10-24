using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpTests_ParallelForEach
{
    internal class Program
    {
        private static async Task Main()
        {
            // 2 million
            var limit = 20_000_000;
            var numbers = Enumerable.Range(0, limit).ToList();

            await One(numbers);
            Two(numbers);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static async Task One(List<int> numbers)
        {
            var watch = Stopwatch.StartNew();

            var stopwatches = await Task.WhenAll(
                Task.Run(() => GetTimeClassical(numbers)),
                Task.Run(() => GetTimeParallel(numbers)));

            Stopwatch watchClassical = stopwatches[0];
            Stopwatch watchForParallel = stopwatches[1];

            Console.WriteLine($"Classical foreach loop | Time Taken : {watchClassical.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel.ForEach loop  | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");

            watch.Stop();
            Console.WriteLine($"Time 1: {watch.ElapsedMilliseconds} ms.");
        }

        private static void Two(List<int> numbers)
        {
            var watch = Stopwatch.StartNew();

            Stopwatch watchClassical = GetTimeClassical(numbers);
            Stopwatch watchForParallel = GetTimeParallel(numbers);

            Console.WriteLine($"Classical foreach loop | Time Taken : {watchClassical.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel.ForEach loop  | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");

            watch.Stop();
            Console.WriteLine($"Time 2: {watch.ElapsedMilliseconds} ms.");
        }

        private static Stopwatch GetTimeParallel(List<int> numbers)
        {
            var watchForParallel = Stopwatch.StartNew();
            GetPrimeListWithParallel(numbers);
            watchForParallel.Stop();
            return watchForParallel;
        }

        private static Stopwatch GetTimeClassical(List<int> numbers)
        {
            var watch = Stopwatch.StartNew();
            GetPrimeList(numbers);
            watch.Stop();
            return watch;
        }

        /// <summary>
        /// GetPrimeList returns Prime numbers by using sequential ForEach
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeList(IList<int> numbers) => numbers.Where(IsPrime).ToList();

        /// <summary>
        /// GetPrimeListWithParallel returns Prime numbers by using Parallel.ForEach
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
        {
            var primeNumbers = new ConcurrentBag<int>();

            Parallel.ForEach(numbers, number =>
            {
                if (IsPrime(number))
                {
                    primeNumbers.Add(number);
                }
            });

            return primeNumbers.ToList();
        }

        /// <summary>
        /// IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}