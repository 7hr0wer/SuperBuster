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
using System.Diagnostics;
using System.IO.Compression;

namespace Update
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] Version;
        string b;
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    button1.Enabled = false;
                    label1.Text = "正在检查更新......";
                    Version = File.ReadAllLines("Version.txt");
                    b = HttpGet("https://thrower.cc/SuperBusterLatestVersion.html");
                    if (b != Version[0])
                    {
                        MessageBox.Show("发现新版本！最新版本为：" + b + " 当前版本为：" + Version[0], "提示");
                        button1.Enabled = true;
                        button2.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("当前为最新版本！", "提示");
                        button1.Enabled = true;
                    }
                }
                catch
                {
                    MessageBox.Show("错误！", "提示");
                }
                label1.Text = "";
            });
        }
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            request.Timeout = 20000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                label1.Text = "正在下载......";
                button2.Enabled = false;
                DirectoryInfo info = new DirectoryInfo(Application.StartupPath);
                string a = info.Parent.FullName;
                DirectoryInfo dir = new DirectoryInfo(a);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                    }
                    else
                    {
                        File.Delete(i.FullName);
                    }
                }
                WebClient webClient = new WebClient();
                webClient.DownloadFile(new Uri("https://thrower.cc/files/SuperBuster.zip"), "Latest.zip");
                label1.Text = "正在解压......";
                ZipFile.ExtractToDirectory(Directory.GetCurrentDirectory() + "/Latest.zip", a);
                File.Delete("Latest.zip");
                label1.Text = "更新完成！";
                Version[0] = b;
                File.WriteAllLines("Version.txt", Version);
                string c = a + "/SuperBuster.exe";
                Process.Start(c);
                Environment.Exit(0);
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo info = new DirectoryInfo(Application.StartupPath);
            string a = info.Parent.FullName;
            string c = a + "/SuperBuster.exe";
            Process.Start(c);
            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("SuperBuster").ToList().Count > 0)
            {
                Process[] processes = Process.GetProcesses();
                foreach(Process p in processes)
                {
                    if(p.ProcessName == "SuperBuster")
                    {
                        p.CloseMainWindow();
                    }
                }
                TopMost = true;
            }
            else
            {
                MessageBox.Show("请在主程序中打开更新程序！", "提示");
                Environment.Exit(0);
            }
        }
    }
}
