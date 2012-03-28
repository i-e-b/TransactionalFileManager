namespace System.IO.Transactions
{
    static class FileUtils
    {
        internal static readonly string TempFolder = Path.Combine(Path.GetTempPath(), "TransactionalIO");

        /// <summary>
        /// Ensures that the folder that contains the temporary files exists.
        /// </summary>
        public static void EnsureTempFolderExists()
        {
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
        }

        /// <summary>
        /// Returns a unique temporary file name.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetTempFileName(string extension)
        {
            var g = Guid.NewGuid();
            var retVal = Path.Combine(TempFolder, g.ToString().Substring(0, 16)) + extension;

            return retVal;
        }
    }
}
