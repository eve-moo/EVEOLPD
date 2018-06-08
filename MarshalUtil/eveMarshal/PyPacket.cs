using eveMarshal.Extended;
using System;
using System.IO;
using System.Text;

namespace eveMarshal
{
    public class PyPacket : PyRep
    {
        public string typeString;
        public long packetType;
        public long userID;
        public PyAddress source;
        public PyAddress dest;
        public PyRep payload;
        public PyRep namedPayload;

        public PyPacket(PyObject packetData)
            : base(PyObjectType.Packet)
        {
            if (!packetData.Name.StartsWith("carbon.common.script.net.machoNetPacket."))
            {
                // This is not a valid addressed packet.
                throw new InvalidDataException("PyPacket: Invalid typeString.");
            }
            typeString = packetData.Name;
            if (packetData.Arguments.Type != PyObjectType.Tuple)
            {
                // This is not a valid addressed packet.
                throw new InvalidDataException("PyPacket: Arguments not a Tuple.");
            }
            PyTuple args = packetData.Arguments as PyTuple;
            if (args.Items.Count != 9)
            {
                // This is not a correct size packet.
                throw new InvalidDataException("PyPacket: Expected Tuple size 9 got " + args.Items.Count + ".");
            }
            if (args.Items[0].Type != PyObjectType.Int)
            {
                // Incorrect packet type identifier.
                throw new InvalidDataException("PyPacket: Packet Type identifier.");
            }
            packetType = args.Items[0].IntValue;
            source = new PyAddress(args.Items[1]);
            dest = new PyAddress(args.Items[2]);
            userID = 0;
            if (args.Items[3].Type == PyObjectType.Int)
            {
                userID = args.Items[3].IntValue;
            }
            else if (args.Items[3].Type != PyObjectType.None)
            {
                throw new InvalidDataException("PyPacket: UserID expected Int or None got " + args.Items[3].Type.ToString() + ".");
            }
            payload = args.Items[4];
            namedPayload = args.Items[5];
            if (payload.Type != PyObjectType.Tuple && payload.Type != PyObjectType.Buffer)
            {
                throw new InvalidDataException("PyPacket: payload expected Tuple or Buffer got " + args.Items[3].Type.ToString() + ".");
            }
            if (namedPayload.Type != PyObjectType.None && namedPayload.Type != PyObjectType.Dict)
            {
                throw new InvalidDataException("PyPacket: named payload expected Dict or None got " + args.Items[3].Type.ToString() + ".");
            }
            RawOffset = args.RawOffset;
            RawSource = args.RawSource;
            try
            {
                if (typeString.EndsWith(".SessionChangeNotification"))
                {
                    payload = new SessionChangeNotification(payload as PyTuple);
                }
                else if (typeString.EndsWith(".CallRsp"))
                {
                    payload = new CallRsp(payload as PyTuple);
                }
                else if (typeString.EndsWith(".CallReq"))
                {
                    payload = new PyCallStream(payload as PyTuple);
                }
                else if (typeString.EndsWith(".ErrorResponse"))
                {
                    payload = new ErrorResponse(payload as PyTuple);
                }
                else if (typeString.EndsWith(".Notification"))
                {
                    payload = new NotificationStream(payload as PyTuple);
                }
            }
            catch (InvalidDataException e)
            {
                throw new InvalidDataException("PyPacket: Extended decode failed: " + e.Message, e);
            }
        }

        public override string ToString()
        {
            string src = source.ToString();
            string dst = dest.ToString();
            string packet = "Packet:\nSource:\n" + src + "\nDest:\n" + dst + "\n" + payload + "\n" + namedPayload;
            return "";
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyPacket typeID=" + packetType + "  userID=" + userID + "  name='" + typeString + "']");
            printer.indentLevel++;
            printer.addLine("Source:");
            printer.addItem(source);
            printer.addLine("Destination:");
            printer.addItem(dest);
            printer.addLine("Payload:");
            printer.addItem(payload);
            printer.addLine("Named Payload:");
            printer.addItem(namedPayload);
            printer.indentLevel--;
        }
    }
}
