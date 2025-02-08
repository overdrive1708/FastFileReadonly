namespace FastFileReadonly
{
    /// <summary>
    /// 例外クラス
    /// </summary>
    internal class Exception
    {
        /// <summary>
        /// ロガーインスタンス
        /// </summary>
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //--------------------------------------------------
        // メソッド
        //--------------------------------------------------
        /// <summary>
        /// UnobservedTaskExceptionイベント発生時の処理
        /// </summary>
        /// <param name="sender">イベントソース</param>
        /// <param name="e">イベントデータ</param>
        internal static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) => HandleException(e.Exception.InnerException);

        /// <summary>
        /// UnhandledExceptionイベント発生時の処理
        /// </summary>
        /// <param name="sender">イベントソース</param>
        /// <param name="e">イベントデータ</param>
        internal static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) => HandleException((System.Exception)e.ExceptionObject);

        /// <summary>
        /// 例外発生時の処理
        /// </summary>
        /// <param name="e">例外情報</param>
        private static void HandleException(System.Exception exception)
        {
            // 例外の詳細情報をログ出力する｡
            logger.Fatal("Exception occurred.", exception);

            // 終了する｡
            Environment.Exit(1);
        }
    }
}
