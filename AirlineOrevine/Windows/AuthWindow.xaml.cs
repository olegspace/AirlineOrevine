using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common.Utilities;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;

namespace AirlineOrevine.Windows
{
    public partial class AuthWindow
    {
        private readonly AirlineOrevineDbContext _dbContext;

        public List<UserTypes> UserTypes { get; set; }

        private AccessRight AccessRight { get; set; }

        private List<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

        public AuthWindow()
        {
            InitializeComponent();
            _dbContext = new AirlineOrevineDbContext();
            DataContext = this;
        }

        private AccessRight AddAccessRight(bool v1, bool v2, bool v3, bool v4, FormTypes formType)
        {
            AccessRight = new AccessRight();
            AccessRight.Read = v1;
            AccessRight.Write = v2;
            AccessRight.Edit = v3;
            AccessRight.Delete = v4;

            AccessRight.Form = formType;
            return AccessRight;
        }

        private void RegistrButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                var login = RegisterLoginTextBox.Text;
                if (String.IsNullOrEmpty(RegisterLoginTextBox.Text))
                {
                    MessageBox.Show("Логин не заполнен. Введите логин");
                    return;
                }

                if (RegisterLoginTextBox.Text.Length < 5)
                {
                    MessageBox.Show("Логин не может быть меньше 5 символов");
                    return;
                }

                if (String.IsNullOrEmpty(RegisterPasswordTextBox.Password))
                {
                    MessageBox.Show("Пароль не заполнен. Введите пароль");
                    return;
                }

                if (RegisterPasswordTextBox.Password.Length < 5)
                {
                    MessageBox.Show("Пароль не может быть меньше 5 символов");
                    return;
                }

                if (RegisterPasswordConfirmTextBox.Password != RegisterPasswordTextBox.Password)
                {
                    MessageBox.Show("Пароли не совпадают. Подтвердите новый пароль");
                    return;
                }

                if (!_dbContext.Users.Any(x => x.Login == login))
                {

                    var passwordHash = PasswordEncrypter.GetHash(RegisterPasswordTextBox.Password);
                    User user = new User();
                    user.Login = login;
                    user.Password = passwordHash;
                    user.UserType = DbContext.Enums.UserTypes.User;


                    foreach (var form in Enum.GetValues(typeof(FormTypes)).Cast<FormTypes>())
                    {
                        AccessRights.Add(AddAccessRight(false, false, false, false, form));
                    }

                    user.AccessRights = AccessRights;

                    Cursor = Cursors.Wait;
                    _dbContext.Add(user);
                    try
                    {
                        _dbContext.SaveChanges();
                    }
                    catch (DbUpdateException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }

                    var mainWindow = new MainWindow(_dbContext, user);
                    mainWindow.Show();
                    Close();
                    
                }
                else
                {
                    MessageBox.Show("Данный логин уже существует");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации пользователя");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (String.IsNullOrEmpty(UserLoginTextBox.Text))
                {
                    MessageBox.Show("Логин не заполнен. Введите логин");
                    return;
                }

                if (String.IsNullOrEmpty(LoginPasswordTextBox.Password))
                {
                    MessageBox.Show("Пароль не заполнен. Введите пароль");
                    return;
                }

                var login = UserLoginTextBox.Text;
                var passwordHash = PasswordEncrypter.GetHash(LoginPasswordTextBox.Password);

                Cursor = Cursors.Wait;
                User? user = _dbContext.Users
                    .Include(x => x.AccessRights)
                    .FirstOrDefault(u => u.Login == login && u.Password == passwordHash);
                if (user != null)
                {
                    var mainWindow = new MainWindow(_dbContext, user);
                    Cursor = Cursors.Arrow;
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    Cursor = Cursors.Arrow;
                    MessageBox.Show("Неправильный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке программы");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}