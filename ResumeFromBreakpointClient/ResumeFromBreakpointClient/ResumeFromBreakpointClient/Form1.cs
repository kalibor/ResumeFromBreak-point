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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("網址或下載資料夾不得為空");
            }
            else
            {
                if (s == null || !s.IsAlive)
                {
                    this.s = new Thread(new ThreadStart(() => {

                        ResumeFromBreakpointClient client = new ResumeFromBreakpointClient(textBox2.Text, textBox1.Text);
                        client.fileloading = new onFileLoading(onProgressLoad);
                        client.fileloaded = new onFileLoaded(onLoaded);
                        client.Download();
                    }));
                    s.Start();
                }
                else
                {
                    Monitor.Exit(s);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Monitor.Enter(s);
            onLoaded();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dialog = this.folderBrowserDialog1.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                textBox1.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }


        private void UpdateUI(int max, int now, Control ctl)
        {
            if (InvokeRequired)
            {
                UpdateUICallBack uu = new UpdateUICallBack(UpdateUI);
                Invoke(uu, max, now, ctl);
            }
            else
            {
                this.progressBar1.Maximum = max;
                this.progressBar1.Value = now;
   
            }
        }


        public void onProgressLoad(long now,long max)
        {
            UpdateUI((int)max,(int)now,progressBar1);
        }

        public void onLoaded()
        {
            MessageBox.Show("下載完畢");
            if (s.IsAlive)
            {
                s.Abort();
            }
        }

        Thread s { get; set; }

        private delegate void UpdateUICallBack(int max, int now, Control ctl);
    }
}
