using Hms.AwsConsole.BLL;
using System;
using Hms.AwsConsole.Model;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Hms.AwsConsole
{
    public partial class FormLogs : FormMdiChildBase
    {
        List<Model.Log> dataSource;
        public FormLogs()
        {
            InitializeComponent();
        }

        private void FormLogs_Load(object sender, EventArgs e)
        {
            List<string> lstEnv = new List<string>(Enum.GetNames(typeof(Model.Environment)));
            lstEnv.Insert(0, string.Empty);
            ddlEnvironment.DataSource = lstEnv;
            ddlEnvironment.SelectedItem = GlobalVariables.Enviroment.ToString();

            List<string> lstLogType = (Enum.GetNames(typeof(Model.LogType))).OfType<string>().ToList();
            lstLogType.Insert(0, string.Empty);
            ddlType.DataSource = lstLogType;

            BindData();
            gvLogs.Columns[0].Visible = gvLogs.Columns[4].Visible = false;
            gvLogs.Columns[1].Width = 70;
            gvLogs.Columns[2].Width = 80;
            gvLogs.Columns[3].Width = 750;
            gvLogs.Columns[5].Width = 120;
            gvLogs.Columns[6].Width = 120;
            gvLogs.Columns[1].HeaderText = "Type";
            gvLogs.Columns[2].HeaderText = "Environment";

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void gvLogs_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var log = dataSource.Find(o => o.Id == gvLogs.Rows[e.RowIndex].Cells[0].Value.ToString());
            var form = new FormLogDetails(log);
            form.ShowDialog();
        }

        private void BindData()
        {
            gvLogs.DataSource = dataSource = LogServices.GetLogList(
                ddlEnvironment.SelectedItem.ToString(),
                ddlType.SelectedItem.ToString(),
                txtMessage.Text);
        }
    }
}
