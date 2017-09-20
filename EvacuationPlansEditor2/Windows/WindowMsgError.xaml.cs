using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EvacuationPlansEditor2
{
    /// <summary>
    /// Логика взаимодействия для WindowMsgError.xaml
    /// </summary>
    public partial class WindowMsgError : Window
    {
        public WindowMsgError(string header, string text)
        {
            InitializeComponent();
            msgHeader.Header = header;
            msgText.Content = text;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
