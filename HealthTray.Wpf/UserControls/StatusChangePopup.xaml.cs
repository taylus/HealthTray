using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace HealthTray.Wpf
{
    public partial class StatusChangePopup : UserControl
    {
        public StatusChangePopup()
        {
            InitializeComponent();
            MouseLeftButtonUp += ShowDashboard_ClickHandler;
        }

        private void ShowDashboard_ClickHandler(object sender, MouseButtonEventArgs e)
        {
            new ShowDashboardCommand().Execute(null);
        }

        public StatusChangePopup(ImageSource image, string statusText) : this()
        {
            StatusImage.Source = image;
            StatusText.Text = statusText;
        }
    }
}
