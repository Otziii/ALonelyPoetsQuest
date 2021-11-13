using System;
using System.Collections.Generic;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

namespace GameJamSadConsoleSample
{
    public class GameLoop
    {
        private const int Width = 91;
        private const int Height = 30;
        private const int gameWidth = 75;
        private const int gameHeight = 29;
        private static int battery = 4;
        private static List<string> itemList = new();
        private static DateTime startOfGame;
        private static int playerStartX = 3;
        private static int playerStartY = 1;
        private static Point playerMapPosition;
        private static int steps;
        private static int introSteps;
        private static TimeSpan completedTime;

        private static HUD hud;
        private static Map map;
        private static Intro intro;
        private static EndGame _endGame;

        private static Console fullHudConsole =
            new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, Width, Height));

        private static Console gameConsole =
            new ScrollingConsole(Width, Height, Global.FontDefault, new Rectangle(0, 0, gameWidth, gameHeight));

        private static string[] menuList = { "START GAME", "HOW TO PLAY", "EXIT" };

        private static string[] howToPlayList =
        {
            "- Navigate the dark, tight cave of Uranus to find your",
            "    Venusian lover's missing items.",
            "- You move Patrick William the Third around with the arrow keys.",
            "- Pick up power-tokens '8' to fill up the flashlight battery.",
            "",
            "",
            "Your goal: Pick up all the items and get back to the ship 'S'",
            "before your flashlight battery runs out."
        };

        private static int selectedMenuItem;
        private static int gameState = 2;

        private static void Main(string[] args)
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
            _endGame = new EndGame();

            playerMapPosition = new Point(9, 5);
        }

        private static void Update(GameTime time)
        {
            fullHudConsole.Clear();
            fullHudConsole.Children.Clear();
            gameConsole.Clear();

            switch (gameState)
            {
                // Intro
                case 0:
                    CheckIntroKeyboard();
                    RenderIntro();
                    break;
                // How to play
                case 1:
                    CheckMenuKeyboard();
                    RenderHowToPlay();
                    break;
                // Menu
                case 2:
                    CheckMenuKeyboard();
                    RenderMenu();
                    break;
                // Playing the game
                case 3:
                    CheckPlayerKeyboard();
                    RenderHud();
                    break;
                // Game Over
                case 4:
                    CheckEndKeyboard();
                    RenderGameOver();
                    break;
                // Game completed
                case 5:
                    CheckEndKeyboard();
                    RenderGameCompleted();
                    break;
            }

            Global.CurrentScreen.Children.Add(gameConsole);
            Global.CurrentScreen.Children.Add(fullHudConsole);
        }

        #region Keyboard

        private static void CheckEndKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter)
                || Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                gameState = 2;
            }
        }

        private static void CheckIntroKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter)
                || Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                introSteps++;
                if (introSteps > 3)
                {
                    introSteps = 3;
                    gameState = 3;
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
                    battery = 4;
                    break;
                case 'S':
                    if (itemList.Count == 5)
                    {
                        completedTime = DateTime.Now - startOfGame;
                        gameState = 5;
                    }

                    break;
                case '1':
                    if (!itemList.Contains("1"))
                    {
                        itemList.Add("1");
                        battery += 2;
                    }

                    break;
                case '2':
                    if (!itemList.Contains("2"))
                    {
                        itemList.Add("2");
                        battery += 2;
                    }

                    break;
                case '3':
                    if (!itemList.Contains("3"))
                    {
                        itemList.Add("3");
                        battery += 2;
                    }

                    break;
                case '4':
                    if (!itemList.Contains("4"))
                    {
                        itemList.Add("4");
                        battery += 2;
                    }

                    break;
                case '5':
                    if (!itemList.Contains("5"))
                    {
                        itemList.Add("5");
                        battery += 2;
                    }

                    break;
            }

            if (battery > 4)
            {
                battery = 4;
            }

            playerMapPosition = newPlayerPosition;
            steps++;
        }

        private static void CheckMenuKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                if (selectedMenuItem == 0)
                {
                    selectedMenuItem = menuList.Length - 1;
                }
                else
                {
                    selectedMenuItem--;
                }
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                if (selectedMenuItem == menuList.Length - 1)
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
                    case 1:
                        gameState = 2;
                        break;
                    case 2:
                        if (selectedMenuItem == 2)
                        {
                            Environment.Exit(1);
                        }
                        else
                        {
                            gameState = selectedMenuItem;
                            if (gameState == 0)
                            {
                                startOfGame = DateTime.Now;
                            }
                        }

                        break;
                }
            }
        }

        #endregion

        #region Render

        private static void RenderHud()
        {
            if (steps == 150)
            {
                steps = 0;
                battery--;

                if (battery == 0)
                {
                    gameState = 4;
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
                        hudLine = hud.GetBatteryLine(4, battery);
                        break;
                    case 8:
                        hudLine = hud.GetBatteryLine(3, battery);
                        break;
                    case 9:
                        hudLine = hud.GetBatteryLine(2, battery);
                        break;
                    case 10:
                        hudLine = hud.GetBatteryLine(1, battery);
                        break;
                    case 19:
                        hudLine = hud.GetItemsLine(itemList.Count);
                        break;
                    case 26:
                        var time = DateTime.Now - startOfGame;
                        hudLine = hud.GetTimerLine(time.Minutes.ToString("D2"), time.Seconds.ToString("D2"));
                        break;
                }

                fullHudConsole.Print(GetXForText(hudLine), i, hudLine, ColorAnsi.CyanBright);
            }

            RenderGame();
            RenderPlayer();
        }

        private static void RenderGame()
        {
            for (int i = 0; i <= gameHeight; i++)
            {
                if (i is >= 6 and <= 14)
                {
                    string line = map.GetLine(i - 8, playerMapPosition);
                    gameConsole.Print(30, 4 + i, line, ColorAnsi.White);
                }
            }
        }

        private static void RenderPlayer()
        {
            gameConsole.Print(gameWidth / 2, gameHeight / 2, "W", ColorAnsi.YellowBright);
        }

        private static void RenderMenu()
        {
            var welcome = "Welcome to A Lonely Poet's Quest";
            fullHudConsole.Print(GetXForText(welcome), 1, welcome, ColorAnsi.White);
            
            fullHudConsole.Print(0, 2, "", ColorAnsi.CyanBright);
            fullHudConsole.Print(0, 3, "", ColorAnsi.CyanBright);
            fullHudConsole.Print(0, 4, "", ColorAnsi.CyanBright);

            for (int i = 0; i < menuList.Length; i++)
            {
                var menuItem = menuList[i];
                var menuText = "";
                if (i == selectedMenuItem)
                {
                    menuText = "--- " + menuItem + " ---";
                }
                else
                {
                    menuText = menuItem;
                }

                fullHudConsole.Print(GetXForText(menuText), i + 7, menuText, ColorAnsi.Magenta);
            }
        }

        private static void RenderHowToPlay()
        {
            var howToPlay = "How to play A Lonely Poet's Quest";
            fullHudConsole.Print(GetXForText(howToPlay), 1, howToPlay, ColorAnsi.White);

            for (int i = 0; i < howToPlayList.Length; i++)
            {
                var howText = howToPlayList[i];
                fullHudConsole.Print(10, i + 10, howText, ColorAnsi.CyanBright);
            }

            var ok = "--- OK ---";
            fullHudConsole.Print(GetXForText(ok), 10 + howToPlayList.Length + 2, ok, ColorAnsi.Magenta);
        }

        #endregion

        private static void RenderIntro()
        {
            for (int i = 0; i < intro.intro[introSteps].Count; i++)
            {
                var line = intro.intro[introSteps][i];
                fullHudConsole.Print(2, i, line, ColorAnsi.CyanBright);
            }
        }

        private static void RenderGameOver()
        {
            for (int i = 0; i < _endGame.gameOver.Count; i++)
            {
                var line = _endGame.gameOver[i];
                fullHudConsole.Print(GetXForText(line), i, line, ColorAnsi.CyanBright);
            }

            var text = "-- You ran out of batteries -- ";
            fullHudConsole.Print(GetXForText(text), _endGame.gameOver.Count, text, ColorAnsi.CyanBright);
        }

        private static void RenderGameCompleted()
        {
            for (int i = 0; i < _endGame.gameCompleted.Count; i++)
            {
                var line = _endGame.gameCompleted[i];
                fullHudConsole.Print(0, i, line, ColorAnsi.CyanBright);
            }

            var text = "-- Your final time was: " + completedTime.Minutes.ToString("D2") + ":" +
                       completedTime.Seconds.ToString("D2") + " -- ";
            fullHudConsole.Print(GetXForText(text), _endGame.gameCompleted.Count, text, ColorAnsi.CyanBright);
        }

        #region Helpers

        private static int GetXForText(string text)
        {
            return (Width / 2) - ((text.Length - 1) / 2);
        }

        #endregion
    }
}