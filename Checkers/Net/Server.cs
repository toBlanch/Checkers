using Checkers.Events;
using Checkers.Net.IO;
using Checkers.ViewModel;
using System.Net.Sockets;

namespace Checkers.Net;

public delegate void ClientMadeMoveEventHandler(MoveMadeEventArgs e);
class Server
{
  readonly TcpClient _client;
  public PacketReader? PacketReader;

  public static event ConnectedPlayersChangedEventhandler? ConnectedPlayersChanged;
  public static event ServerMadeMoveEventHander? ServerMadeMove;

  public Server()
  {
    _client = new TcpClient();
    CheckersBoardViewModel.ClientMadeMove += SendMove;
  }

  public void ConnectToServer()
  {
    if (!_client.Connected)
    {
      _client.Connect("127.0.0.1", 7891);
      PacketReader = new(_client.GetStream());

      SendMessage("progress", 0);

      ReadPackets();
    }
  }

  private void ReadPackets()
  {
    Task.Run(() =>
    {
      while (true)
      {
        byte opcode = PacketReader!.ReadByte();
        switch (opcode)
        {
          case 1:
            int.TryParse(PacketReader.ReadMessage().Split(":")[1], out int connectedUsers);
            ConnectedPlayersChanged?.Invoke(connectedUsers);
            break;
          case 2:
            string[] moveData = PacketReader.ReadMessage().Split(":");
            ServerMadeMove?.Invoke(new MoveMadeEventArgs(
                    int.Parse(moveData[0]),
                    int.Parse(moveData[1]))
                );
            break;
          default:
            Console.WriteLine("No value assigned");
            break;
        }
      }
    });
  }
  private void SendMove(MoveMadeEventArgs e)
  {
    SendMessage($"{e.CheckerInitialIndex}:{e.CheckerNewIndex}", 2);
  }
  private void SendMessage(string message, byte opcode)
  {
    PacketBuilder connectPacket = new();
    connectPacket.WriteOpCode(opcode);
    connectPacket.WriteMessage(message);
    _client.Client.Send(connectPacket.GetPacketBytes());
  }
}
