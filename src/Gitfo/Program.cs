using CommandLine;
using Gitfo;
using LibGit2Sharp;

const int NameWidth = 30;

string path;

int folderCount = 0;

Console.BackgroundColor = ConsoleColor.Black;

Console.WriteLine("| Hello Gitfo v0.1.0");
Console.WriteLine("|");

Options o = Parser.Default.ParseArguments<Options>(args).Value;

//update to add -f --fetch as a command line param
//update to add -C --directory as a param
//update to add -c --checkout 
//update to add -p --pull
//update ot add -v --version
path = Environment.CurrentDirectory;

var folders = Directory.GetDirectories(path);

Console.WriteLine("| Repo name                      | Current branch                 | Ahead  | Behind | Dirty |");
Console.WriteLine("|--------------------------------|--------------------------------|--------|--------|-------|");

foreach (var folder in folders)
{
    try
    {
        using (var repo = new Repository(folder))
        {
            foreach (Remote remote in repo.Network.Remotes)
            {
                try
                {
                    if(o.Fetch)
                    {
                        FetchOptions options = new FetchOptions();
                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, options, "");
                    }
                }
                catch
                {

                }
            }

          //  int delta = repo.Head.TrackedBranch.Commits.Count() - repo.Head.Commits.Count();

            var name = Path.GetFileName(folder).PadRight(NameWidth);
            var friendly = repo.Head.FriendlyName.PadRight(NameWidth);
            var ahead = $"{repo.Head.TrackingDetails.AheadBy}".PadRight(6);
            var behind = $"{repo.Head.TrackingDetails.BehindBy}".PadRight(6);
            var dirty = $"{repo.RetrieveStatus().IsDirty}".PadRight(5);

            //  Console.WriteLine($"| {name} | {friendly} | {ahead} | {behind} | {dirty} |");

            Console.Write("| ");
            ConsoleWriteWithColor(name, ConsoleColor.White);

            ConsoleWriteWithColor(friendly, ahead[0] == ' ' ? ConsoleColor.Yellow:ConsoleColor.White);
            ConsoleWriteWithColor(ahead, ahead[0] == '0' ? ConsoleColor.White : ConsoleColor.Cyan);
            ConsoleWriteWithColor(behind, behind[0] == '0' ? ConsoleColor.White : ConsoleColor.Cyan);
            ConsoleWriteWithColor(dirty, repo.RetrieveStatus().IsDirty?ConsoleColor.Red:ConsoleColor.White);
            Console.WriteLine();

            folderCount++;
        }
    }
    catch(RepositoryNotFoundException ex)
    {

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {folder} - {ex.Message}");
    }
}


if (folderCount == 0)
{
    Console.WriteLine("| No git repos found");
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