using CommandLine;

namespace FastFileReadonly
{
    /// <summary>
    /// CommandLineParser用オプションクラス
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Readonlyにしたいディレクトリのパス
        /// </summary>
        [Value(0, MetaName = "ChangeReadonlyDirectoryPath", HelpText = "The directory to change to read-only.", Required = true)]
        public string ChangeReadonlyDirectoryPath { get; set; }
    }
}
