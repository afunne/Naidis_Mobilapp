namespace Naidis_Mobilapp;

public partial class DateTimePage : ContentPage
{
    public DateTimePage()
    {
        InitializeComponent();
        // Näita tänast kuupäeva kohe käivitamisel
        UpdateDisplay(datePicker.Date ?? DateTime.Today, timePicker.Time ?? TimeSpan.Zero);
    }

    // DateSelected - käivitub kui kasutaja valib uue kuupäeva
    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        UpdateDisplay(e.NewDate ?? DateTime.Today, timePicker.Time ?? TimeSpan.Zero);
    }

    // PropertyChanged - käivitub kui TimePicker muutub
    private void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
        {
            UpdateDisplay(datePicker.Date ?? DateTime.Today, timePicker.Time ?? TimeSpan.Zero);
        }
    }

    private void UpdateDisplay(DateTime date, TimeSpan time)
    {
        // Kuva valitud kuupäev ja kellaaeg lblResult labelis
        lblResult.Text = $"Valitud: {date:dd.MM.yyyy} kell {time:hh\\:mm\\:ss}";

        // Kuva eraldi labelites
        lblSelectedDate.Text = $"Kuupäev: {date:dd.MM.yyyy}";
        lblSelectedTime.Text = $"Kellaaeg: {time:hh\\:mm\\:ss}";

        // Nädalapäev eesti keeles
        string[] nadalaPaevad = { "Pühapäev", "Esmaspäev", "Teisipäev", "Kolmapäev", "Neljapäev", "Reede", "Laupäev" };
        lblWeekday.Text = $"Nädalapäev: {nadalaPaevad[(int)date.DayOfWeek]}";
    }

    private async void tagasi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void avaleht_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
