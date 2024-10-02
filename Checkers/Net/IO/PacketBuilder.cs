using System.IO;
using System.Text;

namespace Checkers.Net.IO;

class PacketBuilder
{
    readonly MemoryStream _ms;
    public PacketBuilder()
    {
        _ms = new();
    }

    public void WriteOpCode(byte opcode)
    {
        _ms.WriteByte(opcode);
    }

    public void WriteMessage(string str)
    {
        int strLength = str.Length;
        byte[] bytes = BitConverter.GetBytes(strLength);
        _ms.Write(bytes);
        _ms.Write(Encoding.UTF8.GetBytes(str));
    }
    public byte[] GetPacketBytes()
    {
        return _ms.ToArray();
    }
}
