using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hms.AwsConsole
{
    public partial class FormLogDetails : Form
    {
        Log logDetails;
        public FormLogDetails(Log log)
        {
            InitializeComponent();
            logDetails = log;
        }

        private void FormLogDetails_Load(object sender, EventArgs e)
        {
            lblDate.Text = logDetails.Date.ToString();
            lblKey.Text = logDetails.LogKey;
            txtMessage.Text = logDetails.Message;
            lblOwner.Text = logDetails.User;
            lblType.Text = logDetails.LogType.ToString();
        }
    }
}
