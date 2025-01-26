namespace Gitfo;

internal static class FolderScanner
{
    /// <summary>
    /// Finds all subdirectories (one level deep) in <paramref name="rootPath"/>
    /// that contain a .git folder, and creates Repo objects for them.
    /// </summary>
    public static IEnumerable<Repo> FindLocalRepos(string rootPath)
    {
        var results = new List<Repo>();
        if (!Directory.Exists(rootPath))
            return results;

        // Look one level deep from rootPath
        string[] subDirs = Directory.GetDirectories(rootPath);
        foreach (var subDir in subDirs)
        {
            string gitDir = Path.Combine(subDir, ".git");
            if (Directory.Exists(gitDir))
            {
                // Use a "dummy" RepositoryInfo so we can instantiate a Repo class
                var dummyInfo = new GitfoConfiguration.RepositoryInfo
                {
                    Owner = "(local)",
                    RepoName = Path.GetFileName(subDir),
                    DefaultBranch = "main",
                    AuthToken = null
                };

                var repo = new Repo(subDir, dummyInfo);
                results.Add(repo);
            }
        }

        return results;
    }
}