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

/*
 * Buggy as hell, likes to lock files while being used.
 * If you can do better, feel free to do it.
 * I will gladly use it and promote it.
 */

namespace advapi32_controller
{
    public partial class Form1 : Form
    {
        String EVE_DIR = null;
        bool cryptDecrypt = false;
        bool cryptEncrypt = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cryptDecrypt != cryptEncrypt)
            {
                // Out of sequence
                // Just disable both just incase
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt");
                        button2.BackColor = default(Color);
                        cryptDecrypt = false;
                    }
                }
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt");
                        button3.BackColor = default(Color);
                        cryptEncrypt = false;
                    }
                }
            }
            else if (cryptDecrypt && cryptEncrypt)
            {
                // enabled, now disable
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt");
                        button2.BackColor = default(Color);
                        cryptDecrypt = false;
                    }
                }
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt");
                        button3.BackColor = default(Color);
                        cryptEncrypt = false;
                    }
                }
            }
            else if (!cryptDecrypt && !cryptEncrypt)
            {
                // disabled, now enable
                if (!File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                {
                    File.Create(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt");
                    button2.BackColor = Color.LightGreen;
                    cryptDecrypt = true;
                }
                else
                {
                    MessageBox.Show("ERROR, cryptDecrypt file exists!");
                }

                if (!File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                {
                    File.Create(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt");
                    button3.BackColor = Color.LightGreen;
                    cryptEncrypt = true;
                }
                else 
                {
                    MessageBox.Show("ERROR, cryptEncrypt file exists!");
                }
            }
            //both
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cryptDecrypt)
            {
                // disable
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt");
                        button2.BackColor = default(Color);
                        cryptDecrypt = false;
                    }
                }
            }
            else
            {
                // enable
                if (!File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                {
                    File.Create(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt");
                    button2.BackColor = Color.LightGreen;
                    cryptDecrypt = true;
                }
                else
                {
                    MessageBox.Show("ERROR, cryptDecrypt file exists!");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cryptEncrypt)
            {
                // disable
                if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                {
                    if (new FileInfo(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt").Length == 0)
                    {
                        File.Delete(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt");
                        button3.BackColor = default(Color);
                        cryptEncrypt = false;
                    }
                }
            }
            else
            {
                // enable
                if (!File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                {
                    File.Create(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt");
                    button3.BackColor = Color.LightGreen;
                    cryptEncrypt = true;
                }
                else
                {
                    MessageBox.Show("ERROR, cryptEncrypt file exists!");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // select eve dir
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(fbd.SelectedPath + "\\advapi32.dll"))
                {
                    button4.Enabled = false;
                    EVE_DIR = fbd.SelectedPath;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptDecrypt"))
                    {
                        button2.BackColor = Color.LightGreen;
                        cryptDecrypt = true;
                    }
                    if (File.Exists(EVE_DIR + "\\advapi32_config_dump_cryptEncrypt"))
                    {
                        button3.BackColor = Color.LightGreen;
                        cryptEncrypt = true;
                    }
                }
                else
                {
                    MessageBox.Show("advapi32.dll not found in given dir.");
                }
            }
        }
    }
}
