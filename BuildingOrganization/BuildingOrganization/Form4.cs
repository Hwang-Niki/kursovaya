using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form4 : Form
    {

        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\youan\Documents\GitHub\kursovaya\BuildingOrganization\BuildingOrganization\Database1.mdf;Integrated Security=True";
        public static Users currentUser { get; set; } = null;
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 120;

        public Form4()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Авторизация в системе строительной организации";
            dateLabel.Text = DateTime.Today.ToShortDateString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Войти")
            {
                string username = textBox1.Text.Trim();
                string password = textBox2.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Введите логин и пароль", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"SELECT u.UserID, u.LastName, u.FirstName, u.MiddleName, 
                                u.Position, u.Id_role, r.role, u.Password, u.IsLocked, 
                                u.FailedLoginAttempts, u.LockedUntil
                                FROM Users u
                                JOIN Role r ON u.Id_role = r.Id_role
                                WHERE u.Username = @Username";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool isLocked = Convert.ToBoolean(reader["IsLocked"]);
                                DateTime? lockedUntil = reader["LockedUntil"] as DateTime?;

                                if (isLocked && lockedUntil.HasValue && lockedUntil > DateTime.Now)
                                {
                                    MessageBox.Show($"Учетная запись заблокирована до {lockedUntil.Value:dd.MM.yyyy HH:mm}",
                                        "Учетная запись заблокирована",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else if (isLocked)
                                {
                                    // Автоматическая разблокировка по истечении времени
                                    UnlockUserAccount(Convert.ToInt32(reader["UserID"]));
                                }

                                string storedPassword = reader["Password"].ToString();
                                string inputPasswordHash = HashPassword(password);


                                if (inputPasswordHash.Trim() == storedPassword.Trim())
                                {
                                     ResetFailedLoginAttempts(Convert.ToInt32(reader["UserID"]));
                                    // Используем статическое свойство вместо локальной переменной
                                    Form4.currentUser = new Users(
                                        Convert.ToInt32(reader["UserID"]),
                                        reader["LastName"].ToString(),
                                        reader["FirstName"].ToString(),
                                        reader["MiddleName"].ToString(),
                                        reader["Position"].ToString(),
                                        Convert.ToInt32(reader["Id_role"])
                                    );

                                    button1.Text = "Выйти";
                                    label1.Visible = false;
                                    label2.Visible = false;
                                    textBox1.Visible = false;
                                    textBox2.Visible = false;

                                    label3.Text = $"Добро пожаловать, {reader["FirstName"]} {reader["MiddleName"]}!";


                                    UpdateLastLogin(Form4.currentUser.UserID);

                                    Form frm3 = new Form3(Form4.currentUser);
                                    frm3.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    int failedAttempts = Convert.ToInt32(reader["FailedLoginAttempts"]);
                                    failedAttempts++;

                                    if (failedAttempts >= MaxFailedAttempts)
                                    {
                                        LockUserAccount(Convert.ToInt32(reader["UserID"]));
                                        MessageBox.Show($"Неверный пароль. Учетная запись заблокирована на {LockoutMinutes} минут.",
                                            "Ошибка авторизации",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        UpdateFailedLoginAttempts(Convert.ToInt32(reader["UserID"]), failedAttempts);
                                        MessageBox.Show($"Неверный пароль. Осталось попыток: {MaxFailedAttempts - failedAttempts}",
                                            "Ошибка авторизации",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден", "Ошибка авторизации",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Form4.currentUser = null;
                button1.Text = "Войти";
                label1.Visible = true;
                label2.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label3.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void UpdateFailedLoginAttempts(int userId, int attempts)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET FailedLoginAttempts = @Attempts WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Attempts", attempts);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Логирование ошибки */ }
        }

        private void ResetFailedLoginAttempts(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET FailedLoginAttempts = 0, IsLocked = 0, LockedUntil = NULL WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Логирование ошибки */ }
        }

        private void LockUserAccount(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Users 
                                   SET FailedLoginAttempts = @Attempts, 
                                       IsLocked = 1, 
                                       LockedUntil = DATEADD(MINUTE, @LockoutMinutes, GETDATE())
                                   WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Attempts", MaxFailedAttempts);
                    command.Parameters.AddWithValue("@LockoutMinutes", LockoutMinutes);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Логирование ошибки */ }
        }

        private void UnlockUserAccount(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET IsLocked = 0, LockedUntil = NULL WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Логирование ошибки */ }
        }

        private void UpdateLastLogin(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET LastLogin = GETDATE() WHERE UserID = @UserID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Игнорируем ошибки обновления времени входа */ }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public class Users
        {
            public int UserID { get; }
            public string LastName { get;}
            public string FirstName { get; }
            public string MiddleName { get; }
            public string Login { get; }
            public string Password { get; }
            public int Id_role { get; }
            public string Position { get; }

            public Users(int userId, string lastName, string firstName, string middleName, string position, int id_role )
            {
                UserID = userId;
                LastName = lastName;
                FirstName = firstName;
                MiddleName = middleName;
                Position = position;
                Id_role = id_role;
   
       
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
