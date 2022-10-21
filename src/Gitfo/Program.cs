// See https://aka.ms/new-console-template for more information
using LibGit2Sharp;

Console.BackgroundColor = ConsoleColor.Black;

Console.WriteLine("| Hello Gitfo");

string path;

int folderCount = 0;

//update to add -f --fetch as a command line param
//update to add -C --directory as a param
//update to add -c --checkout 
//update to add -p --pull
//update ot add -v --verion
if(args.Length > 0)
{
    //assume the arg is the path
    path = args[0];
}
else
{
    path = "C:\\WL";//  Directory.GetCurrentDirectory();
}

var folders = Directory.GetDirectories(path);

Console.WriteLine("| Repo name                      | Current branch       | Ahead  | Behind | Dirty |");
Console.WriteLine("|--------------------------------|----------------------|--------|--------|-------|");

foreach (var folder in folders)
{
    try
    {
        using (var repo = new Repository(folder))
        {
            foreach (Remote remote in repo.Network.Remotes)
            {
                FetchOptions options = new FetchOptions();
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, options, "");
            }

          //  int delta = repo.Head.TrackedBranch.Commits.Count() - repo.Head.Commits.Count();

            var name = Path.GetFileName(folder).PadRight(30);
            var friendly = repo.Head.FriendlyName.PadRight(20);
            var ahead = $"{repo.Head.TrackingDetails.AheadBy}".PadRight(6);
            var behind = $"{repo.Head.TrackingDetails.BehindBy}".PadRight(6);
            var dirty = $"{repo.RetrieveStatus().IsDirty}".PadRight(5);

            //  Console.WriteLine($"| {name} | {friendly} | {ahead} | {behind} | {dirty} |");

            Console.Write("| ");
            ConsoleWriteWithColor(name, ConsoleColor.White);

            ConsoleWriteWithColor(friendly, repo.Head.IsTracking?ConsoleColor.White:ConsoleColor.Red);
            ConsoleWriteWithColor(ahead, ConsoleColor.White);
            ConsoleWriteWithColor(behind, ConsoleColor.White);
            ConsoleWriteWithColor(dirty, repo.RetrieveStatus().IsDirty?ConsoleColor.Red:ConsoleColor.White);
            Console.WriteLine();

            folderCount++;
        }
    }
    catch (Exception ex)
    {
    //    Console.WriteLine($"Exception: {ex}");
    }
}


if (folderCount == 0)
{
    Console.WriteLine("No git repos found");
}

void ConsoleWriteWithColor(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" | ");
}