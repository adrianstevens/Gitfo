using CommandLine;

namespace Gitfo;

internal class BaseOptions
{
    [Option('p', "profile", Required = false, HelpText = "Profile to use")]
    public string? ProfileName { get; set; }
}

[Verb("status", HelpText = "Get the current status of all repositories")]
internal class StatusOptions : BaseOptions
{
}

[Verb("sync", HelpText = "Synchronize repositories according to configuration")]
internal class SyncOptions : BaseOptions
{
    [Option('a', "all", Required = false, HelpText = "Sync all repositories in all profiles")]
    public bool SyncAll { get; set; }
}

[Verb("fetch", HelpText = "Fetch latest changes without merging")]
internal class FetchOptions : BaseOptions
{
    [Option('a', "all", Required = false, HelpText = "Fetch all remotes")]
    public bool FetchAll { get; set; }
}

[Verb("pull", HelpText = "Pull latest changes for repositories")]
internal class PullOptions : BaseOptions
{
}

[Verb("checkout", HelpText = "Checkout specific branch for repositories")]
internal class CheckoutOptions : BaseOptions
{
    [Option('b', "branch", Required = false, HelpText = "Branch name to checkout")]
    public string? BranchName { get; set; }
}

[Verb("generate", HelpText = "Generate a .gitfo configuration file from an existing folder structure")]
internal class GenerateOptions : BaseOptions
{
}
