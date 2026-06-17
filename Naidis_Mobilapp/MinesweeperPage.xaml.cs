using Naidis_Mobilapp.Models;

namespace Naidis_Mobilapp;

public partial class MinesweeperPage : ContentPage
{
    private readonly List<Theme> _themes;
    private readonly List<BoardSize> _boardSizes;
    private readonly IDispatcherTimer _timer;
    private Button[,] _cellButtons = new Button[0, 0];
    private Player _player;
    private Game _game;
    private BoardSize _selectedBoardSize;
    private (int Row, int Column)? _selectedCell;
    private bool _isInitializing = true;

    public MinesweeperPage()
    {
        InitializeComponent();

        _themes = CreateThemes();
        _boardSizes = CreateBoardSizes();
        _selectedBoardSize = _boardSizes[1];
        _player = new Player(PlayerNameEntry.Text);
        _game = CreateGame(_selectedBoardSize);
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateStats();

        ThemePicker.ItemsSource = _themes;
        ThemePicker.SelectedIndex = 0;
        SizePicker.ItemsSource = _boardSizes;
        SizePicker.SelectedItem = _selectedBoardSize;

        _isInitializing = false;
        CreateBoard();
        StartGame();
    }

    private static List<BoardSize> CreateBoardSizes()
    {
        return new List<BoardSize>
        {
            new("Väike 6 x 6", 6, 6, 6),
            new("Keskmine 8 x 8", 8, 8, 10),
            new("Suur 10 x 10", 10, 10, 16),
            new("Ekspert 12 x 12", 12, 12, 24)
        };
    }

    private static List<Theme> CreateThemes()
    {
        return new List<Theme>
        {
            new(
                "Aed",
                Color.FromArgb("#F7F1E5"),
                Color.FromArgb("#FFFDF8"),
                Color.FromArgb("#263238"),
                Color.FromArgb("#2E7D7A"),
                Color.FromArgb("#6E9C89"),
                Color.FromArgb("#79AD62"),
                Color.FromArgb("#EFE7D3"),
                Color.FromArgb("#E3D0AE"),
                Color.FromArgb("#C44536"),
                "OpenSansRegular"),
            new(
                "Kesköö",
                Color.FromArgb("#171923"),
                Color.FromArgb("#242938"),
                Color.FromArgb("#F8FAFC"),
                Color.FromArgb("#6EA8FE"),
                Color.FromArgb("#394150"),
                Color.FromArgb("#323947"),
                Color.FromArgb("#2D3342"),
                Color.FromArgb("#252B38"),
                Color.FromArgb("#F97373"),
                "OpenSansSemibold"),
            new(
                "Kommid",
                Color.FromArgb("#FFF0F5"),
                Color.FromArgb("#FFFFFF"),
                Color.FromArgb("#3B2645"),
                Color.FromArgb("#D9468F"),
                Color.FromArgb("#83C5BE"),
                Color.FromArgb("#76B7B0"),
                Color.FromArgb("#FFE8A3"),
                Color.FromArgb("#F5D986"),
                Color.FromArgb("#EF476F"),
                "BobloxFont")
        };
    }

