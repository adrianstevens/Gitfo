using CommandLine;
using Gitfo;

const int NameWidth = 30;
const int PropertyWidth = 6;

string path;

List<Repo> repos;

Console.BackgroundColor = ConsoleColor.Black;

//update to add -f --fetch as a command line param
//update to add -C --directory as a param
//update to add -c --checkout 
//update to add -p --pull
//update ot add -v --version
path = "C:\\WL"; // Environment.CurrentDirectory;

Console.WriteLine("| Hello Gitfo v0.1.1");
Console.WriteLine("|");

Options o = Parser.Default.ParseArguments<Options>(args).Value;

LoadRepos(path);

if(o.Fetch)
{
    Fetch(repos);
}

ShowGitfoTable(repos);

void LoadRepos(string path)
{
    repos = new List<Repo>();

    var folders = Directory.GetDirectories(path);

    foreach(var folder in folders)
    {
        var repo = new Repo(folder);

        if (repo.IsGitRepo == true)
        {
            repos.Add(repo);
        }
    }
}

void Pull(List<Repo> repos)
{
    foreach (var repo in repos)
    {
        Console.Write($"| Pull ");

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

        Console.WriteLine($" for {repo.Name}");
    }
    Console.WriteLine("|");
}

void Fetch(List<Repo> repos)
{
    foreach(var repo in repos)
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
}

void ShowGitfoTable(List<Repo> repos)
{
    Console.WriteLine($"| {"Repo name".PadRight(NameWidth)} | {"Current branch".PadRight(NameWidth)} | {"Ahead".PadRight(PropertyWidth)} | {"Behind".PadRight(PropertyWidth)} | {"Dirty".PadRight(PropertyWidth)} |");
    Console.WriteLine($"| {"".PadRight(NameWidth, '-')} | {"".PadRight(NameWidth, '-')} | {"".PadRight(PropertyWidth, '-')} | {"".PadRight(PropertyWidth, '-')} | {"".PadRight(PropertyWidth, '-')} |");

    foreach(var repo in repos)
    {
        var name = repo.Name.PadRight(NameWidth);
        var friendly = repo.Branch.PadRight(NameWidth);
        var ahead = $"{repo.Ahead}".PadRight(PropertyWidth);
        var behind = $"{repo.Behind}".PadRight(PropertyWidth);
        var dirty = $"{repo.IsDirty}".PadRight(PropertyWidth);

        Console.Write("| ");
        ConsoleWriteWithColor(name, ConsoleColor.White);

        ConsoleWriteWithColor(friendly, ahead[0] == ' ' ? ConsoleColor.Yellow : ConsoleColor.White);
        ConsoleWriteWithColor(ahead, ahead[0] == '0' ? ConsoleColor.White : ConsoleColor.Cyan);
        ConsoleWriteWithColor(behind, behind[0] == '0' ? ConsoleColor.White : ConsoleColor.Cyan);
        ConsoleWriteWithColor(dirty, repo.IsDirty ? ConsoleColor.Red : ConsoleColor.White);
        Console.WriteLine();
    }

    if(repos.Count == 0)
    {
        Console.WriteLine("| No git repos found");
    }
}

void ConsoleWriteWithColor(string text, ConsoleColor color)
{
    if(text.Length > NameWidth)
    {
        text = string.Concat(text.AsSpan(0, NameWidth - 3), "...");
    }

    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write(" | ");
}