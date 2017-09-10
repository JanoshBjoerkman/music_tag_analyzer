using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace music_tag_checker
{
    class Program
    {
        static void Main(string[] args)
        {
            check c;
            printStartMessage();
            int eingabe = Convert.ToInt32(Console.ReadLine());
            // select checktype
            switch (eingabe)
            {
                case 1:
                    c = new AlbumnameEqualsFoldernameCheck();
                    c.runAndMove();
                    break;
                case 2:
                    c = new NoGenreCheck();
                    c.runAndMove();
                    break;
                case 3:
                    moveToCorrespondingFolder();
                    break;
                case 6:
                    deleteAllEmptyDirectories(getApplicationDir());
                    break;
                default:
                    Console.Clear();
                    printStartMessage();
                    break;
            }
        }

        public static void moveToCorrespondingFolder()
        {
            HashSet<string> files = new HashSet<string>(GetFileList("*.mp3"));
            foreach (var file in files)
            {
                TagLib.File mp3file = TagLib.File.Create(file);
                string foldername = mp3file.Tag.Album;
                bool doesAFolderNamedAfterTheAlbumTagExists = (System.IO.Directory.Exists(foldername));
                bool doesTheCurrentFileInThisFolderExists = (System.IO.File.Exists(System.IO.Path.Combine(foldername, System.IO.Path.GetFileName(file))));
                if (!doesAFolderNamedAfterTheAlbumTagExists)
                {
                    System.IO.Directory.CreateDirectory(foldername);
                }
                bool shouldFileBeMoved = true;
                if (doesTheCurrentFileInThisFolderExists)
                {
                    long fileLength = new System.IO.FileInfo(file).Length;
                    long alreadyExistingFileLength = new System.IO.FileInfo(System.IO.Path.Combine(foldername, System.IO.Path.GetFileName(file))).Length;
                    bool isTheCurrentFileBigger = (fileLength > alreadyExistingFileLength);
                    if (!isTheCurrentFileBigger)
                    {
                        shouldFileBeMoved = false;
                    }
                }
                if (shouldFileBeMoved)
                {
                    // move bitch
                    System.IO.File.Move(file, System.IO.Path.Combine(foldername, System.IO.Path.GetFileName(file)));
                }
            }

            /*
            List<string> files = new List<string>(GetFileList("*.mp3"));
            foreach (var file in files)
            {
                TagLib.File mp3file = TagLib.File.Create(file);
                string foldername = mp3file.Tag.Album;
                // check if folder exists
                if (System.IO.Directory.Exists(foldername))
                {
                    // check if file in this exists
                    if (System.IO.File.Exists(System.IO.Path.Combine(foldername, System.IO.Path.GetFileName(file))))
                    {
                        Console.WriteLine(file);
                        Console.ReadLine();
                    }
                }
            }
            */
        }

        // copy+paste from: https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        public static void deleteAllEmptyDirectories(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                deleteAllEmptyDirectories(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        public static string getApplicationDir()
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static IEnumerable<string> GetFileList(string fileSearchPattern)
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

        private static void printStartMessage()
        {
            Console.WriteLine("------------music_tag_analyzer------------\n");
            Console.WriteLine("Options:\n");
            Console.Write("1) album name == folder name check\n");
            Console.Write("2) no genre check\n");
            Console.Write("3) move to corresponding folder\n");
            Console.Write("6) delete empty folders\n");
        }
    }
}
