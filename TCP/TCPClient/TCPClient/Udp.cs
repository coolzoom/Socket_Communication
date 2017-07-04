﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPClientTest
{
    public class Udp
    {
        #region 获取3dp IP
        public void GetIPof3DP(ref ComboBox cbRemoteIP)
        {

            List<string> AllIP = ClassGetIP.GetLocalIP();
            foreach (string ip in AllIP)
            {
                IPEndPoint LocalP = new IPEndPoint(IPAddress.Parse(ip), 1122);
                string re = SendReceiveMsg(LocalP, "NBD?");
                if (!string.IsNullOrEmpty(re) && !cbRemoteIP.Items.Contains(re))
                    cbRemoteIP.Items.Add(re);
            }
            if (cbRemoteIP.Items.Count > 0)
                cbRemoteIP.SelectedIndex = 0;

        }

        AutoResetEvent SendEvent = new AutoResetEvent(false);
        private string SendReceiveMsg(IPEndPoint LeP, string str)
        {
            UdpClient udpClient = new UdpClient(LeP);
            udpClient.Client.ReceiveTimeout = 10;  //接收超时
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Broadcast, 33582);
            byte[] data = Encoding.ASCII.GetBytes(str);
            udpClient.Send(data, data.Length, remoteEp);

            Thread a = new Thread(() =>
            {
                try
                {
                    udpClient.Receive(ref remoteEp);
                    SendEvent.Set();
                }
                catch { SendEvent.Set(); }
            });
            a.Start();
            SendEvent.WaitOne(200);  //最多等待时间

            string redata;
            if (remoteEp.Address != IPAddress.Broadcast)
                redata = remoteEp.Address.ToString();
            else
                redata = null;

            udpClient.Close();
            return redata;
        }
        #endregion
    }
}
