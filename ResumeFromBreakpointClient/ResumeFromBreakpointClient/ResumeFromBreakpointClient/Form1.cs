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
                ResumeFromBreakpointClient client = new ResumeFromBreakpointClient("http://localhost:64994/api/Home", @"C:\Users\sf104137\Desktop\新增資料夾");

                client.Download();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
         

         s.Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            s.Abort();
        }

        Thread s { get; set; }
    }
}
