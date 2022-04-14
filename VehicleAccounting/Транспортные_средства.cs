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
    public partial class Транспортные_средства : Form
    {
        private SqlConnection sqlConnection = null;
        private PopupNotifier popup = null;
        private SqlDataAdapter adapter = null;
        private DataTable table = null;
        public Транспортные_средства()
        {
            InitializeComponent();
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }
        private void Транспортные_средства_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void Транспортные_средства_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Водители". При необходимости она может быть перемещена или удалена.
            this.водителиTableAdapter.Fill(this.vehicleAccountingDataSet.Водители);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Транспортные_средства". При необходимости она может быть перемещена или удалена.
            this.транспортные_средстваTableAdapter.Fill(this.vehicleAccountingDataSet.Транспортные_средства);
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Транспортные_средства", sqlConnection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;

            if (sqlConnection.State == ConnectionState.Open)
                pictureBox1.Image = Properties.Resources.connected;

            else
                pictureBox1.Image = Properties.Resources.disconnect;
        }

        //REFRESH DB
        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Транспортные_средства", sqlConnection);
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
                adapter = new SqlDataAdapter("SELECT * from Транспортные_средства where Марка like'%" + textBox1.Text + "%'", sqlConnection);
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
            if (this.comboBox1.Text == "" || this.comboBox4.Text == "" || this.табельный_номерTextBox.Text == "" || this.маркаTextBox.Text == "" || this.модельTextBox.Text == "")
            {
                label1.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Транспортные_средства Where Код_ТС ='" + comboBox1.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlDataAdapter Tablet1 = new SqlDataAdapter("Select Count (*) Login From Водители Where Код_водителя ='" + comboBox4.Text + "'", sqlConnection);
                    DataTable dt1 = new DataTable();
                    Tablet1.Fill(dt1);
                    if (dt1.Rows[0][0].ToString() == "1")
                    {
                        sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand("UPDATE Транспортные_средства SET Код_водителя=@Код_водителя, Табельный_номер=@Табельный_номер, Марка=@Марка," +
                        " Модель=@Модель, Пробег=@Пробег, Примечание=@Примечание WHERE Код_ТС=@Код_ТС", sqlConnection);
                        command.Parameters.AddWithValue("Код_ТС", comboBox1.Text);
                        command.Parameters.AddWithValue("Код_водителя", comboBox4.Text);
                        command.Parameters.AddWithValue("Табельный_номер", табельный_номерTextBox.Text);
                        command.Parameters.AddWithValue("Марка", маркаTextBox.Text);
                        command.Parameters.AddWithValue("Модель", модельTextBox.Text);
                        command.Parameters.AddWithValue("Пробег", пробегTextBox.Text);
                        command.Parameters.AddWithValue("Примечание", примечаниеTextBox.Text);
                        popup = new PopupNotifier
                        {
                            Image = Properties.Resources.connected,
                            ImageSize = new Size(96, 96),
                            TitleText = "Транспортные средства",
                            ContentText = "Данные успешно обновлены!"
                        };
                        popup.Popup();
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        label9.Show();
                    }
                }
                else
                {
                    label7.Show();
                }
            }
        }
        
    //Conditions Texboxes for UPDATE
    private void Panel4_MouseMove_1(object sender, MouseEventArgs e)
        {
            label1.Hide();
            label7.Hide();
            label9.Hide();
        }
        private void ComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void ComboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Табельный_номерTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        //INSERT
        private async void Button2_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text == "" || this.табельный_номерTextBox1.Text == "" || this.маркаTextBox1.Text == "" || this.модельTextBox1.Text == "")
            {
                label4.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Водители Where Код_водителя ='" + comboBox2.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO Транспортные_средства (Код_водителя, Табельный_номер, Марка, Модель, Пробег, Примечание) VALUES (@Код_водителя, @Табельный_номер, @Марка, @Модель, @Пробег, @Примечание)", sqlConnection);
                    command.Parameters.AddWithValue("Код_водителя", comboBox2.Text);
                    command.Parameters.AddWithValue("Табельный_номер", табельный_номерTextBox1.Text);
                    command.Parameters.AddWithValue("Марка", маркаTextBox1.Text);
                    command.Parameters.AddWithValue("Модель", модельTextBox1.Text);
                    command.Parameters.AddWithValue("Пробег", пробегTextBox1.Text);
                    command.Parameters.AddWithValue("Примечание", примечаниеTextBox1.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Транспортные средства",
                        ContentText = "Данные успешно добавлены!"
                    };
                    popup.Popup();
                    await command.ExecuteNonQueryAsync();
                }
                else
                {
                    label8.Show();
                }
                    
            }
        }
        //Conditions Texboxes for INSERT
        private void Panel5_MouseMove_1(object sender, MouseEventArgs e)
        {
            label4.Hide();
            label8.Hide();
        }
        private void ComboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Табельный_номерTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        //DELETE
        private async void Button3_Click_1(object sender, EventArgs e)
        {
            if (this.comboBox3.Text == "")
            {
                label5.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Транспортные_средства Where Код_ТС ='" + comboBox3.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Транспортные_средства WHERE Код_ТС=@Код_ТС", sqlConnection);
                    command.Parameters.AddWithValue("Код_ТС", comboBox3.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Транспортные средства",
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
        //Conditions Texboxes for DELETE
        private void Panel6_MouseMove_1(object sender, MouseEventArgs e)
        {
            label5.Hide();
            label6.Hide();
        }
        private void ComboBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }

        }

        //SAVE TO .CVF
        private void СохранитьКакCVFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dt = new DataTable();
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM Транспортные_средства", sqlConnection);
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

        private void ВодителиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Водители водители = new Водители();
            водители.Show();
        }

        private void ПользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.Show();
        }

        private void ЗапчастиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Запчасти запчасти = new Запчасти();
            запчасти.Show();
        }

        private void ДТПToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            ДТП дтп = new ДТП();
            дтп.Show();
        }

        private void ТехническоеОбслToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Техническое_обслуживание техническое_обслуживание = new Техническое_обслуживание();
            техническое_обслуживание.Show();
        }

        private void ОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            О_программе о_программе = new О_программе();
            о_программе.Show();
        }
    }
    
}
