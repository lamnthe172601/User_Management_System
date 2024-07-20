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
using User_Management_System.Models;

namespace User_Management_System
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        UserManagementSystemContext context = new UserManagementSystemContext();
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = chkShowPassword.IsChecked == true ? txtPasswordVisible.Text.Trim() : txtPassword.Password.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
                MessageBox.Show("Nhập đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string passSHA1=InputValidation.toSHA1(password);


            var user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == passSHA1);          
            if (user != null)
            {
                if (IsUserAdmin(user.UserId))
                {                    
                    AdminPanel adminPanel = new AdminPanel(user);
                    adminPanel.Show();
                   this.Close();
                }
                else
                {
                    if (!user.IsActive)
                    {
                        MessageBox.Show("Tài khoản đã bị khóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    UserProfileWindow profileWindow = new UserProfileWindow(user);
                    profileWindow.Show();
                    this.Close();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
            }

        }

        public bool IsUserAdmin(int userId)
        {

            return context.Users
                .Any(ur => ur.UserId == userId && ur.Role.RoleName == "Admin");

        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
         
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void chkShowPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == true)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                
                txtPassword.Visibility = Visibility.Collapsed;
               
                txtPasswordVisible.Visibility = Visibility.Visible;
              
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
               
                txtPassword.Visibility = Visibility.Visible;
                
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                
            }
        }
    }
}
