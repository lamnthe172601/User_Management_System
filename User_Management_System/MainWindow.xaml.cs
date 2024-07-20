using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace User_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {          
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();            
           
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {            
            RegisterWindow registerWindow = new RegisterWindow();
            this.Close();
            registerWindow.Show();            
            
        }
    }
}