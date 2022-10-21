using CommandLine;

namespace Gitfo
{
    internal class Options
    {
        [Option('f', "fetch", Required = false, HelpText = "Fetch all repos")]
        public bool Fetch { get; set; } = false;

        [Option('c', "checkout", Required = false, HelpText = "Attempt to checkout branch on all repos")]
        public string CheckoutBranch { get; set; }

        //  [Option('p', "pull", Required = false, HelpText = "Pull all repos")]
        //  public bool Pull { get; set; } = false;
    }
}
