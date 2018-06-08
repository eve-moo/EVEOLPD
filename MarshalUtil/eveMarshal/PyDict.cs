using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace eveMarshal
{

    public class PyDict : PyRep
    {
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }
        
        public PyDict()
            : base(PyObjectType.Dict)
        {
            Dictionary = new Dictionary<PyRep, PyRep>();
        }
        
        public PyDict(Dictionary<PyRep, PyRep> dict)
            : base(PyObjectType.Dict)
        {
            Dictionary = dict;
        }

        public PyRep Get(string key)
        {
            var keyObject =
                Dictionary.Keys.Where(k => k.Type == PyObjectType.String && (k as PyString).Value == key).FirstOrDefault();
            return keyObject == null ? null : Dictionary[keyObject];
        }

        public void Set(string key, PyRep value)
        {
            var keyObject = Dictionary.Count > 0 ? Dictionary.Keys.Where(k => k.Type == PyObjectType.String && (k as PyString).Value == key).FirstOrDefault() : null;
            if (keyObject != null)
                Dictionary[keyObject] = value;
            else
                Dictionary.Add(new PyString(key), value);
        }

        public bool Contains(string key)
        {
            return Dictionary.Keys.Any(k => k.Type == PyObjectType.String && (k as PyString).Value == key);
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            var entries = context.reader.ReadSizeEx();
            Dictionary = new Dictionary<PyRep, PyRep>((int)entries);
            for (uint i = 0; i < entries; i++)
            {
                var value = context.ReadObject();
                var key = context.ReadObject();
                Dictionary.Add(key, value);
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Dict);
            output.WriteSizeEx(Dictionary.Count);
            foreach (var pair in Dictionary)
            {
                pair.Value.Encode(output);
                pair.Key.Encode(output);
            }
        }

        public PyRep this[PyRep key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                Dictionary[key] = value;
            }
        }

        public override PyRep this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<\n");
            foreach (var pair in Dictionary)
                sb.AppendLine("\t" + pair.Key + " " + pair.Value);
            sb.Append(">");
            return sb.ToString();
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyDict " + Dictionary.Count + " kvp]" + PrettyPrinter.PrintRawData(this));
            printer.indentLevel++;
            foreach (var kvp in Dictionary)
            {
                printer.addLine("Key:");
                printer.addItem(kvp.Key);
                if (kvp.Value == null)
                {
                    printer.addLine("Value: <nullptr>");
                }
                else
                {
                    printer.addLine("Value:");
                    printer.addItem(kvp.Value);
                }
            }
            printer.indentLevel--;
        }

    }

}