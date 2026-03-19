using Microsoft.Maui.Controls;
using System;

namespace Naidis_Mobilapp
{
    public partial class GridDemoPage : ContentPage
    {
        public GridDemoPage()
        {
            InitializeComponent();
        }

        private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            switch (picker.SelectedIndex)
            {
                case 0:
                    selectedImage.Source = "dotnet_bot.png";
                    break;
                case 1:
                    selectedImage.Source = "winterbg.png";
                    break;
                case 2:
                    selectedImage.Source = "winterbgnight.jpg";
                    break;
            }
        }

        private void OnSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
                selectedImage.BackgroundColor = Colors.Purple;
            else
                selectedImage.BackgroundColor = Colors.Transparent;
        }
    }
}
