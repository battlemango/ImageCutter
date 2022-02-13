using System;
using System.Collections.Generic;
using System.Drawing;
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
        private const int STATE_CHECKED = 2;
        private int state = STATE_NO_READ;

        public string path;
        private string filepath;
        public string filename;
        private string fileExtension;

        public bool isExistRect = false;
        public Point pointStart;
        public Point pointEnd;

        public ImageItem(string path)
        {
            this.path = path;
            this.filepath = Path.GetDirectoryName(path).ToLower();
            this.filename = Path.GetFileNameWithoutExtension(path);
            this.fileExtension = Path.GetExtension(path);
        }

        public string getName()
        {
            string stateStr = "";
            if(state == STATE_MODIFIED)
            {
                stateStr = " - modified";
            }else if (state == STATE_CHECKED)
            {
                stateStr = " - checked";
            }

            return filename + stateStr;
        }

        public void select()
        {
            if(state == STATE_NO_READ)
            {
                state = STATE_CHECKED;
            }
        }

        public void save()
        {
            state = STATE_MODIFIED;
        }

        public void unSave()
        {
            isExistRect = false;
            state = STATE_CHECKED;
        }

        public void startRect(int x, int y)
        {
            isExistRect = true;
            pointStart.X = x;
            pointStart.Y = y;
            pointEnd.X = x;
            pointEnd.Y = y;
        }

        public void endRect(int x, int y)
        {
            pointEnd.X = x;
            pointEnd.Y = y;
        }

        public Rectangle getRect()
        {
            int x = pointStart.X > pointEnd.X ? pointEnd.X: pointStart.X;
            int y = pointStart.Y > pointEnd.Y ? pointEnd.Y : pointStart.Y;

            int w = Math.Abs(pointStart.X - pointEnd.X);
            int h = Math.Abs(pointStart.Y - pointEnd.Y);

            Console.WriteLine("w : " + w + ", h : " + h + ", x : " + x + ", y : " + y);

            return new Rectangle(x, y, w, h);
        }
    }
}
