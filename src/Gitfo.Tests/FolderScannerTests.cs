namespace Gitfo.Tests;

public class FolderScannerTests : IDisposable
{
    private readonly string _root;

    public FolderScannerTests()
    {
        _root = Directory.CreateTempSubdirectory("gitfo-tests-").FullName;
    }

    public void Dispose()
    {
        Directory.Delete(_root, recursive: true);
    }

    private void MakeGitDir(params string[] pathParts)
    {
        var path = Path.Combine([_root, .. pathParts]);
        Directory.CreateDirectory(Path.Combine(path, ".git"));
    }

    private void MakeDir(params string[] pathParts)
    {
        Directory.CreateDirectory(Path.Combine([_root, .. pathParts]));
    }

    [Fact]
    public void FindLocalRepos_EmptyRoot_ReturnsEmpty()
    {
        var results = FolderScanner.FindLocalRepos(_root).ToList();
        Assert.Empty(results);
    }

    [Fact]
    public void FindLocalRepos_NonExistentRoot_ReturnsEmpty()
    {
        var results = FolderScanner.FindLocalRepos(Path.Combine(_root, "does-not-exist")).ToList();
        Assert.Empty(results);
    }

    [Fact]
    public void FindLocalRepos_OneLevelRepo_IsFound()
    {
        MakeGitDir("my-repo");

        var results = FolderScanner.FindLocalRepos(_root).ToList();

        Assert.Single(results);
        Assert.Equal("my-repo", results[0].Name);
        Assert.Equal("(local)", results[0].Owner);
    }

    [Fact]
    public void FindLocalRepos_TwoLevelRepo_IsFoundWithOwner()
    {
        MakeGitDir("adrianstevens", "Gitfo");

        var results = FolderScanner.FindLocalRepos(_root).ToList();

        Assert.Single(results);
        Assert.Equal("Gitfo", results[0].Name);
        Assert.Equal("adrianstevens", results[0].Owner);
    }

    [Fact]
    public void FindLocalRepos_MixedLayout_FindsBoth()
    {
        MakeGitDir("standalone-repo");
        MakeGitDir("owner-a", "repo-1");
        MakeGitDir("owner-a", "repo-2");

        var results = FolderScanner.FindLocalRepos(_root).ToList();

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void FindLocalRepos_DirectoryWithoutGit_IsSkipped()
    {
        MakeDir("not-a-repo");
        MakeGitDir("real-repo");

        var results = FolderScanner.FindLocalRepos(_root).ToList();

        Assert.Single(results);
        Assert.Equal("real-repo", results[0].Name);
    }

    [Fact]
    public void FindLocalRepos_MultipleOwners_AllReposFound()
    {
        MakeGitDir("owner-a", "repo-1");
        MakeGitDir("owner-a", "repo-2");
        MakeGitDir("owner-b", "repo-3");

        var results = FolderScanner.FindLocalRepos(_root).ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal(2, results.Count(r => r.Owner == "owner-a"));
        Assert.Single(results, r => r.Owner == "owner-b");
    }
}
