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
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
            LoadWorkTypes();
        }

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
            {
                string query = @"
        SELECT 
            co.ObjectID AS [Идентификатор объекта],
            co.Name AS [Объект],
            co.Address AS [Адрес],
            co.StartDate AS [Дата начала],
            co.PlannedEndDate AS [Планируемая дата окончания],
            cs.Name AS [Участок],
            c.ContractNumber AS [Контракт],
            ot.TypeName AS [Тип объекта],
            co.FloorsCount AS [Этажность],
            co.MaterialType AS [Тип материала]
        FROM 
            ConstructionObjects co
        JOIN 
            ConstructionSites cs ON co.SiteID = cs.SiteID
        JOIN 
            ObjectType ot ON co.TypeID = ot.TypeID
        JOIN 
            Contracts c ON co.ContractID = c.ContractID;"; 

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void LoadWorkTypes()
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
            {
                string query = "SELECT WorkTypeID, Name FROM WorkTypes"; 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "Name"; 
                    comboBox1.ValueMember = "WorkTypeID"; 
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                int workTypeId;
                if (int.TryParse(comboBox1.SelectedValue.ToString(), out workTypeId))
                {
                    DateTime startDate = dateTimePicker1.Value;
                    DateTime endDate = dateTimePicker2.Value;

                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                    {
                        string query = @"
                SELECT 
                    co.ObjectID AS [Идентификатор объекта],
                    co.Name AS [Объект],
                    co.Address AS [Адрес],
                    co.StartDate AS [Дата начала],
                    co.PlannedEndDate AS [Планируемая дата окончания],
                    cs.Name AS [Участок],
                    c.ContractNumber AS [Контракт],
                    ot.TypeName AS [Тип объекта],
                    co.FloorsCount AS [Этажность],
                    co.MaterialType AS [Тип материала]
                FROM 
                    ConstructionObjects co
                JOIN 
                    ConstructionSites cs ON co.SiteID = cs.SiteID
                JOIN 
                    ObjectType ot ON co.TypeID = ot.TypeID
                JOIN 
                    Contracts c ON co.ContractID = c.ContractID
                WHERE 
                    co.ObjectID IN (
                        SELECT ObjectID FROM WorkSchedules WHERE WorkTypeID=@WorkType AND PlannedStartDate BETWEEN @StartDate AND @EndDate)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@WorkType", workTypeId);
                            command.Parameters.AddWithValue("@StartDate", startDate);
                            command.Parameters.AddWithValue("@EndDate", endDate);

                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dataGridView1.DataSource = dt;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите корректный вид работ.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите вид работ.");
            }
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.Database1ConnectionString))
                {
                    connection.Open(); // Открываем соединение
                    string query = @"
        SELECT 
            co.ObjectID AS [Идентификатор объекта],
            co.Name AS [Объект],
            co.Address AS [Адрес],
            co.StartDate AS [Дата начала],
            co.PlannedEndDate AS [Планируемая дата окончания],
            cs.Name AS [Участок],
            c.ContractNumber AS [Контракт],
            ot.TypeName AS [Тип объекта],
            co.FloorsCount AS [Этажность],
            co.MaterialType AS [Тип материала]
        FROM 
            ConstructionObjects co
        JOIN 
            ConstructionSites cs ON co.SiteID = cs.SiteID
        JOIN 
            ObjectType ot ON co.TypeID = ot.TypeID
        JOIN 
            Contracts c ON co.ContractID = c.ContractID;";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
