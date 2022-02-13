using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCutter.src
{
    internal class FileUtils
    {
        public static void makeFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if(di.Exists == false)
            {
                di.Create();
            }
        }
    }
}
