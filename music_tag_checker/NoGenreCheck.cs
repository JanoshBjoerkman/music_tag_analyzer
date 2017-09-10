using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_tag_checker
{
    public class NoGenreCheck : check
    {
        public NoGenreCheck(): base()
        {
            checkname = this.GetType().Name.ToString();
        }

        // check all files, remove successfully checked files from list
        protected override void runCheck()
        {
            foreach (var file in allFiles)
            {
                mp3file = TagLib.File.Create(file);
                if (mp3file.Tag.FirstGenre != null)
                {
                    failedFiles.Remove(file);
                }
            }
        }
    }
}
