using Naidis_Mobilapp.Models;

namespace Naidis_Mobilapp;

public partial class MinesweeperPage : ContentPage
{
    private const int Rows = 8;
    private const int Columns = 8;
    private const int Mines = 10;
    private const double CellSize = 35;

    private readonly List<Theme> _themes;
    private readonly IDispatcherTimer _timer;
    private readonly Button[,] _cellButtons = new Button[Rows, Columns];
    private Player _player;
    private Game _game;

    public MinesweeperPage()
    {
        InitializeComponent();

        _themes = CreateThemes();
        _player = new Player(PlayerNameEntry.Text);
        _game = new Game(Rows, Columns, Mines, _player);
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateStats();

        ThemePicker.ItemsSource = _themes;
        ThemePicker.SelectedIndex = 0;

        CreateBoard();
        StartGame();
    }

    private static List<Theme> CreateThemes()
    {
        return new List<Theme>
        {
            new(
                "Garden",
                Color.FromArgb("#F7F1E5"),
                Color.FromArgb("#FFFDF8"),
                Color.FromArgb("#263238"),
                Color.FromArgb("#2E7D7A"),
                Color.FromArgb("#6E9C89"),
                Color.FromArgb("#EFE7D3"),
                Color.FromArgb("#C44536"),
                "OpenSansRegular"),
            new(
                "Midnight",
                Color.FromArgb("#171923"),
                Color.FromArgb("#242938"),
                Color.FromArgb("#F8FAFC"),
                Color.FromArgb("#6EA8FE"),
                Color.FromArgb("#394150"),
                Color.FromArgb("#2D3342"),
                Color.FromArgb("#F97373"),
                "OpenSansSemibold"),
            new(
                "Candy",
                Color.FromArgb("#FFF0F5"),
                Color.FromArgb("#FFFFFF"),
                Color.FromArgb("#3B2645"),
                Color.FromArgb("#D9468F"),
                Color.FromArgb("#83C5BE"),
                Color.FromArgb("#FFE8A3"),
                Color.FromArgb("#EF476F"),
                "BobloxFont")
        };
    }

