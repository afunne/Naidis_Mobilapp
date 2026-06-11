namespace Naidis_Mobilapp.Models;

public class Game
{
    private readonly Random _random = new();
    private DateTime _startedAt;
    private TimeSpan _elapsedBeforePause;

    public int Rows { get; }
    public int Columns { get; }
    public int MineCount { get; }
    public MinesweeperCell[,] Board { get; private set; }
    public Player Player { get; }
    public int Score { get; private set; }
    public int OpenedCells { get; private set; }
    public bool IsStarted { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsEnded { get; private set; }
    public bool IsWon => IsEnded && OpenedCells == Rows * Columns - MineCount;

    public TimeSpan Elapsed
    {
        get
        {
            if (!IsStarted)
            {
                return TimeSpan.Zero;
            }

            return IsPaused || IsEnded
                ? _elapsedBeforePause
                : _elapsedBeforePause + (DateTime.UtcNow - _startedAt);
        }
    }

    public int RemainingFlags => MineCount - Board.Cast<MinesweeperCell>().Count(cell => cell.IsFlagged);

    public Game(int rows, int columns, int mineCount, Player player)
    {
        if (rows < 4 || columns < 4)
        {
            throw new ArgumentException("The board must be at least 4x4.");
        }

        if (mineCount <= 0 || mineCount >= rows * columns)
        {
            throw new ArgumentException("Mine count must fit inside the board.");
        }

        Rows = rows;
        Columns = columns;
        MineCount = mineCount;
        Player = player;
        Board = CreateEmptyBoard();
    }

    public void Start()
    {
        Board = CreateEmptyBoard();
        PlaceMines();
        CalculateNearbyMines();

        Score = 0;
        OpenedCells = 0;
        IsStarted = true;
        IsPaused = false;
        IsEnded = false;
        _elapsedBeforePause = TimeSpan.Zero;
        _startedAt = DateTime.UtcNow;
        Player.ResetRound();
    }

    public void Pause()
    {
        if (!IsStarted || IsEnded || IsPaused)
        {
            return;
        }

        _elapsedBeforePause = Elapsed;
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsStarted || IsEnded || !IsPaused)
        {
            return;
        }

        _startedAt = DateTime.UtcNow;
        IsPaused = false;
    }

    public IReadOnlyList<MinesweeperCell> OpenCell(int row, int column)
    {
        if (!CanPlay(row, column))
        {
            return Array.Empty<MinesweeperCell>();
        }

        var changedCells = new List<MinesweeperCell>();
        var cell = Board[row, column];

        if (cell.IsMine)
        {
            RevealCell(cell, changedCells);
            End(false);
            return changedCells;
        }

        RevealSafeArea(cell, changedCells);
        Score = OpenedCells * 10;
        Player.AddPoints(changedCells.Count(opened => !opened.IsMine) * 10);

        if (OpenedCells == Rows * Columns - MineCount)
        {
            End(true);
        }

        return changedCells;
    }

    public bool ToggleFlag(int row, int column)
    {
        if (!CanPlay(row, column))
        {
            return false;
        }

        var cell = Board[row, column];

        if (!cell.IsFlagged && RemainingFlags <= 0)
        {
            return false;
        }

        cell.ToggleFlag();
        return true;
    }

    public IReadOnlyList<MinesweeperCell> RevealAllMines()
    {
        var changedCells = new List<MinesweeperCell>();

        foreach (var cell in Board)
        {
            if (cell.IsMine)
            {
                RevealCell(cell, changedCells);
            }
        }

        return changedCells;
    }

    private MinesweeperCell[,] CreateEmptyBoard()
    {
        var board = new MinesweeperCell[Rows, Columns];

        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                board[row, column] = new MinesweeperCell(row, column);
            }
        }

        return board;
    }

    private void PlaceMines()
    {
        var placed = 0;

        while (placed < MineCount)
        {
            var row = _random.Next(Rows);
            var column = _random.Next(Columns);
            var cell = Board[row, column];

            if (cell.IsMine)
            {
                continue;
            }

            cell.PlaceMine();
            placed++;
        }
    }

    private void CalculateNearbyMines()
    {
        foreach (var cell in Board)
        {
            if (cell.IsMine)
            {
                continue;
            }

            cell.SetNearbyMines(GetNeighbors(cell).Count(neighbor => neighbor.IsMine));
        }
    }

    private void RevealSafeArea(MinesweeperCell startCell, List<MinesweeperCell> changedCells)
    {
        var queue = new Queue<MinesweeperCell>();
        queue.Enqueue(startCell);

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();

            if (!RevealCell(cell, changedCells) || cell.NearbyMines > 0)
            {
                continue;
            }

            foreach (var neighbor in GetNeighbors(cell))
            {
                if (!neighbor.IsOpen && !neighbor.IsMine && !neighbor.IsFlagged)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private bool RevealCell(MinesweeperCell cell, List<MinesweeperCell> changedCells)
    {
        if (!cell.Reveal())
        {
            return false;
        }

        changedCells.Add(cell);

        if (!cell.IsMine)
        {
            OpenedCells++;
        }

        return true;
    }

    private IEnumerable<MinesweeperCell> GetNeighbors(MinesweeperCell cell)
    {
        for (var rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (var columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                if (rowOffset == 0 && columnOffset == 0)
                {
                    continue;
                }

                var row = cell.Row + rowOffset;
                var column = cell.Column + columnOffset;

                if (row >= 0 && row < Rows && column >= 0 && column < Columns)
                {
                    yield return Board[row, column];
                }
            }
        }
    }

    private bool CanPlay(int row, int column)
    {
        return IsStarted
            && !IsPaused
            && !IsEnded
            && row >= 0
            && row < Rows
            && column >= 0
            && column < Columns;
    }

    private void End(bool won)
    {
        IsEnded = true;
        _elapsedBeforePause = Elapsed;

        if (won)
        {
            Score += Math.Max(0, 500 - (int)Elapsed.TotalSeconds * 5);
            Player.RecordWin(Score, Elapsed);
        }
        else
        {
            Player.RecordLoss();
        }
    }
}
