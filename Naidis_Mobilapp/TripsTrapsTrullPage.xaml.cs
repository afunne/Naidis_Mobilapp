using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace Naidis_Mobilapp
{
    public partial class TripsTrapsTrullPage : ContentPage
    {
        TicTacToeLogic logic = null!;
        Button[,] cellButtons = null!;
        int size = 3;
        string playerXSymbol = "X";
        string playerOSymbol = "O";
        Color playerXColor = Colors.Black;
        Color playerOColor = Colors.Red;
        bool playWithBot = false;

        // Statistics
        int gamesPlayed = 0;
        int winsX = 0;
        int winsO = 0;
        int draws = 0;

        // Theme
        bool isDarkTheme = false;
        Color lightBg = Colors.White;
        Color darkBg = Colors.DarkSlateGray;

        public TripsTrapsTrullPage()
        {
            InitializeComponent();
            LoadStatsAndSettings();
            SetupBoard(size);
        }

        void SetupBoard(int boardSize)
        {
            logic = new TicTacToeLogic(boardSize);
            cellButtons = new Button[boardSize, boardSize];
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();
            BackgroundColor = isDarkTheme ? darkBg : lightBg;

            for (int i = 0; i < boardSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    var btn = new Button
                    {
                        BackgroundColor = isDarkTheme ? Colors.Gray : Colors.White,
                        FontSize = 32,
                        Text = "",
                        BorderColor = Colors.Gray,
                        BorderWidth = 2
                    };
                    int rr = r, cc = c;
                    btn.Clicked += async (s, e) => await OnCellClickedAnimated(rr, cc);
                    GameGrid.Add(btn, c, r);
                    cellButtons[r, c] = btn;
                }
            }
        }

        async Task OnCellClickedAnimated(int r, int c)
        {
            if (logic.Board[r, c] != null) return;
            string symbol = logic.CurrentPlayer == "X" ? playerXSymbol : playerOSymbol;
            Color color = logic.CurrentPlayer == "X" ? playerXColor : playerOColor;
            cellButtons[r, c].Text = symbol;
            cellButtons[r, c].TextColor = color;
            await cellButtons[r, c].ScaleToAsync(1.2, 100);
            await cellButtons[r, c].ScaleToAsync(1.0, 100);
            logic.MakeMove(r, c);

            if (logic.CheckWin())
            {
                gamesPlayed++;
                if (logic.CurrentPlayer == "X") winsX++; else winsO++;
                SaveStatsAndSettings();
                await AnimateWinLine();
                await ShowAlert("Võit!", $"{symbol} võitis! Kas soovid veel mängida?");
                NewGame();
            }
            else if (logic.IsDraw())
            {
                gamesPlayed++;
                draws++;
                SaveStatsAndSettings();
                await ShowAlert("Viik!", "Viik! Kas soovid veel mängida?");
                NewGame();
            }
            else
            {
                logic.SwitchPlayer();
                if (playWithBot && logic.CurrentPlayer == "O")
                {
                    BotMove();
                }
            }
        }

        async Task AnimateWinLine()
        {
            // Simple animation: flash winning cells
            var winCells = logic.GetWinCells();
            if (winCells == null) return;
            for (int i = 0; i < 3; i++)
            {
                foreach (var (r, c) in winCells)
                    cellButtons[r, c].BackgroundColor = Colors.Yellow;
                await Task.Delay(200);
                foreach (var (r, c) in winCells)
                    cellButtons[r, c].BackgroundColor = isDarkTheme ? Colors.Gray : Colors.White;
                await Task.Delay(200);
            }
            foreach (var (r, c) in winCells)
                cellButtons[r, c].BackgroundColor = Colors.Yellow;
        }

        void BotMove()
        {
            // Simple bot: pick first empty cell
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    if (logic.Board[r, c] == null)
                    {
                        _ = OnCellClickedAnimated(r, c);
                        return;
                    }
        }

        void NewGame()
        {
            logic.RandomizeFirstPlayer();
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                {
                    cellButtons[r, c].Text = "";
                    cellButtons[r, c].BackgroundColor = isDarkTheme ? Colors.Gray : Colors.White;
                }
            logic = new TicTacToeLogic(size);
            playWithBot = false;
        }

        private void OnNewGameClicked(object sender, EventArgs e)
        {
            NewGame();
        }

        private async void OnWhoStartsClicked(object sender, EventArgs e)
        {
            logic.RandomizeFirstPlayer();
            await ShowAlert("Alustaja", $"Alustab: {logic.CurrentPlayer}");
        }

        private async void OnColorClicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheetAsync("Vali sümbol/värv", "Cancel", null, "X must", "X sinine", "O punane", "O roheline");
            switch (result)
            {
                case "X must": playerXColor = Colors.Black; break;
                case "X sinine": playerXColor = Colors.Blue; break;
                case "O punane": playerOColor = Colors.Red; break;
                case "O roheline": playerOColor = Colors.Green; break;
            }
        }

        private async void OnBotClicked(object sender, EventArgs e)
        {
            playWithBot = true;
            await ShowAlert("Mäng botiga", "Bot on nüüd vastane!");
            if (logic.CurrentPlayer == "O") BotMove();
        }

        private async void OnSizeClicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheetAsync("Välja suurus", "Cancel", null, "3x3", "4x4", "5x5");
            switch (result)
            {
                case "3x3": size = 3; break;
                case "4x4": size = 4; break;
                case "5x5": size = 5; break;
                default: return;
            }
            SetupBoard(size);
        }

        private async void OnRulesClicked(object sender, EventArgs e)
        {
            string stats = $"Mänge: {gamesPlayed}\nX võidud: {winsX}\nO võidud: {winsO}\nViigid: {draws}";
            string theme = isDarkTheme ? "Tume" : "Hele";
            await ShowAlert("Reeglid / Info", $"Trips-Traps-Trull: Mängi X ja O-ga, võidab, kui saad järjest.\n\nStatistika:\n{stats}\n\nTeema: {theme}");
        }

        // Theme toggle
        public void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
            SetupBoard(size);
            SaveStatsAndSettings();
        }

        // Save/load stats and settings
        void SaveStatsAndSettings()
        {
            Preferences.Set("gamesPlayed", gamesPlayed);
            Preferences.Set("winsX", winsX);
            Preferences.Set("winsO", winsO);
            Preferences.Set("draws", draws);
            Preferences.Set("isDarkTheme", isDarkTheme);
        }

        void LoadStatsAndSettings()
        {
            gamesPlayed = Preferences.Get("gamesPlayed", 0);
            winsX = Preferences.Get("winsX", 0);
            winsO = Preferences.Get("winsO", 0);
            draws = Preferences.Get("draws", 0);
            isDarkTheme = Preferences.Get("isDarkTheme", false);
        }

        async Task ShowAlert(string title, string message)
        {
            var popup = new SimpleAlertPopup(title, message);
            await this.ShowPopupAsync(popup);
        }
    }

    public class TicTacToeLogic
    {
        public string[,] Board { get; private set; }
        public string CurrentPlayer { get; private set; } = "X";
        public int BoardSize { get; private set; }

        public TicTacToeLogic(int size)
        {
            BoardSize = size;
            Board = new string[size, size];
        }

        public bool MakeMove(int r, int c)
        {
            if (r < 0 || r >= BoardSize || c < 0 || c >= BoardSize || Board[r, c] != null)
                return false;
            Board[r, c] = CurrentPlayer;
            return true;
        }

        public void SwitchPlayer() => CurrentPlayer = (CurrentPlayer == "X") ? "O" : "X";
        public void RandomizeFirstPlayer() => CurrentPlayer = (new Random().Next(2) == 0) ? "X" : "O";

        public bool CheckWin()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (CheckLine(i, 0, 0, 1) || CheckLine(0, i, 1, 0)) return true;
            }
            return CheckLine(0, 0, 1, 1) || CheckLine(0, BoardSize - 1, 1, -1);
        }

        // Returns winning cells for animation
        public (int, int)[]? GetWinCells()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (CheckLine(i, 0, 0, 1)) return GetLineCells(i, 0, 0, 1);
                if (CheckLine(0, i, 1, 0)) return GetLineCells(0, i, 1, 0);
            }
            if (CheckLine(0, 0, 1, 1)) return GetLineCells(0, 0, 1, 1);
            if (CheckLine(0, BoardSize - 1, 1, -1)) return GetLineCells(0, BoardSize - 1, 1, -1);
            return null;
        }

        private bool CheckLine(int r, int c, int dr, int dc)
        {
            string start = Board[r, c];
            if (start == null) return false;
            for (int i = 1; i < BoardSize; i++)
            {
                if (Board[r + i * dr, c + i * dc] != start) return false;
            }
            return true;
        }

        private (int, int)[] GetLineCells(int r, int c, int dr, int dc)
        {
            var cells = new (int, int)[BoardSize];
            for (int i = 0; i < BoardSize; i++)
                cells[i] = (r + i * dr, c + i * dc);
            return cells;
        }

        public bool IsDraw()
        {
            foreach (var cell in Board) if (cell == null) return false;
            return !CheckWin();
        }
    }
}
