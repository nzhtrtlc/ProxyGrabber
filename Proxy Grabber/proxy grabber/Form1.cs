using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proxy_shit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Proxy> proxyler;
        string ip = "";
        bool connection;
        string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
        private void bgWork1_DoWork(object sender, DoWorkEventArgs e)
        {
            GetProxyList gp = new GetProxyList();
            proxyler = gp.Start_GetProxyList();
        }

        private void btnProxyCek_Click(object sender, EventArgs e)
        {
            try
            {
                if (!bgWork1.IsBusy)
                {
                    status.Text = "Proxyler Çekiliyor..";
                    btnProxyCek.Enabled = false;
                    bgWork1.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Mevcut işlemin bitmesini bekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Bir şeyler ters gitti.");
            }
            
        }

        private void bgWork1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListViewItem item = new ListViewItem();
            byte count = 0;
            foreach (Proxy proxy in proxyler)
            {
                count++;
                item = listView1.Items.Add(count.ToString());
                item.SubItems.Add(proxy.IP + ":" + proxy.Port);
                item.SubItems.Add(proxy.Ulke);
                item.SubItems.Add(proxy.Hiz);
            }
            status.Text = "Bitti";
            btnProxyCek.Enabled = true;
            bgWork1.Dispose();
        }
        void updateExternalIp()
        {
            if(!bgWork2.IsBusy)
            bgWork2.RunWorkerAsync();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                checkForProxy();
                string proxy = listView1.SelectedItems[0].SubItems[1].Text;
                ChangeProxy.SetProxy(proxy);
                updateProxyLog(proxy);
                status.Text = "Proxy atandı.";
                updateExternalIp();
            }
            catch (Exception)
            {
                MessageBox.Show("Herhangi bir proxy seçilmedi!");
                return;
            }
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bgWork2.RunWorkerAsync();
            if (ChangeProxy.GetProxyStatus() == "1")
                updateProxyLog(ChangeProxy.GetProxyServer());
            else
                updateProxyLog("Yok.");
            //MessageBox.Show(ChangeProxy.GetProxyStatus());
        }
        void updateProxyLog(string proxy)
        {
            lblProxy.Text = "Proxy Server: " + proxy;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeProxy.DisableProxy();
            status.Text = "Proxy devre dışı.";
            updateExternalIp();
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void bgWork2_DoWork(object sender, DoWorkEventArgs e)
        {
            connection = CheckForInternetConnection();
            ip = getExternalIp();
        }
        private string getExternalIp()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch { return null; }
        }

        private void bgWork2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!connection)
            {
                status.Text = "Internet bağlantısını kontrol edin.";
            }
            else
            {
                status.Text = "Hazır.";
            }
            lblIP.Text = "IP Adresiniz: "+ip;
            bgWork2.Dispose();
        }

        private void kopyalaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string text = listView1.SelectedItems[0].SubItems[1].Text;
            Clipboard.SetText(text);
            status.Text = "Kopyalandı.";
        }

        private void btnManuelProxy_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(textBox1.Text, pattern))
            {
                MessageBox.Show("IP Formatı Geçerli Değil!");
            }
            else
            {
                ChangeProxy.SetProxy(textBox1.Text.Trim());
                updateProxyLog(textBox1.Text.Trim());
                updateExternalIp();
                status.Text = "Proxy Değiştirildi..";
            }
        }

        private void bgWork3_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        bool checkForProxy()
        {

            return false;
        }
    }
}
