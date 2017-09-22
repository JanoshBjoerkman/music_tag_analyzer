using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
// remove when finished
using System.Diagnostics;

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
            Dictionary<string, string> File_ShouldBeFolder_Map = new Dictionary<string, string>();

            // map files to shouldBeFoldername (album tag)
            foreach (var file in files)
            {
                TagLib.File mp3file = TagLib.File.Create(file);
                string shouldBeFoldername = mp3file.Tag.Album.ToString();
                File_ShouldBeFolder_Map.Add(file, shouldBeFoldername);
            }

            // filter hashset: remove all correct files
            files.RemoveWhere(x => Directory.GetParent(x).Name.ToString() == File_ShouldBeFolder_Map[x].ToString());

            // move files 
            foreach (var file in files)
            {
                try
                {
                    TagLib.File mp3file = TagLib.File.Create(file);
                    string newFolderName = mp3file.Tag.Album;
                    Directory.CreateDirectory(newFolderName);
                    string fullDestinationPath = Path.GetFullPath(Path.Combine(newFolderName, Path.GetFileName(file)));
                    bool doesTheCurrentFileInThisFolderExists = (File.Exists(fullDestinationPath));
                    bool isTheCurrentFileInTheListBigger = fileSizeCompare(file, fullDestinationPath);
                    if (doesTheCurrentFileInThisFolderExists)
                    {
                        if (isTheCurrentFileInTheListBigger)
                        {
                            File.Delete(fullDestinationPath);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    File.Move(file, fullDestinationPath);
                }
                catch (Exception)
                {
                    // TODO: add error to logfile
                }
            }


            /*
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
            */
        }

        private static bool fileSizeCompare(string fileInList, string fileInFolder)
        {
            return true;
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
