/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "Q")
        {
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private static void CalculateSum(int n)
    {
        Task<long> sumTask;
        CancellationTokenSource cts = new CancellationTokenSource();

        bool sumTaskCompleted = false;

        do
        {
            sumTask = StartCalculation(n, cts);

            int newN;
            do
            {
                Console.Write("\nEnter N to cancel the request: ");
                int.TryParse(Console.ReadLine(), out newN);
            } while (newN <= 0 && !sumTask.IsCompleted);

            if (!sumTask.IsCompleted)
            {
                Console.WriteLine($"\nSum for {n} cancelled...");
                cts.Cancel();

                n = newN;
                continue;
            }

            if (sumTask.IsCompletedSuccessfully)
            {
                Console.WriteLine($"\nThe previous sum for {n} has already completed. Sum = {sumTask.Result}");
                sumTaskCompleted = true;
            }
            else
            {
                Console.WriteLine($"\nThere was an error while processing the sum for {n}");
            }

        } while (!sumTaskCompleted);

        Console.WriteLine($"\nEnter N to start a new sum request:");
    }

    private static Task<long> StartCalculation(int n, CancellationTokenSource cts)
    {
        if (cts.Token.IsCancellationRequested)
        {
            cts = new CancellationTokenSource();
        }

        Console.WriteLine($"\nThe task for {n} started...");
        return Calculator.Calculate(n, cts.Token);
    }
}
