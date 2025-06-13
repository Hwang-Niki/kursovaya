using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            LoadConstructionObjects();
            LoadData();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "database1DataSet.WorkSchedules". При необходимости она может быть перемещена или удалена.
            this.workSchedulesTableAdapter.Fill(this.database1DataSet.WorkSchedules);

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

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT ws.ScheduleID, 
                            co.Name AS ObjectName, 
                            wt.Name AS WorkTypeName, 
                            b.Name AS BrigadeName,
                            ws.PlannedStartDate, 
                            ws.PlannedEndDate
                            FROM WorkSchedules ws
                            JOIN ConstructionObjects co ON ws.ObjectID = co.ObjectID
                            JOIN WorkTypes wt ON ws.WorkTypeID = wt.WorkTypeID
                            JOIN Brigades b ON ws.BrigadeID = b.BrigadeID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    if (dataTable.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                        // Изменение заголовков столбцов
                        dataGridView1.Columns["ScheduleID"].HeaderText = "Номер графика";
                        dataGridView1.Columns["ObjectName"].HeaderText = "Название объекта";
                        dataGridView1.Columns["WorkTypeName"].HeaderText = "Вид работ";
                        dataGridView1.Columns["BrigadeName"].HeaderText = "Бригада";
                        dataGridView1.Columns["PlannedStartDate"].HeaderText = "Планируемая дата начала";
                        dataGridView1.Columns["PlannedEndDate"].HeaderText = "Планируемая дата конца";
                    }
                    else
                    {
                        MessageBox.Show("Нет объектов с указанным названием.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBoxObjects.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите объект из списка.");
                return;
            }

            try
            {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                    {
                        connection.Open(); // Открываем соединение
                        string query = @"SELECT ws.ScheduleID, 
                                co.Name AS ObjectName, 
                                wt.Name AS WorkTypeName, 
                                b.Name AS BrigadeName,
                                ws.PlannedStartDate, 
                                ws.PlannedEndDate
                                FROM WorkSchedules ws
                                JOIN ConstructionObjects co ON ws.ObjectID = co.ObjectID
                                JOIN WorkTypes wt ON ws.WorkTypeID = wt.WorkTypeID
                                JOIN Brigades b ON ws.BrigadeID = b.BrigadeID
                                WHERE co.ObjectID = @ObjectID";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ObjectID", comboBoxObjects.SelectedValue); 
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dataTable;
                            // Изменение заголовков столбцов
                            dataGridView1.Columns["ScheduleID"].HeaderText = "Номер графика";
                            dataGridView1.Columns["ObjectName"].HeaderText = "Название объекта";
                            dataGridView1.Columns["WorkTypeName"].HeaderText = "Вид работ";
                            dataGridView1.Columns["BrigadeName"].HeaderText = "Бригада";
                            dataGridView1.Columns["PlannedStartDate"].HeaderText = "Планируемая дата начала";
                            dataGridView1.Columns["PlannedEndDate"].HeaderText = "Планируемая дата конца";
                        }
                        else
                        {
                            MessageBox.Show("Для выбранного объекта нет данных о графике работ.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
        }
    }    
}
