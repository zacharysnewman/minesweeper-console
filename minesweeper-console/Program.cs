using System;
using System.Linq;
using System.Text.RegularExpressions;
using Events.ConsoleEvents;
using Events.InputEvents;
using System.Text.Json;

namespace consoleminesweeper
{
    class Program
    {
        private static MapInformation mapInfo = new MapInformation(10, 10, 10);
        public static bool isRunning = true;

        public static void Main(string[] args)
        {
            Game.Init();
            Renderer.Init();

            EventAggregator.Get<GenerateMapEvent>().Publish(mapInfo);

            while (Program.isRunning)
            {
                var rawInput = Console.ReadLine();
                (CommandType command, Match match) = InterpretRawCommand(rawInput);
                ExecuteCommand(command, match);
            }
        }

        private static void ExecuteCommand(CommandType command, Match match)
        {
            string[] rows = "abcdefghijklmnopqrstuvwxyz".ToCharArray().Select(x => x.ToString()).ToArray();

            switch (command)
            {
                case CommandType.New:
                    Console.WriteLine($"Generating new map...");
                    EventAggregator.Get<GenerateMapEvent>().Publish(mapInfo);
                    break;
                case CommandType.NewWithParams:
                    int rawBombs = int.Parse(match.Groups[1].Value.ToLower());
                    int bombs = rawBombs < 1 ? 1 : rawBombs > 99 ? 99 : rawBombs;
                    mapInfo = mapInfo.WithBombs(bombs);
                    Console.WriteLine($"Generating new map with {bombs} bombs...");
                    EventAggregator.Get<GenerateMapEvent>().Publish(mapInfo);
                    break;
                case CommandType.FlagWithParams:
                    int flagRow = Array.IndexOf(rows, match.Groups[1].Value.ToString().ToLower());
                    int flagCol = int.Parse(match.Groups[2].Value.ToLower());
                    Console.WriteLine("Flagging coordinates or checking nearby tiles...");
                    EventAggregator.Get<ActivateTileEvent>().Publish(new Coords(flagRow, flagCol), true);
                    break;
                case CommandType.CheckCoords:
                    int activateRow = Array.IndexOf(rows, match.Groups[1].Value.ToString().ToLower());
                    int activateCol = int.Parse(match.Groups[2].Value.ToLower());
                    Console.WriteLine("Checking coordinates and nearby tiles...");
                    EventAggregator.Get<ActivateTileEvent>().Publish(new Coords(activateRow, activateCol), false);
                    break;
                case CommandType.None:
                    Console.WriteLine("Invalid Command! Try again.");
                    break;
            }
        }
        private static (CommandType, Match) InterpretRawCommand(string rawCommand)
        {
            if (NewWithParamsRegex.IsMatch(rawCommand))
            {
                return (CommandType.NewWithParams, NewWithParamsRegex.Match(rawCommand));
            }
            else if (FlagSimpleWithParamsRegex.IsMatch(rawCommand))
            {
                return (CommandType.FlagWithParams, FlagSimpleWithParamsRegex.Match(rawCommand));
            }
            else if (FlagWithParamsRegex.IsMatch(rawCommand))
            {
                return (CommandType.FlagWithParams, FlagWithParamsRegex.Match(rawCommand));
            }
            else if (NewRegex.IsMatch(rawCommand))
            {
                return (CommandType.New, null);
            }
            else if (CheckSimpleWithParamsRegex.IsMatch(rawCommand))
            {
                return (CommandType.CheckCoords, CheckSimpleWithParamsRegex.Match(rawCommand));
            }
            else if (CheckWithParamsRegex.IsMatch(rawCommand))
            {
                return (CommandType.CheckCoords, CheckWithParamsRegex.Match(rawCommand));
            }
            else
            {
                return (CommandType.None, null);
            }
        }

        private static Regex NewRegex = new Regex("new", RegexOptions.IgnoreCase);
        private static Regex NewWithParamsRegex = new Regex("new ([0-9]+)", RegexOptions.IgnoreCase);
        private static Regex FlagWithParamsRegex = new Regex("flag ([a-zA-Z])([0-9]+)", RegexOptions.IgnoreCase);
        private static Regex FlagSimpleWithParamsRegex = new Regex("f ([a-zA-Z])([0-9]+)", RegexOptions.IgnoreCase);
        private static Regex CheckWithParamsRegex = new Regex("check ([a-zA-Z])([0-9]+)", RegexOptions.IgnoreCase);
        private static Regex CheckSimpleWithParamsRegex = new Regex("([a-zA-Z])([0-9]+)", RegexOptions.IgnoreCase);

    }
}
