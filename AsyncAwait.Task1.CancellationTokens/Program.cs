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
    private static CancellationTokenSource _cts = new CancellationTokenSource();

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

        do
        {
            sumTask = Calculator.Calculate(n, _cts.Token);
            Console.WriteLine($"\nThe task for {n} started...");

            int newN = 0;
            do
            {
                Console.Write("Enter N to cancel the request: ");
                int.TryParse(Console.ReadLine(), out newN);
            } while (newN <= 0 && !sumTask.IsCompleted);

            if (!sumTask.IsCompleted)
            {
                Console.WriteLine($"\nSum for {n} cancelled...");
                _cts.Cancel();

                n = newN;
            }

        } while (!sumTask.IsCompleted);

        if (sumTask.IsCompletedSuccessfully)
        {
            Console.WriteLine($"The sum for {n} = {sumTask.Result}");
        }
        else
        {
            Console.WriteLine($"There was an error while processing sum for {n}");
        }

        Console.WriteLine($"Enter N to start a new sum request:");
    }
}
