using System;
using System.Data;
using System.Data.SqlClient;
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

            try
            {
                // Проверяем, выбран ли хотя бы один вариант поиска
                if (!radioButtonObjectOnly.Checked && !radioButtonPeriodOnly.Checked)
                {
                    MessageBox.Show("Введите название объекта или выберите период времени",
                                  "Не выбраны условия поиска",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();

                    // Для варианта "Только по периоду" не проверяем название объекта
                    if (radioButtonPeriodOnly.Checked)
                    {
                        DateTime startDate = dateTimePickerStart.Value;
                        DateTime endDate = dateTimePickerEnd.Value;

                        string query = @"
                    SELECT 
                        ce.EquipmentID AS [Идентификатор оборудования],
                        et.Name AS [Тип оборудования],
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
                        eu.StartDate BETWEEN @StartDate AND @EndDate";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    else // Для вариантов с объектом
                    {
                        string objectName = txtObjectName.Text.Trim();

                        if (string.IsNullOrEmpty(objectName))
                        {
                            MessageBox.Show("Введите название объекта.");
                            return;
                        }

                        string getObjectIdQuery = "SELECT ObjectID FROM ConstructionObjects WHERE Name = @ObjectName";
                        SqlCommand getObjectIdCommand = new SqlCommand(getObjectIdQuery, connection);
                        getObjectIdCommand.Parameters.AddWithValue("@ObjectName", objectName);

                        object result = getObjectIdCommand.ExecuteScalar();

                        if (result == null) // Исправлено условие - было if (result != null)
                        {
                            MessageBox.Show("Объект не найден.");
                            return;
                        }

                        int objectId = Convert.ToInt32(result);

                        if (radioButtonObjectOnly.Checked)
                        {
                            string query = @"
                        SELECT 
                            ce.EquipmentID AS [Идентификатор оборудования],
                            et.Name AS [Тип оборудования],
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
                            eu.UsageID = @UsageID";

                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@UsageID", objectId);
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            adapter.Fill(dataTable);
                        }
                    }

                    if (dataTable.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                    }
                    else
                    {
                        MessageBox.Show("Нет данных, соответствующих условиям поиска.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
   
        private void button2_Click(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void radioButtonObjectOnly_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerStart.Enabled = !radioButtonObjectOnly.Checked;
            dateTimePickerEnd.Enabled = !radioButtonObjectOnly.Checked;
        }

        private void radioButtonPeriodOnly_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerStart.Enabled = radioButtonPeriodOnly.Checked;
            dateTimePickerEnd.Enabled = radioButtonPeriodOnly.Checked;
        }
    }
}
    
