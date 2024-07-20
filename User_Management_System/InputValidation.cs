using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using User_Management_System.Models;


namespace User_Management_System
{
    public class InputValidation
    {
        static UserManagementSystemContext context = new UserManagementSystemContext();
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Tên đăng nhập không được để trống.");
                return false;
            }

            if (username.Length < 3 || username.Length > 50)
            {
                ShowError("Tên đăng nhập phải chứa từ 3 đến 50 kí tự.");
                return false;
            }
            if (username.Contains(" "))
            {
                ShowError("Tên đăng nhập không được chứa khoảng trắng.");
                return false;
            }
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                ShowError("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới.");
                return false;
            }

            return true;
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Mật khẩu không được để trống.");
                return false;
            }

            if (password.Length < 8)
            {
                ShowError("Mật khẩu phải có ít nhất 8 kí tự.");
                return false;
            }

            if (password.Contains(" "))
            {
                ShowError("Mật khẩu không được chứa khoảng trắng.");
                return false;
            }

            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$"))
            {
                ShowError("Mật khẩu phải chứa ít nhất một chữ cái viết thường, một chữ cái viết hoa và một số.");
                return false;
            }

            return true;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email không được để trống.");
                return false;
            }

            try
            {
                // Sử dụng System.Net.Mail.MailAddress để kiểm tra email
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new FormatException();
                }
            }
            catch
            {
                ShowError("Email không hợp lệ.");
                return false;
            }

            if(context.Users.Any(u => u.Email.Equals(email)))
            {
                MessageBox.Show("Email đã được sử dụng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;

            }

            return true;
        }

        public static bool IsValidEmail(string email,string curentEmail)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email không được để trống.");
                return false;
            }

            try
            {
                
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new FormatException();
                }
            }
            catch
            {
                ShowError("Email không hợp lệ.");
                return false;
            }
            
            if (context.Users.Any(u => u.Email.Equals(email) && email!=curentEmail))
            {
                MessageBox.Show("Email đã được sử dụng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;

            }

            return true;
        }
        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static bool IsValidFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowError("Họ và tên không được để trống.");
                return false;
            }

            if (fullName.Length > 100)
            {
                ShowError("Họ và tên không hợp lệ, tối đa 100 ký tự.");
                return false;
            }

            if (!Regex.IsMatch(fullName, @"^[\p{L}\s]+$"))
            {
                ShowError("Họ và tên chỉ được chứa chữ cái và khoảng trắng.");
                return false;
            }

            return true;
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {           

            if (string.IsNullOrWhiteSpace(phoneNumber)) { return true; }
            if (!Regex.IsMatch(phoneNumber, @"^[0-9]{10,11}$"))
            {
                ShowError("Số điện thoại không hợp lệ. Phải có 10 hoặc 11 chữ số.");
                return false;
            }

            return true;
        }
        public static bool IsCFPassword(string pass1,string pass2)
        {
            return Regex.IsMatch(pass1, pass2);
        }

        public static string toSHA1(String password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }

        }
    }
}
