using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

namespace GameJamSadConsoleSample
{
    internal enum GameState
    {
        Intro = 0,
        HowToPlay = 1,
        Menu = 2,
        PlayGame = 3,
        GameOver = 4,
        Victory = 5
    }

    public static class GameLoop
    {
        private const int StepsBeforeBatteryGoesDown = 150;
        private const int Width = 91;
        private const int Height = 30;
        private const int GameWidth = 75;
        private const int GameHeight = 29;

        private static int currentBattery = 5;
        private static readonly List<string> ItemList = new();
        private static DateTime startOfGame;
        private static Point playerMapPosition;
        private static int steps;
        private static int introSteps;
        private static TimeSpan completedTime;

        private static HUD hud;
        private static Map map;
        private static Intro intro;
        private static EndGame endGame;

        private static readonly Console FullHudConsole =
            new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));

        private static readonly Console GameConsole =
            new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, GameWidth, GameHeight));

        private static readonly string[] MenuList = { "START GAME", "HOW TO PLAY", "EXIT" };

        private static readonly string[] HowToPlayList =
        {
            "- Navigate the dark, tight cave of Uranus to find your",
            "    Venusian lover's missing items.",
            "- You move Patrick William the Third around with the arrow keys.",
            "- Pick up power-tokens '8' to fill up the flashlight battery.",
            "",
            "",
            "Your goal: Pick up all the items and get back to the Ship",
            "before your flashlight battery runs out."
        };

        private static int selectedMenuItem;
        private static GameState gameState = GameState.Menu;

        private static void Main()
        {
            SadConsole.Game.Create(Width, Height);
            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.OnUpdate = Update;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            hud = new HUD();
            map = new Map();
            intro = new Intro();
            endGame = new EndGame();

            playerMapPosition = new Point(9, 5);
        }

        private static void Update(GameTime time)
        {
            FullHudConsole.Clear();
            FullHudConsole.Children.Clear();
            GameConsole.Clear();

            switch (gameState)
            {
                case GameState.Intro:
                    CheckIntroKeyboard();
                    RenderIntro();
                    break;
                case GameState.HowToPlay:
                    CheckMenuKeyboard();
                    RenderHowToPlay();
                    break;
                case GameState.Menu:
                    CheckMenuKeyboard();
                    RenderMenu();
                    break;
                case GameState.PlayGame:
                    CheckPlayerKeyboard();
                    RenderHud();
                    break;
                case GameState.GameOver:
                    CheckMenuKeyboard();
                    RenderGameOver();
                    break;
                case GameState.Victory:
                    CheckMenuKeyboard();
                    RenderGameCompleted();
                    break;
            }

            Global.CurrentScreen.Children.Add(GameConsole);
            Global.CurrentScreen.Children.Add(FullHudConsole);
        }

        #region Keyboard

        private static void CheckIntroKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter)
                || Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                introSteps++;
                if (introSteps > 3)
                {
                    introSteps = 3;
                    gameState = GameState.PlayGame;
                }
            }
        }

        private static void CheckPlayerKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                UpdatePlayerPositionIfPossible(new Point(playerMapPosition.X, playerMapPosition.Y - 1));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                UpdatePlayerPositionIfPossible(new Point(playerMapPosition.X, playerMapPosition.Y + 1));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                UpdatePlayerPositionIfPossible(new Point(playerMapPosition.X - 1, playerMapPosition.Y));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                UpdatePlayerPositionIfPossible(new Point(playerMapPosition.X + 1, playerMapPosition.Y));
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                ResetGame();
                gameState = GameState.Menu;
            }
        }

        private static void CheckMenuKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                if (selectedMenuItem == 0)
                {
                    selectedMenuItem = MenuList.Length - 1;
                }
                else
                {
                    selectedMenuItem--;
                }
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                if (selectedMenuItem == MenuList.Length - 1)
                {
                    selectedMenuItem = 0;
                }
                else
                {
                    selectedMenuItem++;
                }
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter)
                || Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                switch (gameState)
                {
                    case GameState.HowToPlay:
                    case GameState.GameOver:
                    case GameState.Victory:
                        gameState = GameState.Menu;
                        break;
                    case GameState.Menu:
                        if (selectedMenuItem == 2)
                        {
                            Environment.Exit(1);
                        }
                        else if (selectedMenuItem == 1)
                        {
                            gameState = GameState.HowToPlay;
                        }
                        else
                        {
                            ResetGame();
                            startOfGame = DateTime.Now;
                            gameState = GameState.Intro;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Render

        private static void RenderHud()
        {
            if (steps == StepsBeforeBatteryGoesDown)
            {
                steps = 0;
                currentBattery--;

                if (currentBattery == 0)
                {
                    completedTime = DateTime.Now - startOfGame;
                    gameState = GameState.GameOver;
                }
            }

            for (var i = 0; i < hud.HUDViewList.Length; i++)
            {
                var hudLine = hud.HUDViewList[i];

                // 6, 7, 8, 9, 10 = batteries
                // 19 = items
                // 26 = time
                switch (i)
                {
                    case 7:
                        hudLine = hud.GetBatteryLine(4, currentBattery);
                        break;
                    case 8:
                        hudLine = hud.GetBatteryLine(3, currentBattery);
                        break;
                    case 9:
                        hudLine = hud.GetBatteryLine(2, currentBattery);
                        break;
                    case 10:
                        hudLine = hud.GetBatteryLine(1, currentBattery);
                        break;
                    case 19:
                        hudLine = hud.GetItemsLine(ItemList.Count);
                        break;
                    case 26:
                        var time = DateTime.Now - startOfGame;
                        hudLine = hud.GetTimerLine(time.Minutes.ToString("D2"), time.Seconds.ToString("D2"));
                        break;
                }

                FullHudConsole.Print(GetXForCenterText(hudLine), i, hudLine, ColorAnsi.CyanBright);
            }

            RenderGame();
            RenderPlayer();
        }

        private static void RenderGame()
        {
            for (var i = 0; i <= GameHeight; i++)
            {
                if (i is >= 6 and <= 14)
                {
                    var line = map.GetLine(i - 8, playerMapPosition);
                    GameConsole.Print(30, 4 + i, line, ColorAnsi.White);

                    PrintItemForLine(line, i);
                }
            }
        }

        private static void RenderPlayer()
        {
            GameConsole.Print(GameWidth / 2, GameHeight / 2, "W", ColorAnsi.YellowBright);
        }

        private static void RenderMenu()
        {
            var welcome = "Welcome to A Lonely Poet's Quest";
            FullHudConsole.Print(GetXForCenterText(welcome), 1, welcome, ColorAnsi.White);

            FullHudConsole.Print(0, 2, "", ColorAnsi.CyanBright);
            FullHudConsole.Print(0, 3, "", ColorAnsi.CyanBright);
            FullHudConsole.Print(0, 4, "", ColorAnsi.CyanBright);

            for (int i = 0; i < MenuList.Length; i++)
            {
                var menuItem = MenuList[i];
                string menuText;
                if (i == selectedMenuItem)
                {
                    menuText = "--- " + menuItem + " ---";
                }
                else
                {
                    menuText = menuItem;
                }

                FullHudConsole.Print(GetXForCenterText(menuText), i + 7, menuText, ColorAnsi.Magenta);
            }
        }

        private static void RenderHowToPlay()
        {
            var howToPlay = "How to play A Lonely Poet's Quest";
            FullHudConsole.Print(GetXForCenterText(howToPlay), 1, howToPlay, ColorAnsi.White);

            for (int i = 0; i < HowToPlayList.Length; i++)
            {
                var howText = HowToPlayList[i];
                FullHudConsole.Print(10, i + 10, howText, ColorAnsi.CyanBright);
            }

            var ok = "--- OK ---";
            FullHudConsole.Print(GetXForCenterText(ok), 10 + HowToPlayList.Length + 2, ok, ColorAnsi.Magenta);
        }

        private static void RenderIntro()
        {
            for (int i = 0; i < intro.intro[introSteps].Count; i++)
            {
                var line = intro.intro[introSteps][i];
                FullHudConsole.Print(2, i, line, ColorAnsi.CyanBright);
            }
        }

        private static void RenderGameOver()
        {
            for (int i = 0; i < endGame.gameOver.Count; i++)
            {
                var line = endGame.gameOver[i];
                FullHudConsole.Print(GetXForCenterText(line), i, line, ColorAnsi.CyanBright);
            }

            var text = "-- You ran out of batteries -- ";
            FullHudConsole.Print(GetXForCenterText(text), endGame.gameOver.Count, text, ColorAnsi.CyanBright);
            var text2 = "-- This is how long you survived: " + completedTime.Minutes.ToString("D2") + ":" +
                        completedTime.Seconds.ToString("D2") + " --";
            FullHudConsole.Print(GetXForCenterText(text2), endGame.gameOver.Count + 1, text2, ColorAnsi.CyanBright);
        }

        private static void RenderGameCompleted()
        {
            for (int i = 0; i < endGame.gameCompleted.Count; i++)
            {
                var line = endGame.gameCompleted[i];
                if (i >= 7 && i <= 14)
                {
                    FullHudConsole.Print(GetXForCenterText(line), i, line, ColorAnsi.RedBright);
                }
                else
                {
                    FullHudConsole.Print(GetXForCenterText(line), i, line, ColorAnsi.CyanBright);
                }
            }

            var text = "-- Your final time was: " + completedTime.Minutes.ToString("D2") + ":" +
                       completedTime.Seconds.ToString("D2") + " -- ";
            FullHudConsole.Print(GetXForCenterText(text), endGame.gameCompleted.Count, text, ColorAnsi.CyanBright);
            var text2 = "-- Now you can finally travel to Mars and meet up with your lover --";
            FullHudConsole.Print(GetXForCenterText(text2), endGame.gameCompleted.Count + 1, text2,
                ColorAnsi.CyanBright);
        }

        #endregion

        #region Helpers

        private static int GetXForCenterText(string text)
        {
            return (Width / 2) - ((text.Length - 1) / 2);
        }

        private static void PrintItemForLine(string line, int lineNumber)
        {
            if (line.Contains("88"))
            {
                var pos = line.IndexOf("88", StringComparison.Ordinal);
                GameConsole.Print(30 + pos, 4 + lineNumber, "88", ColorAnsi.Green);
            }
            else if (line.Contains("8"))
            {
                var pos = line.IndexOf("8", StringComparison.Ordinal);
                GameConsole.Print(30 + pos, 4 + lineNumber, "8", ColorAnsi.Green);
            }
            
            if (line.Contains("Ship"))
            {
                var pos = line.IndexOf("Ship", StringComparison.Ordinal);
                GameConsole.Print(30 + pos, 4 + lineNumber, "Ship", ColorAnsi.BlueBright);
            }

            foreach (var item in map.itemLines)
            {
                foreach (var itemLine in item)
                {
                    if (line.Contains(itemLine))
                    {
                        var pos = line.IndexOf(itemLine, StringComparison.Ordinal);
                        GameConsole.Print(30 + pos, 4 + lineNumber, itemLine, ColorAnsi.RedBright);
                    }
                }
            }
        }

        private static void UpdatePlayerPositionIfPossible(Point newPlayerPosition)
        {
            var line = map.mapList[newPlayerPosition.Y - 1];
            var stepChar = line[newPlayerPosition.X];

            switch (stepChar)
            {
                case '|' or '#' or '=' or '@':
                    return;
                case '8':
                    currentBattery = 4;
                    break;
                case 'S':
                    if (ItemList.Count == 5)
                    {
                        completedTime = DateTime.Now - startOfGame;
                        gameState = GameState.Victory;
                    }

                    break;
                case '1':
                    if (!ItemList.Contains("1"))
                    {
                        ItemList.Add("1");
                        currentBattery += 2;
                    }

                    break;
                case '2':
                    if (!ItemList.Contains("2"))
                    {
                        ItemList.Add("2");
                        currentBattery += 2;
                    }

                    break;
                case '3':
                    if (!ItemList.Contains("3"))
                    {
                        ItemList.Add("3");
                        currentBattery += 2;
                    }

                    break;
                case '4':
                    if (!ItemList.Contains("4"))
                    {
                        ItemList.Add("4");
                        currentBattery += 2;
                    }

                    break;
                case '5':
                    if (!ItemList.Contains("5"))
                    {
                        ItemList.Add("5");
                        currentBattery += 2;
                    }

                    break;
            }

            if (currentBattery > 4)
            {
                currentBattery = 4;
            }

            playerMapPosition = newPlayerPosition;
            steps++;
        }

        private static void ResetGame()
        {
            steps = 0;
            currentBattery = 4;
            ItemList.Clear();
            introSteps = 0;
        }

        #endregion
    }
}