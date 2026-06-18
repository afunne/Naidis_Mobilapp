using Naidis_Mobilapp.ViewModels;

namespace Naidis_Mobilapp.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
