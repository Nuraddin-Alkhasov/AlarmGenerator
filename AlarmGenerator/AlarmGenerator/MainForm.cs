using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace AlarmGenerator
{
    public partial class MainForm : Form
    {
        string OPEN_PATH_DBs = "";
        string OPEN_PATH_UDTs = "";
        string SAVE_PATH = "";
        Excel.Application App = new Excel.Application();
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch watch = new Stopwatch();
        int Count_HMITags = 0;
        int Count_HMIAlarms = 0;
        int Count_VisiWin_V = 0;
        int Count_VisiWin_A = 0;

        public MainForm()
        {
            InitializeComponent();
            connection.Text = "HMI_MOP_CPU1";
            path.Text = @"Alarms\";
            EWPrefix.Text = @"";
            errorsID.Text = "1";
            PLCName.Text = "CPU1";
        }

        #region - - - Event Handlers - - -

        private void btn_Browse_DB_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
           
               
            openFileDialog1.ShowDialog();
            OPEN_PATH_DBs = openFileDialog1.FileName;
            DBList.Items.Clear();
            UDTList.Items.Clear();
            if (OPEN_PATH_DBs != "")
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {

                    if (CheckDBs(openFileDialog1.SafeFileNames[i]))
                    {
                        DBList.Items.Add(new SiemensDB(PLCName.Text, int.Parse(openFileDialog1.SafeFileNames[i].Replace(".db", "").Replace("DB", "")), openFileDialog1.FileNames[i], EWPrefix.Text, path.Text ,connection.Text, ""));
                        foreach (NeedUDT s in ((SiemensDB)DBList.Items[i]).needUDTs)
                        {
                            if (s.isUDT)
                                UDTList.Items.Add("MISSING UDT -> " + s.Type);
                            else
                                UDTList.Items.Add("Not supported data type: " + s.DBNumber+"-"+s.DBName + "-" + s.TypeName + "-" + s.Type);
                        }
                    }
                    else
                    {
                        DBList.Items.Add("Wrong Name Format -> " + openFileDialog1.SafeFileNames[i].Replace(".db", ""));
                    }
                }
                ClearUDTList();
                SortDBList();
            }
        }

        private void btn_Browse_UDT_Click(object sender, EventArgs e)
        {
            if (DBList.Items.Count > 0)
            {
                openFileDialog2.ShowDialog();
                OPEN_PATH_UDTs = openFileDialog2.FileName;

                UDTList.Items.Clear();
                if (OPEN_PATH_UDTs != "")
                {
                    foreach (string udt in openFileDialog2.FileNames)
                    {
                        UDTList.Items.Add(new SiemensUDT(Path.GetFileName(udt).Replace(".udt", ""), File.ReadAllText(udt)));
                    }
                    SiemensUDT[] temp = new SiemensUDT[UDTList.Items.Count];
                    UDTList.Items.CopyTo(temp, 0);

                    List<SiemensUDT> a = new List<SiemensUDT>();
                    a.AddRange(temp);
                    foreach (SiemensDB db in DBList.Items)
                    {
                        db.ClearUDT();
                        db.AddUDT(a);
                    }
                    for (int i = 0; i < DBList.Items.Count; i++)
                    {
                        foreach (NeedUDT s in ((SiemensDB)DBList.Items[i]).needUDTs)
                        {
                            if (s.isUDT)
                                UDTList.Items.Add("MISSING UDT -> " + s.Type);
                            else
                                UDTList.Items.Add("Not supported data type: " + s.DBNumber + "-" + s.DBName + "-" + s.TypeName + "-" + s.Type);
                        }
                    }
                    ClearUDTList();
                }
            }
          
        }

        private void btn_Convert_Click(object sender, EventArgs e)
        {
            hmitagslabel.Text = "";
            hmialarmslabel.Text = "";
            visiwinalabel.Text = "";
            visiwinvlabel.Text = "";

            if (checkIfcanConvert())
            {
                var folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.SelectedPath = OPEN_PATH_DBs;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SAVE_PATH = folderBrowserDialog1.SelectedPath.Length<=4 ? folderBrowserDialog1.SelectedPath : folderBrowserDialog1.SelectedPath+"\\";
                }

                btn_Browse_DB.Enabled = false;
                btn_Browse_UDT.Enabled = false;
                btn_Convert.Enabled = false;

                Thread TH = new Thread(doWork);
                TH.Start();
            }
        }

        #endregion

        #region - - - Help Methods - - -

        bool CheckDBs(string _a)
        {
            if (!_a.Contains("DB"))
            {
                return false;
            }
            try
            {
                int x = int.Parse((_a.Replace("DB", "")).Replace(".db", ""));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ClearUDTList()
        {
            for (int i = 0; i < UDTList.Items.Count; i++)
            {
                for (int j = i + 1; j < UDTList.Items.Count; j++)
                {
                    if (UDTList.Items[i].ToString() == UDTList.Items[j].ToString())
                    {
                        UDTList.Items.RemoveAt(j);
                        j--;
                    }
                }
            }

        }

        private void SortDBList()
        {
            SiemensDB[] temp = new SiemensDB[DBList.Items.Count];
            DBList.Items.CopyTo(temp, 0);
            DBList.Items.Clear();
            Array.Sort(temp);
            foreach (SiemensDB s in temp)
            {
                DBList.Items.Add(s);
            }

        }

        private bool checkIfcanConvert()
        {
            if (DBList.Items.Count == 0)
            {
                MessageBox.Show("No DBs!");
                return false;
            }

               
            foreach (object s in UDTList.Items)
            {
                if (s.GetType().ToString() == "System.String")
                {
                    return false;
                }
            }
            if (connection.Text == "")
            {
                MessageBox.Show("Missing Connection Name!");
                return false;
            }
            if (path.Text == "")
            {
                MessageBox.Show("Missing Path!");
                return false;
            }
            if (PLCName.Text == "")
            {
                MessageBox.Show("Missing PLC Name!");
                return false;
            }
            if (errorsID.Text == "")
            {
                MessageBox.Show("Missing Start Errors Id!");
                return false;
            }


         
            return true;
        }

        private void End()
        {
            btn_Browse_DB.Invoke((System.Action)delegate { btn_Browse_DB.Enabled = true; });
            btn_Browse_UDT.Invoke((System.Action)delegate { btn_Browse_UDT.Enabled = true; });
            btn_Convert.Invoke((System.Action)delegate { btn_Convert.Enabled = true; });
            label2.Invoke((System.Action)delegate { label2.Text = "The Job was done in " + TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).ToString(@"hh\:mm\:ss"); });

            watch.Stop();
            watch.Reset();
        }

        private void IncremetnProgressBar(int param1, System.Windows.Forms.Label _a)
        {
            if (param1 == progressBar1.Maximum)
            {
                _a.Invoke((System.Action)delegate { _a.Text = "Done!"; });
            }

            progressBar1.Invoke((System.Action)delegate { progressBar1.Value = param1; });
        }

        //long averagetimeexecute = 0;
        List<long> averagetimeexecute = new List<long>();
        private void TimeRemating(long param1, int param2, string param3)
        {
            averagetimeexecute.Add(param1);
           
            long temp = (progressBar1.Maximum - param2) * averagetimeexecute.AsQueryable().Sum()/ averagetimeexecute.Count;
            var time = TimeSpan.FromMilliseconds(temp);
            label2.Invoke((System.Action)delegate { label2.Text = "Time remating for " + param3 + " : " + time.ToString(@"hh\:mm\:ss"); });
            progressBar1.Invoke((System.Action)delegate { if(progressBar1.Value < progressBar1.Maximum) progressBar1.Value = progressBar1.Value + 1; });

        }

        #endregion

        #region - - - HMITag - - -

        private void doWorkHMITag()
        {
            try
            {
                Excel.Application ExcelApp = new Excel.Application();
                Workbook WB = ExcelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

                PrepareHMITag(WB);
                pictureBox1.Invoke((System.Action)delegate { pictureBox1.Visible = false; });
                WriteHMITag(WB);

                WB.SaveAs(SAVE_PATH + "HMITags.xlsx");
                WB.Close();
                ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(WB);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
            }
            catch { }
        }

        private void PrepareHMITag(Workbook oWB)
        {
            try
            {
                oWB.Worksheets.Add();

                oWB.Worksheets[1].Name = "Hmi Tags";

                oWB.Worksheets[1].Cells[1, 1] = "Name";
                oWB.Worksheets[1].Cells[1, 2] = "Path";
                oWB.Worksheets[1].Cells[1, 3] = "Connection";
                oWB.Worksheets[1].Cells[1, 4] = "PLC tag";
                oWB.Worksheets[1].Cells[1, 5] = "DataType";
                oWB.Worksheets[1].Cells[1, 6] = "Length";
                oWB.Worksheets[1].Cells[1, 7] = "Coding";
                oWB.Worksheets[1].Cells[1, 8] = "Access Method";
                oWB.Worksheets[1].Cells[1, 9] = "Address";
                oWB.Worksheets[1].Cells[1, 10] = "Indirect addressing";
                oWB.Worksheets[1].Cells[1, 11] = "Index tag";
                oWB.Worksheets[1].Cells[1, 12] = "Start value";
                oWB.Worksheets[1].Cells[1, 13] = "ID tag";
                oWB.Worksheets[1].Cells[1, 14] = "Display name [de-DE]";
                oWB.Worksheets[1].Cells[1, 15] = "Comment [de-DE]";
                oWB.Worksheets[1].Cells[1, 16] = "Acquisition mode";
                oWB.Worksheets[1].Cells[1, 17] = "Acquisition cycle";
                oWB.Worksheets[1].Cells[1, 18] = "Limit Upper 2 Type";
                oWB.Worksheets[1].Cells[1, 19] = "Limit Upper 2";
                oWB.Worksheets[1].Cells[1, 20] = "Limit Upper 1 Type";
                oWB.Worksheets[1].Cells[1, 21] = "Limit Upper 1";
                oWB.Worksheets[1].Cells[1, 22] = "Limit Lower 1 Type";
                oWB.Worksheets[1].Cells[1, 23] = "Limit Lower 1";
                oWB.Worksheets[1].Cells[1, 24] = "Limit Lower 2 Type";
                oWB.Worksheets[1].Cells[1, 25] = "Limit Lower 2";
                oWB.Worksheets[1].Cells[1, 26] = "Linear scaling";
                oWB.Worksheets[1].Cells[1, 27] = "End value PLC";
                oWB.Worksheets[1].Cells[1, 28] = "Start value PLC";
                oWB.Worksheets[1].Cells[1, 29] = "End value HMI";
                oWB.Worksheets[1].Cells[1, 30] = "Start value HMI";
                oWB.Worksheets[1].Cells[1, 31] = "Gmp relevant";
                oWB.Worksheets[1].Cells[1, 32] = "Confirmation Type";
                oWB.Worksheets[1].Cells[1, 33] = "Mandatory Commenting";

                

                oWB.Worksheets[2].Name = "Multiplexing";

                oWB.Worksheets[2].Cells[1, 1] = "HMI Tag name";
                oWB.Worksheets[2].Cells[1, 2] = "Multiplex Tag"; 
                oWB.Worksheets[2].Cells[1, 3] = "Index";
            }
            catch
            {
                MessageBox.Show("Something Went Wrong. I cant figure out it by myself :(");
            }

        }

        private void WriteHMITag(Workbook oWB)
        {
            int i = 2;

            for(int j =0; j < DBList.Items.Count; j++)
            {
               

                ((SiemensDB)DBList.Items[j]).doWorkHMITags();
                foreach (HMITag hmitag in ((SiemensDB)DBList.Items[j]).HMITags)
                {
                    if (!hmitag.Name.Contains("Reserve")) 
                    {
                        stopwatch.Start();

                        oWB.Worksheets[1].Cells[i, 1] = hmitag.CPU + " " + hmitag.Name;
                        oWB.Worksheets[1].Cells[i, 2] = hmitag.Path;
                        oWB.Worksheets[1].Cells[i, 3] = hmitag.Connection;
                        oWB.Worksheets[1].Cells[i, 4] = hmitag.PLCtag;
                        oWB.Worksheets[1].Cells[i, 5] = hmitag.DataType;
                        oWB.Worksheets[1].Cells[i, 6] = hmitag.Length;
                        oWB.Worksheets[1].Cells[i, 7] = hmitag.Coding;
                        oWB.Worksheets[1].Cells[i, 8] = hmitag.AccessMethod;
                        oWB.Worksheets[1].Cells[i, 9] = hmitag.Address;
                        oWB.Worksheets[1].Cells[i, 10] = hmitag.IndirectAddressing;
                        oWB.Worksheets[1].Cells[i, 11] = hmitag.IndexTag;
                        oWB.Worksheets[1].Cells[i, 12] = hmitag.StartValue;
                        oWB.Worksheets[1].Cells[i, 13] = hmitag.IDTag;
                        oWB.Worksheets[1].Cells[i, 14] = hmitag.DisplayName;
                        oWB.Worksheets[1].Cells[i, 15] = hmitag.Comment;
                        oWB.Worksheets[1].Cells[i, 16] = hmitag.AcquisitionMode;
                        oWB.Worksheets[1].Cells[i, 17] = hmitag.AcquisitionCycle;
                        oWB.Worksheets[1].Cells[i, 18] = hmitag.LimitUpper2Type;
                        oWB.Worksheets[1].Cells[i, 19] = hmitag.LimitUpper2;
                        oWB.Worksheets[1].Cells[i, 20] = hmitag.LimitUpper1Type;
                        oWB.Worksheets[1].Cells[i, 21] = hmitag.LimitUpper1;
                        oWB.Worksheets[1].Cells[i, 22] = hmitag.LimitLower1Type;
                        oWB.Worksheets[1].Cells[i, 23] = hmitag.LimitLower1;
                        oWB.Worksheets[1].Cells[i, 24] = hmitag.LimitLower2Type;
                        oWB.Worksheets[1].Cells[i, 25] = hmitag.LimitLower2;
                        oWB.Worksheets[1].Cells[i, 26] = hmitag.LinearScaling;
                        oWB.Worksheets[1].Cells[i, 27] = hmitag.EndValuePLC;
                        oWB.Worksheets[1].Cells[i, 28] = hmitag.StartValuePLC;
                        oWB.Worksheets[1].Cells[i, 29] = hmitag.EndValueHMI;
                        oWB.Worksheets[1].Cells[i, 30] = hmitag.StartValueHMI;
                        oWB.Worksheets[1].Cells[i, 31] = hmitag.GmpRelevant;
                        oWB.Worksheets[1].Cells[i, 32] = hmitag.ConfirmationType;
                        oWB.Worksheets[1].Cells[i, 33] = hmitag.MandatoryCommenting;

                        i++;

                        stopwatch.Stop();
                        TimeRemating(stopwatch.ElapsedMilliseconds, i - 2, "HMITags");
                        stopwatch.Reset();
                    }
                    
                }

               

            }

            hmitagslabel.Invoke((System.Action)delegate { hmitagslabel.Text = "Done!"; });
        }

        #endregion

        #region - - - HMIAlarm - - -

        private void doWorkHMIAlarm()
        {
            try
            {
                Excel.Application ExcelApp = new Excel.Application();
                Workbook WB = ExcelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

                PrepareHMIAlarm(WB);
                WriteHMIAlarm(WB);

                WB.SaveAs(SAVE_PATH + "HMIAlarms.xlsx");
                WB.Close();
                ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(WB);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
            }
            catch { }
          
        }

        private void PrepareHMIAlarm(Workbook oWB)
        {
            try
            {
                oWB.Worksheets[1].Name = "DiscreteAlarms";

                oWB.Worksheets[1].Cells[1, 1] = "ID";
                oWB.Worksheets[1].Cells[1, 2] = "Name";
                oWB.Worksheets[1].Cells[1, 3] = "Alarm text [de-DE], Alarm text";
                oWB.Worksheets[1].Cells[1, 4] = "FieldInfo [Alarm text]";
                oWB.Worksheets[1].Cells[1, 5] = "Class";
                oWB.Worksheets[1].Cells[1, 6] = "Trigger tag";
                oWB.Worksheets[1].Cells[1, 7] = "Trigger bit";
                oWB.Worksheets[1].Cells[1, 8] = "Acknowledgement tag";
                oWB.Worksheets[1].Cells[1, 9] = "Acknowledgement bit";
                oWB.Worksheets[1].Cells[1, 10] = "PLC acknowledgement tag";
                oWB.Worksheets[1].Cells[1, 11] = "PLC acknowledgement bit";
                oWB.Worksheets[1].Cells[1, 12] = "Group";
                oWB.Worksheets[1].Cells[1, 13] = "Report";
                oWB.Worksheets[1].Cells[1, 14] = "Info text [de-DE], Info text";
            }
            catch
            {
                MessageBox.Show("Something Went Wrong. I cant figure out it by myself :(");
            }

        }

        private void WriteHMIAlarm(Workbook oWB)
        {
            int i = 2;
            int ErrorID = int.Parse(errorsID.Text);

            for (int j = 0; j < DBList.Items.Count; j++)
            {
                

                ((SiemensDB)DBList.Items[j]).doWorkHMIAlarms(ErrorID);
                foreach (HMIAlarm hmialarm in ((SiemensDB)DBList.Items[j]).HMIAlarms)
                {
                    stopwatch.Start();
                   
                    oWB.Worksheets[1].Cells[i, 1] = hmialarm.ID;
                    oWB.Worksheets[1].Cells[i, 2] = hmialarm.Name;
                    oWB.Worksheets[1].Cells[i, 3] = hmialarm.AlarmText == "" ? "-" : hmialarm.AlarmText;
                    oWB.Worksheets[1].Cells[i, 4] = hmialarm.FieldInfo;
                    oWB.Worksheets[1].Cells[i, 5] = hmialarm.Class;
                    oWB.Worksheets[1].Cells[i, 6] = hmialarm.TriggerTag.Contains("Reserve") ? "" : hmialarm.CPU + " " + hmialarm.TriggerTag;
                    oWB.Worksheets[1].Cells[i, 7] = hmialarm.TriggerBit;
                    oWB.Worksheets[1].Cells[i, 8] = hmialarm.AcknowledgementTag;
                    oWB.Worksheets[1].Cells[i, 9] = hmialarm.AcknowledgementBit;
                    oWB.Worksheets[1].Cells[i, 10] = hmialarm.PLCAcknowledgementTag;
                    oWB.Worksheets[1].Cells[i, 11] = hmialarm.AcknowledgementBit;
                    oWB.Worksheets[1].Cells[i, 12] = hmialarm.Group;
                    oWB.Worksheets[1].Cells[i, 13] = hmialarm.Report;
                    oWB.Worksheets[1].Cells[i, 14] = hmialarm.InfoText;
                  
                    

                    i++;

                    stopwatch.Stop();
                    TimeRemating(stopwatch.ElapsedMilliseconds, i-2, "HMIAlarms");
                    stopwatch.Reset();
                }

               

                ErrorID = ((SiemensDB)DBList.Items[j]).LastId;
            }
            hmialarmslabel.Invoke((System.Action)delegate { hmialarmslabel.Text = "Done!"; });
        }
        #endregion

        #region - - - VisiWin - - -

        private void doWorkVisiWin()
        {
            try
            {
                Excel.Application ExcelApp = new Excel.Application();
                Workbook WB = ExcelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                averagetimeexecute.Clear();
                PrepareVisiWinVariable(WB);
                WriteVisiWinVariable(WB);

                averagetimeexecute.Clear();
                PrepareVisiWinAlarm(WB);
                progressBar1.Invoke((System.Action)delegate { progressBar1.Value = 0; progressBar1.Maximum = Count_VisiWin_A; });

                WriteVisiWinAlarm(WB);

                WB.SaveAs(SAVE_PATH + "VisiWin.xlsx");
                WB.Close();
                ExcelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(WB);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
            }
            catch{ }
            
        }

        #region - - - VisiWin Variable- - -

        private void PrepareVisiWinVariable(Workbook oWB)
        {
            try
            {
                oWB.Worksheets.Add();

                oWB.Worksheets[1].Name = "VisiWin Variables";

                oWB.Worksheets[1].Cells[1, 1] = "< Kernel.Channels.Driver >";

                oWB.Worksheets[1].Cells[2, 1] = "Name";
                oWB.Worksheets[1].Cells[2, 2] = "DriverName";
                oWB.Worksheets[1].Cells[2, 3] = "IgnorBadQualityValues";
                oWB.Worksheets[1].Cells[2, 4] = "BrowserConfig";
                oWB.Worksheets[1].Cells[2, 5] = "BrowserStructurePrefix";
                oWB.Worksheets[1].Cells[2, 6] = "IsVirtual";
                oWB.Worksheets[1].Cells[2, 7] = "Enabled";
                oWB.Worksheets[1].Cells[2, 8] = "Device";
                oWB.Worksheets[1].Cells[2, 9] = "BrowserProperties";
                oWB.Worksheets[1].Cells[2, 10] = "Configuration";
                oWB.Worksheets[1].Cells[2, 11] = "Type";
                oWB.Worksheets[1].Cells[2, 12] = "StartSuspended";
                oWB.Worksheets[1].Cells[2, 13] = "Browser";
                oWB.Worksheets[1].Cells[2, 14] = "Separator";
                oWB.Worksheets[1].Cells[2, 15] = "ShortDescriptor";
                oWB.Worksheets[1].Cells[2, 16] = "Comment";
                oWB.Worksheets[1].Cells[2, 17] = "DontReadInitValuesSynchron";
                oWB.Worksheets[1].Cells[2, 18] = "AllowSynchronousCommunication";

                oWB.Worksheets[1].Cells[3, 1] = PLCName.Text;
                oWB.Worksheets[1].Cells[3, 2] = "SiemensS7";
                oWB.Worksheets[1].Cells[3, 3] = "False";
                oWB.Worksheets[1].Cells[3, 4] = "";
                oWB.Worksheets[1].Cells[3, 5] = "";
                oWB.Worksheets[1].Cells[3, 6] = "False";
                oWB.Worksheets[1].Cells[3, 7] = "True";
                oWB.Worksheets[1].Cells[3, 8] = "";
                oWB.Worksheets[1].Cells[3, 9] = "";
                oWB.Worksheets[1].Cells[3, 10] = "";
                oWB.Worksheets[1].Cells[3, 11] = "7";
                oWB.Worksheets[1].Cells[3, 12] = "False";
                oWB.Worksheets[1].Cells[3, 13] = @"NET\SiemensS7\Neue Browser\VisiWin.S7.Brw.dll";
                oWB.Worksheets[1].Cells[3, 14] = ".";
                oWB.Worksheets[1].Cells[3, 15] = "Driver for SiemensS7 Devices";
                oWB.Worksheets[1].Cells[3, 16] = "";
                oWB.Worksheets[1].Cells[3, 17] = "False";
                oWB.Worksheets[1].Cells[3, 18] = "";
            
                oWB.Worksheets[1].Cells[5, 1] = "<Kernel.Items>";

                oWB.Worksheets[1].Cells[6, 1] = "[Parent]";
                oWB.Worksheets[1].Cells[6, 2] = "Alias";
                oWB.Worksheets[1].Cells[6, 3] = "SendOnCancel";
                oWB.Worksheets[1].Cells[6, 4] = "DefaultValue";
                oWB.Worksheets[1].Cells[6, 5] = "AccessRights";
                oWB.Worksheets[1].Cells[6, 6] = "Comment";
                oWB.Worksheets[1].Cells[6, 7] = "DataType";
                oWB.Worksheets[1].Cells[6, 8] = "UnitClassProcess";
                oWB.Worksheets[1].Cells[6, 9] = "ItemAccess";
                oWB.Worksheets[1].Cells[6, 10] = "SendOnStartup";
                oWB.Worksheets[1].Cells[6, 11] = "DisableDisintegration";
                oWB.Worksheets[1].Cells[6, 12] = "Substitution value mode";
                oWB.Worksheets[1].Cells[6, 13] = "WriteThru";
                oWB.Worksheets[1].Cells[6, 14] = "AutoBlockOptimization";
                oWB.Worksheets[1].Cells[6, 15] = "MaxValue";
                oWB.Worksheets[1].Cells[6, 16] = "FieldSize";
                oWB.Worksheets[1].Cells[6, 17] = "MinValueItem";
                oWB.Worksheets[1].Cells[6, 18] = "ClientIntern";
                oWB.Worksheets[1].Cells[6, 19] = "TextParams";
                oWB.Worksheets[1].Cells[6, 20] = "Substitution value";
                oWB.Worksheets[1].Cells[6, 21] = "Group";
                oWB.Worksheets[1].Cells[6, 22] = "Enabled";
                oWB.Worksheets[1].Cells[6, 23] = "AccessPath";
                oWB.Worksheets[1].Cells[6, 24] = "UnitClassDisplay";
                oWB.Worksheets[1].Cells[6, 25] = "MinValue";
                oWB.Worksheets[1].Cells[6, 26] = "DenyPublicOPCAccessldSize";
                oWB.Worksheets[1].Cells[6, 27] = "MaxValueItem";
                oWB.Worksheets[1].Cells[6, 28] = "SendOnExit";
                oWB.Worksheets[1].Cells[6, 29] = "LogChanges";
                oWB.Worksheets[1].Cells[6, 30] = "SendToOPCServer";
                oWB.Worksheets[1].Cells[6, 31] = "";
                oWB.Worksheets[1].Cells[6, 32] = "Text.1031";
            }
            catch
            {
                MessageBox.Show("Something Went Wrong. I cant figure out it by myself :(");
            }

        }

        private void WriteVisiWinVariable(Workbook oWB)
        {
            int i = 7;

            for (int j = 0; j < DBList.Items.Count; j++)
            {
                ((SiemensDB)DBList.Items[j]).doWorkVisiWinVariables();
                foreach (VisiWinVariable vw in ((SiemensDB)DBList.Items[j]).VisiWinVariables)
                {
                    if (vw.ItemAccess != "")
                    {
                        if (!vw.Alias.Contains("Reserve"))
                        {
                            stopwatch.Start();
                            oWB.Worksheets[1].Cells[i, 1] = vw.Parent;
                            oWB.Worksheets[1].Cells[i, 2] = vw.Alias;
                            oWB.Worksheets[1].Cells[i, 3] = vw.SendOnCancel;
                            oWB.Worksheets[1].Cells[i, 4] = vw.DefaultValue;
                            oWB.Worksheets[1].Cells[i, 5] = vw.AccessRights;
                            oWB.Worksheets[1].Cells[i, 6] = vw.Comment;
                            oWB.Worksheets[1].Cells[i, 7] = vw.DataType;
                            oWB.Worksheets[1].Cells[i, 8] = vw.UnitClassProcess;
                            oWB.Worksheets[1].Cells[i, 9] = vw.ItemAccess;
                            oWB.Worksheets[1].Cells[i, 10] = vw.SendOnStartup;
                            oWB.Worksheets[1].Cells[i, 11] = vw.DisableDisintegration;
                            oWB.Worksheets[1].Cells[i, 12] = vw.SubstitutionValueMode;
                            oWB.Worksheets[1].Cells[i, 13] = vw.WriteThru;
                            oWB.Worksheets[1].Cells[i, 14] = vw.AutoBlockOptimization;
                            oWB.Worksheets[1].Cells[i, 15] = vw.MaxValue;
                            oWB.Worksheets[1].Cells[i, 16] = vw.FieldSize;
                            oWB.Worksheets[1].Cells[i, 17] = vw.MinValueItem;
                            oWB.Worksheets[1].Cells[i, 18] = vw.ClientIntern;
                            oWB.Worksheets[1].Cells[i, 19] = vw.TextParams;
                            oWB.Worksheets[1].Cells[i, 20] = vw.SubstitutionValue;
                            oWB.Worksheets[1].Cells[i, 21] = vw.Group;
                            oWB.Worksheets[1].Cells[i, 22] = vw.Enabled;
                            oWB.Worksheets[1].Cells[i, 23] = vw.AccessPath;
                            oWB.Worksheets[1].Cells[i, 24] = vw.UnitClassDisplay;
                            oWB.Worksheets[1].Cells[i, 25] = vw.MinValue;
                            oWB.Worksheets[1].Cells[i, 26] = vw.DenyPublicOPCAccess;
                            oWB.Worksheets[1].Cells[i, 27] = vw.MaxValueItem;
                            oWB.Worksheets[1].Cells[i, 28] = vw.SendOnExit;
                            oWB.Worksheets[1].Cells[i, 29] = vw.LogChanges;
                            oWB.Worksheets[1].Cells[i, 30] = vw.SendToOPCServer;
                            oWB.Worksheets[1].Cells[i, 31] = "";
                            oWB.Worksheets[1].Cells[i, 32] = vw.Text_1031;

                            i++;

                            stopwatch.Stop();
                            TimeRemating(stopwatch.ElapsedMilliseconds, i - 2, "VisiWin Variables");
                            stopwatch.Reset();
                        }
                    }
                   
                }

            }

            visiwinvlabel.Invoke((System.Action)delegate { visiwinvlabel.Text = "Done!"; });
        }

        #endregion

        #region - - - VisiWin Alarm- - -

        private void PrepareVisiWinAlarm(Workbook oWB)
        {
            try
            {
                oWB.Worksheets[2].Name = "VisiWin Alarm";

                oWB.Worksheets[2].Cells[1, 1] = "<Alarm.Groups>";

                oWB.Worksheets[2].Cells[2, 1] = "[Parent]";
                oWB.Worksheets[2].Cells[2, 2] = "Name";
                oWB.Worksheets[2].Cells[2, 3] = "ItemAcknowledgeBitNumber";
                oWB.Worksheets[2].Cells[2, 4] = "Icon";
                oWB.Worksheets[2].Cells[2, 5] = "ItemDisableBitNumber";
                oWB.Worksheets[2].Cells[2, 6] = "TextParams";
                oWB.Worksheets[2].Cells[2, 7] = "Enabled";
                oWB.Worksheets[2].Cells[2, 8] = "ItemAcknowledge";
                oWB.Worksheets[2].Cells[2, 9] = "Comment";
                oWB.Worksheets[2].Cells[2, 10] = "ItemStateBitNumber";
                oWB.Worksheets[2].Cells[2, 11] = "ItemPriority";
                oWB.Worksheets[2].Cells[2, 12] = "ItemState";
                oWB.Worksheets[2].Cells[2, 13] = "ItemDisable";
                oWB.Worksheets[2].Cells[2, 14] = "Text.1031";
                oWB.Worksheets[2].Cells[2, 15] = "Text.1033";
                oWB.Worksheets[2].Cells[2, 16] = "";

                oWB.Worksheets[2].Cells[3, 1] = "";
                oWB.Worksheets[2].Cells[3, 2] = "Error";
                oWB.Worksheets[2].Cells[3, 3] = "0";
                oWB.Worksheets[2].Cells[3, 4] = "";
                oWB.Worksheets[2].Cells[3, 5] = "0";
                oWB.Worksheets[2].Cells[3, 6] = "";
                oWB.Worksheets[2].Cells[3, 7] = "True";
                oWB.Worksheets[2].Cells[3, 8] = "";
                oWB.Worksheets[2].Cells[3, 9] = "";
                oWB.Worksheets[2].Cells[3, 10] = "0";
                oWB.Worksheets[2].Cells[3, 11] = "";
                oWB.Worksheets[2].Cells[3, 12] = "";
                oWB.Worksheets[2].Cells[3, 13] = "";
                oWB.Worksheets[2].Cells[3, 14] = "Error";
                oWB.Worksheets[2].Cells[3, 15] = "Error";
                oWB.Worksheets[2].Cells[3, 16] = "";

                oWB.Worksheets[2].Cells[4, 1] = "";
                oWB.Worksheets[2].Cells[4, 2] = "Warning";
                oWB.Worksheets[2].Cells[4, 3] = "0";
                oWB.Worksheets[2].Cells[4, 4] = "";
                oWB.Worksheets[2].Cells[4, 5] = "0";
                oWB.Worksheets[2].Cells[4, 6] = "";
                oWB.Worksheets[2].Cells[4, 7] = "True";
                oWB.Worksheets[2].Cells[4, 8] = "";
                oWB.Worksheets[2].Cells[4, 9] = "";
                oWB.Worksheets[2].Cells[4, 10] = "0";
                oWB.Worksheets[2].Cells[4, 11] = "";
                oWB.Worksheets[2].Cells[4, 12] = "";
                oWB.Worksheets[2].Cells[4, 13] = "";
                oWB.Worksheets[2].Cells[4, 14] = "Warning";
                oWB.Worksheets[2].Cells[4, 15] = "Warning";
                oWB.Worksheets[2].Cells[4, 16] = "";

                oWB.Worksheets[2].Cells[5, 1] = "";
                oWB.Worksheets[2].Cells[5, 2] = "Message";
                oWB.Worksheets[2].Cells[5, 3] = "0";
                oWB.Worksheets[2].Cells[5, 4] = "";
                oWB.Worksheets[2].Cells[5, 5] = "0";
                oWB.Worksheets[2].Cells[5, 6] = "";
                oWB.Worksheets[2].Cells[5, 7] = "True";
                oWB.Worksheets[2].Cells[5, 8] = "";
                oWB.Worksheets[2].Cells[5, 9] = "";
                oWB.Worksheets[2].Cells[5, 10] = "0";
                oWB.Worksheets[2].Cells[5, 11] = "";
                oWB.Worksheets[2].Cells[5, 12] = "";
                oWB.Worksheets[2].Cells[5, 13] = "";
                oWB.Worksheets[2].Cells[5, 14] = "Message";
                oWB.Worksheets[2].Cells[5, 15] = "Message";
                oWB.Worksheets[2].Cells[5, 16] = "";

                oWB.Worksheets[2].Cells[6, 1] = "";
                oWB.Worksheets[2].Cells[6, 2] = "Predictive";
                oWB.Worksheets[2].Cells[6, 3] = "0";
                oWB.Worksheets[2].Cells[6, 4] = "";
                oWB.Worksheets[2].Cells[6, 5] = "0";
                oWB.Worksheets[2].Cells[6, 6] = "";
                oWB.Worksheets[2].Cells[6, 7] = "True";
                oWB.Worksheets[2].Cells[6, 8] = "";
                oWB.Worksheets[2].Cells[6, 9] = "";
                oWB.Worksheets[2].Cells[6, 10] = "0";
                oWB.Worksheets[2].Cells[6, 11] = "";
                oWB.Worksheets[2].Cells[6, 12] = "";
                oWB.Worksheets[2].Cells[6, 13] = "";
                oWB.Worksheets[2].Cells[6, 14] = "Predictive";
                oWB.Worksheets[2].Cells[6, 15] = "Predictive";
                oWB.Worksheets[2].Cells[6, 16] = "";

                oWB.Worksheets[2].Cells[8, 1] = "<Alarm.Alarms>";

                oWB.Worksheets[2].Cells[9, 1] = "[Parent]";
                oWB.Worksheets[2].Cells[9, 2] = "Name";
                oWB.Worksheets[2].Cells[9, 3] = "LowActive";
                oWB.Worksheets[2].Cells[9, 4] = "Comment";
                oWB.Worksheets[2].Cells[9, 5] = "ItemStateBitNumber";
                oWB.Worksheets[2].Cells[9, 6] = "Enabled";
                oWB.Worksheets[2].Cells[9, 7] = "ItemState";
                oWB.Worksheets[2].Cells[9, 8] = "ItemEventBitNumber";
                oWB.Worksheets[2].Cells[9, 9] = "ItemAcknowledgeBitNumber";
                oWB.Worksheets[2].Cells[9, 10] = "TextParams";
                oWB.Worksheets[2].Cells[9, 11] = "Parameter1";
                oWB.Worksheets[2].Cells[9, 12] = "Class";
                oWB.Worksheets[2].Cells[9, 13] = "Priority";
                oWB.Worksheets[2].Cells[9, 14] = "Type";
                oWB.Worksheets[2].Cells[9, 15] = "ItemAcknowledge";
                oWB.Worksheets[2].Cells[9, 16] = "Parameter2";
                oWB.Worksheets[2].Cells[9, 17] = "ItemEvent";
                oWB.Worksheets[2].Cells[9, 18] = "Text.1031";
                oWB.Worksheets[2].Cells[9, 19] = "Text.1033";
                oWB.Worksheets[2].Cells[9, 20] = "";
            }
            catch
            {
                MessageBox.Show("Something Went Wrong. I cant figure out it by myself :(");
            }

        }

        private void WriteVisiWinAlarm(Workbook oWB)
        {
            int i = 10;
            int ErrorID = int.Parse(errorsID.Text);
            for (int j = 0; j < DBList.Items.Count; j++)
            {

                ((SiemensDB)DBList.Items[j]).doWorkVisiWinAlarms(ErrorID);
                foreach (VisiWinAlarm vw in ((SiemensDB)DBList.Items[j]).VisiWinAlarms)
                {
                    stopwatch.Start();

                    oWB.Worksheets[2].Cells[i, 1] = vw.Parent;
                    oWB.Worksheets[2].Cells[i, 2] = vw.Name;
                    oWB.Worksheets[2].Cells[i, 3] = vw.LowActive;
                    oWB.Worksheets[2].Cells[i, 4] = vw.Comment;
                    oWB.Worksheets[2].Cells[i, 5] = vw.ItemStateBitNumber;
                    oWB.Worksheets[2].Cells[i, 6] = vw.Enabled;
                    oWB.Worksheets[2].Cells[i, 7] = vw.ItemState;
                    oWB.Worksheets[2].Cells[i, 8] = vw.ItemEventBitNumber;
                    oWB.Worksheets[2].Cells[i, 9] = vw.ItemAcknowledgeBitNumber;
                    oWB.Worksheets[2].Cells[i, 10] = vw.TextParams;
                    oWB.Worksheets[2].Cells[i, 11] = vw.Parameter1;
                    oWB.Worksheets[2].Cells[i, 12] = vw.Class;
                    oWB.Worksheets[2].Cells[i, 13] = vw.Priority;
                    oWB.Worksheets[2].Cells[i, 14] = vw.Type;
                    oWB.Worksheets[2].Cells[i, 15] = vw.ItemAcknowledge;
                    oWB.Worksheets[2].Cells[i, 16] = vw.Parameter2;
                    oWB.Worksheets[2].Cells[i, 17] = vw.ItemEvent.Contains("Reserve") ? "" : vw.ItemEvent;
                    oWB.Worksheets[2].Cells[i, 18] = vw.Text_1031;
                    oWB.Worksheets[2].Cells[i, 19] = vw.Text_1031;
                    oWB.Worksheets[2].Cells[i, 20] = "";

                    i++;

                    stopwatch.Stop();
                    TimeRemating(stopwatch.ElapsedMilliseconds, i - 2, "VisiWin Alarms");
                    stopwatch.Reset();
                }

                ErrorID = ((SiemensDB)DBList.Items[j]).LastId;

            }

            visiwinalabel.Invoke((System.Action)delegate 
            { 
                visiwinalabel.Text = "Done!";
                connection.Text = "HMI_MOP_CPU2";
                path.Text = @"Alarm\";
                errorsID.Text = ErrorID.ToString();
                PLCName.Text = "CPU2";
            });
        }

        #endregion

        #endregion

        private void doWork()
        {
            watch.Start();
            //Fill DB
            progressBar1.Invoke((System.Action)delegate { progressBar1.Value = 0; });
            pictureBox1.Invoke((System.Action)delegate { pictureBox1.Visible = true; });

            Count_HMITags = 0;
            Count_HMIAlarms = 0;
            Count_VisiWin_V = 0;
            Count_VisiWin_A = 0;

            foreach (SiemensDB db in DBList.Items)
            {
                db.doWork(PLCName.Text);
                Count_HMITags = Count_HMITags + db.HMITagsCount;
                Count_HMIAlarms = Count_HMIAlarms + db.HMIAlarmsCount;
                Count_VisiWin_V = Count_VisiWin_V + db.VisiWinVariablesCount;
                Count_VisiWin_A = Count_VisiWin_A + db.VisiWinAlarmsCount;
            }
            
            if (checkBox2.Checked)
            {
                averagetimeexecute.Clear();
                progressBar1.Invoke((System.Action)delegate { progressBar1.Minimum = 0; progressBar1.Maximum = Count_HMITags; });
                doWorkHMITag();

                averagetimeexecute.Clear();
                progressBar1.Invoke((System.Action)delegate { progressBar1.Value = 0; progressBar1.Maximum = Count_HMIAlarms; });
                doWorkHMIAlarm();

                progressBar1.Invoke((System.Action)delegate { progressBar1.Value = 0; progressBar1.Maximum = Count_VisiWin_V; });
                doWorkVisiWin();
            }
            else 
            {
                averagetimeexecute.Clear();
                progressBar1.Invoke((System.Action)delegate { progressBar1.Minimum = 0; progressBar1.Maximum = Count_HMITags; });
                doWorkHMITag();

                averagetimeexecute.Clear();
                progressBar1.Invoke((System.Action)delegate { progressBar1.Value = 0; progressBar1.Maximum = Count_HMIAlarms; });
                doWorkHMIAlarm();
            }
            

           
            

            End();
        }

        private void connection_TextChanged(object sender, EventArgs e)
        {
            foreach (SiemensDB sDB in DBList.Items) 
            {
                sDB.Connection = connection.Text;
            }
        }

        private void path_TextChanged(object sender, EventArgs e)
        {
            foreach (SiemensDB sDB in DBList.Items)
            {
                sDB.TagPath = path.Text;
            }
        }
    }
}

