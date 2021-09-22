using System;
using System.Collections.Generic;
using System.Linq;
using Events.ConsoleEvents;
using Events.StateEvents;

namespace consoleminesweeper
{
    public static class Renderer
    {
        private static readonly ConsoleString Hidden = new ConsoleString("#", Color.None, Color.None);
        private static readonly ConsoleString Flagged = new ConsoleString("#", Color.Red, Color.None);
        private static readonly ConsoleString BombRevealed = new ConsoleString("B", Color.Black, Color.Gray);
        private static readonly ConsoleString BombIncorrect = new ConsoleString("X", Color.Red, Color.None);
        private static readonly ConsoleString BombDetonated = new ConsoleString("B", Color.Black, Color.Red);
        private static readonly ConsoleString[] NearbyBombs = {
            new ConsoleString(" ", Color.None, Color.None),
            new ConsoleString("1", Color.Blue, Color.None),
            new ConsoleString("2", Color.Green, Color.None),
            new ConsoleString("3", Color.Red, Color.None),
            new ConsoleString("4", Color.DarkBlue, Color.None),
            new ConsoleString("5", Color.DarkRed, Color.None),
            new ConsoleString("6", Color.Cyan, Color.None),
            new ConsoleString("7", Color.Black, Color.None),
            new ConsoleString("8", Color.Gray, Color.None)};

        private static readonly string CommandList =
            "Commands:\n" +
            "check <row><col> (Ex: 'check a3' or 'g6') [Reveals a tile] \n" +
            "flag <row><col> (Ex: 'flag a3' or 'f g6') [Flags a tile]\n" +
            "new [Generates a new map]\n" +
            "new <bombs> (Ex: 'new 15') [Generates a new map with X bombs]";


        public static void Init()
        {
            EventAggregator.Get<StateChangedEvent>().Subscribe(OnStateChanged);
        }

        public static void OnStateChanged(State newState)
        {
            var (mapInfo, tilesDict) = newState;
            var tiles = tilesDict.Select(tileKeyValuePair => tileKeyValuePair.Value);
            var winLoseStatus = WinOrLoseCheck(tiles);

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Write(CoolRendering.MinesweeperLogoSmall);

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            Write("\n                                               \n     ");
            for (int i = 0; i < 10; i++)
            {
                Write(i + "   ");
            }
            Write("  \n");

            for (int x = 0; x < mapInfo.Width; x++)
            {
                Write("   |---|---|---|---|---|---|---|---|---|---|   \n");
                Write(" " + "abcdefghijklmnopqrstuvwxyz".ToUpper()[x] + " | ");
                for (int y = 0; y < mapInfo.Height; y++)
                {
                    GetTileString(tilesDict, tilesDict[new Coords(x, y)], winLoseStatus).Write();
                    Write(" | ");
                }
                Write("abcdefghijklmnopqrstuvwxyz".ToUpper()[x].ToString());
                Write(" \n");
            }
            Write("   |---|---|---|---|---|---|---|---|---|---|   \n     ");
            for (int i = 0; i < 10; i++)
            {
                Write(i + "   ");
            }
            Write("  \n                                               \n\n");

            Console.ResetColor();

            Write(Renderer.CommandList);
            Write("\n\n");
            var winOrLoseStatus = WinOrLoseCheck(tiles);
            var gameStatus = winOrLoseStatus == WinLoseStatus.win ? "You Won!" : winLoseStatus == WinLoseStatus.lose ? "You Lost!" : "In Progress...";
            Write($"Game Status: {gameStatus}");

            Write("\n\nEnter Command: ");
            Console.ResetColor();
        }

        private static Action<object> Write = Console.Write;

        public enum WinLoseStatus { none, win, lose }
        private static WinLoseStatus WinOrLoseCheck(IEnumerable<Tile> tiles)
        {
            var isBombRevealed = tiles.Any(x => x.isBomb && x.tileState == TileState.revealed);
            if (isBombRevealed)
            {
                return WinLoseStatus.lose;
            }
            var allTilesAreRevealed = tiles.All(x => x.isBomb || (!x.isBomb && x.tileState == TileState.revealed));
            if (allTilesAreRevealed)
            {
                return WinLoseStatus.win;
            }

            return WinLoseStatus.none;
        }

        private static ConsoleString GetTileString(Dictionary<Coords, Tile> tilesDict, Tile tile, WinLoseStatus winLoseStatus)
        {
            ConsoleString tileString = Renderer.Hidden;
            switch (tile.tileState)
            {
                case TileState.hidden:
                    if (tile.isBomb && winLoseStatus == WinLoseStatus.lose)
                    {
                        tileString = Renderer.BombRevealed;
                    }
                    else
                    {
                        tileString = Renderer.Hidden;
                    }
                    break;
                case TileState.revealed:
                    if (tile.isBomb)
                    {
                        tileString = Renderer.BombDetonated;
                    }
                    else
                    {
                        var bombCount = Map.GetNearbyBombCount(tilesDict, tile.coords);
                        tileString = Renderer.NearbyBombs[bombCount];
                    }
                    break;
                case TileState.flagged:
                    if (!tile.isBomb && winLoseStatus == WinLoseStatus.lose)
                    {
                        tileString = Renderer.BombIncorrect;
                    }
                    else
                    {
                        tileString = Renderer.Flagged;
                    }
                    break;
            }
            return tileString;
        }
    }
}
