using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dropbox.Api;
using Dropbox.Api.Users;
using Dropbox.Api.Files;
namespace Dropbox_API
{
    public partial class Form1 : Form
    {      
        static string token = "X0maJoW9_aAAAAAAAAAACZbATZubjd0vm52xwTvrROQEl-6Pc0yl-K-Gm9Bk5UYi";
        static DropboxClient client = new DropboxClient(token);

        public string[] getNodeName(string path,int pos)
        {
            string [] res=new string[2];
            //path = path.Remove(0, 1);
            int spos = path.LastIndexOf("/");
            //if (spos >= 0)
         //   {
                res[1] = path.Substring(spos+1, path.Length-spos-1);
           // }
            path = path.Remove(spos , path.Length - spos);
            spos = path.LastIndexOf("/")+1;
            res[0] = path.Substring(spos , path.Length-spos);
            return res;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
          FullAccount info = await client.Users.GetCurrentAccountAsync();
            detailInfoTB.Text = info.Name.DisplayName +Environment.NewLine+ info.Email;
        }

        private async void tabControl1_Click(object sender, EventArgs e)
        {
            FullAccount info = await client.Users.GetCurrentAccountAsync();
            detailInfoTB.Text = "Display name: " + info.Name.DisplayName + Environment.NewLine + "Surname name: " + 
            info.Name.Surname + Environment.NewLine + "Email: " + info.Email + Environment.NewLine +"Account type: " + 
            (info.AccountType.IsBasic?"Basic":info.AccountType.IsBusiness?"Business":info.AccountType.IsPro?"Pro":"Unknown")
            +Environment.NewLine + "Country: " + info.Country + Environment.NewLine + "Referal link: " + info.ReferralLink;
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            tabControl1_Click(sender, e);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            SpaceUsage space = await client.Users.GetSpaceUsageAsync();
            spaceInfo.Text = "Used space: " + (space.Used/1000000).ToString() + "MB" + Environment.NewLine + "Free space: "+
            (space.Allocation.AsIndividual.Value.Allocated- space.Used)/1000000 + "MB" + Environment.NewLine + "Allocated space: " +
            (space.Allocation.AsIndividual.Value.Allocated/1000000).ToString() + "MB";
        }

        private async void button3_Click(object sender, EventArgs e)
        {
             ListFolderResult list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            string tmp="Folder1";
            TreeNode [] root = treeView1.Nodes.Find("root", false);
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if(pos==0)
                {
                    treeView1.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView1.EndUpdate();
                }
                else
                {
                    string [] names = getNodeName(item.PathDisplay, pos);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView1.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView1.EndUpdate();
                }
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView1.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView1.EndUpdate();
                }
                else
                {
                    string[] names = getNodeName(item.PathDisplay, pos);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView1.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView1.EndUpdate();
                }
                // TreeNode [] q = root[0].Nodes.Find("Folder1", false);
                //treeView1.Nodes.Find(tmp,false);
                //   new TreeNode("sad");
                // q.n
                // int w = 0;
                // Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }
    }
}
