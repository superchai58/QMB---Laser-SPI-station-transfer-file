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

namespace SP_Tranfer_File
{
    public partial class Form1 : Form
    {
        System.Timers.Timer t;
        int h = 0, m = 0, s = 10;

        public string Line = null;
        public string Station = null;
        DataTable dtIP = new DataTable();
        string ip = "";

        public Form1()
        {
            InitializeComponent();
            cboStation.Items.Add("SP1");
            cboStation.Items.Add("SP2");
            DataTable Line = GetDataLine();
            int countRow = Line.Rows.Count;
            for (int i = 0; i < countRow; i++)
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
            cboStation.Enabled = false;
            //--superchai Add (End)--

            Line = cboLine.Text;
            Station = cboStation.Text;

            //--superchai Add (Begin)--
            t.Start();
        }

        public DataTable GetIP()
        {
            DataTable dtTmp = new DataTable();
            connecDB conn = new connecDB();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = "select TOP 1 [Value] from mesPE_ProConfig where Station = @SP and Line = @Line and [Key] = 'MachineIP'";
                cmd.Parameters.Add(new SqlParameter("@SP", cboStation.Text.Trim()));
                cmd.Parameters.Add(new SqlParameter("@Line", cboLine.Text.Trim()));
                cmd.CommandTimeout = 180;
                dtTmp = conn.Query(cmd);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString().Trim();
                MessageBox.Show(strEx, "Error catch !!!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            return dtTmp;
        }

        //--superchai Add (Begin)--
        private void OntimeEvent(object sender, ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {                  
                s -= 1;
                if (s == 0)
                {
                    dtIP = GetIP();
                    if (dtIP.Rows.Count > 0)
                    {
                        ip = dtIP.Rows[0]["Value"].ToString().Trim();
                    }

                    //--Get Result file (Begin)--
                    string LocalPath = string.Empty;
                    string ServerPath = string.Empty;
                    LocalPath = "\\\\" + ip + "\\SHOPFLOORSE300";
                    //LocalPath = "\\\\10.97.2.95\\d$\\SHOPFLOORSE300";       //Test
                    ServerPath = "D:\\SHOPFLOORSE300";
                    try
                    {
                        //--Check directory (Begin)--
                        if (!Directory.Exists(LocalPath))
                        {
                            Directory.CreateDirectory(LocalPath);
                        }
                        if (!Directory.Exists(ServerPath))
                        {
                            Directory.CreateDirectory(ServerPath);
                        }
                        //--Check directory (End)--

                        foreach (string file in Directory.EnumerateFiles(LocalPath))
                        {
                            string destFile = Path.Combine(ServerPath, Path.GetFileName(file));
                            if (!File.Exists(destFile))
                            {
                                //File.Move(file, destFile);

                                File.Copy(file, destFile, true);
                                File.Delete(file);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        setDefaultTime();
                        MessageBox.Show("Please check the Result folder at " + LocalPath + " OR " + ServerPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    s = 10;
                    
                }
                
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
                cmd.CommandText = "Select Distinct line From Line_Data With(nolock)";
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
            cboStation.Enabled = true;
            btnOK.Enabled = true;
            t.Stop();

            h = 0;
            m = 0;
            s = 10;
            lbTime.Text = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0')
                              , s.ToString().PadLeft(2, '0'));
        }
    }
}