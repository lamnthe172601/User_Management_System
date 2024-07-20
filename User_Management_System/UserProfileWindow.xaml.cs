using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using User_Management_System.Models;

namespace User_Management_System
{
    /// <summary>
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        public UserProfileWindow()
        {
            InitializeComponent();
        }

        UserManagementSystemContext context = new UserManagementSystemContext();
        private User currentUser;
        private bool _isAdminMode;
        public UserProfileWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            _isAdminMode = false;
            txtWelcome.Text = "Xin chào, " + currentUser.Username;
            LoadUserData();
        }
        public UserProfileWindow(User user, bool isAdmin)
        {
            InitializeComponent();
            currentUser = user;
            _isAdminMode = isAdmin;
            txtWelcome.Text = currentUser.Username;
            AdjustUIForMode();
            LoadUserData();
        }
        private void AdjustUIForMode()
        {
            if (_isAdminMode)
            {
                btnChagePass.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Collapsed;                
                this.Title = "Cập nhật thông tin người dùng";
            }
        }
        private void LoadUserData()
        {

            txtEmail.Text = currentUser.Email;
            txtFullName.Text = currentUser.FullName;
            male.IsChecked = true;
            try
            {
                var info = context.Information.FirstOrDefault(i => i.UserId == currentUser.UserId);
                if (info != null)
                {
                    dpDateOfBirth.SelectedDate = info.DateOfBirth != null
                        ? new DateTime(info.DateOfBirth.Value.Year, info.DateOfBirth.Value.Month, info.DateOfBirth.Value.Day)
                        : (DateTime?)null;
                    if (info.Gender)
                    {
                        male.IsChecked = true;

                    }
                    else
                    {
                        female.IsChecked = true;
                    }
                    txtAddress.Text = info.Address;
                    txtPhoneNumber.Text = info.PhoneNumber;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           
            var result = MessageBox.Show("Bạn có thực sự muốn lưu thay đổi?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var user = context.Users.Find(currentUser.UserId);
                    if (user != null)
                    {
                        string fullName = txtFullName.Text.Trim();
                        string email = txtEmail.Text.Trim();
                        string address = txtAddress.Text.Trim();
                        string phoneNumber = txtPhoneNumber.Text.Trim();
                        if (!ValidateDateOfBirth())
                        {
                            return;
                        }

                        if (
                            !InputValidation.IsValidFullName(fullName)
                            || !InputValidation.IsValidEmail(email, user.Email)
                            || !InputValidation.IsValidPhoneNumber(phoneNumber))
                        {
                            return;
                        }
                        user.Email = email;
                        user.FullName = fullName;
                        var info = context.Information.FirstOrDefault(i => i.UserId == currentUser.UserId);
                        if (info == null)
                        {
                            info = new Information { UserId = currentUser.UserId };
                            context.Information.Add(info);
                        }

                        info.DateOfBirth = dpDateOfBirth.SelectedDate.HasValue
                            ? DateOnly.FromDateTime(dpDateOfBirth.SelectedDate.Value)
                            : null;
                        info.Gender = male.IsChecked == true;
                        info.Address = address;
                        info.PhoneNumber = phoneNumber;                        
                        context.SaveChanges();
                        MessageBox.Show("Thông tin đã được cập nhật!");
                        if (_isAdminMode)
                        {
                            this.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(currentUser.Username);
            changePasswordWindow.ShowDialog();
        }

        private void male_Checked(object sender, RoutedEventArgs e)
        {
            female.IsChecked = false;
        }

        private void female_Checked(object sender, RoutedEventArgs e)
        {
            male.IsChecked = false;
        }
        private bool ValidateDateOfBirth()
        {
            if (!dpDateOfBirth.SelectedDate.HasValue)
            {
                return true;
            }

            DateTime selectedDate = dpDateOfBirth.SelectedDate.Value;

            if (selectedDate > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không hợp lệ. Vui lòng chọn một ngày trong quá khứ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
