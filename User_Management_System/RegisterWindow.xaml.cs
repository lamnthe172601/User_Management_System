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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        
        UserManagementSystemContext context = new UserManagementSystemContext();

        private bool _isAdminMode;
        private string msg = "Đăng kí thành công";       
        public RegisterWindow()
        {
            InitializeComponent();
            _isAdminMode = false;
            AdjustUIForMode();
        }
             
        public RegisterWindow(bool isAdminMode)
        {
            InitializeComponent();
            _isAdminMode = isAdminMode;
            AdjustUIForMode();
        }

        private void AdjustUIForMode()
        {
            if (_isAdminMode)
            {
                txtHeader.Text = "Thêm người dùng mới";              
                btnLogin.Visibility = Visibility.Collapsed;
                msg = "Thêm thành công!";              
                this.Title = "Thêm người dùng mới";
                btnRegister.Content = "Thêm người dùng";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = chkShowPassword.IsChecked == true ? txtPasswordVisible.Text.Trim() : txtPassword.Password.Trim();
            string cfPassword = chkShowPassword.IsChecked == true ? txtCfPasswordVisible.Text.Trim() : txtCfPassword.Password.Trim();
            string email = txtEmail.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            if (IsExistUser(username))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!InputValidation.IsValidUsername(username)
                || !InputValidation.IsValidPassword(password)
                || !InputValidation.IsValidPassword(cfPassword)
                || !InputValidation.IsValidEmail(email)
                || !InputValidation.IsValidFullName(fullName))

            {
                return;
            }
            
            if (!InputValidation.IsCFPassword(password,cfPassword))
            {
                MessageBox.Show("Xác nhận mật khẩu sai.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var newUser = new User
            {
                Username = username,
                Password = InputValidation.toSHA1(password),
                Email = email,
                FullName = fullName,
                RoleId = 2,
                CreatedAt = DateTime.Now,
                IsActive = true,               
            };

            context.Users.Add(newUser);
            try
            {
                context.SaveChanges();
                MessageBox.Show(msg);
                if (!_isAdminMode)
                {
                    UserProfileWindow profileWindow = new UserProfileWindow(newUser);
                    profileWindow.Show();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng ký: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}");
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Window currentWindow = Window.GetWindow(this);
            currentWindow?.Close();
        }
        public bool IsExistUser(string username)
        {
            return context.Users
              .Any(u => u.Username.Equals(username));
        }
        private void chkShowPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == true)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtCfPasswordVisible.Text = txtCfPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtCfPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtCfPasswordVisible.Visibility = Visibility.Visible;
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtCfPassword.Password = txtCfPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtCfPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtCfPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }
    }
}
