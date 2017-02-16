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
            int spos = path.LastIndexOf("/");
            res[1] = path.Substring(spos+1, path.Length-spos-1);
            path = path.Remove(spos , path.Length - spos);
            spos = path.LastIndexOf("/")+1;
            res[0] = path.Substring(spos , path.Length-spos);
            return res;
        }

        public Form1()
        {
            InitializeComponent();
        }

     

        private async void tabControl1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading info...";
            FullAccount info = await client.Users.GetCurrentAccountAsync();
            if (info != null)
                toolStripStatusLabel1.Text = "Loading info succeed!";
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
            toolStripStatusLabel1.Text = "Refreshing storage space status...";
            SpaceUsage space = await client.Users.GetSpaceUsageAsync();
            if (space != null)
                toolStripStatusLabel1.Text = "Refreshing succeed!";
            spaceInfo.Text = "Used space: " + (space.Used/1000000).ToString() + "MB" + Environment.NewLine + "Free space: "+
            (space.Allocation.AsIndividual.Value.Allocated- space.Used)/1000000 + "MB" + Environment.NewLine + "Allocated space: " +
            (space.Allocation.AsIndividual.Value.Allocated/1000000).ToString() + "MB";
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Getting folders list ...";
            ListFolderResult list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode [] root = treeView1.Nodes.Find("root", false);
            root[0].Nodes.Clear();
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
                treeView1.ExpandAll();
               
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                CreateDir cd = new CreateDir();
                string path = treeView1.SelectedNode.FullPath;
                path = path.Replace("\\", "/");
                path = path.Remove(0, 4);
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    if(cd.richTextBox1.Text.Equals(string.Empty))
                        MessageBox.Show("Directory name cant be empty", "Error", MessageBoxButtons.OK);
                    else
                    path +="/" + cd.richTextBox1.Text;
                }
                toolStripStatusLabel1.Text = "Creating directory...";
                FolderMetadata metadata = await client.Files.CreateFolderAsync(new CreateFolderArg(path));
                if(!metadata.Name.Equals(string.Empty))
                    toolStripStatusLabel1.Text = "Creating directory succeed!";
                button3_Click(sender, e);
                //cd.Close();
                cd.Dispose();
            }
            catch(NullReferenceException er)
            {
              
                MessageBox.Show("Choose branch were to create dir first ", "Error", MessageBoxButtons.OK);
            }

        }
    }
}
