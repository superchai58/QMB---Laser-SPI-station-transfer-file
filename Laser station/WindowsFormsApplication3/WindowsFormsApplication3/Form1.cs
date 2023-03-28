using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Timers;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        //--superChai Add (Begin)--
        System.Timers.Timer t;
        int h = 0, m = 0, s = 10;
        //--superChai Add (End)--

        //static System.Windows.Forms.Timer SendRequest_Timer = new System.Windows.Forms.Timer();
        //static System.Windows.Forms.Timer GetResult_Timer = new System.Windows.Forms.Timer();
        public string Line = null;
        public string ip = null;

        public Form1()
        {
            InitializeComponent();
            DataTable Line = GetDataLine();
            int countRow = Line.Rows.Count;
            for(int i=0;i< countRow; i++)
            {
                cboLine.Items.Insert(i, Line.Rows[i]["Line"].ToString().Trim());
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboLine.Text == "")
            {
                MessageBox.Show("Please select Line!");
                return;
            }

            //--superchai Add (Begin)--
            btnOK.Enabled = false;
            cboLine.Enabled = false;
            //--superchai Add (End)--

            Line = cboLine.Text;
            DataTable IP = GetIP();
            ip = IP.Rows[0]["Value"].ToString().Trim();

            //--superchai Add (Begin)--
            t.Start();
           
            //SendRequest_Timer.Tick += new EventHandler(SendRequest_Timer_Tick);
            //SendRequest_Timer.Interval = 10000;
            //SendRequest_Timer.Start();

            //label1.Text = SendRequest_Timer.Interval.ToString();

            //GetResult_Timer.Tick += new EventHandler(GetResult_Timer_Tick);
            //GetResult_Timer.Interval = 10000;
            //GetResult_Timer.Start();
        }

        //--superchai Add (Begin)--
        private void OntimeEvent(object sender, ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                s -= 1;
                if (s == 0)
                {
                    //--Send Request file (Begin)--
                    string LcoalPath = string.Empty;
                    string MachinePath = string.Empty;
                    LcoalPath = "D:\\SHOPFLOOR\\Request";
                    MachinePath = "\\\\" + ip + "\\SHOPFLOOR\\Request";
                    //MachinePath = "D:\\SHOPFLOOR\\Request2nd";
                    try
                    {
                        //--Check directory (Begin)--
                        if (!Directory.Exists(LcoalPath))
                        {
                            Directory.CreateDirectory(LcoalPath);
                        }
                        if (!Directory.Exists(MachinePath))
                        {
                            Directory.CreateDirectory(MachinePath);
                        }
                        //--Check directory (End)--

                        if (!File.Exists(MachinePath + "\\Request.txt"))
                        {
                            if (File.Exists(LcoalPath + "\\Request.txt"))
                            {
                                File.Move(LcoalPath + "\\Request.txt", MachinePath + "\\Request.txt");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please check the Request folder at " + LcoalPath + " OR " + MachinePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        setDefaultTime();

                        return;
                    }
                    //--Send Request file (End)--

                    //--Get Result file (Begin)--
                    LcoalPath = string.Empty;
                    MachinePath = string.Empty;
                    LcoalPath = "D:\\SHOPFLOOR\\Result";
                    MachinePath = "\\\\" + ip + "\\SHOPFLOOR\\Result";
                    //MachinePath = "D:\\SHOPFLOOR\\Result2nd";
                    try
                    {
                        //--Check directory (Begin)--
                        if (!Directory.Exists(LcoalPath))
                        {
                            Directory.CreateDirectory(LcoalPath);
                        }
                        if (!Directory.Exists(MachinePath))
                        {
                            Directory.CreateDirectory(MachinePath);
                        }
                        //--Check directory (End)--

                        foreach (string file in Directory.EnumerateFiles(MachinePath))
                        {
                            string destFile = Path.Combine(LcoalPath, Path.GetFileName(file));
                            if (!File.Exists(destFile))
                            {
                                File.Move(file, destFile);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Please check the Result folder at " + LcoalPath + " OR " + MachinePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                        setDefaultTime();
                        return;
                    }
                    //--Get Result file (Begin)--
                    transLogEQ();
                    s = 10;
                    //m += 1;
                }
                //if (m == 60)
                //{
                //    m = 0;
                //    h += 1;
                //}
                lbTime.Text = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0') 
                              , s.ToString().PadLeft(2, '0'));
            }));
        }
        //--superchai Add (End)--

        public DataTable GetDataLine()
        {
            DataTable dtTmp = new DataTable();
            connecDB conn = new connecDB();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = "select Line from LineMantain";
                dtTmp = conn.Query(cmd);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString().Trim();
                MessageBox.Show(strEx, "Error catch !!!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return dtTmp;
        }

        public DataTable GetIP()
        {
            DataTable dtTmp = new DataTable();
            connecDB conn = new connecDB();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = "select [Value] from mesPE_ProConfig where Station = 'MB2DSN_Laser' and Line = 'D34' and [Key] = 'MachineIP'";
                dtTmp = conn.Query(cmd);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString().Trim();
                MessageBox.Show(strEx, "Error catch !!!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return dtTmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //--superchai Add (Begin)--
            t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += OntimeEvent;
            //--superchai Add (End)--
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //--superchai Add (Begin)--
            t.Stop();
            s = 10;

            if (MessageBox.Show("Do you want to close the program ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.DoEvents();
            }
            else
            {
                e.Cancel = true;
                setDefaultTime();
            }
            //--superchai Add (End)--
        }

        public void setDefaultTime()
        {
            cboLine.Enabled = true;
            btnOK.Enabled = true;
            t.Stop();

            h = 0;
            m = 0;
            s = 10;
            lbTime.Text = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0')
                              , s.ToString().PadLeft(2, '0'));
        }

        public void transLogEQ()
        {
            string LcoalPath = string.Empty;
            string MachinePath = string.Empty;
            LcoalPath = string.Empty;
            MachinePath = string.Empty;
            LcoalPath = "D:\\WST\\LC800\\ErrLog\\2023";
            MachinePath = "\\\\" + ip + "\\WST\\LC800\\ErrLog\\2023";
            try
            {
                //--Check directory (Begin)--
                if (!Directory.Exists(LcoalPath))
                {
                    Directory.CreateDirectory(LcoalPath);
                }
                if (!Directory.Exists(MachinePath))
                {
                    Directory.CreateDirectory(MachinePath);
                }
                //--Check directory (End)--

                foreach (string file in Directory.EnumerateFiles(MachinePath))
                {
                    string destFile = Path.Combine(LcoalPath, Path.GetFileName(file));
                    if (!File.Exists(destFile))
                    {
                        File.Move(file, destFile);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check folder at " + LcoalPath + " OR " + MachinePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                setDefaultTime();
                return;
            }
        }
    }
}
