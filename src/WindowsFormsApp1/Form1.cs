using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OtpNet;
using System.Linq.Expressions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int n, m, i, j;
        private readonly string filePath=Path.Combine(Application.StartupPath,"data.txt");
        private Totp totpkey;
        private byte[] secretKey;
        private char __NL__;

        public Form1()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button2.Enabled = true;
            } 
            else
            {
                button2.Enabled=false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if (!File.Exists(filePath))
            {
                try
                {
                    File.WriteAllText(filePath, string.Empty);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show($"Ошибка при создании файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
                {
                    n = Convert.ToInt32(sr.ReadLine());
                    m = Convert.ToInt32(sr.ReadLine());
                    for (i = 0; i < n; i++)
                    {
                        int rowNumber = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowNumber].Cells["ID"].Value = sr.ReadLine();
                        dataGridView1.Rows[rowNumber].Cells[1].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                        dataGridView1.Rows[rowNumber].Cells[2].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                        dataGridView1.Rows[rowNumber].Cells[3].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                        dataGridView1.Rows[rowNumber].Cells[4].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                        dataGridView1.Rows[rowNumber].Cells[5].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                        dataGridView1.Rows[rowNumber].Cells[6].Value = sr.ReadLine()?.Replace("|NEWLINE|", Environment.NewLine);
                    }


                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            service.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            login.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            passw.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            keyauth.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            keyrec1.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            keyrec2.Text= dataGridView1.CurrentRow.Cells[6].Value.ToString();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (keyauth.Text == "")
            {
                MessageBox.Show("Поле с ключем 2FA не может быть пустым", "Введите ключ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                secretKey = Base32Encoding.ToBytes(keyauth.Text.Trim().Replace(" ", "").Replace("-", "").ToUpper());
                totpkey = new Totp(secretKey);
                UpdateKey();
                timer1.Enabled = true;
            }
        }
        private void UpdateKey()
        {
            string currentKey = totpkey.ComputeTotp();
            secret.Text = currentKey;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerl.Text = "Ключ требует обновления!";
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            int rowNumber = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowNumber].Cells["ID"].Value = rowNumber;
            dataGridView1.Rows[rowNumber].Cells[1].Value = service.Text;
            dataGridView1.Rows[rowNumber].Cells[2].Value = login.Text;
            dataGridView1.Rows[rowNumber].Cells[3].Value = passw.Text;
            dataGridView1.Rows[rowNumber].Cells[4].Value = keyauth.Text;
            dataGridView1.Rows[rowNumber].Cells[5].Value = keyrec1.Text;
            dataGridView1.Rows[rowNumber].Cells[6].Value = keyrec2.Text;
            service.Clear();
            login.Clear();
            passw.Clear();
            keyauth.Clear();
            keyrec1.Clear();
            keyrec2.Clear();
            service.Focus();
            SaveDataFile();

        }

        private void SaveDataFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, append: false, encoding: System.Text.Encoding.UTF8))
                {
                    n = dataGridView1.RowCount;
                    m = dataGridView1.ColumnCount;
                    sw.WriteLine(n);
                    sw.WriteLine(m);
                    for (i = 0; i < n; i++)
                    {
                        for (j = 0; j < m; j++)
                        {
                            string cellData = Convert.ToString(dataGridView1[j, i].Value);
                            if (!string.IsNullOrEmpty(cellData))
                            {
                                cellData=cellData.Replace("\r\n", "|NEWLINE|").Replace("\n", "|NEWLINE|");
                            }
                            else
                            {
                                cellData = "";
                            }
                            sw.WriteLine(cellData);
                        }
                    }

                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Нет прав для записи в файл", "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                dataGridView1.Rows.RemoveAt(cell.RowIndex);
                SaveDataFile();
            }
        }
    }
}
