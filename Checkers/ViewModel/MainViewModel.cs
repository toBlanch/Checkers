using CanvasRect;
using Checkers.Events;
using Checkers.Model;
using Checkers.Net;
using System.Windows;

namespace Checkers.ViewModel;


public delegate void ConnectedPlayersChangedEventhandler(int e);
public delegate void MoveMadeEventhandler(MoveMadeEventArgs e);
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

        _server = new();
        _server.ConnectToServer();
        Server.ConnectedPlayersChanged += CheckersBoard.UpdateConnectedPlayers;
        Server.MoveMade += CheckersBoard.MoveMade;
    }
    public void Window_SizeChanged(double actualWidth, double actualHeight)
    {
        Graphics.window = new Vector(actualWidth, actualHeight); // Updates the window size
        CheckersBoard.RenderCanvasRect();
    }
}