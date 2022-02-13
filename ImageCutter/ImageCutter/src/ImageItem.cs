using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCutter.src
{
    internal class ImageItem
    {
        private const int STATE_NO_READ = 0;
        private const int STATE_MODIFIED = 1;
        private const int STATE_IGNORE = 2;
        private int state = STATE_NO_READ;

        public string path;
        private string filepath;
        private string filename;
        private string fileExtension;

        public ImageItem(string path)
        {
            this.path = path;
            this.filepath = Path.GetDirectoryName(path).ToLower();
            this.filename = Path.GetFileNameWithoutExtension(path);
            this.fileExtension = Path.GetExtension(path);
        }

        public string getName()
        {
            return filename + " "+state;
        }
    }
}
