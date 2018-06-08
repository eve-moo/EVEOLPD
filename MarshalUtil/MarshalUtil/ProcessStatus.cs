using System;
using System.Windows.Forms;
using eveMarshal;
using System.IO;
using System.Threading;
using Python;
using static Python.Bytecode;

namespace MarshalUtil
{
    public partial class ProcessStatus : UserControl
    {
        string[] PACKET_FILES = new string[] { };
        string workingDirectory = null;
        public bool analizeInput = false;
        public bool decompilePython = false;

        public ProcessStatus(string dir)
        {
            workingDirectory = dir;
            InitializeComponent();
            setDirectory();
        }

        public string getStatus()
        {
            return txtOutput.Text;
        }

        delegate void setProgressCallback(int value);
        private void setProgress(int value)
        {
            if (directoryProgress.InvokeRequired)
            {
                setProgressCallback d = new setProgressCallback(setProgress);
                Invoke(d, new object[] { value });
            }
            else
            {
                directoryProgress.Value = value;
            }
        }

        delegate void setDirectoryCallback();
        private void setDirectory()
        {
            if (txtOutput.InvokeRequired)
            {
                setDirectoryCallback d = new setDirectoryCallback(setDirectory);
                Invoke(d, new object[] { });
            }
            else
            {
                PACKET_FILES = Directory.GetFiles(workingDirectory, "*.eve*", SearchOption.TopDirectoryOnly);
                // Make sure packets are in order.
                Array.Sort<string>(PACKET_FILES);
                // Set controls.
                directoryName.Text = workingDirectory;
                txtOutput.Lines = PACKET_FILES;
                directoryProgress.Maximum = PACKET_FILES.Length;
            }
        }

        delegate void clearTextCallback();
        private void clearText()
        {
            if (txtOutput.InvokeRequired)
            {
                clearTextCallback d = new clearTextCallback(clearText);
                Invoke(d, new object[] { });
            }
            else
            {
                txtOutput.Clear();
            }
        }

        delegate void addTextCallback(string text);
        private void addText(string text)
        {
            if (txtOutput.InvokeRequired)
            {
                addTextCallback d = new addTextCallback(addText);
                Invoke(d, new object[] { text });
            }
            else
            {
                txtOutput.AppendText(text);
            }
        }

        public void stop()
        {
            pleaseStop = true;
        }

        /*
        * Data processing.
        */

        public const byte HeaderByte = 0x7E;
        // not a real magic since zlib just doesn't include one..
        public const byte ZlibMarker = 0x78;
        public const byte PythonMarker = 0x03;

        string singleFile = "";
        object threadLock = new object();
        Thread thread = null;
        Action<ProcessStatus> completeAction = null;
        bool pleaseStop = false;
        static int maxActive
        {
            get
            {
                return Math.Max(2, 2 * Environment.ProcessorCount);
            }
        }
        static Semaphore pool = new Semaphore(maxActive, maxActive);

        public void processDirectory(bool useSingleFile, Action<ProcessStatus> complete)
        {
            lock (threadLock)
            {
                if (thread != null)
                {
                    // Already running.
                    return;
                }
                setDirectory();
                if (useSingleFile)
                {
                    singleFile = workingDirectory + ".txt";
                }
                else
                {
                    singleFile = "";
                }
                completeAction = complete;
                thread = new Thread(doProcess);
                pleaseStop = false;
                thread.Start();
            }
        }

        private void doProcess()
        {
            int i = 0;
            setProgress(0);
            clearText();
            pool.WaitOne();
            StreamWriter singleWriter = null;
            // If we have a totalFile create a totalWriter.
            if (singleFile.Length > 0)
            {
                singleWriter = new StreamWriter(singleFile);
            }
            // Make sure packets are in order.
            Array.Sort<string>(PACKET_FILES);
            foreach (string file in PACKET_FILES)
            {
                if (!process(file, singleWriter))
                {
                    addText("Fail: " + file + Environment.NewLine);
                }
                if(pleaseStop)
                {
                    addText("Abort: " + file + Environment.NewLine);
                    singleWriter.WriteLine("Abort: " + file);
                    break;
                }
                i++;
                setProgress(i);
            }
            if (singleWriter != null)
            {
                singleWriter.Close();
                singleWriter = null;
            }
            lock(threadLock)
            {
                if (completeAction != null)
                {
                    completeAction(this);
                }
                thread = null;
                completeAction = null;
            }
            addText("Process finished.");
            pool.Release();
        }

