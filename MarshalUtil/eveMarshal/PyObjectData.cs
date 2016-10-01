using System.Data;
using System.IO;
using System.Text;

namespace eveMarshal
{
    
    public class PyObjectData : PyObject
    {
        public PyObjectData()
            : base(PyObjectType.ObjectData)
        {
            
        }

        public PyObjectData(string objectName, PyObject arguments)
            : base(PyObjectType.ObjectData)
        {
            Name = objectName;
            Arguments = arguments;
        }

        public string Name { get; set; }
        public PyObject Arguments { get; set; }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            var nameObject = context.ReadObject(source);
            if (nameObject.Type != PyObjectType.String)
                throw new DataException("Expected PyString");
            Name = (nameObject as PyString).Value;

            Arguments = context.ReadObject(source);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Object);
            new PyString(Name).Encode(output);
            Arguments.Encode(output);
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PyObjectData Name: " + Name + "]" + PrettyPrinter.PrintRawData(this));
            PrettyPrinter.Print(builder, prefix + PrettyPrinter.Spacer, Arguments);
            return builder.ToString();
        }

    }

}