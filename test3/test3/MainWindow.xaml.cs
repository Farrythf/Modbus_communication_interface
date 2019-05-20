using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace test3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread threadclient = null;
        Socket socketclient = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btconnect_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("connect");
            this.bt_connect.IsEnabled = false;
            if(textbox_ip.Text == "" || textbox_port.Text == "")
            {
                MessageBox.Show("IP or Port empty");
                return;
            }
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Parse(textbox_ip.Text.Trim());
            int port = Convert.ToInt32(textbox_port.Text.Trim());
            IPEndPoint point = new IPEndPoint(ipaddr, port);
            try
            {
                socketclient.Connect(point);
                string connect_su = string.Format("Connected, server ip:{0}, port:{1}", textbox_ip.Text, textbox_port.Text);
                this.textbox_log.AppendText(connect_su + "\n\n");
            }
            catch(Exception)
            {
                Debug.WriteLine("Failed");
                MessageBox.Show("Connect failed");
                this.bt_connect.IsEnabled = true;
                return;
            }
            threadclient = new Thread(Recv);
            threadclient.IsBackground = true;
            threadclient.Start();
        }
        void Recv()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024];
                    socketclient.Receive(data);
                    int length = data[5];  
                    Byte[] datashow = new byte[length + 6]; 
                    for (int i = 0; i <= length + 5; i++)
                        datashow[i] = data[i];
                    string stringdata = BitConverter.ToString(datashow);
                    this.textbox_log.Dispatcher.Invoke(new Action(delegate { textbox_log.AppendText("Server:" + GetCurrentTime() + "\r\n" + stringdata + "\r\n\n"); }));
                    /*byte[] arrRecvmsg = new byte[1024 * 1024];
                    int length = socketclient.Receive(arrRecvmsg);
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);
                    this.textbox_log.Dispatcher.Invoke(new Action(delegate { textbox_log.AppendText("Server:" + GetCurrentTime() + "\r\n" + strRevMsg + "\r\n\n"); }));
                    Debug.WriteLine("Server:" + GetCurrentTime() + "\r\n" + strRevMsg + "\r\n\n");*/
                }
                catch (Exception)
                {
                    MessageBox.Show("Disconnected");
                    Debug.WriteLine("connection break" + "\r\n");
                    break;
                }
            }
            this.bt_connect.Dispatcher.Invoke(new Action(delegate { bt_connect.IsEnabled = true; }));
        }
        DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        private void Btsend_Click(object sender, RoutedEventArgs e)
        {
            ClientSendMsg(this.textbox_send.Text.Trim());
            this.textbox_send.Clear();
        }

        void ClientSendMsg(string sendMsg)
        {   
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg); 
            try
            {
                socketclient.Send(arrClientSendMsg);
            }
            catch(Exception)
            {
                MessageBox.Show("No connection");
                return;
            }
            Debug.WriteLine("send" + ": " + GetCurrentTime() + "\r\n" + sendMsg + "\r\n\n");
            this.textbox_log.AppendText("Send" + ": " + GetCurrentTime() + "\r\n" + sendMsg + "\r\n\n");
        }

        private void BtHeart_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 , 0x01, 0x04, 0x27, 0x67, 0x00, 0x01 };
            try
            {
                socketclient.Send(data);
            }
            catch(Exception)
            {
                MessageBox.Show("No connection");
                return;
            }
            string str = BitConverter.ToString(data);
            this.textbox_log.AppendText("Send" + ": " + GetCurrentTime() + "\r\n" + str + "\r\n\n");
           
        }

        private void BtChState_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x01, 0x04, 0x27, 0x60, 0x00, 0x01 };
            try
            {
                socketclient.Send(data);
            }
            catch (Exception)
            {
                MessageBox.Show("No connection");
                return;
            }
            string str = BitConverter.ToString(data);
            this.textbox_log.AppendText("Send" + ": " + GetCurrentTime() + "\r\n" + str + "\r\n\n");

        }

        private void BtIP_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x01, 0x03, 0x27, 0x14, 0x00, 0x02 };
            try
            {
                socketclient.Send(data);
            }
            catch (Exception)
            {
                MessageBox.Show("No connection");
                return;
            }
            string str = BitConverter.ToString(data);
            this.textbox_log.AppendText("Send" + ": " + GetCurrentTime() + "\r\n" + str + "\r\n\n");

        }

        private void Disc_Click(object sender, RoutedEventArgs e)
        {
            socketclient.Close();
        }
    }
}


