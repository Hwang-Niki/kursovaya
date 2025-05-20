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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            LoadData();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData(string filter = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT e.EstimateID AS [Номер сметы], o.Name AS [Имя объекта], e.CreationDate AS [Дата составления] , em.FullName AS [Утверждающее лицо], e.TotalCost AS [Общая стоимость] " +
                                   "FROM Estimates e " +
                                   "JOIN ConstructionObjects o ON e.ObjectID = o.ObjectID " + 
                                   "JOIN Employees em ON e.ApprovedBy = em.EmployeeID";

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query += " WHERE o.Name LIKE @Filter";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    if (!string.IsNullOrEmpty(filter))
                    {
                        command.Parameters.AddWithValue("@Filter", "%" + filter + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView2.DataSource = dataTable;

                    dataGridView2.Columns["Имя объекта"].Width = 170;
                    dataGridView2.Columns["Утверждающее лицо"].Width = 200;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string filter = txtObjectName.Text.Trim(); 
            LoadData(filter); // Загружаем данные с фильтром по имени объекта
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadData(); // Загружаем все данные без фильтрации
        }
    }
}
