using CheckersServer;
using CheckersServer.Net.IO;
using System.Net;
using System.Net.Sockets;

internal class Program
{
    static List<Client>? _users;
    static TcpListener? _listener;
    private static void Main(string[] args)
    {
        _users = new();
        _listener = new(IPAddress.Parse("127.0.0.1"), 7891);
        _listener.Start();

        while (true)
        {
            Client client = new(_listener.AcceptTcpClient());
            _users.Add(client);
            BroadcastConnection();


        }
    }

    static void BroadcastConnection()
    {
        foreach (Client user in _users)
        {
            PacketBuilder broadcastPacket = new();
            broadcastPacket.WriteOpCode(1);
            broadcastPacket.WriteMessage($"Connected users: {_users.Count}");
            user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
        }
    }
}