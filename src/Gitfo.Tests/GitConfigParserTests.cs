namespace Gitfo.Tests;

public class GitConfigParserTests
{
    // Helper to build a minimal git config string with a given remote URL
    private static string GitConfig(string url) =>
        $"[core]\n\trepositoryformatversion = 0\n[remote \"origin\"]\n\turl = {url}\n\tfetch = +refs/heads/*:refs/remotes/origin/*\n[branch \"main\"]\n\tremote = origin\n";

    [Theory]
    [InlineData("https://github.com/adrianstevens/Gitfo.git", "adrianstevens")]
    [InlineData("https://gitlab.com/someorg/myproject.git", "someorg")]
    [InlineData("https://bitbucket.org/teamname/repo.git", "teamname")]
    [InlineData("https://dev.azure.com/myorg/myproject/_git/myrepo", "myorg")]
    [InlineData("http://selfhosted.example.com/owner/repo.git", "owner")]
    public void ExtractOwnerFromConfig_HttpsUrls_ReturnsOwner(string url, string expectedOwner)
    {
        var config = GitConfig(url);
        var owner = GitConfigParser.ExtractOwnerFromConfig(config);
        Assert.Equal(expectedOwner, owner);
    }

    [Theory]
    [InlineData("git@github.com:adrianstevens/Gitfo.git", "adrianstevens")]
    [InlineData("git@gitlab.com:someorg/myproject.git", "someorg")]
    [InlineData("git@bitbucket.org:teamname/repo.git", "teamname")]
    public void ExtractOwnerFromConfig_SshUrls_ReturnsOwner(string url, string expectedOwner)
    {
        var config = GitConfig(url);
        var owner = GitConfigParser.ExtractOwnerFromConfig(config);
        Assert.Equal(expectedOwner, owner);
    }

    [Fact]
    public void ExtractOwnerFromConfig_NoOriginSection_ReturnsNull()
    {
        var config = "[core]\n\trepositoryformatversion = 0\n";
        Assert.Null(GitConfigParser.ExtractOwnerFromConfig(config));
    }

    [Fact]
    public void ExtractOwnerFromConfig_OriginSectionWithNoUrl_ReturnsNull()
    {
        var config = "[core]\n\trepositoryformatversion = 0\n[remote \"origin\"]\n\tfetch = +refs/heads/*:refs/remotes/origin/*\n";
        Assert.Null(GitConfigParser.ExtractOwnerFromConfig(config));
    }

    [Fact]
    public void ExtractOwnerFromConfig_StopsAtNextSection()
    {
        // URL appears after a new section header — should not be picked up
        var config = "[core]\n[remote \"origin\"]\n[branch \"main\"]\n\turl = https://github.com/owner/repo.git\n";
        Assert.Null(GitConfigParser.ExtractOwnerFromConfig(config));
    }

    [Fact]
    public void ExtractOwnerFromConfig_WindowsLineEndings_ReturnsOwner()
    {
        var config = "[core]\r\n\trepositoryformatversion = 0\r\n[remote \"origin\"]\r\n\turl = https://github.com/adrianstevens/Gitfo.git\r\n";
        var owner = GitConfigParser.ExtractOwnerFromConfig(config);
        Assert.Equal("adrianstevens", owner);
    }

    [Fact]
    public void ExtractOwnerFromConfig_EmptyString_ReturnsNull()
    {
        Assert.Null(GitConfigParser.ExtractOwnerFromConfig(string.Empty));
    }
}
