using CommandLine;

namespace Gitfo
{
    internal class Options
    {
        [Option('f', "fetch", Required = false, HelpText = "Fetch all repos")]
        public bool Fetch { get; set; } = false;

      //  [Option('p', "pull", Required = false, HelpText = "Pull all repos")]
      //  public bool Pull { get; set; } = false;
    }
}
