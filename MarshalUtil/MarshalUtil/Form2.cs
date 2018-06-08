using System;
using System.Collections.Generic;
using System.IO;
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

        string lastPath = "";
        private List<ProcessStatus> statusList = new List<ProcessStatus>();
        bool closeOnDone = false;

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
                    progressBar2.Value = 0;
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            createStatusIndicators();
            // Get controls.
            lock (statusList)
            {
                statusList.Clear();
                foreach (var ctl in progressIndicators.Controls)
                {
                    if (ctl is ProcessStatus)
                    {
                        statusList.Add(ctl as ProcessStatus);
                    }
                }
                if (statusList.Count == 0)
                {
                    return;
                }
                // Disable controls till we are done.
                button1.Enabled = false;
                btnProcess.Enabled = false;
                // Setup progress bar.
                progressBar2.Maximum = statusList.Count;
                progressBar2.Value = 0;
                List<ProcessStatus> list = new List<ProcessStatus>(statusList);
                foreach (ProcessStatus st in list)
                {
                    st.processDirectory(singleFile.Checked, finishProcess);
                }
            }
        }

        delegate void finishProcessCallback(ProcessStatus status);
        private void finishProcess(ProcessStatus status)
        {
            if (progressBar2.InvokeRequired)
            {
                finishProcessCallback d = new finishProcessCallback(finishProcess);
                Invoke(d, new object[] { status });
            }
            else
            {
                lock(statusList)
                {
                    statusList.Remove(status);
                    progressBar2.Value++;
                    if(status.getStatus().Length == 0)
                    {
                        progressIndicators.Controls.Remove(status);
                    }
                    if (statusList.Count == 0)
                    {
                        button1.Enabled = true;
                        btnProcess.Enabled = true;
                        if(closeOnDone)
                        {
                            Close();
                        }
                    }
                }
            }
        }

        private bool createStatusIndicators()
        {
            progressIndicators.Controls.Clear();
            if (!packetSubDirs.Checked)
            {
                ProcessStatus prc = new ProcessStatus(lastPath);
                prc.analizeInput = selectAnalize.Checked;
                prc.decompilePython = decompilePython.Checked;
                prc.imbededPython = imbededPython.Checked;
                progressIndicators.Controls.Add(prc);
            }
            else
            {
                string[] dirs = Directory.GetDirectories(lastPath);
                Array.Sort<string>(dirs);
                foreach (string dir in dirs)
                {
                    ProcessStatus prc = new ProcessStatus(dir);
                    prc.analizeInput = selectAnalize.Checked;
                    prc.decompilePython = decompilePython.Checked;
                    prc.imbededPython = imbededPython.Checked;
                    progressIndicators.Controls.Add(prc);
                }
            }
            return true;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (statusList)
            {
                if (statusList.Count > 0)
                {
                    closeOnDone = true;
                    foreach (ProcessStatus st in statusList)
                    {
                        st.stop();
                    }
                    e.Cancel = true;
                }
            }
        }

    }
}
