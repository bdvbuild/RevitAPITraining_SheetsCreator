using Autodesk.Revit.UI;
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
using TextBox = System.Windows.Controls.TextBox;

namespace RevitAPITraining_SheetsCreator
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(ExternalCommandData commandData)
        {
            InitializeComponent();
            MainViewViewModel vm = new MainViewViewModel(commandData);
            vm.CloseRequest += (s, e) => this.Close();
            DataContext = vm;

        }
        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            MainViewViewModel vm = DataContext as MainViewViewModel;
            TextBox textBox = sender as TextBox;

            if (!int.TryParse(textBox.Text, out int result) || textBox.Text == null)
            {
                MessageBox.Show("Введите целое число.", "Ошибка ввода");
            }
            else if (result <= 1)
            {
                vm.SheetsCount = 1;
                TextBox1.Text = vm.SheetsCount.ToString();
            }
            else
            {
                vm.SheetsCount = (int)Math.Round((double)result);
                TextBox1.Text = vm.SheetsCount.ToString();
            }
        }

    }
}
