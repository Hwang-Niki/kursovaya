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
            LoadConstructionObjects();
        }

        private void LoadConstructionObjects()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ObjectID, Name FROM ConstructionObjects ORDER BY Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    comboBoxObjects.DataSource = dataTable;
                    comboBoxObjects.DisplayMember = "Name";
                    comboBoxObjects.ValueMember = "ObjectID";
                    comboBoxObjects.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке списка объектов: " + ex.Message);
            }
        }

        private void LoadData(int? objectId = null)
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

                    if (objectId.HasValue)
                    {
                        query += " WHERE o.ObjectID = @ObjectID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    if (objectId.HasValue)
                    {
                        command.Parameters.AddWithValue("@ObjectID", objectId.Value);
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
            if (comboBoxObjects.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите объект из списка");
                return;
            }

            int selectedObjectId = (int)comboBoxObjects.SelectedValue;
            LoadData(selectedObjectId);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadData(); // Загружаем все данные без фильтрации
            comboBoxObjects.SelectedIndex = -1;
        }
    }
}
