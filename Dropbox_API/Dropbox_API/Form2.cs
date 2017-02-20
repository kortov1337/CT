using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dropbox_API
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 form = (Form1)Owner;
            TreeNodeCollection collection = form.treeView1.Nodes;
            TreeNode [] root = form.treeView1.Nodes.Find("root",true);
            foreach (TreeNode q in  root[0].Nodes)
            {
              
                comboBox1.Items.Add(q.Text);
            }
            
        }
    }
}
