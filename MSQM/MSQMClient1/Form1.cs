using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSQMConsole.Common;
using MSQMConsole.Helpers;
using System.Messaging;
using MSQMConsole;


namespace MSQMClient1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach(MessageQueue q in MhQueue.GetAllQueues())
            {
                listBox1.Items.Add(q.Path);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Book book = new Book(
                textBox1.Text, 
                textBox2.Text, 
                textBox3.Text, 
                Convert.ToInt32(textBox4.Text), 
                textBox5.Text, 
                textBox6.Text);
            string name = listBox1.SelectedItem.ToString();
            int pos = name.LastIndexOf("\\");
            name = name.Substring(pos + 1, name.Length - pos - 1);
            MhQueue queue = new MhQueue(name);
             
            queue.Send(book);

        }
    }
}
