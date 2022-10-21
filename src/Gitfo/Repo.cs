using LibGit2Sharp;

namespace Gitfo
{
    internal class Repo
    {
        public string Name { get; set; }

        public string Folder { get; protected set; }

        public bool IsGitRepo { get; protected set; } = false;

        public string Branch { get; set; }

        public bool IsPrivate { get; set; }

        public bool HasRemote { get; set; } = false;

        public int? Ahead { get; set; }

        public int? Behind { get; set; }

        public bool IsDirty { get; protected set; }

        public Repo(string folder)
        {
            Folder = folder;
            Name = Path.GetFileName(folder);

            try
            {
                using var repo = new Repository(folder);

                IsGitRepo = true;

                Branch = repo.Head.FriendlyName;
                HasRemote = true;

                Ahead = repo.Head.TrackingDetails.AheadBy;
                Behind = repo.Head.TrackingDetails.BehindBy;
                IsDirty = repo.RetrieveStatus().IsDirty;
            }
            catch
            {

            }
        }

        public bool Pull()
        {


            if (IsPrivate || HasRemote == false)
            {
                return false;
            }

            using var repo = new Repository(Folder);

            if(Pull() == false) return false;

            foreach (Remote remote in repo.Network.Remotes)
            {
                try
                {
                    var options = new LibGit2Sharp.PullOptions();
                    var signature = new Signature(new Identity(Secrets.UserName, Secrets.Email), DateTimeOffset.Now);

                    Commands.Pull(repo, signature, options);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public bool Fetch()
        {
            if(IsPrivate || HasRemote == false)
            {
                return false;
            }

            using var repo = new Repository(Folder);

            foreach (Remote remote in repo.Network.Remotes)
            {
                try
                {
                    FetchOptions options = new FetchOptions();
                    IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repo, remote.Name, refSpecs, options, "");
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
