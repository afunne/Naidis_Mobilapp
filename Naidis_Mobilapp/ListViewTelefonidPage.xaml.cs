using System.Collections.ObjectModel;

namespace Naidis_Mobilapp;

public partial class ListViewTelefonidPage : ContentPage
{
    public ObservableCollection<Telefon> Telefons { get; } = new();

    string valitudPildiTee = "";

    public ListViewTelefonidPage()
    {
        InitializeComponent();

        Telefons.Add(new Telefon { Nimetus = "Samsung Galaxy S22 Ultra", Tootja = "Samsung", Hind = 1349, Pilt = "galaxy_phone.svg" });
        Telefons.Add(new Telefon { Nimetus = "Xiaomi Mi 11 Lite 5G NE", Tootja = "Xiaomi", Hind = 399, Pilt = "xiaomi_phone.svg" });
        Telefons.Add(new Telefon { Nimetus = "iPhone 13 mini", Tootja = "Apple", Hind = 1179, Pilt = "iphone_phone.svg" });

        PhonesList.ItemsSource = Telefons;
    }

    async void OnChooseImageClicked(object sender, EventArgs e)
    {
        try
        {
            IEnumerable<FileResult> photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Vali telefoni pilt"
            });

            FileResult? photo = photos.FirstOrDefault();

            if (photo == null)
            {
                return;
            }

            valitudPildiTee = photo.FullPath;
            SelectedImageLabel.Text = $"Valitud: {photo.FileName}";
            SelectedImageLabel.TextColor = Colors.Green;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", "Pildi valimine ebaõnnestus: " + ex.Message, "OK");
        }
    }

    async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNimetus.Text) || string.IsNullOrWhiteSpace(EntryTootja.Text))
        {
            await DisplayAlertAsync("Viga", "Palun täida vähemalt mudeli ja tootja väljad.", "OK");
            return;
        }

        int.TryParse(EntryHind.Text, out int hind);

        string pildiNimi = string.IsNullOrWhiteSpace(valitudPildiTee)
            ? "default_phone.svg"
            : valitudPildiTee;

        Telefons.Add(new Telefon
        {
            Nimetus = EntryNimetus.Text.Trim(),
            Tootja = EntryTootja.Text.Trim(),
            Hind = hind,
            Pilt = pildiNimi
        });

        ClearForm();
    }

    async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (PhonesList.SelectedItem is not Telefon telefon)
        {
            await DisplayAlertAsync("Viga", "Palun vali nimekirjast telefon, mida soovid kustutada.", "OK");
            return;
        }

        bool answer = await DisplayAlertAsync(
            "Kinnitus",
            $"Kas oled kindel, et soovid mudeli {telefon.Nimetus} kustutada?",
            "Jah",
            "Ei");

        if (!answer)
        {
            return;
        }

        Telefons.Remove(telefon);
        PhonesList.SelectedItem = null;
    }

    async void OnPhoneTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not Telefon telefon)
        {
            return;
        }

        await DisplayAlertAsync(
            "Telefoni info",
            $"Tootja: {telefon.Tootja}\nMudel: {telefon.Nimetus}\nHind: {telefon.Hind} EUR",
            "Sulge");
    }

    void ClearForm()
    {
        EntryNimetus.Text = "";
        EntryTootja.Text = "";
        EntryHind.Text = "";
        valitudPildiTee = "";
        SelectedImageLabel.Text = "Pilti pole valitud (kasutatakse vaikimisi pilti)";
        SelectedImageLabel.TextColor = Colors.Gray;
    }
}

public class Telefon
{
    public string Nimetus { get; set; } = "";
    public string Tootja { get; set; } = "";
    public int Hind { get; set; }
    public string Pilt { get; set; } = "default_phone.svg";
}