    private void CreateBoard()
    {
        var cellSize = GetCellSize();

        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();
        BoardGrid.Children.Clear();

        _cellButtons = new Button[_game.Rows, _game.Columns];
        BoardGrid.WidthRequest = _game.Columns * cellSize + (_game.Columns - 1) * BoardGrid.ColumnSpacing;

        for (var row = 0; row < _game.Rows; row++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = cellSize });
        }

        for (var column = 0; column < _game.Columns; column++)
        {
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = cellSize });
        }

        for (var row = 0; row < _game.Rows; row++)
        {
            for (var column = 0; column < _game.Columns; column++)
            {
                var button = new Button
                {
                    Padding = 0,
                    CornerRadius = 0,
                    BorderWidth = 0,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = cellSize > 32 ? 16 : 14,
                    HeightRequest = cellSize,
                    WidthRequest = cellSize,
                    MinimumHeightRequest = cellSize,
                    MinimumWidthRequest = cellSize,
                    BindingContext = (Row: row, Column: column)
                };

                ApplyCellBackground(button, row, column, false);
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
        _timer.Stop();
        PauseButton.Text = "Paus";
        PauseButton.IsEnabled = false;
        HideCellActionPopup();
        SelectedCellLabel.Text = string.Empty;
        BoardGrid.Rotation = 0;
        BoardGrid.TranslationX = 0;

        foreach (var button in _cellButtons)
        {
            button.IsEnabled = true;
            button.Text = string.Empty;
            button.ImageSource = null;
            button.Opacity = 1;
            button.Scale = 1;
            var position = ((int Row, int Column))button.BindingContext;
            ApplyCellBackground(button, position.Row, position.Column, false);
            button.SetDynamicResource(Button.TextColorProperty, "MinesTextColor");
        }

        UpdateStats();
    }

    private Game CreateGame(BoardSize boardSize)
    {
        return new Game(boardSize.Rows, boardSize.Columns, boardSize.Mines, _player);
    }

    private double GetCellSize()
    {
        return _game.Columns switch
        {
            <= 6 => 42,
            <= 8 => 35,
            <= 10 => 31,
            _ => 27
        };
    }

    private async void OnCellClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not ValueTuple<int, int> position)
        {
            return;
        }

        await button.ScaleToAsync(0.88, 45, Easing.CubicOut);
        await button.ScaleToAsync(1, 70, Easing.CubicIn);

        var cell = _game.Board[position.Item1, position.Item2];

        if (cell.IsOpen)
        {
            return;
        }

        _selectedCell = (position.Item1, position.Item2);
        PopupOpenButton.IsEnabled = !cell.IsFlagged;
        SelectedCellLabel.Text = $"Ruut: {position.Item1 + 1}, {position.Item2 + 1}";
        await ShowCellActionPopup();
    }

    private async Task OpenSelectedCell()
    {
        if (_selectedCell is not { } position)
        {
            return;
        }

        await HideCellActionPopupAsync();

        var changedCells = _game.OpenCell(position.Row, position.Column);

        foreach (var cell in changedCells)
        {
            DrawCell(cell);
        }

        if (_game.IsStarted && !_game.IsEnded)
        {
            _timer.Start();
            PauseButton.IsEnabled = true;
        }

        UpdateStats();

        if (_game.IsEnded)
        {
            await FinishGame();
        }
    }

    private async Task ToggleSelectedFlag()
    {
        if (_selectedCell is not { } position)
        {
            return;
        }

        await HideCellActionPopupAsync();

        if (_game.ToggleFlag(position.Row, position.Column))
        {
            DrawCell(_game.Board[position.Row, position.Column]);
            UpdateStats();
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
                "Võit",
                $"{_player.Name} võitis {_game.Score} punktiga ajaga {FormatTime(_game.Elapsed)}.",
                "Hästi");
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
                "Pauk",
                $"{_player.Name} kogus {_game.Score} punkti. Proovi uuesti!",
                "Uuesti");
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
            button.Text = string.Empty;
            button.ImageSource = "mines_flag.svg";
            ApplyCellBackground(button, cell.Row, cell.Column, false);
            button.TextColor = Colors.White;
            return;
        }

        if (!cell.IsOpen)
        {
            button.Text = string.Empty;
            button.ImageSource = null;
            ApplyCellBackground(button, cell.Row, cell.Column, false);
            button.SetDynamicResource(Button.TextColorProperty, "MinesTextColor");
            return;
        }

        button.IsEnabled = false;
        button.ImageSource = null;
        if (cell.IsMine)
        {
            button.SetDynamicResource(Button.BackgroundColorProperty, "MinesDangerColor");
        }
        else
        {
            ApplyCellBackground(button, cell.Row, cell.Column, true);
        }
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

    private static void ApplyCellBackground(Button button, int row, int column, bool isOpen)
    {
        var isAlternate = (row + column) % 2 == 1;
        var resourceName = isOpen
            ? isAlternate ? "MinesOpenCellAltColor" : "MinesOpenCellColor"
            : isAlternate ? "MinesHiddenCellAltColor" : "MinesHiddenCellColor";

        button.SetDynamicResource(Button.BackgroundColorProperty, resourceName);
    }

    private void UpdateStats()
    {
        ScoreLabel.Text = $"Punktid: {_game.Score}";
        TimeLabel.Text = $"Aeg: {FormatTime(_game.Elapsed)}";
        FlagsLabel.Text = $"Lipud: {_game.RemainingFlags}";
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
        if (!_game.IsStarted)
        {
            return;
        }

        if (_game.IsPaused)
        {
            _game.Resume();
            _timer.Start();
            PauseButton.Text = "Paus";
            SetBoardEnabled(true);
        }
        else
        {
            _game.Pause();
            _timer.Stop();
            PauseButton.Text = "Jätka";
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

        if (!isEnabled)
        {
            HideCellActionPopup();
        }
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if (ThemePicker.SelectedItem is Theme selectedTheme)
        {
            selectedTheme.Apply(this);
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (_isInitializing || SizePicker.SelectedItem is not BoardSize boardSize)
        {
            return;
        }

        _selectedBoardSize = boardSize;
        _game = CreateGame(_selectedBoardSize);
        CreateBoard();
        StartGame();
    }

    private void OnPlayerNameChanged(object sender, TextChangedEventArgs e)
    {
        _player.Name = string.IsNullOrWhiteSpace(e.NewTextValue) ? "Mängija" : e.NewTextValue.Trim();
    }

    private async Task ShowCellActionPopup()
    {
        CellActionPopup.IsVisible = true;
        CellActionPopup.AbortAnimation("CellActionPopup");
        await Task.WhenAll(
            CellActionPopup.FadeToAsync(1, 90, Easing.CubicOut),
            CellActionPopup.ScaleToAsync(1, 90, Easing.CubicOut));
    }

    private void HideCellActionPopup()
    {
        _selectedCell = null;
        CellActionPopup.IsVisible = false;
        CellActionPopup.Opacity = 0;
        CellActionPopup.Scale = 0.85;
    }

    private async Task HideCellActionPopupAsync()
    {
        if (!CellActionPopup.IsVisible)
        {
            _selectedCell = null;
            return;
        }

        await Task.WhenAll(
            CellActionPopup.FadeToAsync(0, 70, Easing.CubicIn),
            CellActionPopup.ScaleToAsync(0.85, 70, Easing.CubicIn));
        HideCellActionPopup();
        SelectedCellLabel.Text = string.Empty;
    }

    private async void OnPopupOpenClicked(object sender, EventArgs e)
    {
        await OpenSelectedCell();
    }

    private async void OnPopupFlagClicked(object sender, EventArgs e)
    {
        await ToggleSelectedFlag();
    }

    private async void OnPopupCloseClicked(object sender, EventArgs e)
    {
        await HideCellActionPopupAsync();
    }

    private async void OnTopScoreClicked(object sender, EventArgs e)
    {
        var bestTime = _player.BestTime is null ? "--:--" : FormatTime(_player.BestTime.Value);
        await DisplayAlertAsync(
            "Parim tulemus",
            $"Parim punktisumma: {_player.BestScore}\nParim aeg: {bestTime}\nVõidud: {_player.GamesWon}\nKaotused: {_player.GamesLost}",
            "OK");
    }

    private sealed class BoardSize
    {
        public string Name { get; }
        public int Rows { get; }
        public int Columns { get; }
        public int Mines { get; }

        public BoardSize(string name, int rows, int columns, int mines)
        {
            Name = name;
            Rows = rows;
            Columns = columns;
            Mines = mines;
        }

        public override string ToString()
        {
            return $"{Name} - {Mines} miini";
        }
    }
}