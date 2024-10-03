using Checkers.Net.IO;
using Checkers.ViewModel;
using System.Net.Sockets;

namespace Checkers.Net;

class Server
{
    readonly TcpClient _client;
    public PacketReader PacketReader;
    public int ID = 0;

    public static event ConnectedPlayersChangedEventhandler? ConnectedPlayersChanged;
    public static event MoveMadeEventhandler? MoveMade;

    public Server()
    {
        _client = new TcpClient();
    }

    public void ConnectToServer()
    {
        if (!_client.Connected)
        {
            _client.Connect("127.0.0.1", 7891);
            PacketReader = new(_client.GetStream());

            PacketBuilder connectPacket = new();
            connectPacket.WriteOpCode(0);
            connectPacket.WriteMessage("Hello World");
            _client.Client.Send(connectPacket.GetPacketBytes());

            ReadPackets();
        }
    }

    private void ReadPackets()
    {
        Task.Run(() =>
        {
            while (true)
            {
                byte opcode = PacketReader.ReadByte();
                switch (opcode)
                {
                    case 1:
                        int.TryParse(PacketReader.ReadMessage().Split(":")[1], out int connectedUsers);
                        ConnectedPlayersChanged?.Invoke(this, connectedUsers);
                        break;
                    default:
                        Console.WriteLine("No value assigned");
                        break;
                }
            }
        });
    }
}
