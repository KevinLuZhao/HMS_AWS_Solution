using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.BLL;

namespace Hms.AwsConsole
{
    public partial class FormMain : Form, IWindowForm
    {
        public FormMain()
        {
            InitializeComponent();
        }

        public void ShowCallbackMessage(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtMonitor.AppendText(DateTime.Now.ToString() + "\t");
                txtMonitor.AppendText(message + Environment.NewLine);
            });
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                InfraBuilder builder = new InfraBuilder();
                button1.Click += new EventHandler(async (s, arg) => await builder.CreateNewInfrastructure("QA", this));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Stack trace: " + ex.StackTrace);
            }
        }      
    }
}
