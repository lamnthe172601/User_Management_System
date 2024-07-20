using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();

        }
        private User _currentUser;
        public AdminPanel(User user)
        {
            InitializeComponent();
            _currentUser = user;
            PopulateMonthComboBox();
            LoadUsers();
        }
        UserManagementSystemContext context = new UserManagementSystemContext();

        private void LoadUsers()
        {

            using (var newContext = new UserManagementSystemContext())
            {
                dgUsers.ItemsSource = newContext.Users
                    .Where(u => u.RoleId != 1)
                    .ToList();
            }

        }

        public void PopulateMonthComboBox()
        {
            // Sử dụng DateTimeFormatInfo để lấy tên tháng theo ngôn ngữ hệ thống
            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

            for (int i = 1; i <= 12; i++)
            {
                cbMonth.Items.Add(dateTimeFormat.GetMonthName(i));
            }

            // Đặt tháng hiện tại là giá trị mặc định
            cbMonth.SelectedIndex = DateTime.Now.Month - 1;
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(true);
            registerWindow.ShowDialog();
            LoadUsers();
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                UserProfileWindow profileWindow = new UserProfileWindow(selectedUser, true);
                profileWindow.ShowDialog();
                LoadUsers();
                dgUsers.SelectedItem = null;
                dgUsers.UnselectAll();

            }
            else
            {
                MessageBox.Show("Vui lòng chọn một người dùng để chỉnh sửa.");
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa người dùng {selectedUser.Username}?", "Xác nhận xóa", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var user = context.Users.Find(selectedUser.UserId);
                    if (user != null)
                    {    
                        var userInfo= context.Information.FirstOrDefault(i=>i.UserId==user.UserId);
                        if(userInfo != null)
                        {
                            context.Information.Remove(userInfo);
                        }
                        context.Users.Remove(user);
                        context.SaveChanges();
                        LoadUsers();
                    }                   

                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một người dùng để xóa.");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadUsers();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(_currentUser.Username);
            changePasswordWindow.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
        }

        private void btnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn thay đổi trạng thái {selectedUser.Username}?", "Xác nhận thay đổi", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                    var user = context.Users.Find(selectedUser.UserId);
                    if (user != null)
                    {
                        user.IsActive = !user.IsActive;
                        context.SaveChanges();
                        LoadUsers();
                    }

                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng.");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            var data = context.Users
                .Where(u => u.RoleId != 1)
                .Where(u => u.FullName.Contains(searchTerm))
                .ToList();
            if (data == null) { 
             
            }
            dgUsers.ItemsSource = data;
        }

        private void dgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                UserProfileWindow profileWindow = new UserProfileWindow(selectedUser, true);
                profileWindow.ShowDialog();
                LoadUsers();
                dgUsers.SelectedItem = null;
                dgUsers.UnselectAll();

            }
        }

        private void cboStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var selectedStatus = cboStatusFilter.SelectedItem as ComboBoxItem;
            int selectedMonth = cbMonth.SelectedIndex + 1;
            int currentYear = DateTime.Now.Year;

            using (var newContext = new UserManagementSystemContext())
            {
                var query = newContext.Users.Where(u => u.RoleId != 1);

                
                query = query.Where(u => u.CreatedAt.Value.Month == selectedMonth && u.CreatedAt.Value.Year == currentYear);

               
                if (selectedStatus != null)
                {
                    switch (selectedStatus.Content.ToString())
                    {
                        case "Tất cả":
                            
                            break;
                        case "Đang hoạt động":
                            query = query.Where(u => u.IsActive);
                            break;
                        case "Ngừng hoạt động":
                            query = query.Where(u => !u.IsActive);
                            break;
                    }
                }

                dgUsers.ItemsSource = query.ToList();
            }
        }
       
    }
}
