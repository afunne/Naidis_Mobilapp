namespace Naidis_Mobilapp;

public partial class MinecraftPopupQuizPage : ContentPage
{
    readonly Random random = new();
    string playerName = "Steve";
    int correctAnswers = 0;
    int answeredQuestions = 0;

    static readonly List<(string Name, int Score, int Total)> leaderboard = new()
    {
        ("Alex", 5, 5),
        ("Notch", 8, 10),
        ("Herobrine", 2, 8)
    };

    readonly Dictionary<string, List<QuizQuestion>> questionsByTopic = new()
    {
        ["Kollid"] = new List<QuizQuestion>
        {
            new("Milline koll plahvatab, kui ta sulle lähedale jõuab?", "Creeper", "Zombi", "Luukere", "Enderman"),
            new("Milline koll kukutab niiti?", "Ämblik", "Lehm", "Külaelanik", "Kana"),
            new("Milline koll kaupleb asjadega smaragdide eest?", "Külaelanik", "Creeper", "Hunt", "Lima")
        },
        ["Meisterdamine"] = new List<QuizQuestion>
        {
            new("Mitu lauda on vaja meisterdamislaua tegemiseks?", "4", "2", "6", "8"),
            new("Mida sa sulatad, et klaasi saada?", "Liiv", "Kruus", "Savi", "Muld"),
            new("Millist eset on vaja tõrvikute tegemiseks?", "Süsi", "Redstone", "Raud", "Kondijahu")
        },
        ["Bioomid"] = new List<QuizQuestion>
        {
            new("Millises bioomis on palju kaktusi?", "Kõrb", "Taiga", "Džungel", "Soo"),
            new("Kus pandad loomulikult tekivad?", "Džungel", "Tühermaa", "Ookean", "Lumetundrad"),
            new("Milline bioom on täis seeneniidistikku?", "Seeneväljad", "Savann", "Mets", "Rand")
        }
    };

    public MinecraftPopupQuizPage()
    {
        InitializeComponent();
        UpdateScore();
    }

    async void OnPlayerNameClicked(object sender, EventArgs e)
    {
        string? name = await DisplayPromptAsync(
            "Tere!",
            "Mis on sinu Minecrafti mängijanimi?",
            "Salvesta",
            "Loobu",
            "Näiteks: Alex",
            maxLength: 16);

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        playerName = name.Trim();
        UpdateScore();
        await DisplayAlertAsync("Valmis!", $"Tere tulemast, {playerName}! Sinu viktoriinimaailm on laetud.", "OK");
    }

    async void OnRiddleClicked(object sender, EventArgs e)
    {
        bool wantsToAnswer = await DisplayAlertAsync(
            "Minecrafti mõistatus",
            "Ma olen roheline, vaikne ja hävitan maju ühe valju pauguga. Kas soovid vastata?",
            "Vasta",
            "Jäta vahele");

        if (!wantsToAnswer)
        {
            await DisplayAlertAsync("Vahele jäetud", "Pole lugu. Mõistatus ootab endiselt.", "OK");
            return;
        }

        string? answer = await DisplayPromptAsync(
            "Sinu vastus",
            "Milline Minecrafti koll see on?",
            "Kontrolli",
            "Loobu",
            "Sisesta kolli nimi");

        await CheckAnswer(answer, "Creeper", "Õige! See oli Creeper.", "Päris lähedale! Vastus oli Creeper.");
    }

    async void OnStartQuizClicked(object sender, EventArgs e)
    {
        string? topic = await DisplayActionSheetAsync(
            "Vali Minecrafti viktoriini teema",
            "Loobu",
            null,
            questionsByTopic.Keys.ToArray());

        if (string.IsNullOrWhiteSpace(topic) || topic == "Loobu")
        {
            return;
        }

        QuizQuestion question = GetRandomQuestion(topic);
        string? selection = await DisplayActionSheetAsync(
            $"{topic}:\n{question.Question}",
            "Loobu",
            null,
            question.ShuffledAnswers(random));

        if (string.IsNullOrWhiteSpace(selection) || selection == "Loobu")
        {
            return;
        }

        await CheckAnswer(
            selection,
            question.CorrectAnswer,
            $"Tubli, {playerName}! {question.CorrectAnswer} on õige.",
            $"Mitte seekord. Õige vastus: {question.CorrectAnswer}.");

        bool keepPlaying = await DisplayAlertAsync(
            "Järgmine ring?",
            "Kas soovid veel üht Minecrafti küsimust?",
            "Jah",
            "Ei");

        if (keepPlaying)
        {
            OnStartQuizClicked(sender, e);
        }
        else
        {
            await ShowResultsAndSave();
        }
    }

