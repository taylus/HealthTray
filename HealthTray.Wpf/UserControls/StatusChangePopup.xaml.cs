using System.Windows.Media;
using System.Windows.Controls;

namespace HealthTray.Wpf
{
    public partial class StatusChangePopup : UserControl
    {
        public StatusChangePopup()
        {
            InitializeComponent();
        }

        public StatusChangePopup(ImageSource image, string statusText) : this()
        {
            StatusImage.Source = image;
            StatusText.Text = statusText;
        }
    }
}
