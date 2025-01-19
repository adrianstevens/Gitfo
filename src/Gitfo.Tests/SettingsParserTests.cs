namespace Gitfo.Tests;

public class SettingsParserTests
{
    [Fact]
    public void Test1()
    {
        var json = File.ReadAllText("./inputs/.gitfo.json");
        if (GitfoOptions.TryParse(json, out GitfoOptions opts))
        {
        }
    }
}