    async void OnStopAndShowResultsClicked(object sender, EventArgs e)
    {
        await ShowResultsAndSave();
    }

    async Task ShowResultsAndSave()
    {
        if (answeredQuestions > 0)
        {
            leaderboard.Add((playerName, correctAnswers, answeredQuestions));
            
            await DisplayAlertAsync(
                "Tulemused",
                $"Mängija: {playerName}\nSinu skoor: {correctAnswers} / {answeredQuestions}\n\nTulemus salvestati edetabelisse!",
                "OK");

            correctAnswers = 0;
            answeredQuestions = 0;
            UpdateScore();
        }
        else
        {
            await DisplayAlertAsync("Pole tulemusi", "Sa pole veel ühelegi küsimusele vastanud.", "OK");
        }
    }

    async void OnLeaderboardClicked(object sender, EventArgs e)
    {
        if (leaderboard.Count == 0)
        {
            await DisplayAlertAsync("Edetabel", "Edetabel on tühi.", "OK");
            return;
        }

        var sortedBoard = leaderboard.OrderByDescending(l => l.Score).ThenBy(l => l.Total).ToList();
        string boardText = string.Join("\n", sortedBoard.Select((l, i) => $"{i + 1}. {l.Name} - {l.Score}/{l.Total}"));

        await DisplayAlertAsync("Edetabel (Parimad mängijad)", boardText, "Sulge");
    }

    async void OnThemeClicked(object sender, EventArgs e)
    {
        string? theme = await DisplayActionSheetAsync(
            "Vali lehe teema",
            "Loobu",
            null,
            "Murublokk",
            "Nether",
            "Teemant");

        switch (theme)
        {
            case "Murublokk":
                BackgroundColor = Color.FromArgb("#F4E6C8");
                await DisplayAlertAsync("Teema muudetud", "Murubloki teema valitud.", "OK");
                break;
            case "Nether":
                BackgroundColor = Color.FromArgb("#3B1010");
                await DisplayAlertAsync("Teema muudetud", "Netheri teema valitud.", "OK");
                break;
            case "Teemant":
                BackgroundColor = Color.FromArgb("#C7F7F4");
                await DisplayAlertAsync("Teema muudetud", "Teemanti teema valitud.", "OK");
                break;
        }
    }

    QuizQuestion GetRandomQuestion(string topic)
    {
        List<QuizQuestion> questions = questionsByTopic[topic];
        return questions[random.Next(questions.Count)];
    }

    async Task CheckAnswer(string? answer, string correctAnswer, string successMessage, string failMessage)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            return;
        }

        answeredQuestions++;
        bool isCorrect = answer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            correctAnswers++;
        }

        UpdateScore();
        await DisplayAlertAsync(isCorrect ? "Õige!" : "Oih!", isCorrect ? successMessage : failMessage, "OK");
    }

    void UpdateScore()
    {
        PlayerLabel.Text = $"Mängija: {playerName}";
        ScoreLabel.Text = $"Skoor: {correctAnswers} / {answeredQuestions}";
        ScoreProgress.Progress = answeredQuestions == 0 ? 0 : (double)correctAnswers / answeredQuestions;
    }

    record QuizQuestion(string Question, string CorrectAnswer, string WrongAnswer1, string WrongAnswer2, string WrongAnswer3)
    {
        public string[] ShuffledAnswers(Random random)
        {
            return new[] { CorrectAnswer, WrongAnswer1, WrongAnswer2, WrongAnswer3 }
                .OrderBy(_ => random.Next())
                .ToArray();
        }
    }
}