using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace StudentMIS
{
    public partial class modifyGradeForm : Form
    {
        public modifyGradeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.DataSource = null;
            }
            if (comboBoxterm.Text == "" || comboBoxcourse.Text == "")
            {
                MessageBox.Show("请输入查询信息！");
            }
            else if (comboBoxterm.Text != "" && comboBoxcourse.Text != "")
            {
                Console.WriteLine("学期" + comboBoxterm.SelectedItem.ToString());
                Console.WriteLine("课程" + comboBoxcourse.SelectedItem.ToString());

                SqlConnection conn = new SqlConnection(loginForm.connectionString);
                conn.Open();
                string sql = "select student.stuxuehao as 学生学号,student.stuname as 学生姓名,sc.grades as 成绩,class.claname as 课程名,class.term as 学期,class.teacher as 老师 from class,student,sc  where student.stuid=sc.stuid and sc.claid=class.claid and class.term = '" + comboBoxterm.SelectedItem.ToString() + "'and class.claname='" + comboBoxcourse.SelectedItem.ToString() + "'";
                Console.WriteLine(sql);
                SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                adp1.Fill(ds);
                //载入基本信息
                dataGridView1.DataSource = ds.Tables[0].DefaultView;
                conn.Close();
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            textBoxteacher.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            textBoxgrade.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            textBoxkecheng.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            textBoxxuehao.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int grade = 0;
            //判断是否有选中需要求改的记录
            if (textBoxteacher.Text == "" || textBoxkecheng.Text == "" || textBoxxuehao.Text == "")
            {
                MessageBox.Show("请选择需要修改的成绩记录！");
            }
            //判断输入的成绩是否合理
            else if (!int.TryParse(textBoxgrade.Text, out grade))
            {
                MessageBox.Show("输入的成绩必须为数字！");
            }
            else if (grade > 100 || grade < 0)
            {
                MessageBox.Show("成绩范围为0到100！请重新输入！");
            }
            else
            {
                //根据学生的学号得到学生的id
                string stuxuehao = textBoxxuehao.Text;
                SqlConnection conn = new SqlConnection(loginForm.connectionString);
                conn.Open();
                string sql = "select stuid from student where stuxuehao = '" + stuxuehao + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                String id1 = cmd.ExecuteScalar().ToString();
                int stuid = 0;
                int.TryParse(id1, out stuid);
                sql = "update sc set grades =  " + grade + "where stuid=" + stuid;
                cmd.CommandText = sql;
                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("更改成功！");
                }
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void modifyGradeForm_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxterm_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空课程名
            comboBoxcourse.Text = "";
            if (comboBoxcourse.Items.Count > 0)
            {//清空所有项
                comboBoxcourse.Items.Clear();
            }
            string term = comboBoxterm.SelectedItem.ToString();
            Console.WriteLine(term);
            SqlConnection conn = new SqlConnection(loginForm.connectionString);
            conn.Open();
            string sql = "select claname  from class where term='" + term + "'";
            SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            adp1.Fill(ds);
            //载入基本信息
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxcourse.Items.Add(row[0].ToString());
            }
            //dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
