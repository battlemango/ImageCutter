using ImageCutter.src;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCutter
{
    public partial class Form1 : Form
    {
        const int MAX_SIZE = 1200;
        string newFolderPath = null;
        Bitmap modifiedBitmap = null;

        private List<ImageItem> imageItemList;

        public Form1()
        {
            InitializeComponent();
            imageItemList = new List<ImageItem>();

            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                listBox1.Items.Clear();

                string[] files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if(files.Length < 1)
                {
                    Console.WriteLine("there is no file");
                    return;
                }

                string file = files[0];
                lblFolderName.Text = file;

                newFolderPath = Path.GetDirectoryName(file).ToLower() + "\\[new]" + Path.GetFileNameWithoutExtension(file);

                FileUtils.makeFolder(newFolderPath);

                string[] innerfiles = Directory.GetFiles(file);

                imageItemList.Clear();
                foreach (string f in innerfiles)
                {

                    string fullpath = f;
                    string filepath = Path.GetDirectoryName(fullpath).ToLower();
                    string filename = Path.GetFileNameWithoutExtension(fullpath);
                    string fileExtension = Path.GetExtension(fullpath);

                    imageItemList.Add(new ImageItem(f));
                    //listBox1.Items.Add(fullpath);
                }
                updateListBox();
            }
        }

        private void updateListBox()
        {
            listBox1.Items.Clear();
            foreach (ImageItem iItem in imageItemList)
            {
                listBox1.Items.Add(iItem.getName());
            }
        }

        private ImageItem findImageItemByName(string name)
        {
            foreach (ImageItem iItem in imageItemList)
            {
                if(iItem.getName() == name)
                {
                    return iItem;
                }
            }
            return null;
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(modifiedBitmap != null)
            {
                modifiedBitmap.Save(newFolderPath + "\\a.jpg", ImageFormat.Jpeg);
                modifiedBitmap.Dispose();
                modifiedBitmap = null;
            }

            ImageItem imageItem = findImageItemByName(listBox1.SelectedItem.ToString());
            if(imageItem == null)
            {
                Console.WriteLine("imageItem is null");
                return;
            }

            Bitmap bitmap = new Bitmap(imageItem.path);
            //무엇을 기준으로? width, height각각 기준을 잡아야한다. 둘중에 큰걸 찾는다
            //
            float bigger = bitmap.Width < bitmap.Height ? bitmap.Height : bitmap.Width;
            if(bigger > MAX_SIZE)
            {
                Size resize = new Size((int)(bitmap.Width/bigger * MAX_SIZE), (int)(bitmap.Height / bigger * MAX_SIZE));
                bitmap = new Bitmap(bitmap, resize);

            }

            modifiedBitmap = bitmap;
            pictureBox1.Image = bitmap;
            //pictureBox1.Image = new Bitmap(listBox1.SelectedItem.ToString());
        }
    }
}
