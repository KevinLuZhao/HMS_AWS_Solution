using Hms.AwsConsole.BLL;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using Hms.AwsConsole.Contros;

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

                PopulateData();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnCreateLevel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (isLevel1ApplicationInfraExisting())
                {
                    MessageBox.Show("Application Infrascture Level 1 is existing. If you want to create a new one, please delete the exising one first");
                    return;
                }
                var builder = new ApplicationsLevel1Builder(GlobalVariables.Enviroment.ToString(), this);
                NotifyToMainStatus("Creating Application Infrascture Level 1 begin.", System.Drawing.Color.Green);
                var response = await builder.Creat();
                WriteNotification("Application Infrascture Level 1 is created");
                PopulateData();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnDestroyLevel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLevel1ApplicationInfraExisting())
                {
                    MessageBox.Show("Delete failed. Application Infrascture Level 1 is not existing.");
                    return;
                }
                if (isLevel2ApplicationInfraExisting())
                {
                    MessageBox.Show("Delete failed. Please delete Application Infrascture Level 2 first.");
                    return;
                }
                var builder = new ApplicationsLevel1Builder(GlobalVariables.Enviroment.ToString(), this);
                NotifyToMainStatus("Deleting Application Infrascture Level 1 begin.", System.Drawing.Color.Green);
                await builder.Destroy(appEntities);
                WriteNotification("Application Infrascture Level 1 is deleted");
                PopulateData();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnCreateLevel2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLevel1ApplicationInfraExisting())
                {
                    MessageBox.Show("Please create Application Infrascture Level 1 first.");
                    return;
                }
                if (isLevel2ApplicationInfraExisting())
                {
                    MessageBox.Show("Application Infrastructure Level 2 is existing. If you want to create a new one, please delete the exising one first");
                    return;
                }
                var builder = new ApplicationsLevel2Builder(appEntities, GlobalVariables.Enviroment.ToString(), this);
                NotifyToMainStatus("Creating Application Infrastructure Level 2 begin.", System.Drawing.Color.Green);
                var response = await builder.Creat();
                WriteNotification("Application Infrastructure Level 2 is created");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnDestroyLevel2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLevel2ApplicationInfraExisting())
                {
                    MessageBox.Show("Delete failed. Application Infrastructure Level 2 is not existing.");
                    return;
                }
                var builder = new ApplicationsLevel2Builder(appEntities, GlobalVariables.Enviroment.ToString(), this);
                NotifyToMainStatus("Deleting Application Infrastructure Level 1 begin.", System.Drawing.Color.Green);
                await builder.Destroy(appEntities);
                WriteNotification("Application Infrastructure Level 1 is deleted");
                PopulateData();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnCreateRDS_Click(object sender, EventArgs e)
        {
            try
            {
                if (dbEntities != null)
                {
                    MessageBox.Show("RDS Infrastructure is existing. If you want to create a new one, please delete the exising one first");
                    return;
                }
                var builder = new DBLevel1InfraBuilder(GlobalVariables.Enviroment, this);
                NotifyToMainStatus("Creating RDS Infrastructure begin.", System.Drawing.Color.Green);
                var response = await builder.Creat();
                WriteNotification("RDS Infrastructure is created");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnDestroyRDS_Click(object sender, EventArgs e)
        {
            try
            {
                if (dbEntities == null)
                {
                    MessageBox.Show("Delete failed. There is no RDS Infrastructure to destroy.");
                    return;
                }
                var builder = new DBLevel1InfraBuilder(GlobalVariables.Enviroment, this);
                NotifyToMainStatus("Deleting RDS Infrastructure begin.", System.Drawing.Color.Green);
                var response = await builder.Delete(dbEntities);
                WriteNotification("RDS Infrastructure is deleted");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async void btnCreateVpcConnection_Click(object sender, EventArgs e)
        {
            var service = new VpcPeeringConnectionServices();
            var lstVpcs = await service.GetAvailablePeeringVpcList();
            await service.CreatePeeringConnection(
                lstVpcs.Find(o => o.VpcId == dbEntities.VpcId), 
                lstVpcs.Find(o => o.VpcId == appEntities.VpcId), 
                tsComboEnv.SelectedItem.ToString());
        }

        private void PopulateData()
        {
            var services = new InfraEntitiesServices();
            appEntities = services.GetApplicationInfraEntities(tsComboEnv.SelectedItem.ToString());

            dbEntities = services.GetDbInfraEntities(tsComboEnv.SelectedItem.ToString());
            LoadApplicationInfraStatus();
            LoadDbStatus();
        }

        private void LoadApplicationInfraStatus()
        {
            
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
            if (dbEntities != null)
            {
                lblInfraInfo.Text = $"VPC: {dbEntities.VpcId}, Subnet Group: {dbEntities.DBSubnetGoupId}, Instance: {dbEntities.DBInstanceId} were created";
            }
            else
            {
                lblInfraInfo.Text = "RDS Infrastructure is not created";
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

        private async Task LoadApplicationInstances()
        {
            if (appEntities != null && appEntities.Instances != null && appEntities.Instances.Count > 0)
            {
                var builder = new ApplicationsLevel2Builder(appEntities, GlobalVariables.Enviroment.ToString(), this);
                while (true)
                {
                    pnlApplicationInstances.Controls.Clear();
                    var instances = await builder.GetAllAppInstances(appEntities);
                    int counter = 0;
                    foreach (var instance in instances)
                    {
                        var ctrlControl = new Ctrl_ApplicationInstance();
                        ctrlControl.Location = new System.Drawing.Point( 10 + 200 * counter, 10);
                        ctrlControl.UpdateUI(instance);
                        pnlApplicationInstances.Controls.Add(ctrlControl);
                    }
                    if (instances.FindIndex(o=>o.state != "available")>=0)
                    {
                        await Task.Delay(30000);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private bool isLevel1ApplicationInfraExisting()
        {
            return appEntities != null;
        }

        private bool isLevel2ApplicationInfraExisting()
        {
            if (!isLevel1ApplicationInfraExisting())
                return false;
            if (appEntities.Instances == null || appEntities.Instances.Count == 0)
                return false;
            else
                return true;
        }

        private bool isRdsInfraExisting()
        {
            return dbEntities != null;
        }

        private void SetButtons()
        {

        }
    }
}
