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
            ToolTip ToolTip = new ToolTip();
            ToolTip.SetToolTip(singleFile, "Output all packets to a single file.");
            ToolTip = new ToolTip();
            ToolTip.SetToolTip(this.packetSubDirs, "Proccess all directorys in the chosen directory as packet groups." + Environment.NewLine +"If single file is chosen one file will be created for each packet directory.");
        }

        string[] PACKET_FILES = new string[] { };
        List<string> PACKET_SUCCESS = new List<string>();
        List<string> PACKET_FAILURE = new List<string>();
        System.IO.StreamWriter totalWriter = null;
        string totalFile = "";
        string lastPath = "";
        string workingDir = "";

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = lastPath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (fbd.SelectedPath != string.Empty)
                {
                    lastPath = fbd.SelectedPath;
                    evePathTxtBox.Text = fbd.SelectedPath;
                    btnProcess.Enabled = true;

                    workingDir = fbd.SelectedPath;
                    populateFileList();

                    txtOutput.Lines = PACKET_FILES;
                    progressBar1.Value = 0;
                }
            }
        }

        private void populateFileList()
        {
            PACKET_FILES = Directory.GetFiles(workingDir, "*.eve*", SearchOption.TopDirectoryOnly);
            // Make sure packets are in order.
            Array.Sort<string>(PACKET_FILES);
            if (singleFile.Checked)
            {
                totalFile = workingDir + ".txt";
            }
            else
            {
                totalFile = "";
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
                if(f.Length == 0)
                {
                    if (totalWriter != null)
                    {
                        // Write the filename.
                        totalWriter.WriteLine(Path.GetFileName(filename));
                        // Write the decoded file.
                        totalWriter.WriteLine("Zero Length file.");
                    }
                    return true;
                }
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
                    obj = analyse(obj);
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
                    string err = "Error: " + e.ToString();
                    txtOutput.AppendText(err + System.Environment.NewLine);
                    // We, had an error but should still produce some kind of notice in the output.
                    if (totalWriter != null)
                    {
                        // Write the filename.
                        totalWriter.WriteLine(Path.GetFileName(filename));
                        // Write the decoded file.
                        totalWriter.WriteLine(decodeDone ? "Printer Error. " : "Decoder Error.");
                        totalWriter.WriteLine(err);
                    }
                    else
                    {
                        File.WriteAllText(filename + ".txt", err);
                    }
                    return false;
                }
            }
            return true;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
            if (!packetSubDirs.Checked)
            {
                progressBar2.Maximum = 1;
                progressBar2.Value = 0;
                workingDir = lastPath;
                processWorkingDirectory();
                progressBar2.Value = 1;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(lastPath);
                Array.Sort<string>(dirs);
                progressBar2.Maximum = dirs.Length;
                progressBar2.Value = 0;
                foreach (string dir in dirs)
                {
                    workingDir = dir;
                    populateFileList();
                    string proc = "Processing directory " + (progressBar2.Value + 1) + " of " + dirs.Length;
                    txtOutput.AppendText(proc + System.Environment.NewLine);
                    processWorkingDirectory();
                    progressBar2.Value++;
                }
            }
        }

        private void processWorkingDirectory()
        {
            int i = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = PACKET_FILES.Length;
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
                    txtOutput.AppendText("Fail: " + file + System.Environment.NewLine);
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

        /*
        Attempt to analyse
        */
        public PyObject analyse(PyObject obj)
        {
            if(obj.Type == PyObjectType.ObjectData)
            {
                PyObjectData packetData = obj as PyObjectData;
                try
                {
                    return new PyPacket(packetData);
                }
                catch (InvalidDataException)
                {
                    return obj;
                }
            }
            return obj;
        }
    }
}
