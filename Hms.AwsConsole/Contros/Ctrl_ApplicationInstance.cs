using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.Contros
{
    public partial class Ctrl_ApplicationInstance : UserControl
    {
        string Name;
        string Identifier;
        string PublicIP;
        string Status;

        public Ctrl_ApplicationInstance()
        {
            InitializeComponent();
        }

        public void UpdateUI(AwsAppInstance instance)
        {
            if (instance != null)
            {
                lblID.Text = instance.Name;
                lblName.Text = instance.Name;
                lblPrivateIP.Text = instance.PrivateIP;
                lblPublicIp.Text = instance.PublicIP;
                lblStatus.Text = instance.state;
            }
            else {
                lblName.Text = "Not available";
            }
        }
    }
}
