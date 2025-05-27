using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form11 : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\youan\Documents\GitHub\kursovaya\BuildingOrganization\BuildingOrganization\Database1.mdf;Integrated Security=True";
        public Form11()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT u.UserID, u.Username AS [Логин], u.LastName AS [Фамилия], u.FirstName AS [Имя], u.MiddleName AS [Отчество],
                                    u.IsLocked AS [Блокировка], u.LockedUntil AS [Заблокировано до], u.FailedLoginAttempts AS [Ошибки при авторизации], r.role AS [Роль]
                                    FROM Users u
                                    JOIN Role r ON u.Id_role = r.Id_role
                                    ORDER BY u.LastName, u.FirstName";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["UserID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);
            string username = dataGridView1.SelectedRows[0].Cells["Логин"].Value.ToString();

            if (MessageBox.Show($"Заблокировать пользователя {username}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"UPDATE Users 
                                       SET IsLocked = 1, 
                                           LockedUntil = DATEADD(DAY, 1, GETDATE()),
                                           FailedLoginAttempts = @MaxAttempts
                                       WHERE UserID = @UserID";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MaxAttempts", 5);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Пользователь заблокирован", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка блокировки пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);
            string username = dataGridView1.SelectedRows[0].Cells["Логин"].Value.ToString();

            if (MessageBox.Show($"Разблокировать пользователя {username}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"UPDATE Users 
                                       SET IsLocked = 0, 
                                           LockedUntil = NULL,
                                           FailedLoginAttempts = 0
                                       WHERE UserID = @UserID";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Пользователь разблокирован", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка разблокировки пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);
            string username = dataGridView1.SelectedRows[0].Cells["Логин"].Value.ToString();

            // Проверка, что пользователь не пытается удалить самого себя
            if (userId == Form4.currentUser.UserID)
            {
                MessageBox.Show("Вы не можете удалить свою собственную учетную запись!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя {username}?\nЭто действие нельзя отменить!", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Удаляем пользователя
                        string query = "DELETE FROM Users WHERE UserID = @UserID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@UserID", userId);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Пользователь успешно удален", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUsers(); // Обновляем список пользователей
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) // Ошибка внешнего ключа
                    {
                        MessageBox.Show("Нельзя удалить пользователя, так как он связан с другими записями в системе.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка удаления пользователя: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
