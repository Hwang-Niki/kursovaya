using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildingOrganization
{
    public partial class Form3 : Form
    {
        private Form4.Users currentUser;
        private List<Button> activeButtons = new List<Button>();

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
            ArrangeButtons();
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

            var allButtons = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button9, button10 };

            // Сначала скрываем все кнопки
            foreach (var btn in allButtons)
            {
                btn.Visible = false;
            }

            // Настройка доступов в зависимости от роли
            switch (currentUser.Id_role)
            {
                case 1: // Администратор
                    activeButtons.AddRange(allButtons);
                    break;

                case 2: // Начальник участка
                    activeButtons.AddRange(new[] { button1, button2, button3, button4, button5, button6 });
                    break;

                case 3: // Прораб
                    activeButtons.AddRange(new[] { button1, button2, button3, button4, button5, button6 });
                    break;

                case 4: // Бригадир
                    activeButtons.AddRange(new[] { button1, button2, button4, button5, button6 });
                    break;

                case 5: // Пользователь
                    activeButtons.AddRange(new[] { button1, button2, button5 });
                    break;
            }
            foreach (var btn in activeButtons)
            {
                btn.Visible = true;
            }

            // Кнопка выхода всегда видна
            button8.Visible = true;

            // Отображаем информацию о пользователе
            lblUserInfo.Text = $"{currentUser.LastName} {currentUser.FirstName} {currentUser.MiddleName}\n {currentUser.Position}";
        }

        private void ArrangeButtons()
        {
            const int buttonWidth = 170;
            const int buttonHeight = 60;
            const int verticalMargin = 10;
            const int horizontalMargin = 40;
            const int startX = 20;
            const int startY = 80;
            const int formPadding = 40;

            // Определяем количество колонок (максимум 2)
            int columns = (activeButtons.Count > 4) ? 2 : 1;

            // Рассчитываем количество строк
            int rows = (int)Math.Ceiling((double)activeButtons.Count / columns);

            // Располагаем активные кнопки
            for (int i = 0; i < activeButtons.Count; i++)
            {
                int col = i % columns;
                int row = i / columns;

                activeButtons[i].Location = new Point(
                    startX + col * (buttonWidth + horizontalMargin),
                    startY + row * (buttonHeight + verticalMargin)
                );
                activeButtons[i].Size = new Size(buttonWidth, buttonHeight);
            }

            int button8X = startX + buttonWidth + horizontalMargin / 2 - button8.Width / 2;
            int button8Y = startY + rows * (buttonHeight + verticalMargin) + verticalMargin;

            button8.Location = new Point(button8X, button8Y);

            // Рассчитываем новую высоту формы
            int newHeight = button8Y + button8.Height + formPadding;

            // Рассчитываем новую ширину формы (оставляем фиксированную или тоже динамическую)
            int newWidth = this.Width; // Можно оставить как есть или рассчитать

            // Устанавливаем новый размер формы
            this.ClientSize = new Size(newWidth, newHeight);

            // Центрируем форму на экране (опционально)
            this.CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form frm1 = new Form1();
            frm1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form frm2 = new Form2();
            frm2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form frm5 = new Form5();
            frm5.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form frm7 = new Form7();
            frm7.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form frm8 = new Form8();
            frm8.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form frm9 = new Form9();
            frm9.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form frm10 = new Form10();
            frm10.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form4.currentUser = null;
            Close();
            Form frm4 = new Form4();
            frm4.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form frm6 = new Form6();
            frm6.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form frm11 = new Form11();
            frm11.Show();
        }
    }
}