    private void CreateBoard()
    {
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();
        BoardGrid.Children.Clear();

        BoardGrid.WidthRequest = Columns * CellSize + (Columns - 1) * BoardGrid.ColumnSpacing;

        for (var row = 0; row < Rows; row++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = CellSize });
        }

        for (var column = 0; column < Columns; column++)
        {
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = CellSize });
        }

        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                var button = new Button
                {
                    Padding = 0,
                    CornerRadius = 6,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    HeightRequest = CellSize,
                    WidthRequest = CellSize,
                    MinimumHeightRequest = CellSize,
                    MinimumWidthRequest = CellSize,
                    BindingContext = (Row: row, Column: column)
                };

                button.SetDynamicResource(Button.BackgroundColorProperty, "MinesHiddenCellColor");
                button.SetDynamicResource(Button.TextColorProperty, "MinesTextColor");
                button.SetDynamicResource(Button.FontFamilyProperty, "MinesFontFamily");
                button.Clicked += OnCellClicked;

                _cellButtons[row, column] = button;
                BoardGrid.Add(button, column, row);
            }
        }
    }

    private void StartGame()
    {
        _game.Start();
        _timer.Start();
        PauseButton.Text = "Pause";
        FlagModeCheckBox.IsChecked = false;
        BoardGrid.Rotation = 0;

        foreach (var button in _cellButtons)
        {
            button.IsEnabled = true;
            button.Text = string.Empty;
            button.Opacity = 1;
            button.Scale = 1;
            button.SetDynamicResource(Button.BackgroundColorProperty, "MinesHiddenCellColor");
            button.SetDynamicResource(Button.TextColorProperty, "MinesTextColor");
        }

        UpdateStats();
    }

    private async void OnCellClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not ValueTuple<int, int> position)
        {
            return;
        }

        await button.ScaleToAsync(0.88, 45, Easing.CubicOut);
        await button.ScaleToAsync(1, 70, Easing.CubicIn);

        if (FlagModeCheckBox.IsChecked)
        {
            if (_game.ToggleFlag(position.Item1, position.Item2))
            {
                DrawCell(_game.Board[position.Item1, position.Item2]);
                UpdateStats();
            }

            return;
        }

        var changedCells = _game.OpenCell(position.Item1, position.Item2);

        foreach (var cell in changedCells)
        {
            DrawCell(cell);
        }

        UpdateStats();

        if (_game.IsEnded)
        {
            await FinishGame();
        }
    }

    private async Task FinishGame()
    {
        _timer.Stop();

        if (_game.IsWon)
        {
            foreach (var cell in _game.Board)
            {
                if (cell.IsMine && !cell.IsFlagged)
                {
                    cell.ToggleFlag();
                    DrawCell(cell);
                }
            }

            await BoardGrid.RotateToAsync(360, 550, Easing.CubicInOut);
            await DisplayAlertAsync(
                "Victory",
                $"{_player.Name} won with {_game.Score} points in {FormatTime(_game.Elapsed)}.",
                "Nice");
        }
        else
        {
            foreach (var cell in _game.RevealAllMines())
            {
                DrawCell(cell);
            }

            await BoardGrid.TranslateToAsync(-10, 0, 40);
            await BoardGrid.TranslateToAsync(10, 0, 40);
            await BoardGrid.TranslateToAsync(0, 0, 40);
            await DisplayAlertAsync(
                "Boom",
                $"{_player.Name} scored {_game.Score} points. Try again!",
                "Again");
        }

        foreach (var button in _cellButtons)
        {
            button.IsEnabled = false;
        }

        UpdateStats();
    }

    private void DrawCell(MinesweeperCell cell)
    {
        var button = _cellButtons[cell.Row, cell.Column];

        if (cell.IsFlagged && !cell.IsOpen)
        {
            button.Text = "!";
            button.SetDynamicResource(Button.BackgroundColorProperty, "MinesAccentColor");
            button.TextColor = Colors.White;
            return;
        }

        if (!cell.IsOpen)
        {
            button.Text = string.Empty;
            button.SetDynamicResource(Button.BackgroundColorProperty, "MinesHiddenCellColor");
            button.SetDynamicResource(Button.TextColorProperty, "MinesTextColor");
            return;
        }

        button.IsEnabled = false;
        button.SetDynamicResource(Button.BackgroundColorProperty, cell.IsMine ? "MinesDangerColor" : "MinesOpenCellColor");
        button.TextColor = cell.IsMine ? Colors.White : GetNumberColor(cell.NearbyMines);
        button.Text = cell.IsMine
            ? "*"
            : cell.NearbyMines == 0 ? string.Empty : cell.NearbyMines.ToString();
        _ = button.FadeToAsync(cell.IsMine ? 1 : 0.9, 120);
    }

    private static Color GetNumberColor(int nearbyMines)
    {
        return nearbyMines switch
        {
            1 => Color.FromArgb("#1976D2"),
            2 => Color.FromArgb("#2E7D32"),
            3 => Color.FromArgb("#C62828"),
            4 => Color.FromArgb("#512DA8"),
            _ => Color.FromArgb("#374151")
        };
    }

    private void UpdateStats()
    {
        ScoreLabel.Text = $"Score: {_game.Score}";
        TimeLabel.Text = $"Time: {FormatTime(_game.Elapsed)}";
        FlagsLabel.Text = $"Flags: {_game.RemainingFlags}";
    }

    private static string FormatTime(TimeSpan elapsed)
    {
        return $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}";
    }

    private void OnNewGameClicked(object sender, EventArgs e)
    {
        StartGame();
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        if (_game.IsPaused)
        {
            _game.Resume();
            _timer.Start();
            PauseButton.Text = "Pause";
            SetBoardEnabled(true);
        }
        else
        {
            _game.Pause();
            _timer.Stop();
            PauseButton.Text = "Resume";
            SetBoardEnabled(false);
        }

        UpdateStats();
    }

    private void SetBoardEnabled(bool isEnabled)
    {
        foreach (var button in _cellButtons)
        {
            var position = ((int Row, int Column))button.BindingContext;
            button.IsEnabled = isEnabled && !_game.Board[position.Row, position.Column].IsOpen;
        }
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if (ThemePicker.SelectedItem is Theme selectedTheme)
        {
            selectedTheme.Apply(this);
        }
    }

    private void OnPlayerNameChanged(object sender, TextChangedEventArgs e)
    {
        _player.Name = string.IsNullOrWhiteSpace(e.NewTextValue) ? "Player" : e.NewTextValue.Trim();
    }

    private async void OnTopScoreClicked(object sender, EventArgs e)
    {
        var bestTime = _player.BestTime is null ? "--:--" : FormatTime(_player.BestTime.Value);
        await DisplayAlertAsync(
            "Top score",
            $"Best score: {_player.BestScore}\nBest time: {bestTime}\nWon: {_player.GamesWon}\nLost: {_player.GamesLost}",
            "OK");
    }
}
