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
        // Handle both HTTPS and SSH URLs

        // HTTPS format: https://github.com/owner/repo.git
        if (url.Contains("github.com/"))
        {
            var parts = url.Split(new[] { "github.com/" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var ownerAndRepo = parts[1].Split('/');
                if (ownerAndRepo.Length >= 1)
                {
                    return ownerAndRepo[0];
                }
            }
        }

        // SSH format: git@github.com:owner/repo.git
        if (url.Contains("github.com:"))
        {
            var parts = url.Split(new[] { "github.com:" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var ownerAndRepo = parts[1].Split('/');
                if (ownerAndRepo.Length >= 1)
                {
                    return ownerAndRepo[0];
                }
            }
        }

        return null;
    }
}
