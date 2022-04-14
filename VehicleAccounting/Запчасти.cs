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
    public partial class Запчасти : Form
    {
        private SqlConnection sqlConnection = null;
        private PopupNotifier popup = null;
        private SqlDataAdapter adapter = null;
        private DataTable table = null;
        public Запчасти()
        {
            InitializeComponent();
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }
        private void Запчасти_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Запчасти_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "vehicleAccountingDataSet.Запчасти". При необходимости она может быть перемещена или удалена.
            this.запчастиTableAdapter.Fill(this.vehicleAccountingDataSet.Запчасти);
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
            sqlConnection.Open();
            adapter = new SqlDataAdapter("SELECT * FROM Запчасти", sqlConnection);
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
            adapter = new SqlDataAdapter("SELECT * FROM Запчасти", sqlConnection);
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
                adapter = new SqlDataAdapter("SELECT * from Запчасти where Наименование like'%" + textBox1.Text + "%'", sqlConnection);
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
            if (this.comboBox1.Text == "" || this.наименованиеTextBox.Text == "")
            {
                label1.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Запчасти Where Код_запчасти ='" + comboBox1.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("UPDATE Запчасти SET Наименование=@Наименование, Дата_выпуска=@Дата_выпуска, Цена_за_ед=@Цена_за_ед," +
                    " Количество_на_складе=@Количество_на_складе, Примечание=@Примечание WHERE Код_запчасти=@Код_запчасти", sqlConnection);
                    command.Parameters.AddWithValue("Код_запчасти", comboBox1.Text);
                    command.Parameters.AddWithValue("Наименование", наименованиеTextBox.Text);
                    command.Parameters.AddWithValue("Дата_выпуска", дата_выпускаTextBox.Text);
                    command.Parameters.AddWithValue("Цена_за_ед", цена_за_едTextBox.Text);
                    command.Parameters.AddWithValue("Количество_на_складе", количество_на_складеTextBox.Text);
                    command.Parameters.AddWithValue("Примечание", примечаниеTextBox.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Запчасти",
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
        private void ComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _ = e.KeyChar;
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
        private void Panel4_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Hide();
            label7.Hide();

        }
        //INSERT
        private async void Button2_Click(object sender, EventArgs e)
        {
            if (this.наименованиеTextBox1.Text == "")
            {
                label4.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Запчасти (Наименование, Дата_выпуска, Цена_за_ед, Количество_на_складе, Примечание) VALUES (@Наименование, @Дата_выпуска, @Цена_за_ед, @Количество_на_складе, @Примечание)", sqlConnection);
                command.Parameters.AddWithValue("Наименование", наименованиеTextBox1.Text);
                command.Parameters.AddWithValue("Дата_выпуска", дата_выпускаTextBox1.Text);
                command.Parameters.AddWithValue("Цена_за_ед", цена_за_едTextBox1.Text);
                command.Parameters.AddWithValue("Количество_на_складе", количество_на_складеTextBox1.Text);
                command.Parameters.AddWithValue("Примечание", примечаниеTextBox1.Text);
                popup = new PopupNotifier
                {
                    Image = Properties.Resources.connected,
                    ImageSize = new Size(96, 96),
                    TitleText = "Запчасти",
                    ContentText = "Данные успешно добавлены!"
                };
                popup.Popup();
                await command.ExecuteNonQueryAsync();
            }
        }
        //Conditions Texboxes for INSERT
        private void Panel5_MouseMove(object sender, MouseEventArgs e)
        {
            label4.Hide();
        }
        //DELETE
        private async void Button3_Click(object sender, EventArgs e)
        {
            if (this.comboBox3.Text == "")
            {
                label5.Show();
            }
            else
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                sqlConnection.Open();
                SqlDataAdapter Tablet = new SqlDataAdapter("Select Count (*) Login From Запчасти Where Код_запчасти ='" + comboBox3.Text + "'", sqlConnection);
                DataTable dt = new DataTable();
                Tablet.Fill(dt);
                if (dt.Rows[0][0].ToString() == "1")
                {
                    sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["VehicleAccounting.Properties.Settings.VehicleAccountingConnectionString"].ConnectionString);
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Запчасти WHERE Код_запчасти=@Код_запчасти", sqlConnection);
                    command.Parameters.AddWithValue("Код_запчасти", comboBox3.Text);
                    popup = new PopupNotifier
                    {
                        Image = Properties.Resources.connected,
                        ImageSize = new Size(96, 96),
                        TitleText = "Запчасти",
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
                adapter = new SqlDataAdapter("SELECT * FROM Запчасти", sqlConnection);
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

        //OTHER BD's FORMS
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

        private void ПользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Пользователи пользователи = new Пользователи();
            пользователи.Show();
        }

        private void ВодителиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Водители водители = new Водители();
            водители.Show();
        }

        private void ТранспортныеСредстваToolStripMenuItem_Click(object sender, EventArgs e)
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
