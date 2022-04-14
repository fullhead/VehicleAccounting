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
    public partial class ДТП : Form
    {
        private SqlConnection sqlConnection = null;
        private PopupNotifier popup = null;
        private SqlDataAdapter adapter = null;
        private DataTable table = null;
        public ДТП()
        {
            InitializeComponent();
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void ДТП_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void ДТП_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Транспортные_средства". При необходимости она может быть перемещена или удалена.
            this.транспортные_средстваTableAdapter.Fill(this.vehicleAccountingDataSet.Транспортные_средства);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Водители". При необходимости она может быть перемещена или удалена.
            this.водителиTableAdapter.Fill(this.vehicleAccountingDataSet.Водители);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.ДТП". При необходимости она может быть перемещена или удалена.
            this.дТПTableAdapter.Fill(this.vehicleAccountingDataSet.ДТП);
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM ДТП", sqlConnection);
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
            adapter = new SqlDataAdapter("SELECT * FROM ДТП", sqlConnection);
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
                adapter = new SqlDataAdapter("SELECT * from ДТП where Дата like'%" + textBox1.Text + "%'", sqlConnection);
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

        private async void Button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text == "" || this.comboBox2.Text == "" || this.comboBox3.Text == "" || this.вид_ДТПTextBox.Text == "")
            {
                label1.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From ДТП Where Код_ДТП ='" + comboBox1.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlDataAdapter Tablet1 = new SqlDataAdapter("Select Count (*) Login From Водители Where Код_водителя ='" + comboBox2.Text + "'", sqlConnection);
                    DataTable dt1 = new DataTable();
                    Tablet1.Fill(dt1);
                    if (dt1.Rows[0][0].ToString() == "1")
                    {
                        sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                        sqlConnection.Open();
                        SqlDataAdapter Tablet2 = new SqlDataAdapter("Select Count (*) Login From Транспортные_средства Where Код_ТС ='" + comboBox3.Text + "'", sqlConnection);
                        DataTable dt2 = new DataTable();
                        Tablet2.Fill(dt2);
                        if (dt2.Rows[0][0].ToString() == "1")
                        {
                            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                            sqlConnection.Open();
                            SqlCommand command = new SqlCommand("UPDATE ДТП SET Код_водителя=@Код_водителя, Код_ТС=@Код_ТС, Вид_ДТП=@Вид_ДТП, Причина=@Причина," +
                            " Штраф=@Штраф, Место_ДТП=@Место_ДТП, Дата=@Дата, Время=@Время, Примечание=@Примечание WHERE Код_ДТП=@Код_ДТП", sqlConnection);
                            command.Parameters.AddWithValue("Код_ДТП", comboBox1.Text);
                            command.Parameters.AddWithValue("Код_водителя", comboBox2.Text);
                            command.Parameters.AddWithValue("Код_ТС", comboBox3.Text);
                            command.Parameters.AddWithValue("Вид_ДТП", вид_ДТПTextBox.Text);
                            command.Parameters.AddWithValue("Причина", причинаTextBox.Text);
                            command.Parameters.AddWithValue("Штраф", штрафTextBox.Text);
                            command.Parameters.AddWithValue("Место_ДТП", место_ДТПTextBox.Text);
                            command.Parameters.AddWithValue("Дата", датаTextBox.Text);
                            command.Parameters.AddWithValue("Время", времяTextBox.Text);
                            command.Parameters.AddWithValue("Примечание", примечаниеTextBox.Text);
                            popup = new PopupNotifier
                            {
                                Image = Properties.Resources.connected,
                                ImageSize = new Size(96, 96),
                                TitleText = "ДТП",
                                ContentText = "Данные успешно обновлены!"
                            };
                            popup.Popup();
                            await command.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            label11.Show();
                        }
                    }
                    else
                    {
                        label10.Show();
                    }
                }
                else
                {
                    label7.Show();
                }
            }
        }

        //Conditions Texboxes for UPDATE
        private void Panel4_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Hide();
            label7.Hide();
            label10.Hide();
            label11.Hide();
        }

        private void ComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        private void ComboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        private void ComboBox3_KeyPress(object sender, KeyPressEventArgs e)
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
            if (this.comboBox4.Text == "" || this.comboBox5.Text == "" || this.вид_ДТПTextBox1.Text == "")
            {
                label4.Show();
            }
            else
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
                    SqlDataAdapter Tablet2 = new SqlDataAdapter("Select Count (*) Login From Транспортные_средства Where Код_ТС ='" + comboBox5.Text + "'", sqlConnection);
                    DataTable dt2 = new DataTable();
                    Tablet2.Fill(dt2);
                    if (dt2.Rows[0][0].ToString() == "1")
                    {
                        sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand("INSERT INTO ДТП (Код_водителя, Код_ТС, Вид_ДТП, Причина, Штраф, Место_ДТП, Дата, Время, Примечание) VALUES (@Код_водителя, @Код_ТС, @Вид_ДТП, @Причина, @Штраф, @Место_ДТП, @Дата, @Время, @Примечание)", sqlConnection);
                        command.Parameters.AddWithValue("Код_водителя", comboBox4.Text);
                        command.Parameters.AddWithValue("Код_ТС", comboBox5.Text);
                        command.Parameters.AddWithValue("Вид_ДТП", вид_ДТПTextBox1.Text);
                        command.Parameters.AddWithValue("Причина", причинаTextBox1.Text);
                        command.Parameters.AddWithValue("Штраф", штрафTextBox1.Text);
                        command.Parameters.AddWithValue("Место_ДТП", место_ДТПTextBox1.Text);
                        command.Parameters.AddWithValue("Дата", датаTextBox1.Text);
                        command.Parameters.AddWithValue("Время", времяTextBox1.Text);
                        command.Parameters.AddWithValue("Примечание", примечаниеTextBox1.Text);
                        popup = new PopupNotifier
                        {
                            Image = Properties.Resources.connected,
                            ImageSize = new Size(96, 96),
                            TitleText = "ДТП",
                            ContentText = "Данные успешно обновлены!"
                        };
                        popup.Popup();
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        label8.Show();
                    }
                }
                else
                {
                    label9.Show();
                }

            }
        }

        //Conditions Texboxes for INSERT
        private void Panel5_MouseMove(object sender, MouseEventArgs e)
        {
            label4.Hide();
            label8.Hide();
            label9.Hide();
        }

        private void ComboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        private void ComboBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        //DELETE
        private async void Button3_Click(object sender, EventArgs e)
        {
            if (this.comboBox6.Text == "")
            {
                label5.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From ДТП Where Код_ДТП ='" + comboBox6.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM ДТП WHERE Код_ДТП=@Код_ДТП", sqlConnection);
                    command.Parameters.AddWithValue("Код_ДТП", comboBox6.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "ДТП",
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
        private void Panel6_MouseMove(object sender, MouseEventArgs e)
        {
            label5.Hide();
            label6.Hide();
        }

        private void ComboBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
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
                adapter = new SqlDataAdapter("SELECT * FROM ДТП", sqlConnection);
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
        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bmp = new Bitmap(dataGridView1.Size.Width + 10, dataGridView1.Size.Height + 10);
            dataGridView1.DrawToBitmap(bmp, dataGridView1.Bounds);
            e.Graphics.DrawImage(bmp, 0, 0);
        }

        private void ПечатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }


        //OTHER DB's FORMS
        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.Show();
        }

        private void водителиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Водители водители = new Водители();
            водители.Show();
        }

        private void транспортныеСредстваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Транспортные_средства транспортные_средства = new Транспортные_средства();
            транспортные_средства.Show();
        }

        private void запчастиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Запчасти запчасти = new Запчасти();
            запчасти.Show();
        }

        private void техническоеОбслуживаниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Техническое_обслуживание техническое_обслуживание = new Техническое_обслуживание();
            техническое_обслуживание.Show();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            О_программе о_программе = new О_программе();
            о_программе.Show();
        }
    }
}
