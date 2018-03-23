using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace always_run
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        WindowState ws;
        WindowState wsl;
        System.Windows.Forms.NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            btn_stop.IsEnabled = false;
            icon();
            //保证窗体显示在上方。
            wsl = WindowState;
        }

        private void icon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "欢迎使用"; //设置程序启动时显示的文本
            this.notifyIcon.Text = "AlwaysRun";//最小化到托盘时，鼠标点击时显示的文本
//this.notifyIcon.Icon = new System.Drawing.Icon("Downloads.ico");//程序图标
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = wsl;
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            string addr = txt_addr.Text;
            string space = txt_space.Text;
            if (string.IsNullOrEmpty(addr) || string.IsNullOrEmpty(space))
            {
                MessageBox.Show(string.IsNullOrEmpty(addr) ? "请输入地址" : "请输入时间间隔");
                return;
            }
            int timespace = 0;
            if (!Int32.TryParse(space, out timespace))
            {
                MessageBox.Show("间隔必须为数字");
                return;
            }
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, timespace);   //间隔1秒
            timer.Tick += new EventHandler(Scan);
            timer.Start();
            txt_addr.IsEnabled = false;
            txt_space.IsEnabled = false;
            txt_msg.Content = "开始运行";
            btn_start.IsEnabled = false;
            btn_stop.IsEnabled = true;
            urlTest();

        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            txt_addr.IsEnabled = true;
            txt_space.IsEnabled = true;
            btn_start.IsEnabled = true;
        }

        private void Scan(object sender, EventArgs e)
        {
            urlTest();
        }

        private void urlTest()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(txt_addr.Text);
            req.Method = "GET";
            HttpWebResponse wr = req.GetResponse() as HttpWebResponse;
            if (wr.StatusCode == HttpStatusCode.OK)
            {
                txt_msg.Content = "访问成功：" + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm");
            }
            else
            {
                txt_msg.Content = "无法访问到远程地址";
            }
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState ws = WindowState;
            if (ws == WindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}
