using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Messaging;
using MSQMConsole;
using MSQMConsole.Common;

namespace MSQMClient2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (MessageQueue q in MhQueue.GetAllQueues())
            {
                listBox1.Items.Add(q.Path);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string name = listBox1.SelectedItem.ToString();
                int pos = name.LastIndexOf("\\");
                name = name.Substring(pos + 1, name.Length - pos - 1);
                richTextBox1.Text = "";
                MhQueue queue = new MhQueue(name);
                IEnumerable<IMhMessage> messages = queue.GetMessages();

                foreach (var message in messages)
                {
                    richTextBox1.Text += message.Body.ToString();
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (MessageQueue q in MhQueue.GetAllQueues())
            {
                listBox1.Items.Add(q.Path);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string name = listBox1.SelectedItem.ToString();
            int pos = name.LastIndexOf("\\");
            name = name.Substring(pos + 1, name.Length - pos - 1);
            MhQueue queue = new MhQueue(name);
            queue.Purge();
            button2_Click(sender, e);
        }
    }
}
