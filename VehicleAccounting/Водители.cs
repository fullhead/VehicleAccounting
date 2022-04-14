using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using Tulpep.NotificationWindow;
using System.IO;

namespace VehicleAccounting
{
    public partial class Водители : Form
    {
        private SqlConnection sqlConnection = null;
        private PopupNotifier popup = null;
        private SqlDataAdapter adapter = null;
        private DataTable table = null;
        public Водители()
        {
            InitializeComponent();
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void Водители_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Водители". При необходимости она может быть перемещена или удалена.
            this.водителиTableAdapter.Fill(this.vehicleAccountingDataSet.Водители);
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Водители", sqlConnection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;

            if (sqlConnection.State == ConnectionState.Open)
                pictureBox1.Image = Properties.Resources.connected;

            else
                pictureBox1.Image = Properties.Resources.disconnect;
        }

        private void Водители_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        //REFRESH DB
        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Водители", sqlConnection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
        }

        //SEARCH
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                adapter = new SqlDataAdapter("SELECT * from Водители where ФИО like'%" + textBox1.Text + "%'", sqlConnection);
                table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        //UPDATE
        private async void Button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox3.Text == "" || this.фИОTextBox.Text == "" || this.возрастTextBox.Text == "" || this.comboBox2.Text == "")
            {
                label1.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Водители Where Код_водителя ='" + comboBox3.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("UPDATE Водители SET ФИО=@ФИО, Возраст=@Возраст, Пол=@Пол, Адрес=@Адрес," +
                    " Телефон=@Телефон, Паспортные_данные=@Паспортные_данные, Стаж_работы=@Стаж_работы, Примечание=@Примечание WHERE Код_водителя=@Код_водителя", sqlConnection);
                    command.Parameters.AddWithValue("Код_водителя", comboBox3.Text);
                    command.Parameters.AddWithValue("ФИО", фИОTextBox.Text);
                    command.Parameters.AddWithValue("Возраст", возрастTextBox.Text);
                    command.Parameters.AddWithValue("Пол", comboBox2.Text);
                    command.Parameters.AddWithValue("Адрес", адресTextBox.Text);
                    command.Parameters.AddWithValue("Телефон", телефонTextBox.Text);
                    command.Parameters.AddWithValue("Паспортные_данные", паспортные_данныеTextBox.Text);
                    command.Parameters.AddWithValue("Стаж_работы", стаж_работыTextBox.Text);
                    command.Parameters.AddWithValue("Примечание", примечаниеTextBox.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Водители",
                        ContentText = "Данные успешно обновлены!"
                    };
                    popup.Popup();
                    await command.ExecuteNonQueryAsync();
                }
                else
                {
                    label7.Show();
                }
            }
        }

        //Conditions Texboxes for UPDATE
        private void ФИОTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char l = e.KeyChar;
            if ((l < 'А' || l > 'я') && l != '\b' && l != '.')
            {
                e.Handled = true;
            }
        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void ВозрастTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Panel4_MouseMove(object sender, MouseEventArgs e)
        {
            label7.Hide();
            label1.Hide();
        }

        //INSERT
        private async void Button2_Click(object sender, EventArgs e)
        {
            if (this.фИОTextBox1.Text == "" || this.возрастTextBox1.Text == "" || this.comboBox1.Text == "")
            {
                label4.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Водители (ФИО, Возраст, Пол, Адрес, Телефон, Паспортные_данные, Стаж_работы, Примечание) VALUES (@ФИО, @Возраст, @Пол, @Адрес, @Телефон, @Паспортные_данные, @Стаж_работы, @Примечание)", sqlConnection);
                command.Parameters.AddWithValue("ФИО", фИОTextBox1.Text);
                command.Parameters.AddWithValue("Возраст", возрастTextBox1.Text);
                command.Parameters.AddWithValue("Пол", comboBox1.Text);
                command.Parameters.AddWithValue("Адрес", адресTextBox1.Text);
                command.Parameters.AddWithValue("Телефон", телефонTextBox1.Text);
                command.Parameters.AddWithValue("Паспортные_данные", паспортные_данныеTextBox1.Text);
                command.Parameters.AddWithValue("Стаж_работы", стаж_работыTextBox1.Text);
                command.Parameters.AddWithValue("Примечание", примечаниеTextBox1.Text);
                popup = new PopupNotifier
                {
                    Image = Properties.Resources.connected,
                    ImageSize = new Size(96, 96),
                    TitleText = "Водители",
                    ContentText = "Данные успешно добавлены!"
                };
                popup.Popup();
                await command.ExecuteNonQueryAsync();
            }
        }

        //DELETE
        private async void Button3_Click(object sender, EventArgs e)
        {
            if (this.comboBox4.Text == "")
            {
                label5.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Водители Where Код_водителя ='" + comboBox4.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Водители WHERE Код_водителя=@Код_водителя", sqlConnection);
                    command.Parameters.AddWithValue("Код_водителя", comboBox4.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Водители",
                        ContentText = "Данные успешно удалены!"
                    };
                    popup.Popup();
                    await command.ExecuteNonQueryAsync();

                }
                else
                {
                    label6.Show();
                } 
            }
        }

        //SAVE TO .CVF
        private void СохранитьКакCVFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dt = new DataTable();
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM Водители", sqlConnection);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
            string path = "";
            using (var path_dialog = new FolderBrowserDialog())
                if (path_dialog.ShowDialog() == DialogResult.OK)
                {
                    //Путь к директории
                    path = path_dialog.SelectedPath;
                };
            sqlConnection.Close();
            ToCSV(dt, path + @"\" + @"Отчёт_документы.csv");
        }
        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(";");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(';'))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(";");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        //PRINT
        private void ПечатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bmp = new Bitmap(dataGridView1.Size.Width + 10, dataGridView1.Size.Height + 10);
            dataGridView1.DrawToBitmap(bmp, dataGridView1.Bounds);
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        //Conditions Texboxes for INSERT
        private void ФИОTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char l = e.KeyChar;
            if ((l < 'А' || l > 'я') && l != '\b' && l != '.')
            {
                e.Handled = true;
            }
        }
        private void ВозрастTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Panel5_MouseMove(object sender, MouseEventArgs e)
        {
            label4.Hide();
        }
        //Conditions Texboxes for DELETE
        private void Код_водителяTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Panel6_MouseMove(object sender, MouseEventArgs e)
        {
            label6.Hide();
            label5.Hide();

        }

        //OTHER DB's FORMS
        private void ТранспортныеСредстваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Транспортные_средства транспортные_средства = new Транспортные_средства();
            транспортные_средства.Show();
        }

        private void ТехническоеОбслToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Техническое_обслуживание техническое_обслуживание = new Техническое_обслуживание();
            техническое_обслуживание.Show();
        }

        private void ДТПToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ДТП дтп = new ДТП();
            дтп.Show();
        }

        private void ЗапчастиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Запчасти запчасти = new Запчасти();
            запчасти.Show();
        }

        private void ПользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.Show();
        }

        private void ОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            О_программе о_программе = new О_программе();
            о_программе.Show();
        }
    }
}
