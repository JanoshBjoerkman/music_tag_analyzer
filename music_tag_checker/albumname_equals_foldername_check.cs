using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_tag_checker
{
    public class AlbumnameEqualsFoldernameCheck : check
    {
        public AlbumnameEqualsFoldernameCheck() : base()
        {
            checkname = this.GetType().Name.ToString();
        }

        // check all files, remove successfully checked files from list
        protected override void runCheck()
        {
            foreach (var file in allFiles)
            {
                mp3file = TagLib.File.Create(file);
                parentFolder = System.IO.Directory.GetParent(file).Name;
                if (mp3file.Tag.Album == parentFolder)
                {
                    failedFiles.Remove(file);
                }
            }
        }
    }
}
