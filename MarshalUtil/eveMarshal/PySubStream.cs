using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PySubStream : PyRep
    {
        public byte[] RawData { get; set; }
        public PyRep Data { get; set; }
        public Unmarshal DataUnmarshal { get; set; }

        public PySubStream()
            : base(PyObjectType.SubStream)
        {
            
        }

        public PySubStream(byte[] data)
             : base(PyObjectType.SubStream)
        {
            RawData = data;
            DataUnmarshal = new Unmarshal();
            Data = DataUnmarshal.Process(data);
        }

        public PySubStream(PyRep data)
            : base(PyObjectType.SubStream)
        {
            Data = data;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            uint len = context.reader.ReadSizeEx();
            RawData = context.reader.ReadBytes((int) len);
            DataUnmarshal = new Unmarshal();
            DataUnmarshal.analizeInput = context.analizeInput;
            Data = DataUnmarshal.Process(RawData);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.SubStream);
            var tempMs = new MemoryStream();
            var temp = new BinaryWriter(tempMs);
            temp.Write((byte)0x7E);
            temp.Write((uint)0);
            Data.Encode(temp);
            output.WriteSizeEx((uint)tempMs.Length);
            output.Write(tempMs.ToArray());
        }

        public override string ToString()
        {
            return "<SubStream: " + Data + ">";
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            if (RawData != null)
            {
                builder.AppendLine("[PySubStream " + RawData.Length + " bytes]");
            }
            else {
                builder.AppendLine("[PySubStream]");
            }
            PrettyPrinter.Print(builder, prefix + PrettyPrinter.Spacer, Data);
            return builder.ToString();
        }

    }

}