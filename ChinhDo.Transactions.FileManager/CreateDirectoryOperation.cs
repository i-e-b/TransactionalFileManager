﻿namespace System.IO.Transactions
{
    /// <summary>
    /// Creates all directories in the specified path.
    /// </summary>
    sealed class CreateDirectoryOperation : IRollbackableOperation
    {
        private readonly string path;
        private string backupPath;

        /// <summary>
        /// Instantiates the class.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        public CreateDirectoryOperation(string path)
        {
            this.path = path;
        }

        public void Execute()
        {
            // find the topmost directory which must be created
            var children = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var parent = Path.GetDirectoryName(children);
            while (parent != null /* children is a root directory */
                && !Directory.Exists(parent))
            {
                children = parent;
                parent = Path.GetDirectoryName(children);
            }

        	if (Directory.Exists(children)) return;

        	Directory.CreateDirectory(path);
        	backupPath = children;
        }

        public void Rollback()
        {
            if (backupPath != null)
            {
                Directory.Delete(backupPath, true);
            }
        }
    }
}
