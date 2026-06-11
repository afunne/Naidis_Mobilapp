namespace Naidis_Mobilapp.Models;

public class MinesweeperCell
{
    public int Row { get; }
    public int Column { get; }
    public bool IsMine { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsFlagged { get; private set; }
    public int NearbyMines { get; private set; }

    public MinesweeperCell(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public void PlaceMine()
    {
        IsMine = true;
    }

    public void SetNearbyMines(int nearbyMines)
    {
        NearbyMines = nearbyMines;
    }

    public bool Reveal()
    {
        if (IsOpen || IsFlagged)
        {
            return false;
        }

        IsOpen = true;
        return true;
    }

    public void ToggleFlag()
    {
        if (!IsOpen)
        {
            IsFlagged = !IsFlagged;
        }
    }
}
