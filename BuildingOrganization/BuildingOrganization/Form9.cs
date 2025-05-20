using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form9 : Form
    {
        private string connectionString = Properties.Settings.Default.Database1ConnectionString;

        public Form9()
        {
            InitializeComponent();
            LoadConstructionSites();
            LoadConstructionManagements();
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadConstructionSites()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT SiteID, Name FROM ConstructionSites", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(new { Value = reader["SiteID"], Text = reader["Name"].ToString() });
                }
                reader.Close();
            }
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
        }

        private void LoadConstructionManagements()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT ManagementID, Name FROM ConstructionManagement", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox2.Items.Add(new { Value = reader["ManagementID"], Text = reader["Name"].ToString() });
                }
                reader.Close();
            }
            comboBox2.DisplayMember = "Text";
            comboBox2.ValueMember = "Value";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите участок.");
                return;
            }

            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите управление.");
                return;
            }

            var selectedSiteId = ((dynamic)comboBox1.SelectedItem).Value;
            var selectedManagementId = ((dynamic)comboBox2.SelectedItem).Value;

            string query = @"
    SELECT DISTINCT wt.WorkTypeID AS [Номер типа работ], 
                    wt.Name AS [Наименование], 
                    wt.Description AS [Описание], 
                    wt.TimeNorm AS [Норматив времени], 
                    ot.TypeName AS [Тип объекта]
    FROM 
        WorkSchedules ws
    JOIN 
        WorkReports wr ON ws.ScheduleID = wr.ScheduleID
    JOIN 
        ConstructionObjects wo ON ws.ObjectID = wo.ObjectID
    JOIN 
        WorkTypes wt ON ws.WorkTypeID = wt.WorkTypeID
    JOIN 
        ConstructionSites cs ON wo.SiteID = cs.SiteID
    JOIN 
        ConstructionManagement cm ON cs.ManagementID = cm.ManagementID
    JOIN 
        ObjectType ot ON wo.TypeID = ot.TypeID
    WHERE 
        wr.ActualEndDate IS NOT NULL AND 
        wr.ActualEndDate > ws.PlannedEndDate AND
        cs.SiteID = @SiteID AND
        cm.ManagementID = @ManagementID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SiteID", selectedSiteId);
                command.Parameters.AddWithValue("@ManagementID", selectedManagementId);

                DataTable dtResults = new DataTable();

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtResults);

                    if (dtResults.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет данных для отображения.");
                        dataGridView1.DataSource = null;
                    }
                    else
                    {
                        dataGridView1.DataSource = dtResults;
                        dataGridView1.Columns["Номер типа работ"].HeaderText = "Номер типа работ";
                        dataGridView1.Columns["Наименование"].HeaderText = "Наименование";
                        dataGridView1.Columns["Описание"].HeaderText = "Описание";
                        dataGridView1.Columns["Норматив времени"].HeaderText = "Норматив времени";
                        dataGridView1.Columns["Тип объекта"].HeaderText = "Тип объекта";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }



    private void LoadData()
        {

            string query = @"
    SELECT DISTINCT wt.WorkTypeID AS [Номер типа работ], 
                    wt.Name AS [Наименование], 
                    wt.Description AS [Описание], 
                    wt.TimeNorm AS [Норматив времени], 
                    ot.TypeName AS [Тип объекта]
    FROM 
        WorkSchedules ws
    JOIN 
        WorkReports wr ON ws.ScheduleID = wr.ScheduleID
    JOIN 
        ConstructionObjects wo ON ws.ObjectID = wo.ObjectID
    JOIN 
        WorkTypes wt ON ws.WorkTypeID = wt.WorkTypeID
    JOIN 
        ConstructionSites cs ON wo.SiteID = cs.SiteID
    JOIN 
        ConstructionManagement cm ON cs.ManagementID = cm.ManagementID
    JOIN 
        ObjectType ot ON wo.TypeID = ot.TypeID
    WHERE 
        wr.ActualEndDate IS NOT NULL;"; 

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
                        dataGridView1.DataSource = null;
                    }
                    else
                    {
                        dataGridView1.DataSource = dtResults;
                     
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = @"
    SELECT DISTINCT wt.WorkTypeID AS [Номер типа работ], 
                    wt.Name AS [Наименование], 
                    wt.Description AS [Описание], 
                    wt.TimeNorm AS [Норматив времени], 
                    ot.TypeName AS [Тип объекта]
    FROM 
        WorkSchedules ws
    JOIN 
        WorkReports wr ON ws.ScheduleID = wr.ScheduleID
    JOIN 
        ConstructionObjects wo ON ws.ObjectID = wo.ObjectID
    JOIN 
        WorkTypes wt ON ws.WorkTypeID = wt.WorkTypeID
    JOIN 
        ConstructionSites cs ON wo.SiteID = cs.SiteID
    JOIN 
        ConstructionManagement cm ON cs.ManagementID = cm.ManagementID
    JOIN 
        ObjectType ot ON wo.TypeID = ot.TypeID
    WHERE 
        wr.ActualEndDate IS NOT NULL;";

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
                        dataGridView1.DataSource = null;
                    }
                    else
                    {
                        dataGridView1.DataSource = dtResults;

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
    