using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp;

public partial class NewRecipePage : ContentPage
{
    static readonly List<string> MinuteOptions = Enumerable
        .Range(0, 37)
        .Select(index => $"{index * 5} min")
        .ToList();

    public NewRecipePage()
    {
        InitializeComponent();
        PrepTimePicker.ItemsSource = MinuteOptions;
        CookTimePicker.ItemsSource = MinuteOptions;
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            CategoryPicker.SelectedItem == null ||
            string.IsNullOrWhiteSpace(ImageLinkEntry.Text) ||
            string.IsNullOrWhiteSpace(InstructionsEditor.Text))
        {
            await DisplayAlertAsync("Puuduvad andmed", "Palun täida retsepti nimi, kategooria, pilt ja valmistamisjuhend.", "OK");
            return;
        }

        FileManager.AddRecipe(new Recipe
        {
            Name = NameEntry.Text,
            Category = CategoryPicker.SelectedItem.ToString() ?? "",
            ImageLink = ImageLinkEntry.Text,
            Description = DescriptionEditor.Text,
            Ingredients = IngredientsEditor.Text,
            Instructions = InstructionsEditor.Text,
            PrepTime = PrepTimePicker.SelectedItem?.ToString() ?? "",
            CookTime = CookTimePicker.SelectedItem?.ToString() ?? "",
            Servings = ServingsEntry.Text,
            Rating = 0
        });

        ClearForm();
        await DisplayAlertAsync("Salvestatud", "Retsept salvestati edukalt.", "OK");
    }

    async void OnChoosePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            IEnumerable<FileResult> photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Vali retsepti pilt"
            });
            FileResult? photo = photos.FirstOrDefault();

            if (photo == null)
            {
                return;
            }

            await SavePhotoToAppDataAsync(photo, "Valitud");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", "Pildi valimine ebaõnnestus: " + ex.Message, "OK");
        }
    }

    async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlertAsync("Kaamera puudub", "Selles seadmes ei saa kaameraga pilti teha.", "OK");
                return;
            }

            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Tee retsepti pilt"
            });

            if (photo == null)
            {
                return;
            }

            await SavePhotoToAppDataAsync(photo, "Pildistatud");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", "Pildistamine ebaõnnestus: " + ex.Message, "OK");
        }
    }

    async Task SavePhotoToAppDataAsync(FileResult photo, string statusText)
    {
        string imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "recipe_images");
        Directory.CreateDirectory(imagesFolder);

        string extension = Path.GetExtension(photo.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        string fileName = $"{Guid.NewGuid():N}{extension}";
        string savedPhotoPath = Path.Combine(imagesFolder, fileName);

        await using Stream sourceStream = await photo.OpenReadAsync();
        await using FileStream destinationStream = File.Create(savedPhotoPath);
        await sourceStream.CopyToAsync(destinationStream);

        ImageLinkEntry.Text = savedPhotoPath;
        SelectedPhotoLabel.Text = $"{statusText}: {photo.FileName}";
        SelectedPhotoLabel.TextColor = Colors.Green;
    }

    void ClearForm()
    {
        NameEntry.Text = "";
        CategoryPicker.SelectedIndex = -1;
        ImageLinkEntry.Text = "";
        DescriptionEditor.Text = "";
        IngredientsEditor.Text = "";
        InstructionsEditor.Text = "";
        PrepTimePicker.SelectedIndex = -1;
        CookTimePicker.SelectedIndex = -1;
        ServingsEntry.Text = "";
        SelectedPhotoLabel.Text = "Telefonist pilti valitud ei ole";
        SelectedPhotoLabel.TextColor = Color.FromArgb("#66706A");
    }
}
