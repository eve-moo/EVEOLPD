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

        string[] PACKET_FILES = new string[] { };
        List<string> PACKET_SUCCESS = new List<string>();
        List<string> PACKET_FAILURE = new List<string>();
        System.IO.StreamWriter totalWriter = null;
        string totalFile = "";
        string lastPath = "";

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = lastPath;
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                lastPath = fbd.SelectedPath;
                evePathTxtBox.Text = fbd.SelectedPath;
                btnProcess.Enabled = true;
                PACKET_FILES = Directory.GetFiles(fbd.SelectedPath, "*.eve", SearchOption.TopDirectoryOnly);
                // Make sure packets are in order.
                Array.Sort<string>(PACKET_FILES);
                if (singleFile.Checked)
                {
                    totalFile = fbd.SelectedPath + ".txt";
                }
                else
                {
                    totalFile = "";
                }
                txtOutput.Lines = PACKET_FILES;
                progressBar1.Value = 0;
            }
        }

        public const byte HeaderByte = 0x7E;
        // not a real magic since zlib just doesn't include one..
        public const byte ZlibMarker = 0x78;
        public const byte PythonMarker = 0x03;

        private bool process(string filename)
        {
            // Does the file exist?
            if (!File.Exists(filename))
            {
                // No, fail!
                return false;
            }
            using (var f = File.Open(filename, FileMode.Open))
            {
                byte[] data = new byte[f.Length];
                f.Read(data, 0, (int)f.Length);
                // Is this a compressed file?
                if (data[0] == ZlibMarker)
                {
                    // Yes, decompress it.
                    data = Zlib.Decompress(data);
                }
                // Is this a python file?
                if (data != null && data[0] == PythonMarker)
                {
                    // Yes, ignore it but dont cause an error.
                    return true;
                }
                // Is this a proper python serial stream?
                if (data == null || data[0] != HeaderByte)
                {
                    // No, fail!
                    return false;
                }
                bool decodeDone = false;
                try
                {
                    Unmarshal un = new Unmarshal();
                    PyObject obj = un.Process(data);
                    decodeDone = true;
                    string decoded = PrettyPrinter.Print(obj);
                    if (totalWriter != null)
                    {
                        // Write the filename.
                        totalWriter.WriteLine(Path.GetFileName(filename));
                        // Write the decoded file.
                        totalWriter.Write(decoded);
                    }
                    else
                    {
                        File.WriteAllText(filename + ".txt", decoded);
                    }
                }
                catch (Exception e)
                {
                    txtOutput.AppendText("Error: " + e.ToString() + System.Environment.NewLine);
                    // We, had an error but should still produce some kind of notice in the output.
                    if (totalWriter != null)
                    {
                        // Write the filename.
                        totalWriter.WriteLine(Path.GetFileName(filename));
                        // Write the decoded file.
                        totalWriter.WriteLine(decodeDone ? "Printer Error. " : "Decoder Error.");
                    }
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
            totalWriter = null;
            // If we have a totalFile create a totalWriter.
            if (totalFile.Length > 0)
            {
                totalWriter = new System.IO.StreamWriter(totalFile);
            }
            // Make sure packets are in order.
            Array.Sort<string>(PACKET_FILES);
            foreach (string file in PACKET_FILES)
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
            if (totalWriter != null)
            {
                totalWriter.Close();
                totalWriter = null;
            }
        }
    }
}
