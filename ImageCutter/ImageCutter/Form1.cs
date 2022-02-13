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
        private ImageItem selectedImageItem = null;
        private bool isMouseDown = false;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            imageItemList = new List<ImageItem>();

            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
            pictureBox1.Paint += new PaintEventHandler(listBox1_Paint);
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
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
                    imageItemList.Add(new ImageItem(f));
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

        private int findImageItemIndexByName(string name)
        {
            for(int i=0; i<imageItemList.Count; i++)
            {
                if (imageItemList[i].getName() == name)
                {
                    return i;
                }
            }
            return -1;
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

        private void listBox1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = this.CreateGraphics();
            if(selectedImageItem == null)
            {
                return;
            }
            Graphics g = e.Graphics;
            //g.DrawRectangle(new Pen(Brushes.Red), new Rectangle(15, 15, 100, 100));

            if (selectedImageItem.isExistRect)
            {
                g.DrawRectangle(new Pen(Brushes.Red), selectedImageItem.getRect());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedImageItem = findImageItemByName(listBox1.SelectedItem.ToString());
            if(selectedImageItem == null)
            {
                Console.WriteLine("imageItem is null");
                return;
            }

            selectedImageItem.select();
            updateListBox();

            Bitmap bitmap = new Bitmap(selectedImageItem.path);
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedImageItem == null)
            {
                Console.WriteLine("pictureBox1_MouseDown - no selectedImageItem");
                return;
            }
            isMouseDown = true;
            selectedImageItem.startRect(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedImageItem == null)
            {
                Console.WriteLine("pictureBox1_MouseUp - no selectedImageItem");
                return;
            }
            if (isMouseDown)
            {
                selectedImageItem.endRect(e.X, e.Y);
                saveModifiedBitmap();
                pictureBox1.Refresh();
            }

            isMouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown)
            {
                return;
            }
            if (selectedImageItem == null)
            {
                Console.WriteLine("pictureBox1_MouseMove - no selectedImageItem");
                return;
            }
            selectedImageItem.endRect(e.X, e.Y);
            pictureBox1.Refresh();
        }

        private void saveModifiedBitmap()
        {
            if (selectedImageItem == null)
            {
                Console.WriteLine("saveModifiedBitmap - no selectedImageItem");
                return;
            }
            if(modifiedBitmap != null)
            {
                Bitmap cropBitmap = cropAtRect(modifiedBitmap, selectedImageItem.getRect());
                cropBitmap.Save(newFolderPath + "\\"+ selectedImageItem.filename + ".jpg", ImageFormat.Jpeg);
                cropBitmap.Dispose();
                cropBitmap = null;
                selectedImageItem.save();
            }
        }

        public Bitmap cropAtRect(Bitmap orgImg, Rectangle sRect)
        {
            Rectangle destRect = new Rectangle(Point.Empty, sRect.Size);

            var cropImage = new Bitmap(destRect.Width, destRect.Height);
            using (var graphics = Graphics.FromImage(cropImage))
            {
                graphics.DrawImage(orgImg, destRect, sRect, GraphicsUnit.Pixel);
            }
            return cropImage;
        }

        private void removeModifiedBitmap()
        {
            if (selectedImageItem == null)
            {
                Console.WriteLine("saveModifiedBitmap - no selectedImageItem");
                return;
            }
            FileUtils.removeFile(newFolderPath + "\\" + selectedImageItem.filename + ".jpg");

            selectedImageItem.unSave();
        }

        private void btnUnSave_Click(object sender, EventArgs e)
        {
            removeModifiedBitmap();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:

                    if(selectedImageItem != null)
                    {
                        int index = findImageItemIndexByName(selectedImageItem.getName());
                        index++;
                        if(index >= listBox1.Items.Count)
                        {
                            index = 0;
                        }

                        listBox1.SelectedItem = listBox1.Items[index];
                    }                    
                    break;
                case Keys.A:

                    if (selectedImageItem != null)
                    {
                        int index = findImageItemIndexByName(selectedImageItem.getName());
                        index--;
                        if (index <= 0)
                        {
                            index = listBox1.Items.Count-1;
                        }

                        listBox1.SelectedItem = listBox1.Items[index];
                    }
                    break;
            }
        }
    }
}
