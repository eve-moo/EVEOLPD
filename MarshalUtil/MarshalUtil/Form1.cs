using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eveMarshal;
using System.IO;

namespace MarshalUtil
{
    public partial class MarshalUtil : Form
    {
        public MarshalUtil()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Length == 0)
            {
                txtOutput.Text = "";
                return;
            }
            string hex = txtInput.Text.Replace(" ", "").Replace(System.Environment.NewLine, "");
            try
            {
                //txtOutput.Text = hex;
                byte[] raw = new Byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
                Console.WriteLine(raw.Length + ", " + hex.Length);
                Unmarshal un = new Unmarshal();
                //un.DebugMode = true;
                //PyObject obj = un.Process(BinaryReader.);
                PyRep obj = un.Process(raw);
                PrettyPrinter pp = new PrettyPrinter();
                txtOutput.Text = pp.Print(obj);
            }
            catch
            {
                hex = txtInput.Text.Substring(8, txtInput.Text.Length - 8).Replace(" ", "").Replace(System.Environment.NewLine, "");
                byte[] raw = new Byte[hex.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
                Console.WriteLine(raw.Length + ", " + hex.Length);
                Unmarshal un = new Unmarshal();
                un.DebugMode = true;
                // un.DebugMode = true;
                //PyObject obj = un.Process(BinaryReader.);
                PyRep obj = un.Process(raw);
                PrettyPrinter pp = new PrettyPrinter();
                txtOutput.Text = pp.Print(obj);
            }
            /*
            //txtOutput.Text = hex;
            byte[] raw = new Byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++) {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            Console.WriteLine(raw.Length+", "+hex.Length);
            Unmarshal un = new Unmarshal();
           // un.DebugMode = true;
            //PyObject obj = un.Process(BinaryReader.);
            PyObject obj = un.Process(raw);
            txtOutput.Text = PrettyPrinter.Print(obj);
             */
        }

        private void MarshalUtil_Resize(object sender, EventArgs e)
        {
            //this.txtInput.Width = (this.Width / 100) * 30;
            this.txtInput.Height = this.Height - 120;
            this.txtOutput.Width = this.Width - this.txtInput.Width - 50;
            this.txtOutput.Height = this.Height - 65;
        }

        private void txtInput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            txtInput.Text = "";
            txtInput.Paste();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }
    }
}
