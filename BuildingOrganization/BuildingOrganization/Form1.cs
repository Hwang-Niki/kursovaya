using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void LoadAllData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                             ce.EquipmentID AS [Идентификатор оборудования],
                             et.Name  AS [Тип оборудования],
                             ce.Name AS [Наименование],
                             ce.RegistrationNumber AS [Регистрационный номер],
                             ce.ManufactureYear AS [Год выпуска],
                             ce.Condition AS [Состояние],
                             co.Name AS [Объект],
                             eu.StartDate AS [Дата начала],
                             eu.EndDate AS [Дата окончания]
                        FROM 
                            EquipmentUsage eu
                        JOIN 
                            ConstructionEquipment ce ON eu.EquipmentID = ce.EquipmentID
                        JOIN 
                            EquipmentTypes et ON ce.TypeID = et.TypeID
                        JOIN 
                            ConstructionObjects co ON eu.UsageID = co.ObjectID;";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable; // Обновляем DataGridView с результатами
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string objectName = txtObjectName.Text.Trim(); // Получаем название объекта
            DateTime startDate = dateTimePickerStart.Value; // Получаем начальную дату из DateTimePicker
            DateTime endDate = dateTimePickerEnd.Value; // Получаем конечную дату из DateTimePicker

            if (!string.IsNullOrEmpty(objectName))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                    {
                        connection.Open(); // Открываем соединение

                        // Получаем ObjectID по названию объекта
                        string getObjectIdQuery = "SELECT ObjectID FROM ConstructionObjects WHERE Name = @ObjectName";
                        SqlCommand getObjectIdCommand = new SqlCommand(getObjectIdQuery, connection);
                        getObjectIdCommand.Parameters.AddWithValue("@ObjectName", objectName);

                        object result = getObjectIdCommand.ExecuteScalar();

                        if (result != null)
                        {
                            int objectId = Convert.ToInt32(result);

                            // Выполняем запрос для получения отфильтрованных данных
                            string query = @"
                                SELECT 
                                    ce.EquipmentID AS [Идентификатор оборудования],
                                    et.Name  AS [Тип оборудования],
                                    ce.Name AS [Наименование],
                                    ce.RegistrationNumber AS [Регистрационный номер],
                                    ce.ManufactureYear AS [Год выпуска],
                                    ce.Condition AS [Состояние],
                                    co.Name AS [Объект],
                                    eu.StartDate AS [Дата начала],
                                    eu.EndDate AS [Дата окончания]
                                FROM 
                                    EquipmentUsage eu
                                JOIN 
                                    ConstructionEquipment ce ON eu.EquipmentID = ce.EquipmentID
                                JOIN 
                                    EquipmentTypes et ON ce.TypeID = et.TypeID
                                JOIN 
                                    ConstructionObjects co ON eu.UsageID = co.ObjectID
                                WHERE 
                                    eu.UsageID = @UsageID AND 
                                    eu.StartDate BETWEEN @StartDate AND @EndDate";

                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@UsageID", objectId);
                            command.Parameters.AddWithValue("@StartDate", startDate);
                            command.Parameters.AddWithValue("@EndDate", endDate);

                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            if (dataTable.Rows.Count > 0)
                            {
                                dataGridView1.DataSource = dataTable; // Обновляем DataGridView с результатами
                            }
                            else
                            {
                                MessageBox.Show("Нет техники, выделенной на указанный объект в указанный период.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Объект не найден.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Введите название объекта.");
            }
        }
   

        private void button2_Click(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
    
