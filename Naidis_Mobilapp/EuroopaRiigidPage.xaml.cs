using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Naidis_Mobilapp;

public partial class EuroopaRiigidPage : ContentPage
{
    readonly ObservableCollection<Riik> riigid = new();
    Riik? selectedRiik;

    public EuroopaRiigidPage()
    {
        InitializeComponent();

        riigid.Add(new Riik { Nimi = "Eesti", Pealinn = "Tallinn", Rahvaarv = 1365000, Lipp = "estonia_flag.svg" });
        riigid.Add(new Riik { Nimi = "Soome", Pealinn = "Helsingi", Rahvaarv = 5580000, Lipp = "finland_flag.svg" });
        riigid.Add(new Riik { Nimi = "Prantsusmaa", Pealinn = "Pariis", Rahvaarv = 68000000, Lipp = "france_flag.svg" });
        riigid.Add(new Riik { Nimi = "Saksamaa", Pealinn = "Berliin", Rahvaarv = 84200000, Lipp = "germany_flag.svg" });

        CountriesList.ItemsSource = riigid;
    }

    async void OnChooseFlagClicked(object sender, EventArgs e)
    {
        try
        {
            FileResult? photo = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Vali lipupilt",
                FileTypes = FilePickerFileType.Images
            });

            if (photo == null)
            {
                return;
            }

            string extension = Path.GetExtension(photo.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png";
            }

            string localPath = Path.Combine(
                FileSystem.CacheDirectory,
                $"flag_{Guid.NewGuid():N}{extension}");

            await using Stream sourceStream = await photo.OpenReadAsync();
            await using FileStream localFileStream = File.Create(localPath);
            await sourceStream.CopyToAsync(localFileStream);

            FlagEntry.Text = localPath;
            FlagPreview.Source = ImageSource.FromFile(localPath);
            SelectedFlagLabel.Text = $"Valitud pilt: {photo.FileName}";
            SelectedFlagLabel.TextColor = Colors.Green;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", "Lipupildi valimine ebaõnnestus: " + ex.Message, "OK");
        }
    }

    async void OnCountryTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not Riik riik)
        {
            return;
        }

        selectedRiik = riik;
        FillEntries(riik);

        await DisplayAlertAsync(
            "Riigi info",
            $"Riik: {riik.Nimi}\nPealinn: {riik.Pealinn}\nRahvaarv: {riik.Rahvaarv} inimest",
            "OK");
    }

    async void OnAddClicked(object sender, EventArgs e)
    {
        if (!TryReadForm(out string nimi, out string pealinn, out int rahvaarv, out string lipp))
        {
            return;
        }

        bool riikOnOlemas = riigid.Any(r => r.Nimi.Equals(nimi, StringComparison.OrdinalIgnoreCase));

        if (riikOnOlemas)
        {
            await DisplayAlertAsync("Viga", "See riik on juba nimekirjas!", "OK");
            return;
        }

        riigid.Add(new Riik
        {
            Nimi = nimi,
            Pealinn = pealinn,
            Rahvaarv = rahvaarv,
            Lipp = lipp
        });

        ClearForm();
    }

    async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        if (selectedRiik == null)
        {
            await DisplayAlertAsync("Viga", "Vali nimekirjast riik, mida soovid muuta.", "OK");
            return;
        }

        if (!TryReadForm(out string nimi, out string pealinn, out int rahvaarv, out string lipp))
        {
            return;
        }

        bool duplicateName = riigid.Any(r => r != selectedRiik && r.Nimi.Equals(nimi, StringComparison.OrdinalIgnoreCase));

        if (duplicateName)
        {
            await DisplayAlertAsync("Viga", "Sellise nimega riik on juba nimekirjas!", "OK");
            return;
        }

        selectedRiik.Nimi = nimi;
        selectedRiik.Pealinn = pealinn;
        selectedRiik.Rahvaarv = rahvaarv;
        selectedRiik.Lipp = lipp;

        await DisplayAlertAsync("Salvestatud", "Riigi andmed uuendati.", "OK");
    }

    async void OnDeleteClicked(object sender, EventArgs e)
    {
        Riik? riik = selectedRiik ?? CountriesList.SelectedItem as Riik;

        if (riik == null)
        {
            await DisplayAlertAsync("Viga", "Vali nimekirjast riik, mida soovid kustutada.", "OK");
            return;
        }

        bool answer = await DisplayAlertAsync(
            "Kinnitus",
            $"Kas soovid riigi {riik.Nimi} kustutada?",
            "Jah",
            "Ei");

        if (!answer)
        {
            return;
        }

        riigid.Remove(riik);
        ClearForm();
    }

    void OnClearClicked(object sender, EventArgs e)
    {
        ClearForm();
    }

    bool TryReadForm(out string nimi, out string pealinn, out int rahvaarv, out string lipp)
    {
        nimi = NameEntry.Text?.Trim() ?? "";
        pealinn = CapitalEntry.Text?.Trim() ?? "";
        lipp = FlagEntry.Text?.Trim() ?? "";
        rahvaarv = 0;

        if (string.IsNullOrWhiteSpace(nimi) || string.IsNullOrWhiteSpace(pealinn))
        {
            _ = DisplayAlertAsync("Viga", "Riigi nimi ja pealinn peavad olema täidetud.", "OK");
            return false;
        }

        if (!int.TryParse(PopulationEntry.Text, out rahvaarv) || rahvaarv < 0)
        {
            _ = DisplayAlertAsync("Viga", "Rahvaarv peab olema positiivne täisarv.", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(lipp))
        {
            lipp = "europe_flag.svg";
        }

        return true;
    }

    void FillEntries(Riik riik)
    {
        NameEntry.Text = riik.Nimi;
        CapitalEntry.Text = riik.Pealinn;
        PopulationEntry.Text = riik.Rahvaarv.ToString();
        FlagEntry.Text = riik.Lipp;
        FlagPreview.Source = ImageSource.FromFile(riik.Lipp);
        SelectedFlagLabel.Text = $"Praegune lipp: {GetFlagDisplayName(riik.Lipp)}";
        SelectedFlagLabel.TextColor = Colors.Gray;
    }

    void ClearForm()
    {
        selectedRiik = null;
        CountriesList.SelectedItem = null;
        NameEntry.Text = "";
        CapitalEntry.Text = "";
        PopulationEntry.Text = "";
        FlagEntry.Text = "";
        FlagPreview.Source = "europe_flag.svg";
        SelectedFlagLabel.Text = "Pildi valimiseks kasuta nuppu või kirjuta faili nimi.";
        SelectedFlagLabel.TextColor = Colors.Gray;
    }

    static string GetFlagDisplayName(string flagSource)
    {
        if (Path.IsPathRooted(flagSource))
        {
            return Path.GetFileName(flagSource);
        }

        return flagSource;
    }
}

public class Riik : INotifyPropertyChanged
{
    string nimi = "";
    string pealinn = "";
    int rahvaarv;
    string lipp = "europe_flag.svg";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Nimi
    {
        get => nimi;
        set => SetProperty(ref nimi, value);
    }

    public string Pealinn
    {
        get => pealinn;
        set => SetProperty(ref pealinn, value);
    }

    public int Rahvaarv
    {
        get => rahvaarv;
        set => SetProperty(ref rahvaarv, value);
    }

    public string Lipp
    {
        get => lipp;
        set => SetProperty(ref lipp, string.IsNullOrWhiteSpace(value) ? "europe_flag.svg" : value);
    }

    void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}