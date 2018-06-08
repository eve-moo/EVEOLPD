using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Python
{
    public class PyObject : IEquatable<PyObject>
    {
        public ObjectType type = ObjectType.None;
        public PyDict attributes;

        public PyObject()
        { }

        public PyObject(ObjectType objectType)
        {
            type = objectType;
        }

        public string asString()
        {
            switch (type)
            {
                case ObjectType.False:
                    return "false";
                case ObjectType.None:
                    return "None";
                case ObjectType.True:
                    return "true";
                case ObjectType.Code:
                    return "<code>";
                case ObjectType.StringRef:
                    return "<ref>";
                case ObjectType.String:
                case ObjectType.UnicodeString:
                case ObjectType.Interned:
                    return (this as PyString).str;
                case ObjectType.Tuple:
                    return "<tuple>";
                case ObjectType.Int:
                    return (this as PyInt).value.ToString();
                default:
                    return "Unknown object";
            }
        }

        public virtual bool load(BinaryReader reader)
        {
            return true;
        }
        public virtual void dump(PrettyPrinter printer)
        {
            switch (type)
            {
                case ObjectType.False:
                    printer.addLine("false");
                    break;
                case ObjectType.None:
                    printer.addLine("None");
                    break;
                case ObjectType.True:
                    printer.addLine("true");
                    break;
                default:
                    printer.addLine("Error!");
                    break;
            }
        }

        public virtual bool Equals(PyObject other)
        {
            if(other == null)
            {
                return false;
            }
            return other.ToString() == ToString();
        }
        public override int GetHashCode()
        {
            return hash();
        }
        public virtual int hash()
        {
            return type.GetHashCode() ^ attributes.GetHashCode();
        }

        public override string ToString()
        {
            if (attributes == null)
            {
                return "<" + type.ToString() + ">";
            }
            else
            {
                return "<" + type.ToString() + " attributes: " + attributes.ToString() + ">";
            }
        }
    }

    public class PyInt : PyObject
    {
        public Int32 value = 0;
        public PyInt()
        {
            type = ObjectType.Int;
        }
        public PyInt(int val)
        {
            type = ObjectType.Int;
            value = val;
        }
        public override bool load(BinaryReader reader)
        {
            value = reader.ReadInt32();
            return true;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("{" + value.ToString() + "}");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyInt)
            {
                return (other as PyInt).value == value;
            }
            if (other is PyLong)
            {
                return (other as PyLong).value == value;
            }
            if (other is PyString)
            {
                PyString str = other as PyString;
                UInt32 ov = UInt32.Parse(str.str);
                return ov == value;
            }
            return false;
        }
        public override int hash()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class PyLong : PyObject
    {
        public Int64 value = 0;
        public PyLong()
        {
            type = ObjectType.Long;
        }
        public PyLong(int val)
        {
            type = ObjectType.Long;
            value = val;
        }
        public override bool load(BinaryReader reader)
        {
            value = reader.ReadInt64();
            return true;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("{" + value.ToString() + "}");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyInt)
            {
                return (other as PyInt).value == value;
            }
            if (other is PyLong)
            {
                return (other as PyLong).value == value;
            }
            if (other is PyString)
            {
                PyString str = other as PyString;
                UInt32 ov = UInt32.Parse(str.str);
                return ov == value;
            }
            return false;
        }
        public override int hash()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class PyStringRef : PyObject
    {
        public UInt32 index = 0;
        public override bool load(BinaryReader reader)
        {
            index = reader.ReadUInt32();
            return true;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("#{Ref: " + index.ToString() + ":dec}");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyStringRef)
            {
                return (other as PyStringRef).index == index;
            }
            return false;
        }
        public override int hash()
        {
            return index.GetHashCode();
        }
    }

    public class PyString : PyObject
    {
        public int size = 0;
        public string str;
        public PyString()
        {
            type = ObjectType.String;
        }
        public PyString(string val)
        {
            type = ObjectType.String;
            str = val;
            size = str.Length;
        }
        public override bool load(BinaryReader reader)
        {
            size = reader.ReadInt32();
            if (type == ObjectType.String)
            {
                str = new string(reader.ReadChars(size));
            }
            else
            {
                str = System.Text.Encoding.GetEncoding("utf-8").GetString(reader.ReadBytes(size));
            }
            return true;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("{String: " + str + "}");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyString)
            {
                return (other as PyString).str == str;
            }
            if (other is PyInt)
            {
                PyInt ov = other as PyInt;
                return ov.value.ToString() == str;
            }
            if (other is PyLong)
            {
                PyLong ov = other as PyLong;
                return ov.value.ToString() == str;
            }
            return false;
        }
        public override int hash()
        {
            return str.GetHashCode();
        }
    }

    public class PyTuple : PyObject
    {
        public int size = 0;
        public List<PyObject> items;
        public override bool load(BinaryReader reader)
        {
            size = reader.ReadInt32();
            items = new List<PyObject>(size);
            for (int i = 0; i < size; i++)
            {
                items.Add(Bytecode.loadObject(reader));
            }
            return true;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("{Tuple " + size.ToString() + ":dec}");
            for (int i = 0; i < size; i++)
            {
                printer.addItem(items[i]);
            }
            printer.addLine("{end Tuple}");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyTuple)
            {
                PyTuple tup = other as PyTuple;
                if (tup.items.Count != items.Count)
                {
                    return false;
                }
                for(int i = 0;i < items.Count;i++)
                {
                    if(tup.items[i] != items[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override int hash()
        {
            int hash = items.Count.GetHashCode();
            foreach(PyObject obj in items)
            {
                hash ^= obj.GetHashCode();
            }
            return hash;
        }
        public PyObject this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
    }

    public class PyDict : PyObject
    {
        public Dictionary<PyObject, PyObject> items;
        public PyDict()
        {
            items = new Dictionary<PyObject, PyObject>();
        }
        public PyDict(PyDict dict)
        {
            items = new Dictionary<PyObject, PyObject>(dict.items);
        }

        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyDict)
            {
                PyDict tup = other as PyDict;
                if (tup.items.Count != items.Count)
                {
                    return false;
                }
                foreach (PyObject key in items.Keys)
                {
                    if (!tup.items.ContainsKey(key))
                    {
                        return false;
                    }
                    if (tup.items[key] != items[key])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override int hash()
        {
            int hash = items.Count.GetHashCode();
            foreach (var obj in items)
            {
                hash ^= obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
            }
            return hash;
        }
        public PyObject this[PyObject index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
        public PyObject this[string index]
        {
            get
            {
                return items[new PyString(index)];
            }
            set
            {
                items[new PyString(index)] = value;
            }
        }
    }

    public class PyList : PyObject
    {
        public List<PyObject> items;
        public PyList()
        {
            items = new List<PyObject>();
        }
        public PyList(PyList dict)
        {
            items = new List<PyObject>(dict.items);
        }

        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is PyList)
            {
                PyList tup = other as PyList;
                if (tup.items.Count != items.Count)
                {
                    return false;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if (tup.items[i] != items[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override int hash()
        {
            int hash = items.Count.GetHashCode();
            foreach (PyObject obj in items)
            {
                hash ^= obj.GetHashCode();
            }
            return hash;
        }
        public PyObject this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
    }

    public class Module : PyObject
    {
        public string name;
        public string source;
        public Module(string moduleName, string src)
        {
            name = moduleName;
            source = src;
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("<Module '" + name + " from ' (" + source + ")>");
        }
        public override bool Equals(PyObject other)
        {
            if (other == null)
            {
                return false;
            }
            if (other is Module)
            {
                Module mod = other as Module;
                return (mod.name == name) && (mod.source == source);
            }
            return false;
        }
        public override int hash()
        {
            return name.GetHashCode() ^ source.GetHashCode();
        }
    }
}
