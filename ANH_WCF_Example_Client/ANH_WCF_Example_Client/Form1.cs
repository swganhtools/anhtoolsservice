using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using ANH_WCF_Example_Client.ANHService;
using ANH_WCF_Interface;
using System.Security.Cryptography;

namespace ANH_WCF_Example_Client
{
    public partial class Form1 : Form
    {
        AnhCallback ServerCallback = null;
        AnhServiceClient client = null;
        public Form1()
        {
            ServerCallback = new AnhCallback();
            InitializeComponent();
            ServerCallback.MessageReceived += new EventHandler<MessageEventArgs>(ServerCallback_MessageReceived);

            InstanceContext ic = new InstanceContext(ServerCallback);
            client = new AnhServiceClient(ic);

            Servers s = new Servers(ServerType.ConnectionServer, "");
            comboBox1.Items.Add(s);
            s = new Servers(ServerType.ChatServer, "");
            comboBox1.Items.Add(s);
            s = new Servers(ServerType.LoginServer, "");
            comboBox1.Items.Add(s);
            s = new Servers(ServerType.PingServer, "");
            comboBox1.Items.Add(s);
            s = new Servers(ServerType.ZoneServer, "tutorial");
            comboBox1.Items.Add(s);
            s = new Servers(ServerType.ZoneServer, "tatooine");
            comboBox1.Items.Add(s);
            comboBox1.SelectedIndex = 0;
        }

        void ServerCallback_MessageReceived(object sender, MessageEventArgs e)
        {
            listBox1.Items.Add(e.ServerType.ToString() + ": " + e.Message);
        }

        private void Auth_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Authenticating with server");
            bool result = client.AuthenticateSession(HashCalc.GetSHA(textBox1.Text));
            if (result)
            {
                listBox1.Items.Add("You are now Authenticated");
            }
            else
            {
                listBox1.Items.Add("Authentication Failed");
            }
        }
        private void CloseChannel_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                if(client.State == CommunicationState.Opened)
                    client.Close();
            }
        }

        private void TestEvents_Click(object sender, EventArgs e)
        {
            ServerCallback.TestEventSystem();
        }
        private void GetServerStatus(object sender, EventArgs e)
        {
            Object[] t = null;
            try
            {
                t = client.GetServerStatuses();
            }
            catch { }
            if (t != null)
            {
                listBox1.Items.Add("Got statuses for " + t.Length + " Servers");
                foreach (IServerStatus iss in t)
                {
                    listBox1.Items.Add(iss.type.ToString() + ": " + iss.args + " Running: " + iss.IsRunning + " Crashed: " + iss.IsCrashed + " Uptime (s): " + iss.Uptime / 1000);
                }
            }

            listBox1.Items.Add("GetServerStatus Ran");
        }
        private void GetAvailableServers(object sender, EventArgs e)
        {
            ServerType[] stype = client.GetAvailableServers();
            
            if (stype != null)
            {
                List<ServerType> t = new List<ServerType>(stype);
                listBox1.Items.Add(t.Count + " types of servers are available");
                foreach (ServerType st in t)
                {
                    listBox1.Items.Add(st);
                }
            }
        }
        void StartServer(object sender, EventArgs e)
        {
            Servers s = (Servers)comboBox1.SelectedItem;
            if (s == null) return;

            listBox1.Items.Add(client.StartServer(s.type, s.args));
        }
        void StopServer(object sender, EventArgs e)
        {
            Servers s = (Servers)comboBox1.SelectedItem;
            if (s == null) return;

            listBox1.Items.Add(client.StopServer(s.type, s.args));
        }
        void SubscribeToStatusUpdates(object sender, EventArgs e)
        {
            listBox1.Items.Add(client.SubscribeToStatusUpdates());
        }
        void UnsubscribeFromStatusUpdates(object sender, EventArgs e)
        {
            listBox1.Items.Add(client.UnsubscribeFromStatusUpdates());
        }
    }
}
