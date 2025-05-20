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
    public partial class Form10 : Form
    {
        private string connectionString = Properties.Settings.Default.Database1ConnectionString;

        public Form10()
        {
            InitializeComponent();
            LoadTableNames();
        }

        private void LoadTableNames()
        {
            comboBox1.Items.Add("Brigades");
            comboBox1.Items.Add("ConstructionEquipment");
            comboBox1.Items.Add("ConstructionManagement");
            comboBox1.Items.Add("ConstructionMaterials");
            comboBox1.Items.Add("ConstructionObjects");
            comboBox1.Items.Add("ConstructionSites");
            comboBox1.Items.Add("Contracts");
            comboBox1.Items.Add("Customers");
            comboBox1.Items.Add("Employees");
            comboBox1.Items.Add("EquipmentTypes");
            comboBox1.Items.Add("Estimates");
            comboBox1.Items.Add("MaterialPlans");
            comboBox1.Items.Add("MaterialUsage");
            comboBox1.Items.Add("ObjectType");
            comboBox1.Items.Add("Positions");
            comboBox1.Items.Add("WorkReports");
            comboBox1.Items.Add("WorkSchedules");
            comboBox1.Items.Add("WorkTypes");
           
            
            
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите таблицу.");
                return;
            }

            string selectedTable = comboBox1.SelectedItem.ToString();
            LoadTableData(selectedTable);
        }

        private void LoadTableData(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                DataTable dtResults = new DataTable();

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtResults);

                    if (dtResults.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет данных для отображения.");
                        dataGridView1.DataSource = null; // Очищаем DataGridView.
                    }
                    else
                    {
                        dataGridView1.DataSource = dtResults; // Устанавливаем источник данных.
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
 }
