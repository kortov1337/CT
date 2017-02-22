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
using Dropbox.Api.Files.Routes;
using System.Text.RegularExpressions;

namespace Dropbox_API
{
    public partial class Form1 : Form
    {      
        static string token = "X0maJoW9_aAAAAAAAAAACZbATZubjd0vm52xwTvrROQEl-6Pc0yl-K-Gm9Bk5UYi";
        static DropboxClient client = new DropboxClient(token);
        static public List<string> namesCollection = new List<string>();
        static public List<string> namesCollection2 = new List<string>();
        ListFolderResult list;
        bool fromReady = false;
        bool toReady = false;
        string from = "";
        string to = "";

        public async void Delete(string path)
        {
            toolStripStatusLabel1.Text = "Delete in progress...";
            Metadata data = await client.Files.DeleteAsync(path);
            if(data!=null)
                toolStripStatusLabel1.Text = "Delete succeed!";
        }

        public int countSlashes(string path)
        {
            return new Regex("/").Matches(path).Count;
        }

        public string[] getChildParent(string path,int pos)
        {
            int slashes = countSlashes(path);
            string [] res=new string[slashes];
            int spos = path.LastIndexOf("/");
            
            res[1] = path.Substring(spos+1, path.Length-spos-1);
            path = path.Remove(spos , path.Length - spos);
            spos = path.LastIndexOf("/")+1;
            res[0] = path.Substring(spos , path.Length-spos);
            return res;
           
        }

