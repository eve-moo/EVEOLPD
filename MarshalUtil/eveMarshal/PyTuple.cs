using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PyTuple : PyRep, IEnumerable<PyRep>
    {
        public List<PyRep> Items { get; private set; }

        public PyTuple()
            : base(PyObjectType.Tuple)
        {
            Items = new List<PyRep>();
        }

        public PyTuple(List<PyRep> items)
            : base(PyObjectType.Tuple)
        {
            Items = items;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            int count = -1;
            switch (op)
            {
                case MarshalOpcode.TupleEmpty:
                    count = 0;
                    break;
                case MarshalOpcode.TupleOne:
                    count = 1;
                    break;
                case MarshalOpcode.TupleTwo:
                    count = 2;
                    break;
                case MarshalOpcode.Tuple:
                    count = (int)context.reader.ReadSizeEx();
                    break;
            }

            if (count >= 0)
            {
                Items = new List<PyRep>(count);
                for (int i = 0; i < count; i++)
                    Items.Add(context.ReadObject());
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            if (Items.Count == 0)
                output.WriteOpcode(MarshalOpcode.TupleEmpty);
            else
            {
                if (Items.Count == 1)
                    output.WriteOpcode(MarshalOpcode.TupleOne);
                else if (Items.Count == 2)
                    output.WriteOpcode(MarshalOpcode.TupleTwo);
                else
                {
                    output.WriteOpcode(MarshalOpcode.Tuple);
                    output.WriteSizeEx(Items.Count);
                }

                foreach (var item in Items)
                    item.Encode(output);
            }
        }

        public override PyRep this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }

        public IEnumerator<PyRep> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<\n");
            foreach (var obj in Items)
                sb.AppendLine("\t" + obj);
            sb.Append(">");
            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyTuple " + Items.Count + " items]" + PrettyPrinter.PrintRawData(this));
            foreach (var item in Items)
            {
                if (item != null)
                {
                    printer.addItem(item);
                }
                else
                {
                    printer.addLine("<nullptr>");
                }
            }
        }
    }

}