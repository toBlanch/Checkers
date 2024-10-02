using CheckersServer.Net.IO;
using System.Net.Sockets;

namespace CheckersServer;
internal class Client
{
    public Guid UID { get; set; }
    public TcpClient ClientSocket { get; set; }
    readonly PacketReader _packetReader;

    public Client(TcpClient client)
    {
        ClientSocket = client;
        UID = Guid.NewGuid();
        _packetReader = new(ClientSocket.GetStream());

        byte opcode = _packetReader.ReadByte();


        Console.WriteLine($"Client with UID {UID} connected with additional message {_packetReader.ReadMessage()}");
    }
}
