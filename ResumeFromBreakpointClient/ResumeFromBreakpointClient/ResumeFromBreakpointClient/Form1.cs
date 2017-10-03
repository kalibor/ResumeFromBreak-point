using ResumeFromBreakpointClientApp.ResumeFromBreakpoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResumeFromBreakpointClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           this.s = new Thread(new ThreadStart(() => {
                ResumeFromBreakpointClient client = new ResumeFromBreakpointClient("http://tpdb.speed2.hinet.net/test_400m.zip", @"C:\Users\sf104137\Desktop\新增資料夾");
               client.s = new ResumeFromBreakpointClient.onFileLoad(onProgressLoad);
               client.Download();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {

           s.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            s.Interrupt();
        }

        private delegate void UpdateUICallBack(int max,int now, Control ctl);

        private void UpdateUI(int max, int now, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack uu = new UpdateUICallBack(UpdateUI);
                Invoke(uu, max, now, ctl);
            }
            else
            {
                if (now /max <1)
                {
                    this.progressBar1.Maximum = max;
                    this.progressBar1.Value = now;
                }
                else
                {
                    MessageBox.Show("下載完畢");
                }
     
            }
        }

        public void onProgressLoad(long now,long max)
        {
            UpdateUI((int)max,(int)now,progressBar1);
        }
        Thread s { get; set; }
    }
}
