using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace Naidis_Mobilapp
{
    public partial class SimpleAlertPopup : Popup
    {
        public SimpleAlertPopup(string title, string message)
        {
            InitializeComponent();
            TitleLabel.Text = title;
            MessageLabel.Text = message;
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            _ = CloseAsync();
        }
    }
}
