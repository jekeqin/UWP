using PrintServer.print;
using PrintServer.server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
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

namespace PrintServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon trayIcon = null;

        private HttpServer server = null;
        public MainWindow()
        {
            InitializeComponent();

            init();
        }

        private void init()
        {
            httpPort.Text = Properties.Settings.Default.DefaultPort;
            cmd1.Text = "netsh http delete urlacl url=http://+:" + httpPort.Text + "/";
            cmd2.Text = "netsh http add urlacl url = http://+:" + httpPort.Text + "/ user=Everyone";

            trayIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipTitle = "自动打印服务",
                //BalloonTipText = "小票自动打印服务",
                Text = "小票自动打印服务",
                Visible = true
            };
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/PrintServer;component/assets/print.ico")).Stream;
            trayIcon.Icon = new System.Drawing.Icon(iconStream);

            trayIcon.DoubleClick += trayIcon_Show;
            System.Windows.Forms.MenuItem menuOpen = new System.Windows.Forms.MenuItem("打开窗口");
            menuOpen.Click += trayIcon_Show;
            System.Windows.Forms.MenuItem menuStart = new System.Windows.Forms.MenuItem("启动服务");
            menuStart.Click += (object sender, EventArgs e) =>
            {
                wsServerStart();
            };
            System.Windows.Forms.MenuItem menuStop = new System.Windows.Forms.MenuItem("停止服务");
            menuStop.Click += (object sender, EventArgs e) =>
            {
                _ = server != null ? server.Stop() : true;
            };
            System.Windows.Forms.MenuItem menuQuit = new System.Windows.Forms.MenuItem("退出");
            menuQuit.Click += (object sender,EventArgs e) =>
            {
                _ = server != null ? server.Stop() : true;
                Environment.Exit(0);
            };
            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] { menuOpen, menuStart, menuStop, menuQuit});

            this.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                e.Cancel = true;
            };
            this.StateChanged += (object sender, EventArgs e) =>
            {
                if (this.WindowState==WindowState.Minimized)
                {
                    this.ShowInTaskbar = false;
                }
            };

            this.loadPrint();

            this.wsServerStart();
        }

        private void trayIcon_Show(object sender,EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
            }
            else
            {
                this.ShowInTaskbar = false;
            }
        }

        private void wsServerStart()
        {
            if ( server==null )
            {
                server = new HttpServer();
            }
            if (server.Start(httpPort.Text))
            {
                wsServerButton.Content = "服务运行中";
                trayIcon.Text = "[运行中]小票自动打印服务";
            }
            else
            {
                trayIcon.Text = "[未启动]小票自动打印服务";
                wsServerButton.Content = "启动服务";
            }
        }

        private void loadPrint()
        {
            var list = new PrintUtil().Prints();
            foreach (string name in list)
            {
                TextBox input = new TextBox();
                input.Text = name;
                input.IsReadOnly = true;
                input.Height = 25;
                listView.Children.Add(input);
            }
        }


        private void ModifyPortButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DefaultPort = httpPort.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("保存成功，请重启服务","操作提示", MessageBoxButton.OK);
        }

        private void WsServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (  (string)wsServerButton.Content== "启动服务" )
            {
                wsServerStart();
            }
            else if(server!=null)
            {
                server.Stop();
                wsServerButton.Content = "启动服务";
                trayIcon.Text = "[未启动]小票自动打印服务";
            }

        }
    }
}