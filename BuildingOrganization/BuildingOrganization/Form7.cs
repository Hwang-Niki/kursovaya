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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            LoadReportIDs();
            LoadData();

        }

        private void LoadReportIDs()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ReportID FROM WorkReports ORDER BY ReportID";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    comboBoxReports.DataSource = dataTable;
                    comboBoxReports.DisplayMember = "ReportID";
                    comboBoxReports.ValueMember = "ReportID";
                    comboBoxReports.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке номеров отчетов: " + ex.Message);
            }
        }

        private void LoadData(int? reportID = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open(); // Открываем соединение

                    string query = @"
                        SELECT wr.ReportID AS [Номер отчета], 
                               co.Name AS [Объект], 
                               wr.ActualStartDate AS [Фактическая дата начала], 
                               wr.ActualEndDate AS [Фактическая дата конца], 
                               wr.CompletionStatus AS [Статус], 
                               wr.Notes AS [Описание]
                        FROM WorkReports wr
                        JOIN WorkSchedules ws ON wr.ScheduleID = ws.ScheduleID
                        JOIN ConstructionObjects co ON ws.ObjectID = co.ObjectID";


                    // Если передан ID отчета, добавляем условие WHERE
                    if (reportID.HasValue)
                    {
                        query += " WHERE wr.ReportID = @ReportID";
                    }

                    SqlCommand command = new SqlCommand(query, connection);

                    // Если передан ID отчета, добавляем параметр
                    if (reportID.HasValue)
                    {
                        command.Parameters.AddWithValue("@ReportID", reportID.Value);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Columns["Описание"].Width = 300;
                    }
                    else if (reportID.HasValue)
                    {
                        MessageBox.Show("Нет отчета с указанным ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxReports.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите номер отчета из списка");
                return;
            }

            int selectedReportID = (int)comboBoxReports.SelectedValue;
            LoadData(selectedReportID);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadData();
            comboBoxReports.SelectedIndex = -1;
        }
    }
}
