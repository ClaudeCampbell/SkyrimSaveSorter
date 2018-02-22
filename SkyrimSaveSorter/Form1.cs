using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace SkyrimSaveSorter
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.SaveFolder;
            textBox2.Text = Properties.Settings.Default.ArchiveFolder;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                textBox1.Text = fbd.SelectedPath;
                Properties.Settings.Default.SaveFolder = fbd.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                textBox2.Text = fbd.SelectedPath;
                Properties.Settings.Default.ArchiveFolder = fbd.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /**
            FileStream fs = File.Open("C:\\Users\\Et-el\\Documents\\my games\\skyrim\\saves\\Save 3641 - Morlae  Skyrim  201.30.06.ess", FileMode.Open);
            byte[] b = new byte[78];
            fs.Read(b, 0, 78);

            char[] t = System.Text.Encoding.UTF8.GetString(b).ToCharArray();

            t = t;

            FileStream fs2 = File.Open("C:\\Users\\Et-el\\Documents\\my games\\skyrim\\saves\\Save 3619 - Sonja  Skyrim  96.08.03.ess", FileMode.Open);
            byte[] b2 = new byte[64];
            fs2.Read(b2, 0, 64);

            char[] t2 = System.Text.Encoding.UTF8.GetString(b2).ToCharArray();

            t2 = t2;
            */
            //Sorter.sortSaves();
            Sorter.bar = progressBar1;
            Thread th = new Thread(Sorter.sortSaves);
            th.Start();            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Sorter.bar = progressBar1;
            Thread th = new Thread(Sorter.deleteTmp);
            th.Start();
        }
    }
}
