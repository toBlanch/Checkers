using System.Net.Sockets;
using System.Text;

namespace CheckersServer.Net.IO;
internal class PacketReader : BinaryReader
{
    private readonly NetworkStream _ns;
    public PacketReader(NetworkStream ns) : base(ns)
    {
        _ns = ns;
    }
    public string ReadMessage()
    {
        byte[] msgBuffer;
        int length = ReadInt32();
        msgBuffer = new byte[length];
        _ns.Read(msgBuffer, 0, length);

        string msg = Encoding.ASCII.GetString(msgBuffer);
        return msg;
    }
}
