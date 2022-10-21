using CommandLine.Text;
using CommandLine;

namespace Gitfo
{
    internal class Options
    {
        [Option('f', "fetch", Required = false, HelpText = "Fetch all repos")]
        public bool Fetch { get; set; } = false;
    }
}