        private bool process(string filename, StreamWriter singleWriter)
        {
            // Does the file exist?
            if (!File.Exists(filename))
            {
                addText("File not found: " + filename);
                // No, fail!
                return false;
            }
            byte[] data = null;
            using (var f = File.Open(filename, FileMode.Open))
            {
                if (f.Length == 0)
                {
                    if (singleWriter != null)
                    {
                        // Write the filename.
                        singleWriter.WriteLine(Path.GetFileName(filename));
                        // Write the decoded file.
                        singleWriter.WriteLine("Zero Length file.");
                    }
                    return true;
                }
                data = new byte[f.Length];
                f.Read(data, 0, (int)f.Length);
                f.Close();
            }
            if (data == null)
            {
                // No data loaded.
                return false;
            }
            // Is this a compressed file?
            if (data[0] == ZlibMarker)
            {
                // Yes, decompress it.
                data = Zlib.Decompress(data);
                if (data == null)
                {
                    // Decompress failed.
                    return false;
                }
            }
            // Is this a proper python serial stream?
            if (data[0] != HeaderByte)
            {
                // No, is this a python file? If yes, ignore it but dont cause an error.
                if(data[0] == PythonMarker && decompilePython)
                {
                    Bytecode code = new Bytecode();
                    code.load(data);
                    string outfile = null;
                    if(code.body != null)
                    {
                        Python.PyString fns = code.body.filename as Python.PyString;
                        if (fns != null)
                        {
                            outfile = fns.str;
                            outfile = outfile.Replace(':', '_');
                            outfile = outfile.Replace('\\', '-');
                            outfile = outfile.Replace('/', '-');
                        }

                    }
                    if (outfile != null)
                    {
                        string pyd = Path.GetDirectoryName(filename);
                        pyd += "\\py\\";
                        if (!Directory.Exists(pyd))
                        {
                            Directory.CreateDirectory(pyd);
                        }
                        outfile = pyd + "\\" + outfile + ".txt";
                        string dump = Python.PrettyPrinter.print(code, true);
                        File.WriteAllText(outfile, dump);
                    }
                }
                return data[0] == PythonMarker;
            }
            bool decodeDone = false;
            try
            {
                Unmarshal un = new Unmarshal();
                un.analizeInput = analizeInput;
                PyRep obj = un.Process(data);
                decodeDone = true;
                obj = analyse(obj, filename);
                string decoded = eveMarshal.PrettyPrinter.Print(obj);
                if (singleWriter != null)
                {
                    // Write the filename.
                    singleWriter.WriteLine(Path.GetFileName(filename));
                    // Write the decoded file.
                    singleWriter.Write(decoded);
                    singleWriter.Flush();
                }
                else
                {
                    File.WriteAllText(filename + ".txt", decoded);
                }
                if (un.unknown.Length > 0)
                {
                    addText(workingDirectory + Environment.NewLine);
                    addText(un.unknown.ToString() + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                string err = Path.GetFileName(filename) + Environment.NewLine + "Error: " + e.ToString();
                // We, had an error but should still produce some kind of notice in the output.
                addText(err + Environment.NewLine);
                if (singleWriter != null)
                {
                    // Write the filename.
                    singleWriter.WriteLine(Path.GetFileName(filename));
                    // Write the decoded file.
                    singleWriter.WriteLine(decodeDone ? "Printer Error. " : "Decoder Error.");
                    singleWriter.WriteLine(err);
                }
                else
                {
                    File.WriteAllText(filename + ".txt", err);
                }
                return false;
            }
            return true;
        }

        /*
        Attempt to analyse
        */
        public PyRep analyse(PyRep obj, string filename)
        {
            if (obj.Type == PyObjectType.ObjectData)
            {
                eveMarshal.PyObject packetData = obj as eveMarshal.PyObject;
                try
                {
                    return new PyPacket(packetData);
                }
                catch (InvalidDataException e)
                {
                    addText(filename + Environment.NewLine);
                    addText("Packet Decode failed: " + e.Message + Environment.NewLine);
                    return obj;
                }
            }
            return obj;
        }

    }
}
