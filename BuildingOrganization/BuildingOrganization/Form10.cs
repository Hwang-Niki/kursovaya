using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form10 : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\youan\Documents\GitHub\kursovaya\BuildingOrganization\BuildingOrganization\Database1.mdf;Integrated Security=True";
        private DataTable currentTable;
        private SqlDataAdapter adapter;
        private SqlCommandBuilder commandBuilder;

        public Form10()
        {
            InitializeComponent();
            LoadTableNames();
            ConfigureDataGridView();
        }

        private void LoadTableNames()
        {
            // Словарь для соответствия английских и русских названий таблиц
            var tableNames = new System.Collections.Generic.Dictionary<string, string>
            {
                {"Brigades", "Бригады"},
                {"ConstructionEquipment", "Строительная техника"},
                {"ConstructionManagement", "Строительные управления"},
                {"ConstructionMaterials", "Строительные материалы"},
                {"ConstructionObjects", "Объекты строительства"},
                {"ConstructionSites", "Участки строительства"},
                {"Contracts", "Договоры"},
                {"Customers", "Заказчики"},
                {"Employees", "Сотрудники"},
                {"EquipmentTypes", "Типы техники"},
                {"Estimates", "Сметы"},
                {"MaterialPlans", "Планы материалов"},
                {"MaterialUsage", "Использование материалов"},
                {"ObjectType", "Типы объектов"},
                {"Positions", "Должности"},
                {"WorkReports", "Отчеты о работах"},
                {"WorkSchedules", "Графики работ"},
                {"WorkTypes", "Виды работ"}
            };

            comboBox1.DataSource = new BindingSource(tableNames, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }

        private void ConfigureDataGridView()
        {
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string selectedTable = ((System.Collections.Generic.KeyValuePair<string, string>)comboBox1.SelectedItem).Key;
            LoadTableData(selectedTable);
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM {tableName}";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    currentTable = new DataTable();
                    adapter.Fill(currentTable);

                    dataGridView1.DataSource = currentTable;
                    TranslateColumnHeaders(tableName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TranslateColumnHeaders(string tableName)
        {
            // Словарь для перевода названий столбцов
            var columnTranslations = GetColumnTranslations(tableName);

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (columnTranslations.ContainsKey(column.Name))
                {
                    column.HeaderText = columnTranslations[column.Name];
                }
            }
        }

        private System.Collections.Generic.Dictionary<string, string> GetColumnTranslations(string tableName)
        {
            // Здесь нужно добавить переводы для всех столбцов всех таблиц
            var translations = new System.Collections.Generic.Dictionary<string, string>();

            switch (tableName)
            {
                case "Brigades":
                    translations.Add("BrigadeID", "ID бригады");
                    translations.Add("Name", "Название");
                    translations.Add("CreationDate", "Дата создания");
                    translations.Add("ForemanID", "ID бригадира");
                    break;
                case "ConstructionEquipment":
                    translations.Add("EquipmentID", "ID оборудования");
                    translations.Add("Name", "Название");
                    translations.Add("RegistrationNumber", "Регистрационный номер");
                    translations.Add("ManufactureYear", "Год выпуска");
                    translations.Add("Condition", "Состояние");
                    translations.Add("TypeID", "Тип техники");
                    translations.Add("ManagementID", "ID управления");
                    break;
                case "ConstructionManagement":
                    translations.Add("ManagementID", "ID управления");
                    translations.Add("Name", "Название");
                    translations.Add("Address", "Адрес");
                    translations.Add("Phone", "Телефон");
                    translations.Add("CreationDate", "Дата создания");
                    break;

                case "ObjectTypes":
                    translations.Add("TypeID", "ID типа");
                    translations.Add("TypeName", "Тип объекта");
                    translations.Add("Description", "Описание");
                    break;

                case "Customers":
                    translations.Add("CustomerID", "ID заказчика");
                    translations.Add("Name", "Название");
                    translations.Add("Address", "Адрес");
                    translations.Add("Phone", "Телефон");
                    translations.Add("ContactPerson", "Контактное лицо");
                    break;

                case "Contracts":
                    translations.Add("ContractID", "ID договора");
                    translations.Add("ContractNumber", "Номер договора");
                    translations.Add("SignDate", "Дата подписания");
                    translations.Add("CompletionTerm", "Срок выполнения");
                    translations.Add("Amount", "Сумма");
                    translations.Add("CustomerID", "ID заказчика");
                    break;

                case "Positions":
                    translations.Add("PositionID", "ID должности");
                    translations.Add("Title", "Должность");
                    translations.Add("Category", "Категория");
                    translations.Add("Description", "Описание");
                    break;

                case "Employees":
                    translations.Add("EmployeeID", "ID сотрудника");
                    translations.Add("FullName", "ФИО");
                    translations.Add("BirthDate", "Дата рождения");
                    translations.Add("PassportData", "Паспортные данные");
                    translations.Add("Phone", "Телефон");
                    translations.Add("Address", "Адрес");
                    translations.Add("HireDate", "Дата приема");
                    translations.Add("PositionID", "ID должности");
                    translations.Add("Qualification", "Квалификация");
                    translations.Add("Rank", "Разряд");
                    translations.Add("Specialization", "Специализация");
                    translations.Add("Education", "Образование");
                    break;

                case "ConstructionSites":
                    translations.Add("SiteID", "ID участка");
                    translations.Add("Name", "Название");
                    translations.Add("Address", "Адрес");
                    translations.Add("ManagementID", "ID управления");
                    translations.Add("ManagerID", "ID менеджера");
                    break;

                case "EquipmentTypes":
                    translations.Add("TypeID", "ID типа");
                    translations.Add("Name", "Название");
                    translations.Add("Description", "Описание");
                    break;
                case "ConstructionObjects":
                    translations.Add("ObjectID", "ID объекта");
                    translations.Add("Name", "Название");
                    translations.Add("Address", "Адрес");
                    translations.Add("StartDate", "Дата начала");
                    translations.Add("PlannedEndDate", "Планируемая дата завершения");
                    translations.Add("SiteID", "ID участка");
                    translations.Add("ContractID", "ID договора");
                    translations.Add("TypeID", "Тип объекта");
                    translations.Add("FloorsCount", "Количество этажей");
                    translations.Add("MaterialType", "Тип материала");
                    break;

                case "WorkTypes":
                    translations.Add("WorkTypeID", "ID вида работ");
                    translations.Add("Name", "Название");
                    translations.Add("Description", "Описание");
                    translations.Add("TimeNorm", "Норма времени");
                    translations.Add("ObjectTypeID", "Тип объекта");
                    break;

                case "WorkSchedules":
                    translations.Add("ScheduleID", "ID графика");
                    translations.Add("ObjectID", "ID объекта");
                    translations.Add("WorkTypeID", "ID вида работ");
                    translations.Add("PlannedStartDate", "Планируемая дата начала");
                    translations.Add("PlannedEndDate", "Планируемая дата завершения");
                    translations.Add("BrigadeID", "ID бригады");
                    break;

                case "Estimates":
                    translations.Add("EstimateID", "ID сметы");
                    translations.Add("ObjectID", "ID объекта");
                    translations.Add("CreationDate", "Дата создания");
                    translations.Add("ApprovedBy", "Утверждающее лицо (ID)");
                    translations.Add("TotalCost", "Общая стоимость");
                    break;

                case "ConstructionMaterials":
                    translations.Add("MaterialID", "ID материала");
                    translations.Add("Name", "Название");
                    translations.Add("Unit", "Единица измерения");
                    translations.Add("Description", "Описание");
                    break;

                case "MaterialPlans":
                    translations.Add("PlanID", "ID плана");
                    translations.Add("EstimateID", "ID сметы");
                    translations.Add("MaterialID", "ID материала");
                    translations.Add("PlannedQuantity", "Планируемое количество");
                    break;

                case "WorkReports":
                    translations.Add("ReportID", "ID отчета");
                    translations.Add("ScheduleID", "ID графика");
                    translations.Add("ActualStartDate", "Фактическая дата начала");
                    translations.Add("ActualEndDate", "Фактическая дата завершения");
                    translations.Add("CompletionStatus", "Статус выполнения");
                    translations.Add("Notes", "Примечания");
                    break;

                case "MaterialUsage":
                    translations.Add("UsageID", "ID использования");
                    translations.Add("PlanID", "ID плана");
                    translations.Add("MaterialID", "ID материала");
                    translations.Add("ActualQuantity", "Фактическое количество");
                    translations.Add("ReportID", "ID отчета");
                    break;

                case "EquipmentUsage":
                    translations.Add("UsageID", "ID использования");
                    translations.Add("EquipmentID", "ID оборудования");
                    translations.Add("ScheduleID", "ID графика");
                    translations.Add("StartDate", "Дата начала");
                    translations.Add("EndDate", "Дата завершения");
                    break;

                case "Role":
                    translations.Add("Id_role", "ID роли");
                    translations.Add("role", "Роль");
                    break;

                case "Users":
                    translations.Add("UserID", "ID пользователя");
                    translations.Add("LastName", "Фамилия");
                    translations.Add("FirstName", "Имя");
                    translations.Add("MiddleName", "Отчество");
                    translations.Add("Username", "Логин");
                    translations.Add("Password", "Пароль");
                    translations.Add("Id_role", "ID роли");
                    translations.Add("Position", "Должность");
                    translations.Add("LastLogin", "Последний вход");
                    break;
                default:
                    // По умолчанию оставляем оригинальные названия
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        translations.Add(column.Name, column.Name);
                    }
                    break;
            }

            return translations;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (currentTable == null || dataGridView1.CurrentRow == null) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {((System.Collections.Generic.KeyValuePair<string, string>)comboBox1.SelectedItem).Key}", connection);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);

                    adapter.Update(currentTable);
                    MessageBox.Show("Изменения успешно сохранены", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex) when (ex.Number == 547) // Ошибка внешнего ключа
            {
                currentTable.RejectChanges();
                string errorMessage = GetForeignKeyErrorMessage(ex);
                MessageBox.Show($"Ошибка при сохранении данных: {errorMessage}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetForeignKeyErrorMessage(SqlException ex)
        {
            // Анализируем сообщение об ошибке для более понятного вывода
            if (ex.Message.Contains("FOREIGN KEY"))
            {
                if (ex.Message.Contains("INSERT"))
                {
                    return "Нельзя добавить запись: ссылка на несуществующую запись в связанной таблице.";
                }
                else if (ex.Message.Contains("UPDATE"))
                {
                    return "Нельзя обновить запись: указанное значение отсутствует в связанной таблице.";
                }
            }
            return "Операция нарушает ограничения целостности данных. Проверьте, что все ссылки на другие таблицы существуют.";
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (currentTable == null || dataGridView1.CurrentRow == null) return;

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранную запись?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DataRowView rowView = (DataRowView)dataGridView1.CurrentRow.DataBoundItem;
                    DataRow row = rowView.Row;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {((System.Collections.Generic.KeyValuePair<string, string>)comboBox1.SelectedItem).Key}", connection);
                        SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);

                        // Удаляем строку из DataTable
                        row.Delete();

                        // Пытаемся сохранить изменения в БД
                        adapter.Update(currentTable);

                        // Обновляем DataTable
                        currentTable.AcceptChanges();

                        MessageBox.Show("Запись успешно удалена", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    }
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    currentTable.RejectChanges();
                    MessageBox.Show("Нельзя удалить запись, так как она связана с другими данными в системе.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}