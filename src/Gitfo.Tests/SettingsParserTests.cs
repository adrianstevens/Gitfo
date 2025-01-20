namespace Gitfo.Tests;

public class SettingsParserTests
{
    [Fact]
    public void Test1()
    {
        var json = File.ReadAllText("./inputs/.gitfo.json");
        if (GitfoConfiguration.TryParse(json, out GitfoConfiguration opts))
        {
        }
    }
}
