﻿using System;
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
    public partial class gradeStatisticsForm : Form
    {
        public gradeStatisticsForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 检查信息是否选择完整
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("请将信息输入完整！");
            }
            else
            {
                string term = comboBox1.SelectedItem.ToString();
                string claname = comboBox2.SelectedItem.ToString();
                string g1 = textBox1.Text;
                int grade1 = 0;
                int.TryParse(g1, out grade1);
                string g2 = textBox2.Text;
                int grade2 = 0;
                int.TryParse(g2, out grade2);
                if (g1 == "" || g2 == "")
                {
                    SqlConnection conn = new SqlConnection(loginForm.connectionString);
                    conn.Open();
                    //textBox1.Text.Trim()  textBox2.Text.Trim()
                    string sql = "select class.claname as 课程名称,student.stuxuehao as 学生学号,stuname as 学生姓名,sc.grades as 成绩,class.term as 开课学期,class.teacher as 开课教师 from student,sc,class where student.stuid = sc.stuid and sc.claid = class.claid and term = '" + term + "' and class.claname = '" + claname + "'";
                    SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    adp1.Fill(ds);
                    //载入基本信息
                    dataGridView1.DataSource = ds.Tables[0].DefaultView;
                    //求的平均成绩并显示
                    sql = "select avg(grades) from sc,class where sc.claid = class.claid and class.claname ='" + claname + "'";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    String avg = cmd.ExecuteScalar().ToString();
                    textBoxav.Text = avg;
                    //求的最高成绩并显示
                    sql = "select max(grades) from sc,class where sc.claid = class.claid and class.claname ='" + claname + "'";

                    cmd.CommandText = sql;
                    String max = cmd.ExecuteScalar().ToString();
                    textBoxmax.Text = max;
                    //求的最低成绩并显示
                    sql = "select min(grades) from sc,class where sc.claid = class.claid and class.claname ='" + claname + "'";
                    cmd.CommandText = sql;
                    String min = cmd.ExecuteScalar().ToString();
                    textBoxmin.Text = min;
                    conn.Close();
                }
                else
                {
                    SqlConnection conn = new SqlConnection(loginForm.connectionString);
                    conn.Open();
                    //textBox1.Text.Trim()  textBox2.Text.Trim()
                    string sql = "select class.claname as 课程名称,stuxuehao as 学生学号,student.stuname as 学生姓名,sc.grades as 成绩,class.term as 开课学期,class.teacher as 开课教师 from student,sc,class where student.stuid = sc.stuid and sc.claid = class.claid and term = '" + term + "' and class.claname = '" + claname + "'and grades between " + grade1 + " and " + grade2;
                    SqlDataAdapter adp1 = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    adp1.Fill(ds);
                    //载入基本信息
                    dataGridView1.DataSource = ds.Tables[0].DefaultView;
                    conn.Close();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gradeStatisticsForm_Load(object sender, EventArgs e)
        {

        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空课程名称
            comboBox2.Text = "";
            if (comboBox2.Items.Count > 0)
            {//清空所有项
                comboBox2.Items.Clear();
            }
            string term = comboBox1.SelectedItem.ToString();
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
                comboBox2.Items.Add(row[0].ToString());
            }
            //dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
        }
    }
}
