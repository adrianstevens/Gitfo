namespace Gitfo;

internal static class GitConfigParser
{
    public static string? GetOwner(string dotgitFolderPath)
    {
        var config = File.ReadAllText(Path.Combine(dotgitFolderPath, "config"));
        return ExtractOwnerFromConfig(config);
    }

    public static string? ExtractOwnerFromConfig(string configContent)
    {
        // Find the remote "origin" section and the url within it
        var lines = configContent.Split('\n');
        bool inOriginSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.Equals("[remote \"origin\"]", StringComparison.OrdinalIgnoreCase))
            {
                inOriginSection = true;
                continue;
            }

            if (inOriginSection && trimmedLine.StartsWith("url = "))
            {
                var url = trimmedLine.Substring("url = ".Length).Trim();
                return ExtractOwnerFromUrl(url);
            }

            // If we hit a new section while in origin section, we're done
            if (inOriginSection && trimmedLine.StartsWith("["))
            {
                break;
            }
        }

        return null;
    }

    private static string? ExtractOwnerFromUrl(string url)
    {
        // HTTPS format: https://host.com/owner/repo.git
        if (url.StartsWith("https://") || url.StartsWith("http://"))
        {
            // Strip scheme, then split by '/' — [0]=host, [1]=owner, [2]=repo
            var path = url[(url.IndexOf("://") + 3)..];
            var parts = path.Split('/');
            return parts.Length >= 2 ? parts[1] : null;
        }

        // SSH format: git@host.com:owner/repo.git
        var colonIndex = url.IndexOf(':');
        if (colonIndex > 0 && url.Contains('@'))
        {
            var ownerAndRepo = url[(colonIndex + 1)..].Split('/');
            return ownerAndRepo.Length >= 1 ? ownerAndRepo[0] : null;
        }

        return null;
    }
}
