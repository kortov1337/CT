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
        static string operation = "",body="";
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
            try
            {
                var b = context.Body;
                var message = BusStuff.Deserialize(context.Body);
                operation = message.OperationType;
                body = message.ToString();
                switch (message.OperationType)
                {
                    case "Create":
                        {
                            using (TransactionScope sc = new TransactionScope(TransactionScopeOption.Suppress))
                            {
                                unit.Books.Create(message.body);
                                unit.Save();
                                messagesQueue.Add(message);
                                unit.Books.TrackEvent("Create item on Sub1", new Dictionary<string, string>
                                {
                                    { "Operation", "Title" },
                                    { "Create", message.body.title }
                                });
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
                                unit.Books.TrackEvent("Delete item on Sub1", new Dictionary<string, string>
                                {
                                    { "Operation", "Title" },
                                    { "Delete", message.body.title }
                                });
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
                                unit.Books.TrackEvent("Update item on Sub1", new Dictionary<string, string>
                                {
                                    { "Operation", "Title" },
                                    { "Update", message.body.title }
                                });
                            }
                            break;
                        }
                }

                return Task.CompletedTask;
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message);
                BusStuff.LogCall("Operation " + operation + " for " + body + " failed");
                unit.Books.TrackException(er);
               
                return Task.CompletedTask;
            }
        }
    }
}
