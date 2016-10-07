using eveMarshal.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal
{

    public class Unmarshal
    {
        public const byte SaveMask = 0x40;
        public const byte UnknownMask = 0x80;
        public const byte HeaderByte = 0x7E;

        // not a real magic since zlib just doesn't include one..
        public const byte ZlibMarker = 0x78;
        public const byte OpcodeMask = 0x3F;

        public bool DebugMode { get; set; }

        public Dictionary<int, int> SavedElementsMap { get; private set; }
        public PyObject[] SavedElements { get; private set; }

        private int _currentSaveIndex;

        public PyObject Process(byte[] data)
        {
            if (data == null)
                return null;
            if (data[0] == ZlibMarker)
                data = Zlib.Decompress(data);
            return Process(new BinaryReader(new MemoryStream(data), Encoding.ASCII));
        }

        private PyObject Process(BinaryReader reader)
        {
            var magic = reader.ReadByte();
            if (magic != HeaderByte)
            {
                throw new InvalidDataException("Invalid magic, expected: " + HeaderByte + " read: " + magic);
            }
            var saveCount = reader.ReadUInt32();

            if (saveCount > 0)
            {
                var currentPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(-saveCount * 4, SeekOrigin.End);
                SavedElementsMap = new Dictionary<int, int>((int)saveCount);
                for (int i = 0; i < saveCount; i++)
                {
                    var index = reader.ReadInt32();
                    if (index < 0)
                        throw new InvalidDataException("Bogus map data in marshal stream");
                    SavedElementsMap.Add(i, index);
                }
                SavedElements = new PyObject[saveCount];
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }

            return ReadObject(reader);
        }

        private PyObject CreateAndDecode<T>(BinaryReader reader, MarshalOpcode op) where T : PyObject, new()
        {
            // -1 for the opcode
            var ret = new T { RawOffset = reader.BaseStream.Position - 1 };
            ret.Decode(this, op, reader);
            if (PyObject.EnableInspection)
            {
                var postOffset = reader.BaseStream.Position;
                reader.BaseStream.Seek(ret.RawOffset, SeekOrigin.Begin);
                ret.RawSource = reader.ReadBytes((int)(postOffset - ret.RawOffset));
            }

            return ret;
        }

        public PyObject ReadObject(BinaryReader reader)
        {
            var header = reader.ReadByte();
            //bool flagUnknown = (header & UnknownMask) > 0;
            bool flagSave = (header & SaveMask) > 0;
            var opcode = (MarshalOpcode)(header & OpcodeMask);
            int saveIndex = 0;
            if (flagSave)
            {
                // Get save index now.
                // If there are nested saves the indexes will be wrong if we wait.
                saveIndex = SavedElementsMap[_currentSaveIndex++];
            }
            PyObject ret;
            //Console.WriteLine("OPCODE: "+opcode);
            switch (opcode)
            {
                case MarshalOpcode.SubStruct:
                    ret = CreateAndDecode<PySubStruct>(reader, opcode);
                    break;

                case MarshalOpcode.BoolFalse:
                case MarshalOpcode.BoolTrue:
                    ret = CreateAndDecode<PyBool>(reader, opcode);
                    break;

                case MarshalOpcode.None:
                    ret = CreateAndDecode<PyNone>(reader, opcode);
                    break;
                case MarshalOpcode.Token:
                    ret = CreateAndDecode<PyToken>(reader, opcode);
                    break;
                case MarshalOpcode.Real:
                case MarshalOpcode.RealZero:
                    ret = CreateAndDecode<PyFloat>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerLongLong:
                    ret = CreateAndDecode<PyLongLong>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerSignedShort:
                case MarshalOpcode.IntegerByte:
                case MarshalOpcode.IntegerMinusOne:
                case MarshalOpcode.IntegerOne:
                case MarshalOpcode.IntegerZero:
                case MarshalOpcode.IntegerLong:
                    ret = CreateAndDecode<PyInt>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerVar:
                    ret = CreateAndDecode<PyIntegerVar>(reader, opcode);
                    break;
                case MarshalOpcode.Buffer:
                    ret = CreateAndDecode<PyBuffer>(reader, opcode);
                    break;
                case MarshalOpcode.StringEmpty:
                case MarshalOpcode.StringChar:
                case MarshalOpcode.StringShort:
                case MarshalOpcode.StringTable:
                case MarshalOpcode.StringLong:
                case MarshalOpcode.WStringEmpty:
                case MarshalOpcode.WStringUCS2:
                case MarshalOpcode.WStringUCS2Char:
                case MarshalOpcode.WStringUTF8:
                    ret = CreateAndDecode<PyString>(reader, opcode);
                    break;
                case MarshalOpcode.Tuple:
                case MarshalOpcode.TupleOne:
                case MarshalOpcode.TupleTwo:
                case MarshalOpcode.TupleEmpty:
                    ret = CreateAndDecode<PyTuple>(reader, opcode);
                    break;
                case MarshalOpcode.List:
                case MarshalOpcode.ListOne:
                case MarshalOpcode.ListEmpty:
                    ret = CreateAndDecode<PyList>(reader, opcode);
                    break;
                case MarshalOpcode.Dict:
                    ret = CreateAndDecode<PyDict>(reader, opcode);
                    break;
                case MarshalOpcode.Object:
                    ret = CreateAndDecode<PyObjectData>(reader, opcode);
                    break;
                case MarshalOpcode.ChecksumedStream:
                    ret = CreateAndDecode<PyChecksumedStream>(reader, opcode);
                    break;
                case MarshalOpcode.SubStream:
                    ret = CreateAndDecode<PySubStream>(reader, opcode);
                    break;
                case MarshalOpcode.SavedStreamElement:
                    uint index = reader.ReadSizeEx();
                    ret = SavedElements[index - 1];
                    break;
                case MarshalOpcode.ObjectEx1:
                case MarshalOpcode.ObjectEx2:
                    ret = CreateAndDecode<PyObjectEx>(reader, opcode);
                    break;
                case MarshalOpcode.PackedRow:
                    ret = CreateAndDecode<PyPackedRow>(reader, opcode);
                    break;
                default:
                    throw new InvalidDataException("Failed to marshal " + opcode);
            }

            if (flagSave)
            {
                if(saveIndex == 0)
                {
                    // This only seams to occure in GPSTransport packets when the server shuts down.
                    saveIndex = 1;
                }
                if (saveIndex > 0)
                {
                    SavedElements[saveIndex - 1] = ret;
                }
            }

            if (DebugMode)
            {
                Console.WriteLine("Offset: " + ret.RawOffset + " Length: " + ret.RawSource.Length + " Opcode: " + opcode + " Type: " + ret.Type + " Result: " + ret);
                Console.WriteLine(Utility.HexDump(ret.RawSource));
                Console.ReadLine();
            }

            return analyse(ret);
        }

        public static T Process<T>(byte[] data) where T : class
        {
            var un = new Unmarshal();
            return un.Process(data) as T;
        }

        private PyObject analyse(PyObject obj)
        {
            try
            {
                if (obj is PyObjectEx)
                {
                    PyObjectEx ex = obj as PyObjectEx;
                    if (!ex.IsType2)
                    {
                        PyTuple headerTuple = ex.Header as PyTuple;
                        if (headerTuple != null && headerTuple.Items.Count > 1)
                        {
                            PyToken token = headerTuple.Items[0] as PyToken;
                            if (token != null)
                            {
                                if (token.Token == "__builtin__.set")
                                {
                                    PyTuple tuple = headerTuple.Items[1] as PyTuple;
                                    if (tuple != null && tuple.Items.Count > 0)
                                    {
                                        return new BuiltinSet(tuple.Items[0] as PyList);
                                    }
                                }
                                if (token.Token == "carbon.common.script.net.machoNetExceptions.WrongMachoNode")
                                {
                                    if (headerTuple.Items.Count == 3 && headerTuple.Items[2] is PyDict)
                                    {
                                        PyDict dict = headerTuple.Items[2] as PyDict;
                                        return new WrongMachoNode(dict);
                                    }
                                }
                                if (token.Token == "blue.DBRowDescriptor")
                                {
                                    return new DBRowDescriptor(headerTuple);
                                }
                            }
                        }
                    }
                    // type 2
                    //eve.common.script.dogma.effect.BrainEffect
                    //carbon.common.script.sys.crowset.CRowset
                    //carbon.common.script.sys.crowset.CIndexedRowset
                }
            }
            catch (InvalidDataException)
            {
                return obj;
            }
            return obj;
        }
    }

}