using System.Windows.Media;
using System.Windows.Controls;

namespace HealthTray.Wpf
{
    public partial class StatusChangePopup : UserControl
    {
        private const string PopupText = "{0} changed status";

        public StatusChangePopup()
        {
            InitializeComponent();
        }

        public StatusChangePopup(ImageSource image, string checkName) : this()
        {
            StatusImage.Source = image;
            StatusText.Text = string.Format(PopupText, checkName);
        }
    }
}
