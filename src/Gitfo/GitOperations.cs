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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("succeeded");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("failed");
                Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("succeeded");
                Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("succeeded");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("failed");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write($" for ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(repo.Name);
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("succeeded");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("failed");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine($" for {repo.Name}");
        }
        Console.WriteLine("|");

        return 0;
    }
}