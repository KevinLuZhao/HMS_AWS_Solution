using Hms.AwsConsole.BLL;
using System;
using Hms.AwsConsole.Model;
using System.Windows.Forms;

namespace Hms.AwsConsole
{
    public partial class FormLogs : FormMdiChildBase
    {
        public FormLogs()
        {
            InitializeComponent();
        }

        private void FormLogs_Load(object sender, EventArgs e)
        {
            gvLogs.DataSource = LogServices.GetLogList(LogType.Information.ToString(), GlobalVariables.Enviroment.ToString());
            gvLogs.Columns[0].Visible = gvLogs.Columns[4].Visible = false;
            gvLogs.Columns[1].Width = 70;
            gvLogs.Columns[2].Width = 80;
            gvLogs.Columns[3].Width = 750;
            gvLogs.Columns[5].Width = 120;
            gvLogs.Columns[6].Width = 120;
            gvLogs.Columns[1].HeaderText = "Type";
            gvLogs.Columns[2].HeaderText = "Environment";

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            gvLogs.DataSource = LogServices.GetLogList();
        }
    }
}
