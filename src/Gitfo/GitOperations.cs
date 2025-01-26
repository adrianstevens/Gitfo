namespace Gitfo;

internal static class GitOperations
{
    public static int Pull(IEnumerable<Repo> repos, PullOptions options)
    {
        foreach (var repo in repos)
        {
            Console.Write($"| Pull {repo.Name}...");

            if (repo.Pull())
            {
                PrintColored("succeeded", ConsoleColor.Green);
            }
            else
            {
                PrintColored("failed", ConsoleColor.Red);
            }

            Console.WriteLine();
        }

        return 0;
    }

    public static int Sync(IEnumerable<Repo> repos, SyncOptions options)
    {
        foreach (var repo in repos)
        {
            Console.Write($"| Sync {repo.Name}...");

            if (repo.Sync())
            {
                PrintColored("succeeded", ConsoleColor.Green);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("failed");

                if (repo.Status != RepoStatus.Good)
                {
                    Console.Write($" ({repo.Status})");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
        }

        return 0;
    }

    public static int Checkout(IEnumerable<Repo> repos, CheckoutOptions options)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"| Attempting to checkout ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(options.BranchName ?? "[default]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($" for all repos");

        foreach (var repo in repos)
        {
            Console.Write($"| Checkout ");

            var branch = options.BranchName ?? repo.DefaultBranch;

            if (repo.Checkout(branch))
            {
                PrintColored("succeeded", ConsoleColor.Green);
            }
            else
            {
                PrintColored("failed", ConsoleColor.Red);
            }

            Console.Write($" for ");
            PrintColored(repo.Name, ConsoleColor.Yellow);
        }
        Console.WriteLine("|");

        return 0;
    }

    public static int Fetch(IEnumerable<Repo> repos, FetchOptions options)
    {
        foreach (var repo in repos)
        {
            Console.Write($"| Fetch ");

            if (repo.Fetch())
            {
                PrintColored("succeeded", ConsoleColor.Green);
            }
            else
            {
                PrintColored("failed", ConsoleColor.Red);
            }

            Console.WriteLine($" for {repo.Name}");
        }
        Console.WriteLine("|");

        return 0;
    }

    static void PrintColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
}