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
            numericUpDown1.Value = Properties.Settings.Default.NumberOfSaves;
            if (Properties.Settings.Default.OldestDate == default(DateTime))
                Properties.Settings.Default.OldestDate = DateTime.Now;
            dateTimePicker.Value = Properties.Settings.Default.OldestDate;
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NumberOfSaves = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OldestDate = dateTimePicker.Value;
            Properties.Settings.Default.Save();
        }
    }
}
