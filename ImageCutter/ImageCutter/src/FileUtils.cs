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

        public static void removeFile(string path)
        {
            bool result = File.Exists(path);
            if (result == true)
            {
                Console.WriteLine("File Found");
                File.Delete(path);
                Console.WriteLine("File Deleted Successfully");
            }
            else
            {
                Console.WriteLine("File Not Found");
            }
        }
    }
}
