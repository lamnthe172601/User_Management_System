using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        UserManagementSystemContext context = new UserManagementSystemContext();
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private readonly string _username;        

        public ChangePasswordWindow(string username)
        {
            InitializeComponent();
            _username = username;            
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = chkShowPassword.IsChecked == true ? txtCurrentPasswordVisible.Text.Trim() : txtCurrentPassword.Password.Trim();
            string newPassword = chkShowPassword.IsChecked == true ? txtNewPasswordVisible.Text.Trim() : txtNewPassword.Password.Trim();
            string confirmNewPassword = chkShowPassword.IsChecked == true ? txtConfirmNewPasswordVisible.Text.Trim() : txtConfirmNewPassword.Password.Trim();


            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!InputValidation.IsValidPassword(newPassword) 
                || !InputValidation.IsValidPassword(confirmNewPassword)) {
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu mới không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           

            try
            {
                bool isPasswordChanged = ChangePassword(_username,currentPassword,newPassword);

                if (isPasswordChanged)
                {
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mật khẩu hiện tại không đúng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ChangePassword(string username, string oldPasswordHash, string newPasswordHash)
        {
            var user = context.Users
                .Where(u => u.Username == username && u.Password == InputValidation.toSHA1(oldPasswordHash))
                .FirstOrDefault();

            if (user != null)
            {
                user.Password = InputValidation.toSHA1( newPasswordHash);
                context.SaveChanges();
                return true;
            }

            return false;
        }

      
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void chkShowPassword_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == true)
            {
                txtCurrentPasswordVisible.Text = txtCurrentPassword.Password;
                txtNewPasswordVisible.Text =txtNewPassword.Password;
                txtConfirmNewPasswordVisible.Text = txtConfirmNewPassword.Password;
                txtCurrentPassword.Visibility = Visibility.Collapsed;
                txtNewPassword.Visibility = Visibility.Collapsed;
                txtConfirmNewPassword.Visibility = Visibility.Collapsed;
                txtCurrentPasswordVisible.Visibility = Visibility.Visible;
                txtNewPasswordVisible.Visibility = Visibility.Visible;
                txtConfirmNewPasswordVisible.Visibility = Visibility.Visible;
            }
            else
            {
                txtCurrentPassword.Password = txtCurrentPasswordVisible.Text;
                txtNewPassword.Password = txtNewPasswordVisible.Text;
                txtConfirmNewPassword.Password = txtConfirmNewPasswordVisible.Text;
                txtCurrentPassword.Visibility = Visibility.Visible;
                txtNewPassword.Visibility = Visibility.Visible;
                txtConfirmNewPassword.Visibility = Visibility.Visible;
                txtCurrentPasswordVisible.Visibility = Visibility.Collapsed;
                txtNewPasswordVisible.Visibility = Visibility.Collapsed;
                txtConfirmNewPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }
    }
}


