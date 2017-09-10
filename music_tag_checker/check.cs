using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace music_tag_checker
{
    public abstract class check
    {
        // modified by check class
        protected List<string> allFiles;
        protected List<string> failedFiles;
        // modified by child classes
        protected TagLib.File mp3file;
        protected string parentFolder;
        public string checkname;

        #region public functions
        public check()
        {
            // initialize variables
            initializeFileVariables();
        }

        public List<string> getFailedFiles()
        {
            initializeFileVariables();
            runCheck();
            return failedFiles;
        }

        public void runAndMove()
        {
            initializeFileVariables();
            // ensure that the right files are in the list
            runCheck();
            string outputFolderPath = System.IO.Path.Combine(getApplicationDir(), "1a_" + checkname);
            checkIfFolderExistsCreateItWhenNot(outputFolderPath);
            // move bitch ^^
            foreach (var file in failedFiles)
            {
                bool isFileAlreadyInOutputDirectory = (file.Contains(checkname));
                if (!isFileAlreadyInOutputDirectory)
                {
                    string destinationPath = System.IO.Path.Combine(outputFolderPath, System.IO.Directory.GetParent(file).Name);
                    checkIfFolderExistsCreateItWhenNot(destinationPath);
                    string fullDestinationPath = System.IO.Path.Combine(destinationPath, System.IO.Path.GetFileName(file));
                    System.IO.File.Move(file, fullDestinationPath);
                }
            }
        }
        #endregion

        #region helpfunctions
        protected void initializeFileVariables()
        {
            allFiles = new List<string>(GetFileList("*.mp3"));
            failedFiles = new List<string>(allFiles);
        }

        protected bool checkIfFolderExistsCreateItWhenNot(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                return false;
            }
            else
            {
                return true;
            }
        }

        protected string getApplicationDir()
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        protected IEnumerable<string> GetFileList(string fileSearchPattern)
        {
            string rootFolderPath = getApplicationDir();
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }
        #endregion

        #region toImplement for child classes
        // check all files, remove successfully checked files from list
        protected abstract void runCheck();
        #endregion
    }
}
