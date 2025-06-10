using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form3 : Form
    {
        private Form4.Users currentUser;
 

        public Form3(Form4.Users user)
        {
            InitializeComponent();
            if (user == null)
            {
                MessageBox.Show("Ошибка авторизации. Пожалуйста, войдите снова.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            currentUser = user;
            ConfigureAccess();
        
        }

        private void ConfigureAccess()
        {
            if (currentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            управлениеПользователямиToolStripMenuItem.Visible = false;
            регистрацияПользователейToolStripMenuItem.Visible = false;
            просмотрИРедактированиеТаблицToolStripMenuItem.Visible = false;
            запросыToolStripMenuItem.Visible=false;

            // Настройка доступов в зависимости от роли
            switch (currentUser.Id_role)
            {
                case 1: // Администратор
                    управлениеПользователямиToolStripMenuItem.Visible = true;
                    регистрацияПользователейToolStripMenuItem.Visible = true;
                    break;

                case 2: // Начальник участка
                    просмотрИРедактированиеТаблицToolStripMenuItem.Visible = true;
                    запросыToolStripMenuItem.Visible = true;
                    break;

                case 3: // Прораб
                    запросыToolStripMenuItem.Visible = true;
                    break;

                case 4: // Бригадир
                    запросыToolStripMenuItem.Visible = true;
                    сметаНаСтроительствоToolStripMenuItem.Visible = false;
                    отчетОСооруженииОбъектаToolStripMenuItem.Visible = false;
                    break;

            }
         
            // Отображаем информацию о пользователе
            lblUserInfo.Text = $"{currentUser.LastName} {currentUser.FirstName} {currentUser.MiddleName}\n {currentUser.Position}";
        }
   

        private void button8_Click(object sender, EventArgs e)
        {
            Form4.currentUser = null;
            Close();
            Form frm4 = new Form4();
            frm4.Show();
        }


        private void просмотрИРедактированиеТаблицToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm10 = new Form10();
            frm10.Show();
        }

        private void графикРаботToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm2 = new Form2();
            frm2.Show();
        }

        private void переченьСтроительнойТехникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm1 = new Form1();
            frm1.Show();
        }

        private void сметаНаСтроительствоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm5 = new Form5();
            frm5.Show();
        }

        private void управлениеПользователямиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm11 = new Form11();
            frm11.Show();
        }

        private void регистрацияПользователейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm6 = new Form6();
            frm6.Show();
        }

        private void отчетОСооруженииОбъектаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm7 = new Form7();
            frm7.Show();
        }

        private void переченьОбъектовПоВидамРаботToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm8 = new Form8();
            frm8.Show();
        }

        private void переченьВидовРаботСПревышениемСроковВыполненияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm9 = new Form9();
            frm9.Show();
        }
    }
}
