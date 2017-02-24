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
using Dropbox.Api.Sharing;
using Dropbox.Api.Files.Routes;
using System.Text.RegularExpressions;
using System.IO;
using AutoMapper.Mappers;
namespace Dropbox_API
{
    public partial class Form1 : Form
    {      
        static string token = "X0maJoW9_aAAAAAAAAAACZbATZubjd0vm52xwTvrROQEl-6Pc0yl-K-Gm9Bk5UYi";
        static DropboxClient client = new DropboxClient(token);
        static public List<string> namesCollection = new List<string>();
        static public List<string> namesCollection2 = new List<string>();
        ListFolderResult list;
        ListSharedLinksResult sharedLinks;
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

        public string getFileName(string path)
        {
            int pos = path.LastIndexOf("\\");
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
                string selectedNode = treeView1.SelectedNode.Name;
                if (!from.Equals(string.Empty))
                { 
                 if(to=="/")
                        to+= treeView1.SelectedNode.Name;
                 else
                        to+="/" + treeView1.SelectedNode.Name;
                
                from = from.Remove(0, 4);
                from = from.Replace("\\", "/");
                Metadata data = list.Entries.First(q => q.Name == selectedNode);
                if (data.IsFolder)
                   fromReady = true;
                else
                   MessageBox.Show("Choose directory, not file", "Error", MessageBoxButtons.OK);
                    
                }
  
                if (fromReady&&toReady)
                {
                    try
                    {
                        toolStripStatusLabel1.Text = "Moving directory...";
                        Metadata data = await client.Files.MoveAsync(from, to, true, true);
                        if (data != null)
                           toolStripStatusLabel1.Text = "Move directory succeed!";
                           button3_Click(sender, e);
                    }
                    catch(Exception er)
                    {
                        MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK);
                    }

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
                try
                {             
                path = path.Remove(0, 4);
                path = path.Replace("\\", "/");
                Delete(path);
                button3_Click(sender, e);
                }
                catch(Exception er)
                {
                    MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK);
                }
               
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
        //download file
        private async void button11_Click(object sender, EventArgs e)
        {
            from = treeView2.SelectedNode.FullPath;
            if (!from.Equals(string.Empty))
            {
                from = from.Remove(0, 4);
                from = from.Replace("\\", "/");
                
                var fileData = await client.Files.DownloadAsync(from);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName=getNodeName(from);
               // sfd.Filter = "Text Files | *.txt PDF Files | *.pdf, MP3 | *.mp3 ";
                sfd.ShowDialog();
                if(!sfd.FileName.Equals(string.Empty)&& list.Entries.First(q => q.Name == treeView2.SelectedNode.Name).IsFile)
                {
                    toolStripStatusLabel1.Text = "Downloading file...";
                    byte[] data = await fileData.GetContentAsByteArrayAsync();
                    toolStripStatusLabel1.Text = "Saving file...";
                    File.WriteAllBytes(sfd.FileName, data);
                    toolStripStatusLabel1.Text = "Saving succeed!";
                }
            }
            else
                MessageBox.Show("Choose file, not directory", "Error", MessageBoxButtons.OK);
        }
        //upload file
        private async void button12_Click(object sender, EventArgs e)
        {
            from = treeView2.SelectedNode.FullPath;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            
            if (!from.Equals(string.Empty)&&!ofd.FileName.Equals(string.Empty))
            {
                from = from.Remove(0, 4);
                from = from.Replace("\\", "/");
                from += "/"+Path.GetFileName(ofd.FileName);
                var mem = new MemoryStream(Encoding.UTF8.GetBytes(ofd.FileName));
                toolStripStatusLabel1.Text = "Uploading file...";
                var upload = await client.Files.UploadAsync(from, WriteMode.Overwrite.Instance, body: mem);
                if(upload!=null)
                    toolStripStatusLabel1.Text = "Upload succeed!";
                else
                    toolStripStatusLabel1.Text = "Upload failed!";
                button7_Click(sender, e);
            }

        }
        //get all for info
        private async void button13_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Getting folders list ...";
            list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode[] root = treeView3.Nodes.Find("root", false);
            root[0].Nodes.Clear();
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView3.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView3.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);
                    // parsePath(item.PathDisplay, root);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView3.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView3.EndUpdate();
                }
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView3.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView3.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);

                    TreeNode[] node = root[0].Nodes.Find(names[0], true);

                    //  if (node.Length == 1)
                    //{
                    treeView3.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView3.EndUpdate();
                    //  }
                    //  else
                    //  {

                    //}
                }


            }
            treeView3.ExpandAll();
        }
        //get file-dir info
        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {
            richTextBox1.Text = "";
            Metadata data = list.Entries.First(q => q.Name == treeView3.SelectedNode.Name);
            richTextBox1.Text += "Name: " + data.Name + Environment.NewLine + "Path: " + data.PathDisplay +  Environment.NewLine +
             "Folder/file" + (data.IsFile ? "is file" : data.IsFolder ? "is folder" : "");
        }
        //get all for shared stuff
        private async void button14_Click(object sender, EventArgs e)
        {

            toolStripStatusLabel1.Text = "Getting folders list ...";
            list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode[] root = treeView4.Nodes.Find("root", false);
            root[0].Nodes.Clear();
            comboBox1.Items.Clear();
            namesCollection.Clear();
            namesCollection.Add("/");

            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                namesCollection.Add(item.PathDisplay);
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView4.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView4.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView4.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView4.EndUpdate();
                }
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView4.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView4.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);

                    TreeNode[] node = root[0].Nodes.Find(names[0], true);

                    if (node.Length == 1)
                    {
                        treeView4.BeginUpdate();
                        node[0].Nodes.Add(names[1], names[1]);
                        treeView4.EndUpdate();
                    }
                    else
                    {

                    }
                }
            }
            treeView4.ExpandAll();
        }
        //list shared
        private async void button15_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            sharedLinks = await client.Sharing.ListSharedLinksAsync();
            foreach(SharedLinkMetadata q in sharedLinks.Links)
            {
                richTextBox2.Text += "Name: " + q.Name + Environment.NewLine + "Path: " + q.PathLower + Environment.NewLine +
              "Folder/file " + (q.IsFile ? "is file" : q.IsFolder ? "is folder" : "");
                
            }
           
        }
        //share
        private async void button16_Click(object sender, EventArgs e)
        {
            try
            {
                string path = treeView4.SelectedNode.FullPath;
                path = path.Remove(0, 4);
                path = path.Replace("\\", "/");
                toolStripStatusLabel1.Text = "Sharing file...";
                var sharedMetadata = await client.Sharing.CreateSharedLinkWithSettingsAsync(path);
                textBox2.Text = sharedMetadata.Url;
                button15_Click(sender, e);
                toolStripStatusLabel1.Text = "Share succeed!";
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK);
            }
        }
        //unshare
        private async void button17_Click(object sender, EventArgs e)
        {
            string url = sharedLinks.Links.First(q => q.Name == treeView4.SelectedNode.Name).Url;
            toolStripStatusLabel1.Text = "Unsharing file...";
            await client.Sharing.RevokeSharedLinkAsync(url);
            button15_Click(sender, e);
            toolStripStatusLabel1.Text = "Unshare succeed!";
        }
        //search
        private async void button18_Click(object sender, EventArgs e)
        {
            string path = treeView5.SelectedNode.FullPath;
            path = path.Remove(0, 4);
            path = path.Replace("\\", "/");
            string query = textBox3.Text;
            SearchMode searchMode = checkBox1.Checked
                ? SearchMode.FilenameAndContent.Instance
                : (SearchMode)SearchMode.Filename.Instance;

            var files = await client.Files.SearchAsync(path, query, 0, 10, searchMode);
            
            foreach (var file in files.Matches)
            {
                richTextBox3.Text = "Name: " + file.Metadata.Name + Environment.NewLine + 
                    "Path: " + file.Metadata.PathDisplay;
            }
        }

        private async void button19_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Getting folders list ...";
            list = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, true));
            if (list != null)
                toolStripStatusLabel1.Text = "Getting folders list succeed!";
            TreeNode[] root = treeView5.Nodes.Find("root", false);
            root[0].Nodes.Clear();

            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView5.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView5.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);
                    TreeNode[] node = root[0].Nodes.Find(names[0], true);
                    treeView5.BeginUpdate();
                    node[0].Nodes.Add(names[1], names[1]);
                    treeView5.EndUpdate();
                }
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                int pos = item.PathDisplay.LastIndexOf("/");
                if (pos == 0)
                {
                    treeView5.BeginUpdate();
                    root[0].Nodes.Add(item.Name, item.Name);
                    treeView5.EndUpdate();
                }
                else
                {
                    string[] names = getChildParent(item.PathDisplay, pos);

                    TreeNode[] node = root[0].Nodes.Find(names[0], true);

                    if (node.Length == 1)
                    {
                        treeView5.BeginUpdate();
                        node[0].Nodes.Add(names[1], names[1]);
                        treeView5.EndUpdate();
                    }
                    else
                    {

                    }
                }
            }
            treeView5.ExpandAll();
        }
    }
}
