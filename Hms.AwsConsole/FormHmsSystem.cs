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
        ApplicationInfraEntities appEntities;
        DBInfraEntities dbEntities;
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
            GlobalVariables.Enviroment = 
                (Model.Environment)Enum.Parse(typeof(Model.Environment), 
                tsComboEnv.SelectedItem.ToString(), true);
            //GlobalVariables.Region = GlobalVariables.EnvironmentAccounts[GlobalVariables.Enviroment.ToString()].Region;
            tsComboRegion.SelectedItem = Regions.GetRegionList().Find(o => o.Key == GlobalVariables.Region);
        }

        private void tsComboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormHmsSystem_Load(object sender, EventArgs e)
        {
            try
            {
                ApplicationsInfraBuilder applicationBuilder = new ApplicationsInfraBuilder();
                btnCreate.Click += new EventHandler(
                    async (s, arg) => await applicationBuilder.CreateNewInfrastructure
                    (1, GlobalVariables.Enviroment.ToString(), this));

                DBInfraBuilder dbBuilder = new DBInfraBuilder();
                btnCreateRDS.Click += new EventHandler(
                    async (s, arg) => await dbBuilder.CreateNewInfrastructure
                    (GlobalVariables.Enviroment.ToString(), this));

                tsComboEnv.ComboBox.DataSource = Enum.GetValues(typeof(Model.Environment));
                //tsComboEnv.SelectedIndex = 1;
                //tsComboColor.ComboBox.DataSource = Enum.GetValues(typeof(Model.Color));
                //tsComboColor.SelectedIndex = 0;

                List<KeyValuePair<string, string>> lstRegions = Regions.GetRegionList();
                tsComboRegion.ComboBox.DataSource = lstRegions;
                tsComboRegion.ComboBox.DisplayMember = "Value";
                tsComboRegion.ComboBox.ValueMember = "Key";
                tsComboRegion.ComboBox.SelectedIndex = 1;
                tsComboRegion.Enabled = false;

                AMIServices service = new AMIServices();
                List<ImageModel> lstAMIs = service.GetAMIs(tsComboEnv.SelectedItem.ToString());
                ddlWebServerAMI.DataSource = lstAMIs;
                ddlWebServerAMI.DisplayMember = "Name";
                ddlWebServerAMI.ValueMember = "AmiId";

                LoadApplicationStatus();
                LoadDbStatus();
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, 
                    LogType.Error, tsComboEnv.SelectedItem.ToString());
            }
        }

        private void LoadApplicationStatus()
        {
            var services = new InfraEntitiesServices();
            appEntities = services.GetApplicationInfraEntities(tsComboEnv.SelectedItem.ToString());
            if (appEntities != null)
            {
                label1.Text = $"VPC: {appEntities.VpcId}, Public Subnet: {appEntities.PublicSubnetId}, Private Subnet: {appEntities.PrivateSubnetId} were created";
            }
            else
            {
                label1.Text = "Application Infrastructure is not created";
            }
        }

        private void LoadDbStatus()
        {
            var services = new InfraEntitiesServices();
            dbEntities = services.GetDbInfraEntities(tsComboEnv.SelectedItem.ToString());
            if (dbEntities != null)
            {
                lblInfraInfo.Text = $"VPC: {dbEntities.VpcId}, Subnet Group: {dbEntities.DBSubnetGoupId}, Instance: {dbEntities.DBInstanceId} were created";
            }
            else
            {
                lblInfraInfo.Text = "RDS infrastructure is not created";
            }
            var instanceServices = new DBInstanceServices(tsComboEnv.SelectedItem.ToString(), tsComboRegion.SelectedItem.ToString());
            var instance = instanceServices.GetDBInstance();
            if (instance != null)
            {
                panel1.Show();
                lblDbInstanceId.Text = instance.DBInstanceIdentifier;
                lblDbInstanceEndpoint.Text = instance.Endpoint;
                //lblDbInstancePort.Text = instance.Port.ToString();
                lblDbInstanceStatus.Text = instance.Status;
            }
            else
            {
                panel1.Hide();
            }
        }

        private void btnCreateVpcConnection_Click(object sender, EventArgs e)
        {
            var service = new VpcConnectionServices();
            service.CreateVpcPeeringConnection(dbEntities.VpcId, appEntities.VpcId, tsComboEnv.SelectedItem.ToString(), this);
        }
    }
}
