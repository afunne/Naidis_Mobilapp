namespace Naidis_Mobilapp;

public partial class SopradeKontaktidPage : ContentPage
{
    readonly List<FriendContact> friends = new();
    readonly Random random = new();
    int currentFriendIndex = -1;
    string? selectedPhotoPath;

    readonly string[] greetings =
    {
        "Häid pühi! Soovin sulle palju rõõmu ja rahu!",
        "Palju õnne! Olgu su päev täis häid üllatusi!",
        "Ilusat uut aastat! Kõike head ja põnevaid seiklusi!",
        "Rõõmsaid jõule! Soovin soojust ja häid hetki perega!",
        "Head sõbrapäeva! Aitäh, et oled nii tore sõber!"
    };

    public SopradeKontaktidPage()
    {
        InitializeComponent();
        MessageEditor.Text = "Tere! Saadan sulle tervitused kontaktiraamatust.";
    }

    async void OnSaveFriendClicked(object sender, EventArgs e)
    {
        if (!await ValidateRequiredFields(requirePhoto: true))
        {
            return;
        }

        FriendContact friend = ReadFriendFromTable();

        if (currentFriendIndex >= 0 && currentFriendIndex < friends.Count)
        {
            friends[currentFriendIndex] = friend;
        }
        else
        {
            friends.Add(friend);
            currentFriendIndex = friends.Count - 1;
        }

        UpdateSavedFriendsInfo();
        await DisplayAlertAsync("Salvestatud", $"{friend.Name} lisati kontaktiraamatusse.", "OK");
    }

    async void OnNextFriendClicked(object sender, EventArgs e)
    {
        if (friends.Count == 0)
        {
            await DisplayAlertAsync("Kontaktiraamat", "Sõpru pole veel salvestatud.", "OK");
            return;
        }

        currentFriendIndex = (currentFriendIndex + 1) % friends.Count;
        LoadFriendToTable(friends[currentFriendIndex]);
    }

    async void OnCallClicked(object sender, EventArgs e)
    {
        string phone = PhoneCell.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(phone))
        {
            await DisplayAlertAsync("Puudub telefon", "Sisesta tabelisse sõbra telefoninumber.", "OK");
            return;
        }

        if (!PhoneDialer.Default.IsSupported)
        {
            await DisplayAlertAsync("Viga", "Helistamine pole selles seadmes toetatud.", "OK");
            return;
        }

