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
    public partial class chooseClassForm : Form
    {
        int stuid = 0;
        public chooseClassForm()
        {
            InitializeComponent();
            //得到stuid
            string stuxuehao = loginForm.getStudent();
            SqlConnection conn = new SqlConnection(loginForm.connectionString);
            conn.Open();
            string sql = "select stuid from student where stuxuehao = '" + stuxuehao + "'";
            SqlCommand cmd = new SqlCommand(sql, conn);
            String id1 = cmd.ExecuteScalar().ToString();
            int.TryParse(id1, out stuid);
            if (listBox1.Items.Count > 0)
            {//清空所有项
                listBox1.Items.Clear();
            }
            sql = "select class.claname  from sc,class where sc.claid = class.claid and stuid=" + stuid;
            SqlDataAdapter adp2 = new SqlDataAdapter(sql, conn);
            DataSet ds2 = new DataSet();
            adp2.Fill(ds2);
            foreach (DataRow row in ds2.Tables[0].Rows)
            {
                listBox1.Items.Add(row[0].ToString());
            }
            conn.Close();
        }

        private void chooseClassForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = loginForm.getStudent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Boolean flag = true;

            //判断必要空格内容是否为空
            if (comboBox1.Text == "")
            {
                MessageBox.Show("请选择学期！");
            }
            else if (textBoxid.Text == "" || textBoxclass.Text == "")
            {
                MessageBox.Show("请选择课程！");
            }
            else
            {
                //得到课程的id
                int claid = 0;
                int.TryParse(textBoxid.Text, out claid);

                //得到课程的学期
                string semster = comboBox1.SelectedItem.ToString();

                //查询是否已选该课程
                SqlConnection conn = new SqlConnection(loginForm.connectionString);
                string sql = "select * from sc where stuid = " + stuid + " and claid = " + claid;
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myreader = cmd.ExecuteReader();
                if (myreader.Read())
                {
                    flag = false;
                    MessageBox.Show("已选过该课程，请勿重复选课！");
                }
                myreader.Close();

                //查询在该时间是否有课
                if (flag)
                {
                    sql = "select sctime from sctime where claid =" + claid;
                    SqlDataAdapter adp = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        string time = dr[0].ToString();//第一列
                        sql = "select * from sc,sctime,class where class.claid = sc.claid and class.claid = sctime.claid and sctime = '" + time + "' and sc.stuid =" + stuid + " and class.term = '" + semster + "'";
                        SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
                        DataSet ds1 = new DataSet();
                        adp1.Fill(ds1);
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            flag = false;
                            MessageBox.Show("课程上课时间冲突！");
                            break;
                        }
                    }
                }

                // 进行选课操作
                if (flag)
                {
                    sql = "insert into sc(claid,stuid) values(" + claid + "," + stuid + ")";
                    cmd.CommandText = sql;
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("选课成功！");
                    }
                }

                // 更新已选课程列表
                if (listBox1.Items.Count > 0)
                {//清空所有项
                    listBox1.Items.Clear();
                }
                sql = "select class.claname  from sc,class where sc.claid = class.claid and stuid=" + stuid;
                SqlDataAdapter adp2 = new SqlDataAdapter(sql, conn);
                DataSet ds2 = new DataSet();
                adp2.Fill(ds2);
                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    listBox1.Items.Add(row[0].ToString());
                }
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空课程id和课程
            textBoxid.Text = "";
            textBoxclass.Text = "";

            while (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.DataSource = null;
            }
            string term = comboBox1.SelectedItem.ToString();
            Console.WriteLine(term);
            SqlConnection conn = new SqlConnection(loginForm.connectionString);
            conn.Open();
            string sql = "select claid as 课程id,claname as 课程 from class where term='" + term + "'";
            SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            adp1.Fill(ds);
            //载入基本信息
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
        }

        private void mos_click(object sender, DataGridViewCellMouseEventArgs e)
        {
            textBoxid.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBoxclass.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //判断是否有选中需要删除的课程
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请选择要删除的课程！");
            }
            else
            {
                string claname = listBox1.SelectedItem.ToString();
                SqlConnection conn = new SqlConnection(loginForm.connectionString);
                conn.Open();
                string sql = "select claid from class where claname = '" + claname + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                String id1 = cmd.ExecuteScalar().ToString();
                int claid = 0;
                int.TryParse(id1, out claid);
                sql = "delete from  sc  where  claid = " + claid + " and stuid = " + stuid;
                cmd.CommandText = sql;
                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("删除成功！");
                    if (listBox1.Items.Count > 0)
                    {//清空所有项
                        listBox1.Items.Clear();
                    }
                    sql = "select class.claname  from sc,class where sc.claid = class.claid and stuid=" + stuid;
                    SqlDataAdapter adp2 = new SqlDataAdapter(sql, conn);
                    DataSet ds2 = new DataSet();
                    adp2.Fill(ds2);
                    foreach (DataRow row in ds2.Tables[0].Rows)
                    {
                        listBox1.Items.Add(row[0].ToString());
                    }
                }
                conn.Close();
            }
        }

        private void getKecheng()
        {
            SqlConnection conn = new SqlConnection(loginForm.connectionString);
            conn.Open();
            string sql = "select class.claname  from sc,class where sc.claid = class.claid and stuid=" + stuid;
            SqlDataAdapter adp2 = new SqlDataAdapter(sql, conn);
            DataSet ds2 = new DataSet();
            adp2.Fill(ds2);
            foreach (DataRow row in ds2.Tables[0].Rows)
            {
                listBox1.Items.Add(row[0].ToString());
            }
            conn.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
