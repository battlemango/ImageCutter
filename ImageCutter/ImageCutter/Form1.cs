using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCutter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

                string[] innerfiles = Directory.GetFiles(file);
                foreach(string f in innerfiles)
                {
                    string fullpath = f;
                    string filepath = Path.GetDirectoryName(fullpath).ToLower();
                    string filename = Path.GetFileNameWithoutExtension(fullpath);
                    string fileExtension = Path.GetExtension(fullpath);

                    listBox1.Items.Add(fullpath);
                }
            }
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
            //Console.WriteLine(listBox1.SelectedItem.ToString());
            pictureBox1.Image = LoadBitmap(listBox1.SelectedItem.ToString());
        }

        private Bitmap LoadBitmap(string path)
        {
            if (File.Exists(path))
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                if(stream == null)
                {
                    return null;
                }
                BinaryReader reader = new BinaryReader(stream);
                if(reader == null)
                {
                    return null;
                }

                MemoryStream memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                return new Bitmap(memoryStream);
            }
            else
            {
                return null;
            }
        }
    }
}
