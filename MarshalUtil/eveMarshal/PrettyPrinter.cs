using System;
using System.Linq;
using System.Text;

namespace eveMarshal
{
    public class PrettyPrinter
    {
        public PrettyPrinter()
        {
        }
        public const string Spacer = "    ";
        public bool analizeInput = false;
        public bool decompilePython = false;
        public string indent = Spacer;
        public int indentLevel;
        public bool interp;
        public StringBuilder builder;

        public string getIndent(int extra = 0)
        {
            return string.Concat(Enumerable.Repeat(indent, indentLevel + extra));
        }

        public static bool IsASCII(string value)
        {
            // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        public void addLine(string line)
        {
            builder.AppendLine(getIndent() + line);
        }

        public void addItem(PyRep rep)
        {
            indentLevel++;
            if (rep == null)
            {
                addLine("<nullptr>");
            }
            else
            {
                rep.dump(this);
            }
            indentLevel--;
        }

        public string Print(PyRep obj)
        {
            builder = new StringBuilder();
            if (obj == null)
            {
                addLine("<nullptr>");
            }
            else
            {
                obj.dump(this);
            }
            return builder.ToString();
        }


        public static string PrintRawData(PyRep obj)
        {
            if (obj.RawSource == null)
                return "";
            return " [" + BitConverter.ToString(obj.RawSource, 0, obj.RawSource.Length > 8 ? 8 : obj.RawSource.Length) + "]";
        }

        public static string StringToHex(string str)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static bool containsBinary(byte[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] < (byte)32 || p[i] > (byte)126)
                    return true;
            }
            return false;
        }

    }

}
