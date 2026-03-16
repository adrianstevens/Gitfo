namespace Gitfo;

internal static class FolderScanner
{
    /// <summary>
    /// Finds git repositories up to two levels deep in <paramref name="rootPath"/>,
    /// matching both root/repo and root/owner/repo structures.
    /// </summary>
    public static IEnumerable<Repo> FindLocalRepos(string rootPath)
    {
        var results = new List<Repo>();
        if (!Directory.Exists(rootPath))
            return results;

        foreach (var subDir in Directory.GetDirectories(rootPath))
        {
            if (Directory.Exists(Path.Combine(subDir, ".git")))
            {
                // root/repo layout
                results.Add(MakeRepo("(local)", subDir));
            }
            else
            {
                // root/owner/repo layout — check one level deeper
                foreach (var repoDir in Directory.GetDirectories(subDir))
                {
                    if (Directory.Exists(Path.Combine(repoDir, ".git")))
                    {
                        results.Add(MakeRepo(Path.GetFileName(subDir), repoDir));
                    }
                }
            }
        }

        return results;
    }

    private static Repo MakeRepo(string owner, string repoDir)
    {
        var info = new GitfoConfiguration.RepositoryInfo
        {
            Owner = owner,
            RepoName = Path.GetFileName(repoDir),
            DefaultBranch = "main",
            AuthToken = null
        };
        return new Repo(repoDir, info);
    }
}