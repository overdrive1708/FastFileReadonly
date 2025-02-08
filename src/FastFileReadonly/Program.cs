using CommandLine;

// ログ関連の設定ファイルを外部ファイルとする
[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log.config", Watch = true)]

namespace FastFileReadonly
{
    internal class Program
    {
        /// <summary>
        /// ロガーインスタンス
        /// </summary>
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// メイン処理(エントリーポイント)
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        static void Main(string[] args)
        {
            EntryExceptionHandler();

            logger.Info("Begins the process of changing the file to read-only.");

            // コマンドライン引数を解析する
            _ = Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(HandleParseSuccess)
                .WithNotParsed(HandleParseError);

            logger.Info("The process of changing the file to read-only has been completed.");
        }

        /// <summary>
        /// 例外ハンドラー登録処理
        /// </summary>
        private static void EntryExceptionHandler()
        {
            // 未処理の例外が発生したときの処理を登録する
            TaskScheduler.UnobservedTaskException += Exception.OnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += Exception.OnUnhandledException;
        }

        /// <summary>
        /// コマンドライン引数解析成功時の処理
        /// </summary>
        /// <param name="opts">解析結果</param>
        private static void HandleParseSuccess(CommandLineOptions opts) => ChangeReadonly(opts);

        /// <summary>
        /// コマンドライン引数解析失敗時の処理
        /// </summary>
        /// <param name="errs">解析エラー</param>
        private static void HandleParseError(IEnumerable<CommandLine.Error> errs)
        {
            logger.Error("The arguments are not valid.");
        }

        /// <summary>
        /// 読み取り専用化処理
        /// </summary>
        /// <param name="opts">コマンドライン引数解析結果</param>
        private static void ChangeReadonly(CommandLineOptions opts)
        {
            logger.Info($"ChangeReadonlyDirectoryPath is \"{opts.ChangeReadonlyDirectoryPath}\".");

            // ディレクトリが存在しない場合はエラーログを出力して終了する
            if (!Directory.Exists(opts.ChangeReadonlyDirectoryPath))
            {
                logger.Error("ChangeReadonlyDirectoryPath does not exist.");
                return;
            }

            // ファイルリストを作成する
            string[] files = Directory.GetFiles(opts.ChangeReadonlyDirectoryPath, "*", SearchOption.AllDirectories);

            // 並列処理で読み取り専用化する
            _ = Parallel.ForEach(files, file =>
            {
                // ファイルの属性を取得する
                FileAttributes attributes = File.GetAttributes(file);

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // 読み取り専用の場合はデバッグログを出力して次へ
                    logger.Debug($"{file} is already read-only.");
                }
                else
                {
                    // 読み取り専用ではない場合は読み取り専用にしてインフォログを出力して次へ
                    File.SetAttributes(file, attributes | FileAttributes.ReadOnly);
                    logger.Info($"{file} has been changed to read-only.");
                }
            });
        }
    }
}
