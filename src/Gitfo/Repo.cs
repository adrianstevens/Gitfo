using LibGit2Sharp;

namespace Gitfo;

internal class RepoOptions
{
    /// <summary>
    /// The default branch to use when cloning the repository
    /// </summary>
    public string? DefaultBranch { get; set; }
}

internal enum RepoStatus
{
    Good,
    DirectoryMissing,
    AuthenticationFailed,
    Conflict,
    MergeHeadNotFound,
    NoRemote
}

internal class Repo
{
    private readonly GitfoConfiguration.RepositoryInfo _repoInfo;

    public string Name { get; set; }

    public string Folder { get; protected set; }

    public bool IsGitRepo { get; protected set; } = false;

    public string CurentBranch { get; set; }

    public bool IsPrivate { get; set; }

    public bool HasRemote { get; set; } = false;

    public int? Ahead { get; set; }

    public int? Behind { get; set; }

    public bool IsDirty { get; protected set; }

    public RepoOptions Options { get; set; }

    public RepoStatus Status { get; set; }

    public string Owner => _repoInfo.Owner;
    public string DefaultBranch => _repoInfo.DefaultBranch ?? "main";

    public Repo(string folder, GitfoConfiguration.RepositoryInfo repoInfo)
    {
        _repoInfo = repoInfo;

        Folder = folder;
        Name = Path.GetFileName(folder);

        Initialize();

        CurentBranch ??= repoInfo.DefaultBranch ?? "[unknown]";
    }

    private void Initialize()
    {
        Status = RepoStatus.Good;

        try
        {
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            using var repo = new Repository(Folder);

            IsGitRepo = true;

            CurentBranch = repo.Head.FriendlyName;

            HasRemote = repo.Head.IsTracking;

            Ahead = repo.Head.TrackingDetails.AheadBy;
            Behind = repo.Head.TrackingDetails.BehindBy;
            IsDirty = repo.RetrieveStatus().IsDirty;
        }
        catch (RepositoryNotFoundException)
        {
            Status = RepoStatus.DirectoryMissing;
        }
        catch (Exception)
        {
            Status = RepoStatus.DirectoryMissing;
        }
    }

    public bool Checkout(string branch)
    {
        using var repo = new Repository(Folder);

        Branch newBranch;

        try
        {
            newBranch = Commands.Checkout(repo, branch);
            Initialize();
        }
        catch (Exception ex)
        {
            Console.Write($"{ex.Message} ");
            return false;
        }

        return newBranch != null;
    }

    public bool Pull()
    {
        using var repo = new Repository(Folder);

        // Get the current branch
        var branch = repo.Head;
        var remote = repo.Network.Remotes["origin"];

        // Pull options including merge options
        var options = new LibGit2Sharp.PullOptions
        {
            FetchOptions = new LibGit2Sharp.FetchOptions
            {
                CredentialsProvider = null
            },
            MergeOptions = new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.Default
            }
        };

        // Set up authentication if token provided
        if (!string.IsNullOrEmpty(_repoInfo.AuthToken))
        {
            options.FetchOptions.CredentialsProvider = (_url, _user, _cred) =>
                new UsernamePasswordCredentials
                {
                    Username = _repoInfo.AuthToken,
                    Password = string.Empty
                };
        }

        // Pull changes
        try
        {
            Commands.Pull(
                repo,
                // TODO: put this in the .gitfo file
                new Signature("gitfo", "gitfo@local", DateTimeOffset.Now), // Signature for merge commit if needed
                options
            );
        }
        catch (CheckoutConflictException)
        {
            Status = RepoStatus.Conflict;
            return false;
        }
        catch (MergeFetchHeadNotFoundException)
        {
            Status = RepoStatus.MergeHeadNotFound;
            return false;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("authentication"))
            {
                Status = RepoStatus.AuthenticationFailed;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Either Clone or Pull the repo, depending on if it's already locally available
    /// </summary>
    /// <returns></returns>
    public bool Sync()
    {
        if (!Directory.Exists(Path.Combine(Folder, ".git")))
        {
            return Clone();
        }
        else
        {
            return Pull();
        }
    }

    private bool Clone()
    {
        // Construct the repository URL
        var repoUrl = $"https://github.com/{Owner}/{Name}.git";

        var options = new CloneOptions
        {
            Checkout = true,
        };

        // Set up authentication if token provided
        if (!string.IsNullOrEmpty(_repoInfo.AuthToken))
        {
            options.FetchOptions.CredentialsProvider = (_url, _user, _cred) =>
                new UsernamePasswordCredentials
                {
                    Username = _repoInfo.AuthToken,
                    Password = string.Empty
                };
        }

        // Set specific branch if provided
        if (!string.IsNullOrEmpty(CurentBranch))
        {
            options.BranchName = CurentBranch;
        }

        // Clone the repository
        try
        {
            Repository.Clone(repoUrl, Folder, options);
        }
        catch (LibGit2SharpException ex)
        {
            if (ex.Message.Contains("authentication"))
            {
                Status = RepoStatus.AuthenticationFailed;
            }

            return false;
        }

        return true;
    }

    public bool Fetch()
    {
        if (HasRemote == false)
        {
            Status = RepoStatus.NoRemote;
            return false;
        }

        using var repo = new Repository(Folder);

        foreach (Remote remote in repo.Network.Remotes)
        {
            try
            {
                var options = new LibGit2Sharp.FetchOptions();
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, options, "");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("authentication"))
                {
                    Status = RepoStatus.AuthenticationFailed;
                }

                return false;
            }
        }

        return true;
    }
}
