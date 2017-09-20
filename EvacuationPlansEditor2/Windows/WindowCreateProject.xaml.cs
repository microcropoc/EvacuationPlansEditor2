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
    /// Логика взаимодействия для WindowCreateProject.xaml
    /// </summary>
    public partial class WindowCreateProject : Window
    {

        string _nameProject;

        public WindowCreateProject()
        {
            InitializeComponent();
            
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _nameProject = txtProjectName.Text;
            if(Program.NewProject(_nameProject))
            {
                DialogResult = true;
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void txtProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtProjectName.Text==string.Empty)
            {
                btnOk.IsEnabled = false;
                return;
            }


            bool permissibleName = true;
            char[] unusedChars=new char[]{'*','|','\\',':','\"','<','>','?','/'};

            foreach(char c in txtProjectName.Text)
            {
                foreach(char unusedC in unusedChars)
                {
                    if (c == unusedC)
                    {
                        permissibleName = false;

                    }
                }
            }

            btnOk.IsEnabled = permissibleName;

        }
    }
}
