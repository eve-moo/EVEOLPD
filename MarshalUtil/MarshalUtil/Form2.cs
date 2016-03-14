using eveMarshal;
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

namespace MarshalUtil
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string PACKET_DIR = string.Empty;
        string[] PACKET_FILES = new string[] { };
        List<string> PACKET_SUCCESS = new List<string>();
        List<string> PACKET_FAILURE = new List<string>();


        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                evePathTxtBox.Text = fbd.SelectedPath;
                btnProcess.Enabled = true;
                PACKET_FILES = Directory.GetFiles(fbd.SelectedPath, "*.eve", SearchOption.TopDirectoryOnly);
                txtOutput.Lines = PACKET_FILES;
            }
        }

        private bool process(string filename)
        {
            // Validate this file is marshalStream
            using (var f = File.Open(filename, FileMode.Open))
            {
                int h = f.ReadByte();
                f.Position = 0;
                if (h != 126)
                {
                    return false;
                }
                byte[] data = new byte[f.Length];
                f.Read(data, 0, (int)f.Length);
                try
                {
                    Unmarshal un = new Unmarshal();
                    PyObject obj = un.Process(data);
                    string decoded = PrettyPrinter.Print(obj);
                    File.WriteAllText(filename + ".txt", decoded);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            int i = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = PACKET_FILES.Length;
            txtOutput.Clear();
            foreach(string file in PACKET_FILES)
            {
                if (process(file))
                {
                    PACKET_SUCCESS.Add(file);
                //    txtOutput.AppendText("S: " + file + System.Environment.NewLine);
                }
                else
                {
                    PACKET_FAILURE.Add(file);
                    txtOutput.AppendText("F: " + file + System.Environment.NewLine);
                }
                i++;
                progressBar1.Value = i;
            }
        }
    }
}
