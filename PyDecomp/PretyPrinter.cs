using System.Linq;
using static Python.Bytecode;

namespace Python
{
    public class PrettyPrinter
    {
        public string indent = "    ";
        public int indentLevel;
        public string dump;
        public bool interp;

        public string getIndent(int extra = 0)
        {
            return string.Concat(Enumerable.Repeat(indent, indentLevel + extra));
        }
        public static string print(Bytecode decomp, bool interp)
        {
            PrettyPrinter printer = new PrettyPrinter();
            printer.interp = interp;
            printer.indentLevel = 0;
            decomp.dump(printer);
            return printer.dump;
        }
        public void addLine(string line)
        {
            dump += getIndent() + line + System.Environment.NewLine;
        }
        public void addItem(PyObject obj)
        {
            indentLevel++;
            if (obj != null)
            {
                obj.dump(this);
            }
            else
            {
                addLine("<nullptr>");
            }
            indentLevel--;
        }
    }
}
