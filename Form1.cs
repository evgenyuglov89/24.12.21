using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace _24._12._21 {
    public partial class Form1 : Form {
        private SqlDataReader reader;
        private DataTable table;
        private SqlConnection conn;
        public Form1() {
            InitializeComponent();
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; 
                                      Initial Catalog=Vegetables and fruits;
                                      Integrated Security=SSPI;";
        }

        private void Create_Click(object sender, EventArgs e) {
            textBox2.Text = ("CREATE TABLE [dbo].[VegetablesAndFruits](\r\n" +
                            "\t[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),\r\n" +
                            "\tname NVARCHAR(20) NOT NULL,\r\n" +
                            "\ttype INT NOT NULL,\r\n" +
                            "\tcolor NVARCHAR(20) NOT NULL,\r\n" +
                            "\tcaloricContent INT NOT NULL\r\n" +
                            ")");
        }

        private void Insert_Click(object sender, EventArgs e) {
            //InsertQuery();
            textBox2.Text = ("INSERT INTO VegetablesAndFruits (name, type, color, caloricContent)\r\n" +
                              "VALUES ('kiwi', 0, 'green', 47), ('avocado', 0, 'green', 160),\r\n" +
                                       "\t('cucumber', 1, 'green', 15), ('broccoli', 1, 'green', 34),\r\n" +
                                       "\t('cherrie', 0, 'red', 50), ('pomegranate', 0, 'red', 83),\r\n" +
                                       "\t('tomato', 1, 'red', 20), ('pepper', 1, 'red', 26),\r\n" +
                                       "\t('peach', 0, 'yellow', 46), ('mangoe', 0, 'yellow', 60),\r\n" +
                                       "\t('corn', 1, 'yellow', 86), ('pumpkin', 1, 'yellow', 26)"); 
        }
        public void InsertQuery() {
            try {
                conn.Open();
                string insertString = @"INSERT INTO VegetablesAndFruits (name, type, color, caloricContent)
                                        VALUES ('kiwi', 0, 'green', 47),('avocado', 0, 'green', 160),
                                       ('cucumber', 1, 'green', 15),('broccoli', 1, 'green', 34),
                                       ('cherrie', 0, 'red', 50),('pomegranate', 0, 'red', 83),
                                       ('tomato', 1, 'red', 20),('pepper', 1, 'red', 26), 
                                       ('peach', 0, 'yellow', 46),('mangoe', 0, 'yellow', 60),
                                       ('corn', 1, 'yellow', 86),('pumpkin', 1, 'yellow', 26)";
                SqlCommand cmd = new SqlCommand(insertString, conn);
                cmd.ExecuteNonQuery();
            }
            finally {
                if (conn != null) {
                    conn.Close();
                }
            }
        }

        private void showResult(string sqlQuery) {
            textBox2.Clear();
            if (conn.State == ConnectionState.Open) {
                try {
                    SqlCommand comm = new SqlCommand();
                    comm.CommandText = sqlQuery;
                    comm.Connection = conn;
                    dataGridView1.DataSource = null;
                    table = new DataTable();
                    reader = comm.ExecuteReader();
                    int line = 0;
                    do {
                        while (reader.Read()) {
                            if (line == 0) {
                                for (int i = 0; i < reader.FieldCount; i++) {
                                    table.Columns.Add(reader.GetName(i));
                                }
                                line++;
                            }
                            DataRow row = table.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++) {
                                row[i] = reader[i];
                            }
                            table.Rows.Add(row);
                        }
                    } while (reader.NextResult());
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex) {
                    MessageBox.Show("Ошибка при подключении к базе данных" + ex.Message);
                }
                finally {
                    if (reader != null) { reader.Close(); }
                }
            } else if(conn.State != ConnectionState.Open) {
                MessageBox.Show("Вы не подключены к базе данных, для выполнения запроса, сперва выполните подключение к базе данных");
            }
        }

        private void ShowAllInfo_Click(object sender, EventArgs e) {
            string sql = "SELECT * FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowAllName_Click(object sender, EventArgs e) {
            string sql = "SELECT name FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowAllColor_Click(object sender, EventArgs e) {
            string sql = "SELECT DISTINCT color FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowMaxCaloricContent_Click(object sender, EventArgs e) {
            string sql = "SELECT MAX(caloricContent) [Максимальная калорийность] FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowMinCaloricContent_Click(object sender, EventArgs e) {
            string sql = "SELECT MIN(caloricContent) [Минимальная калорийность] FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowAverageCaloricContent_Click(object sender, EventArgs e) {
            string sql = "SELECT Avg(caloricContent) [Средняя калорийность] FROM VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowCountVegetables_Click(object sender, EventArgs e) {
            string sql = "SELECT Count(id) [Количество овощей] FROM VegetablesAndFruits WHERE type = 1";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowCountFruits_Click(object sender, EventArgs e) {
            string sql = "SELECT Count(id) [Количество фруктов] FROM VegetablesAndFruits WHERE type = 0";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void greenColor_Click(object sender, EventArgs e) {
            string sql = "SELECT TOP(1)(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'green') as [Кол-во фруктов зеленого цвета], " +
                "(SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'green') as [Кол-во овощей зеленого цвета] from VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void redColor_Click(object sender, EventArgs e) {
            string sql = "SELECT TOP(1)(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'red') as [Кол-во фруктов красного цвета], " +
                "(SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'red') as [Кол-во овощей красного цвета] from VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void yellowColor_Click(object sender, EventArgs e) {
            string sql = "SELECT TOP(1)(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'yellow') as [Кол-во фруктов желтого цвета], " +
                "(SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'yellow') as [Кол-во овощей желтого цвета] from VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowVandFEachColor_Click(object sender, EventArgs e) {
            string sql = "SELECT TOP(1)(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'green') as [Кол-во фруктов зеленого цвета], " +
                "(SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'green') as [Кол-во овощей зеленого цвета]," +
                "(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'yellow') as [Кол-во фруктов желтого цвета]," +
                " (SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'yellow') as [Кол-во овощей желтого цвета]," +
                "(SELECT count(F.type) from VegetablesAndFruits F Where F.type = 0 AND F.color = 'red') as [Кол-во фруктов красного цвета]," +
                "(SELECT count(V.type) from VegetablesAndFruits V Where V.type = 1 AND V.color = 'red') as [Кол-во овощей красного цвета] from VegetablesAndFruits";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent < 150";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent < 100";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent < 50";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent < 25";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 25";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 50";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 100";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void от0До25ToolStripMenuItem_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 0 AND caloricContent < 25";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void от25До50ToolStripMenuItem_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 25 AND caloricContent < 50";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void от50До100ToolStripMenuItem_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent from VegetablesAndFruits Where caloricContent > 50 AND caloricContent < 100";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void ShowAllVandFColorRorY_Click(object sender, EventArgs e) {
            string sql = "SELECT name, caloricContent, color from VegetablesAndFruits Where color = 'yellow'OR color = 'red'";
            textBox1.Text = sql;
            showResult(sql);
        }

        private void Connect_Click(object sender, EventArgs e) {
            if(conn.State != ConnectionState.Open) { 
                conn.Open();
                MessageBox.Show("Выполнено успешное подключение к базе данных");
            }else if (conn.State == ConnectionState.Open) {
                MessageBox.Show("Вы уже подключены к базе данных");
            }
        }
        private void Disconnect_Click(object sender, EventArgs e) {
            if(conn.State != ConnectionState.Closed) {
                conn.Close();
                MessageBox.Show("Вы откючились от базы данных");
            } else if (conn.State == ConnectionState.Closed) {
                MessageBox.Show("Вы не были подключены к базе данных");
            }
        }
    }
}
