using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class NotificationStream : ExtendedObject
    {
        private long zeroOne;
        public PyRep notice { get; private set; }

        public NotificationStream(PyRep payload)
        {
            PyTuple sub = getZeroSubStream(payload as PyTuple, out zeroOne) as PyTuple;
            PyRep args = sub;
            if (sub != null)
            {
                if (zeroOne == 0)
                {
                    args = getZeroOne(sub);
                }
            }
            if (args == null)
            {
                throw new InvalidDataException("NotificationStream: null args.");
            }
            notice = args;
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[NotificationStream " + (zeroOne == 0 ? "Tuple01" : "SubStream") + "]");
            printer.addItem(notice);
        }
    }
}
