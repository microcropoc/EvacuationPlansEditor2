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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EvacuationPlansEditor2
{
    /// <summary>
    /// Логика взаимодействия для SettingToolForWall.xaml
    /// </summary>
    public partial class SettingToolForVisualObject : UserControl
    {
        public static readonly DependencyProperty ThincknessProperty = DependencyProperty.Register("Thinckness", typeof(double), typeof(SettingToolForVisualObject));

        public static double StandartThickness { get; set; }

        static SettingToolForVisualObject()
        {
            StandartThickness = 5;
        }
        public double Thinckness
        {
            get { return (double)GetValue(ThincknessProperty); }
            set { SetValue(ThincknessProperty, value); StandartThickness=value; }
        }
        public SettingToolForVisualObject(string typeObject)
        {
            InitializeComponent();
            txtBoxThinckness.TextChanged += new TextChangedEventHandler(OnTextChanged);
            Thinckness = StandartThickness;
            txtHeader.Text = typeObject;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //заплатка, реализовать проверку на число с плавающей точкой

            if (string.IsNullOrWhiteSpace(txtBoxThinckness.Text))
                txtBoxThinckness.Text = "0";

            try
            {
                if(txtBoxThinckness.Text[txtBoxThinckness.Text.Length-1]==',')
                {
                    txtBoxThinckness.Text = double.Parse(txtBoxThinckness.Text+'0').ToString();
                }
                else
                {
                    txtBoxThinckness.Text = double.Parse(txtBoxThinckness.Text).ToString();
                }
            }
            catch
            {
                txtBoxThinckness.Text = txtBoxThinckness.Text.Substring(0, txtBoxThinckness.Text.Length - 1);
            }

            txtBoxThinckness.CaretIndex = txtBoxThinckness.Text.Length;

            //---------------------------------------------

            this.Thinckness =double.Parse(txtBoxThinckness.Text);
            e.Handled = true;
            if (FileNameChanged != null)
            {
                FileNameChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> FileNameChanged;
    }
}
