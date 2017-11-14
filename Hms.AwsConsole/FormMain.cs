using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.BLL;

namespace Hms.AwsConsole
{
    public partial class FormMain : Form
    {
        public ToolStripStatusLabel MainStatusStrip;
        public List<FormMdiChildBase> OpendFormList = new List<FormMdiChildBase>();
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {           
            MainStatusStrip = toolStripStatusLabel1;
        }

        private void OpenFormMenu_Click(object sender, EventArgs e)
        {
            //If the form is already opened, just make it active.
            foreach (var form in OpendFormList)
            {
                if (form.GetType().Name == ((ToolStripMenuItem)sender).Tag.ToString())
                {
                    form.Activate();
                    return;
                }
            }
            FormMdiChildBase frm = OpenMDIChildForm(((ToolStripMenuItem)sender).Tag.ToString());
            if (frm != null)
            {
                OpendFormList.Add(frm);
                frm.MdiParent = this;
                //frm.Activate();
                frm.Show();
                frm.WindowState = FormWindowState.Maximized;
            }
        }

        private FormMdiChildBase OpenMDIChildForm(string formName)
        {
            FormMdiChildBase frm = (FormMdiChildBase)(Activator.CreateInstance(this.AccessibleName, "Hms.AwsConsole."+formName).Unwrap());
            return frm;
        }
    }
}
