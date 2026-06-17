namespace Naidis_Mobilapp.Models;

public class Player
{
    private const string BestScoreKey = "MinesweeperBestScore";
    private const string BestTimeKey = "MinesweeperBestTime";
    private int _points;

    public string Name { get; set; }
    public int GamesPlayed { get; private set; }
    public int GamesWon { get; private set; }
    public int GamesLost => GamesPlayed - GamesWon;
    public int BestScore { get; private set; }
    public TimeSpan? BestTime { get; private set; }

    public int Points
    {
        get => _points;
        private set => _points = Math.Max(0, value);
    }

    public Player(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Mängija" : name.Trim();
        BestScore = Preferences.Default.Get(BestScoreKey, 0);

        var bestTimeSeconds = Preferences.Default.Get(BestTimeKey, 0);
        BestTime = bestTimeSeconds > 0 ? TimeSpan.FromSeconds(bestTimeSeconds) : null;
    }

    public void ResetRound()
    {
        Points = 0;
    }

    public void AddPoints(int points)
    {
        Points += points;
    }

    public void RecordWin(int score, TimeSpan elapsed)
    {
        GamesPlayed++;
        GamesWon++;

        if (score > BestScore)
        {
            BestScore = score;
            Preferences.Default.Set(BestScoreKey, score);
        }

        if (BestTime is null || elapsed < BestTime.Value)
        {
            BestTime = elapsed;
            Preferences.Default.Set(BestTimeKey, (int)elapsed.TotalSeconds);
        }
    }

    public void RecordLoss()
    {
        GamesPlayed++;
    }
}
