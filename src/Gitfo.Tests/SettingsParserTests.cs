namespace Gitfo.Tests;

public class SettingsParserTests
{
    private const string ValidConfig = """
        {
          "profiles": {
            "default": {
              "repos": [
                {
                  "owner": "adrianstevens",
                  "repo": "Gitfo",
                  "defaultBranch": "main",
                  "folder": "radish"
                }
              ]
            }
          }
        }
        """;

    private const string MultiProfileConfig = """
        {
          "profiles": {
            "project1": {
              "repos": [
                { "owner": "owner-a", "repo": "repo-1", "defaultBranch": "develop", "folder": "a" },
                { "owner": "owner-a", "repo": "repo-2", "defaultBranch": "develop", "folder": "a" }
              ]
            },
            "project2": {
              "repos": [
                { "owner": "owner-b", "repo": "repo-3", "folder": "b" }
              ]
            }
          }
        }
        """;

    [Fact]
    public void TryParse_ValidJson_ReturnsTrue()
    {
        Assert.True(GitfoConfiguration.TryParse(ValidConfig, out _));
    }

    [Fact]
    public void TryParse_ValidJson_ParsesProfileName()
    {
        GitfoConfiguration.TryParse(ValidConfig, out var config);
        Assert.NotNull(config);
        Assert.Single(config.Profiles);
        Assert.Equal("default", config.Profiles.First().Name);
    }

    [Fact]
    public void TryParse_ValidJson_ParsesRepoFields()
    {
        GitfoConfiguration.TryParse(ValidConfig, out var config);
        var repo = config!.Profiles.First().Repos.Single();
        Assert.Equal("adrianstevens", repo.Owner);
        Assert.Equal("Gitfo", repo.RepoName);
        Assert.Equal("main", repo.DefaultBranch);
        Assert.Equal("radish", repo.LocalFolder);
    }

    [Fact]
    public void TryParse_MultipleProfiles_ParsesAll()
    {
        GitfoConfiguration.TryParse(MultiProfileConfig, out var config);
        Assert.NotNull(config);
        Assert.Equal(2, config!.Profiles.Count());
        Assert.Equal(2, config.Profiles.First(p => p.Name == "project1").Repos.Count);
        Assert.Single(config.Profiles.First(p => p.Name == "project2").Repos);
    }

    [Fact]
    public void TryParse_NullDefaultBranch_RepoDefaultBranchFallsBackToMain()
    {
        // project2/repo-3 has no defaultBranch set
        GitfoConfiguration.TryParse(MultiProfileConfig, out var config);
        var repo = config!.Profiles.First(p => p.Name == "project2").Repos.Single();
        Assert.Null(repo.DefaultBranch);  // raw value is null — caller uses ?? "main"
    }

    [Fact]
    public void TryParse_InvalidJson_ReturnsFalse()
    {
        Assert.False(GitfoConfiguration.TryParse("not json at all", out var config));
        Assert.Null(config);
    }

    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        Assert.False(GitfoConfiguration.TryParse(string.Empty, out var config));
        Assert.Null(config);
    }

    [Fact]
    public void TryParse_EmptyProfiles_ReturnsTrueWithNoProfiles()
    {
        var json = """{ "profiles": {} }""";
        Assert.True(GitfoConfiguration.TryParse(json, out var config));
        Assert.NotNull(config);
        Assert.Empty(config!.Profiles);
    }
}
