using CanvasRect;
using Checkers.Events;
using Checkers.Model;
using Checkers.Net;
using Checkers.View;
using System.Windows;
using System.Windows.Input;

namespace Checkers.ViewModel;

public delegate void ConnectedPlayersChangedEventhandler(int e);
public delegate void ServerMadeMoveEventHander(MoveMadeEventArgs e);
public delegate void CheckerPieceClickedEventhandler(object sender, MouseButtonEventArgs e);
class MainViewModel
{
  private readonly Main _main = new();
  public CheckersBoardViewModel CheckersBoard { get; } = new();
  private Server _server
  {
    get => _main.Server;
    set => _main.Server = value;
  }

  public MainViewModel()
  {
    _server.ConnectToServer();
    CheckersBoardView.CheckerPieceClicked += CheckersBoard.Grid_MouseLeftButtonDown;
    Server.ConnectedPlayersChanged += CheckersBoard.UpdateConnectedPlayers;
    Server.ServerMadeMove += CheckersBoard.MoveMade;
  }

  public void Window_SizeChanged(double actualWidth, double actualHeight)
  {
    Graphics.window = new Vector(actualWidth, actualHeight); // Updates the window size
    CheckersBoard.RenderCanvasRect();
  }
}