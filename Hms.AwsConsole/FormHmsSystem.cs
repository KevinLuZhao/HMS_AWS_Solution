using Hms.AwsConsole.BLL;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Hms.AwsConsole
{
    public partial class FormHmsSystem : FormMdiChildBase, IWindowForm
    {
        public FormHmsSystem()
        {
            InitializeComponent();
        }
        public void ShowCallbackMessage(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtMonitor.AppendText(DateTime.Now.ToString() + "\t");
                txtMonitor.AppendText(message + System.Environment.NewLine);
            });
        }

        private void tsComboEnv_SelectedIndexChanged(object sender, EventArgs e)
        {
            GlobalVariables.Enviroment = (Model.Environment)Enum.Parse(typeof(Model.Environment), tsComboEnv.SelectedItem.ToString(), true);
            //GlobalVariables.Region = GlobalVariables.EnvironmentAccounts[GlobalVariables.Enviroment.ToString()].Region;
            tsComboRegion.SelectedItem = Regions.GetRegionList().Find(o => o.Key == GlobalVariables.Region);
        }

        private void tsComboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormHmsSystem_Load(object sender, EventArgs e)
        {
            tsComboEnv.ComboBox.DataSource = Enum.GetValues(typeof(Model.Environment));
            tsComboEnv.SelectedIndex = 1;
            //tsComboColor.ComboBox.DataSource = Enum.GetValues(typeof(Model.Color));
            //tsComboColor.SelectedIndex = 0;

            List<KeyValuePair<string, string>> lstRegions = Regions.GetRegionList();
            tsComboRegion.ComboBox.DataSource = lstRegions;
            tsComboRegion.ComboBox.DisplayMember = "Value";
            tsComboRegion.ComboBox.ValueMember = "Key";
            tsComboRegion.ComboBox.SelectedIndex = 1;
            tsComboRegion.Enabled = false;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                InfraBuilder builder = new InfraBuilder();
                btnCreate.Click += new EventHandler(
                    async (s, arg) => await builder.CreateNewInfrastructure(GlobalVariables.Enviroment.ToString(), this));
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(
                    ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, tsComboEnv.SelectedItem.ToString());
            }
        }
    }
}