        try
        {
            PhoneDialer.Default.Open(phone);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Viga", ex.Message, "OK");
        }
    }

    async void OnSmsClicked(object sender, EventArgs e)
    {
        string phone = PhoneCell.Text?.Trim() ?? "";
        string message = MessageEditor.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(message))
        {
            await DisplayAlertAsync("Puuduvad andmed", "Telefoninumber ja sõnum peavad olema tabelis täidetud.", "OK");
            return;
        }

        if (!Sms.Default.IsComposeSupported)
        {
            await DisplayAlertAsync("Viga", "SMS-i saatmine pole selles seadmes toetatud.", "OK");
            return;
        }

        SmsMessage sms = new(message, new[] { phone });
        await Sms.Default.ComposeAsync(sms);
    }

    async void OnEmailClicked(object sender, EventArgs e)
    {
        await SendEmailAsync(MessageEditor.Text?.Trim());
    }

    async void OnGreetingClicked(object sender, EventArgs e)
    {
        if (!await ValidateRequiredFields(requirePhoto: false))
        {
            return;
        }

        string greeting = greetings[random.Next(greetings.Length)];
        MessageEditor.Text = greeting;

        string choice = await DisplayActionSheetAsync(
            "Kuidas soovid õnnitluse saata?",
            "Tühista",
            null,
            "SMS",
            "Email");

        switch (choice)
        {
            case "SMS":
                OnSmsClicked(sender, e);
                break;
            case "Email":
                await SendEmailAsync(greeting);
                break;
        }
    }

    async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await DisplayAlertAsync("Kaamera", "Kaamera pole selles seadmes toetatud.", "OK");
            return;
        }

        try
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = $"sober_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
            });

            await SaveAndShowPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Kaamera viga", ex.Message, "OK");
        }
    }

    async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            IEnumerable<FileResult> photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Vali sõbra foto"
            });

            FileResult? photo = photos.FirstOrDefault();
            await SaveAndShowPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Foto viga", ex.Message, "OK");
        }
    }

    async Task SendEmailAsync(string? body)
    {
        string email = EmailCell.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(body))
        {
            await DisplayAlertAsync("Puuduvad andmed", "Email ja sõnum peavad olema tabelis täidetud.", "OK");
            return;
        }

        if (!Email.Default.IsComposeSupported)
        {
            await DisplayAlertAsync("Viga", "E-kirja saatmine pole selles seadmes toetatud.", "OK");
            return;
        }

        EmailMessage emailMessage = new()
        {
            Subject = $"Tervitus sõbrale {NameCell.Text}",
            Body = body,
            BodyFormat = EmailBodyFormat.PlainText,
            To = new List<string> { email }
        };

        if (!string.IsNullOrWhiteSpace(selectedPhotoPath) && File.Exists(selectedPhotoPath))
        {
            emailMessage.Attachments = new List<EmailAttachment>
            {
                new(selectedPhotoPath)
            };
        }

        await Email.Default.ComposeAsync(emailMessage);
    }

    async Task SaveAndShowPhotoAsync(FileResult? photo)
    {
        if (photo == null)
        {
            return;
        }

        string localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

        await using Stream sourceStream = await photo.OpenReadAsync();
        await using FileStream localFileStream = File.OpenWrite(localPath);
        await sourceStream.CopyToAsync(localFileStream);

        selectedPhotoPath = localPath;
        FriendImage.Source = ImageSource.FromFile(localPath);
    }

    async Task<bool> ValidateRequiredFields(bool requirePhoto)
    {
        List<string> missingFields = new();

        if (string.IsNullOrWhiteSpace(NameCell.Text))
        {
            missingFields.Add("nimi");
        }

        if (string.IsNullOrWhiteSpace(EmailCell.Text))
        {
            missingFields.Add("email");
        }

        if (string.IsNullOrWhiteSpace(PhoneCell.Text))
        {
            missingFields.Add("telefon");
        }

        if (string.IsNullOrWhiteSpace(DescriptionEditor.Text))
        {
            missingFields.Add("kirjeldus");
        }

        if (requirePhoto && string.IsNullOrWhiteSpace(selectedPhotoPath))
        {
            missingFields.Add("foto");
        }

        if (missingFields.Count == 0)
        {
            return true;
        }

        await DisplayAlertAsync("Täida kohustuslikud väljad", $"Puudu: {string.Join(", ", missingFields)}.", "OK");
        return false;
    }

    FriendContact ReadFriendFromTable()
    {
        return new FriendContact(
            NameCell.Text?.Trim() ?? "",
            EmailCell.Text?.Trim() ?? "",
            PhoneCell.Text?.Trim() ?? "",
            DescriptionEditor.Text?.Trim() ?? "",
            selectedPhotoPath);
    }

    void LoadFriendToTable(FriendContact friend)
    {
        NameCell.Text = friend.Name;
        EmailCell.Text = friend.Email;
        PhoneCell.Text = friend.Phone;
        DescriptionEditor.Text = friend.Description;
        selectedPhotoPath = friend.PhotoPath;
        FriendImage.Source = string.IsNullOrWhiteSpace(friend.PhotoPath)
            ? "dotnet_bot.png"
            : ImageSource.FromFile(friend.PhotoPath);
    }

    void UpdateSavedFriendsInfo()
    {
        SavedFriendsCell.Text = $"Salvestatud sõpru: {friends.Count}";
        SavedFriendsCell.Detail = friends.Count == 0
            ? "Täida vorm ja vajuta SALVESTA."
            : $"Praegune sõber: {friends[currentFriendIndex].Name}";
    }

    record FriendContact(string Name, string Email, string Phone, string Description, string? PhotoPath);
}
