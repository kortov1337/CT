using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using System.Transactions;
using Client1.DBContext;

namespace Subscriber1
{
    public partial class Form1 : Form
    {
        static List<DAL.Operation> messagesQueue = new List<DAL.Operation>();
        static public RichTextBox rtb;
        static UnitOfWork unit;
        public Form1()
        {
            InitializeComponent();
            rtb = richTextBox1;
            unit = new UnitOfWork();
        }

        private  void button1_Click(object sender, EventArgs e)
        {
            rtb.Text = ""; 
           foreach(Operation o in messagesQueue)
            {
                
                rtb.Text +=o.OperationType + ": " + o.ToString() + Environment.NewLine;
            }

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await BusStuff.InitSubOneEndpoint("Subscriber1", OnMessage);
        }

        public static Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
        {
            var b = context.Body;
            var message = BusStuff.Deserialize(context.Body);
            switch (message.OperationType)
            {
                case "Create":
                    {
                        using (TransactionScope sc = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            unit.Books.Create(message.body);
                            unit.Save();
                            messagesQueue.Add(message);
                        }
                            break;
                    }
                case "Delete":
                    {
                        using (TransactionScope sc = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            unit.Books.Delete(message.body.Id);
                            unit.Save();
                            messagesQueue.Add(message);
                        }
                        break;
                    }
                case "Update":
                    {
                        using (TransactionScope sc = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            unit.Books.Update(message.body);
                            unit.Save();
                            messagesQueue.Add(message);
                        }
                        break;
                    }
            }

            return Task.CompletedTask;
        }
    }
}
