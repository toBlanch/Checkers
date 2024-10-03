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

  public static void BroadcastConnection()
  {
    BroadcastMessage($"Connected users: {_users.Count}", 1);
  }
  public static void BroadcastDisconnection(Guid UID)
  {
    Client? disconnectedUser = _users.Where(x => x.UID == UID).FirstOrDefault();
    _users.Remove(disconnectedUser);
    BroadcastConnection();
  }
  public static void BroadcastMessage(string message, byte opcode)
  {
    foreach (Client user in _users)
    {
      PacketBuilder broadcastPacket = new();
      broadcastPacket.WriteOpCode(opcode);
      broadcastPacket.WriteMessage(message);
      user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
    }
  }
}