        public string getNodeName(string path)
        {
            int pos = path.LastIndexOf("/");
            return path.Substring(pos + 1, path.Length - pos - 1);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void tabControl1_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = "Loading info...";
                FullAccount info = await client.Users.GetCurrentAccountAsync();
                if (info != null)
                    toolStripStatusLabel1.Text = "Loading info succeed!";
                detailInfoTB.Text = "Display name: " + info.Name.DisplayName + Environment.NewLine + "Surname name: " +
                info.Name.Surname + Environment.NewLine + "Email: " + info.Email + Environment.NewLine + "Account type: " +
                (info.AccountType.IsBasic ? "Basic" : info.AccountType.IsBusiness ? "Business" : info.AccountType.IsPro ? "Pro" : "Unknown")
                + Environment.NewLine + "Country: " + info.Country + Environment.NewLine + "Referal link: " + info.ReferralLink;
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            tabControl1_Click(sender, e);           
        }
        //get storage status
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
        //get all dirs
        private async void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Getting folders list ...";
            list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode [] root = treeView1.Nodes.Find("root", false);
            root[0].Nodes.Clear();
            comboBox1.Items.Clear();
            namesCollection.Clear();
            namesCollection.Add("/");
            
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                namesCollection.Add(item.PathDisplay);
                int pos = item.PathDisplay.LastIndexOf("/");
                if(pos==0)
                {
                    treeView1.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView1.EndUpdate();
                }
                else
                {
                    string [] names = getChildParent(item.PathDisplay, pos);
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
                      string[] names = getChildParent(item.PathDisplay, pos);

                      TreeNode[] node = root[0].Nodes.Find(names[0], true);
                     
                    if (node.Length==1)
                    {
                      treeView1.BeginUpdate();
                      node[0].Nodes.Add(names[1], names[1]);
                      treeView1.EndUpdate();
                    }
                    else
                    {

                    }
                }


              }
            treeView1.ExpandAll();
            foreach (string q in namesCollection)
            {
                comboBox1.Items.Add(q);
            }
            

        }
        //create dir
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
            catch(Exception err)
            {
                MessageBox.Show(err.Message,"Error", MessageBoxButtons.OK);
            }
        }
        //move dir
        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                from = treeView1.SelectedNode.FullPath;
                if(!from.Equals(string.Empty))
                { 
                 if(to=="/")
                        to+= treeView1.SelectedNode.Name;
                 else
                        to+="/" + treeView1.SelectedNode.Name;
                
                from = from.Remove(0, 4);
                from = from.Replace("\\", "/");
                Metadata data = list.Entries.First(q => q.Name == treeView2.SelectedNode.Name);
                if (data.IsFolder)
                   fromReady = true;
                else
                   MessageBox.Show("Choose directory, not file", "Error", MessageBoxButtons.OK);
                    fromReady = true;
                }
  
                if (fromReady&&toReady)
                {
                toolStripStatusLabel1.Text = "Moving directory...";
                Metadata data = await client.Files.MoveAsync(from, to, true, true);
                if (data != null)
                    toolStripStatusLabel1.Text = "Move directory succeed!";
                    button3_Click(sender, e);

                }
                else
                    MessageBox.Show("Choose were from in tree and were to move folder in dropdown list ", "Error", MessageBoxButtons.OK);

            }
            catch (NullReferenceException er)
            {
                MessageBox.Show("Choose were from in tree and were to move folder in dropdown list", "Error", MessageBoxButtons.OK);
            }
        }
        //delete dir
        private void button6_Click(object sender, EventArgs e)
        {
            string path = treeView1.SelectedNode.FullPath;
            if (!path.Equals(string.Empty)&& list.Entries.First(q => q.Name == treeView1.SelectedNode.Name).IsFolder)
            {
               /* Metadata md = ;
                if (md.IsFolder)
                {*/ 
                path = path.Remove(0, 4);
                path = path.Replace("\\", "/");
                Delete(path);
                button3_Click(sender, e);
                //}
            }
               
            else
                MessageBox.Show("Choose which dir to delete", "Error", MessageBoxButtons.OK);
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            to = comboBox1.SelectedItem.ToString();
            toReady = true;       
        }
      
        //get all files
        private async void button7_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Getting folders list ...";
            list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode[] root = treeView2.Nodes.Find("root", false);
            root[0].Nodes.Clear();
            namesCollection2.Clear();
            namesCollection2.Add("/");
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                namesCollection2.Add(item.PathDisplay);
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView2.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView2.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);
                    // parsePath(item.PathDisplay, root);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView2.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView2.EndUpdate();
                }
            }          

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView2.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView2.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);

                    TreeNode[] node = root[0].Nodes.Find(names[0], true);

                  //  if (node.Length == 1)
                    //{
                        treeView2.BeginUpdate();
                        node[0].Nodes.Add(names[1], names[1]);
                        treeView2.EndUpdate();
                  //  }
                  //  else
                  //  {

                    //}
                }


            }
            treeView2.ExpandAll();
            foreach (string q in namesCollection2)
            {
                comboBox2.Items.Add(q);
                comboBox3.Items.Add(q);
            }

        }
        //move file
        private async void button8_Click(object sender, EventArgs e)
        {
            try
            {
                from = treeView2.SelectedNode.FullPath;
                if (!from.Equals(string.Empty))
                {
                    if (to == "/")
                        to += treeView2.SelectedNode.Name;
                    else
                        to += "/" + treeView2.SelectedNode.Name;

                    from = from.Remove(0, 4);
                    from = from.Replace("\\", "/");
                    Metadata data = list.Entries.First(q => q.Name == treeView2.SelectedNode.Name);
                    if(data.IsFile)
                    fromReady = true;
                    else
                        MessageBox.Show("Choose file, not directory", "Error", MessageBoxButtons.OK);
                }

                if (fromReady && toReady)
                {
                    toolStripStatusLabel1.Text = "Moving file...";
                    Metadata data = await client.Files.MoveAsync(from, to, true, true);
                    if (data != null)
                        toolStripStatusLabel1.Text = "Move file succeed!";
                    button7_Click(sender, e);

                }
                else
                    MessageBox.Show("Choose were from in tree and were to move folder in dropdown list ", "Error", MessageBoxButtons.OK);

            }
            catch (NullReferenceException er)
            {
                MessageBox.Show("Choose were from in tree and were to move folder in dropdown list", "Error", MessageBoxButtons.OK);
            }
        }
        //delete file
        private void button9_Click(object sender, EventArgs e)
        {
            string path = treeView2.SelectedNode.FullPath;
            if (!path.Equals(string.Empty)&& list.Entries.First(q => q.Name == treeView2.SelectedNode.Name).IsFile)
            {
                path = path.Remove(0, 4);
                path = path.Replace("\\", "/");
                Delete(path);
                button7_Click(sender, e);
            }

            else
                MessageBox.Show("Choose which dir to delete", "Error", MessageBoxButtons.OK);
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            to = comboBox2.SelectedItem.ToString();
            toReady = true;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            to = comboBox3.SelectedItem.ToString();
            toReady = true;
        }
        //copy file
        private async void button10_Click(object sender, EventArgs e)
        {
            try
            {
                from = treeView2.SelectedNode.FullPath;
                if (!from.Equals(string.Empty))
                {
                    if (to == "/")
                        to += treeView2.SelectedNode.Name;
                    else
                        to += "/" + treeView2.SelectedNode.Name;

                    from = from.Remove(0, 4);
                    from = from.Replace("\\", "/");
                    Metadata data = list.Entries.First(q => q.Name == treeView2.SelectedNode.Name);
                    if (data.IsFile)
                        fromReady = true;
                    else
                        MessageBox.Show("Choose file, not directory", "Error", MessageBoxButtons.OK);
                }

                if (fromReady && toReady)
                {
                    toolStripStatusLabel1.Text = "Moving file...";
                    Metadata data = await client.Files.CopyAsync(from, to, true, true);
                    if (data != null)
                        toolStripStatusLabel1.Text = "Move file succeed!";
                    button7_Click(sender, e);

                }
                else
                    MessageBox.Show("Choose were from in tree and were to move folder in dropdown list ", "Error", MessageBoxButtons.OK);

            }
            catch (NullReferenceException er)
            {
                MessageBox.Show("Choose were from in tree and were to copy folder in dropdown list", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
