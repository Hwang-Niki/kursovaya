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
                    string query = @"SELECT u.UserID, u.Username, u.LastName, u.FirstName, 
                                    u.IsLocked, u.LockedUntil, u.FailedLoginAttempts, r.role
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
            string username = dataGridView1.SelectedRows[0].Cells["Username"].Value.ToString();

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
            string username = dataGridView1.SelectedRows[0].Cells["Username"].Value.ToString();

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
            LoadUsers();
        }
    }
}
