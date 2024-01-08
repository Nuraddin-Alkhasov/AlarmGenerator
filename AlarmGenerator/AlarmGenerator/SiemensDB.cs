using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AlarmGenerator
{
    public class SiemensDB : IComparable<SiemensDB>
    {
        public string DBName { get; set; }
        public int DBNumber { get; set; }
        public string Path { get; set; }
        public string Prefix { get; set; }
        public string TagPath { get; set; }
        public string Connection { get; set; }
        string Parent { get; set; }
        private SiemensStructure DB { set; get; }
        string DBData { set; get; }

        public List<SiemensUDT> UDT { set; get; }
        public List<HMITag> HMITags { set; get; }
        public List<HMIAlarm> HMIAlarms { set; get; }

        public List<VisiWinVariable> VisiWinVariables { set; get; }
        public List<VisiWinAlarm> VisiWinAlarms { set; get; }
        
        public int HMITagsCount { set; get; }
        public int HMIAlarmsCount { set; get; }
        public int VisiWinVariablesCount { set; get; }
        public int VisiWinAlarmsCount { set; get; }

        public List<NeedUDT> needUDTs { set; get; }

        string[] Dummy { get; set; }
        int C { get; set; }
        string LastOffset { get; set; }
        string LastDataType { get; set; }
        public int LastId { get; set; }
        string CPU { set; get; }

        public SiemensDB(string _CPU, int _DBNumber, string _Path, string _Prefix, string _TagPath, string _Connection, string _Parent)
        {
            CPU = _CPU;
            DBData = File.ReadAllText(_Path);
            UDT = new List<SiemensUDT>();
            Prefix = _Prefix;
            DBName = getBetween(DBData, "\"", "\"").Replace("DB ", "").Replace(" Alarm", "");
            DBNumber = _DBNumber;
            Path = _Path;
            TagPath = _TagPath;
            Connection = _Connection;
            Parent = _Parent;
            needUDTs = generateNeedUDTs(DBData);
        }

        public void doWork(string _Parent)
        {
            Parent = _Parent;

            Dummy = PrepareDB(DBData);
            C = 0;

            DB = (SiemensStructure)StructureGenerator();

            List<HMITag> temp = generateHMITags(DB, DBName);
            VisiWinVariablesCount = HMITagsCount = temp.Count;
            VisiWinAlarmsCount = HMIAlarmsCount = generateHMIAlarms(temp, 1).Count;

        }

        List<NeedUDT> generateNeedUDTs(string _DBData)
        {
            List<NeedUDT> ret_val = new List<NeedUDT>();
            Dummy = PrepareDB(_DBData);

            string[] s = UDTtypesFormat(PrepareDB(_DBData));

            foreach (string line in s)
            {
                string VariableType = ClearString((line.Split(':'))[1]);
                if (VariableType != "Bool" && VariableType != "Int")
                    ret_val.Add(new NeedUDT(VariableType, DBName, "DB" + DBNumber, line)); //"DB" + DBNumber + " -> " + DBName + " -> " + line
            }

            for (int i = 0; i < ret_val.Count; i++)
            {
                for (int j = i + 1; j < ret_val.Count; j++)
                {
                    if (ret_val[i].ToString() == ret_val[j].ToString())
                    {
                        ret_val.RemoveAt(j);
                        j--;
                    }
                }
            }


            return ret_val;
        }

        public void AddUDT(List<SiemensUDT> _UDT)
        {
            for(int i=0; i< needUDTs.Count; i++)
            {
                foreach (SiemensUDT _udt in _UDT)
                {
                    if (needUDTs[i].Type == _udt.UDTName)
                    {
                        UDT.Add(_udt);
                        needUDTs.Remove(needUDTs[i]);
                        i--;
                        break;
                    }
                }
            }
        }

        public void doWorkHMITags()
        {
            HMITags = generateHMITags(DB, DBName);
        }

        public void doWorkHMIAlarms(int _ErrorsID)
        {
            HMIAlarms = generateHMIAlarms(HMITags, _ErrorsID);
        }

        public void doWorkVisiWinVariables()
        {
            VisiWinVariables = generateVisiWinVariables(HMITags);
        }

        public void doWorkVisiWinAlarms(int _ErrorsID)
        {
            VisiWinAlarms = generateVisiWinAlarms(VisiWinVariables, _ErrorsID);
        }

        public void ClearUDT()
        {
            UDT = new List<SiemensUDT>();
            needUDTs = generateNeedUDTs(DBData);
        }

        #region  - - - Prepare DB - - -

        string[] PrepareDB(string _Data)
        {
            return cutData(formatRows(removeProperties(removeComments(splitRows(_Data)))));
        }

        string[] splitRows(string _Data)
        {
            string temp = "";

            int j = 0;
            for (int i = 0; i < _Data.Length; i++)
            {
                if (_Data[i] == '\n')
                {
                    j++;
                }
            }

            string[] _DB = new string[j];
            j = 0;
            for (int i = 0; i < _Data.Length; i++)
            {
                if (_Data[i] == '\n')
                {
                    _DB[j] = temp;
                    temp = "";
                    j++;
                }
                else
                {
                    temp += _Data[i];
                }
            }
            return _DB;
        }

        string[] removeComments(string[] _DB)
        {
            for (int i = 0; i < _DB.Length; i++)
            {
                if (_DB[i].Contains("//"))
                {
                    _DB[i] = _DB[i].Substring(0, _DB[i].IndexOf("//", 0));
                }
            }
            return _DB;
        }

        string[] removeProperties(string[] _DB)
        {
            for (int i = 0; i < _DB.Length; i++)
            {
                _DB[i] = _DB[i].Replace("ExternalAccessible := 'False'", "");
                _DB[i] = _DB[i].Replace("ExternalVisible := 'False'", "");
                _DB[i] = _DB[i].Replace("ExternalWritable := 'False'", "");
                _DB[i] = _DB[i].Replace("S7_SetPoint := 'True'", "");
                _DB[i] = _DB[i].Replace("S7_SetPoint := 'False'", "");
                _DB[i] = _DB[i].Replace("S7_Optimized_Access := 'FALSE'", "");
            
                _DB[i] = _DB[i].Replace("{", "");
                _DB[i] = _DB[i].Replace("}", "");
                _DB[i] = _DB[i].Replace(";", "");
                _DB[i] = _DB[i].Replace("\r", "");
                _DB[i] = _DB[i].Replace("\t", "");
            }
            return _DB;
        }

        string[] formatRows(string[] _DB)
        {
            for (int i = 0; i < _DB.Length; i++)
            {
                _DB[i] = _DB[i].Replace("{", "");
                _DB[i] = _DB[i].Replace("}", "");
                _DB[i] = _DB[i].Replace(";", "");
                _DB[i] = _DB[i].Replace("\r", "");
                _DB[i] = _DB[i].Replace("\t", "");
            }
            return _DB;
        }
 
        string[] cutData(string[] _DB)
        {
            List<string> ret_val = new List<string>();
            ret_val = _DB.ToList();

            for (int i = 0; i < ret_val.Count; i++)
            {
                if (ret_val[i].Contains("STRUCT"))
                {
                    while (i != 0)
                    {
                        ret_val.RemoveAt(i - 1);
                        i--;
                    }
                    break;
                }
            }

            for (int i = ret_val.Count-1; i < ret_val.Count; i--)
            {
                if (!ret_val[i].Contains("END_STRUCT"))
                {
                    ret_val.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            ret_val.RemoveAt(1);
            return ret_val.ToArray();
        }

        string[] UDTtypesFormat(string[] _DB)
        {
            List<string> ret_val = new List<string>();
            ret_val = _DB.ToList();

            for (int i = 0; i < ret_val.Count; i++)
            {
                if (ret_val[i].Contains("STRUCT") || ret_val[i].Contains("Struct"))
                {
                    ret_val.RemoveAt(i);
                    i--;
                }
            }

            return ret_val.ToArray();
        }

        #endregion

        #region  - - - Helping Methods - - -

        string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        string ClearString(string _data)
        {
            if (_data.Contains("\""))
            {
                return getBetween(_data, "\"", "\"");
            }
            else
            {
                return _data.Replace(" ", "");
            }
        }

        public static bool IsOdd(double value)
        {
            return value % 2 != 0;
        }

        

        #endregion

        object StructureGenerator()
        {
            object ret_val = null;
            string Name = "";
            string Type = "";

            if (Dummy[C].Contains("STRUCT"))
            {
                Name = "ROOT";
                Type = "STRUCT";
            }
            else
            {
                Name = ClearString((Dummy[C].Split(':'))[0]);
                Type = ClearString((Dummy[C].Split(':'))[1]);
            }

            switch (Type)
            {
                case "STRUCT" :
                    LastOffset = "2.0";
                    LastDataType = "STRUCT";
                    ret_val = new SiemensStructure(Name, Type, "DB"+DBNumber.ToString(), "2.0");
                    break;
                case "Struct":
                    ret_val = new SiemensStructure(Name, Type, "DB" + DBNumber.ToString(), OffsetCalculator(Type));
                    break;
                case "Bool":
                    return new SiemensVariable(Name, Type, "DB" + DBNumber.ToString(), OffsetCalculator(Type));
                case "Int":
                    return new SiemensVariable(Name, Type, "DB" + DBNumber.ToString(), OffsetCalculator(Type));
                default:
                    foreach (SiemensUDT udt in UDT)
                    {
                        if (udt.UDTName == Type)
                        {
                            ret_val = new SiemensStructure(Name, "Struct", "DB" + DBNumber.ToString(), OffsetCalculator(Type), udt, LastDataType);
                            LastDataType = "Int";
                            LastOffset = ((SiemensStructure)ret_val).LastOffset;
                            break;
                        }
                      
                    }     
                    return ret_val;
            }

            C++;
            while (!Dummy[C].Contains("END_STRUCT"))
            {
                ((SiemensStructure)ret_val).Add(StructureGenerator());

                C++;
            }
            LastOffset = ((SiemensStructure)ret_val).LastOffset;
            LastDataType = "Int";
            return ret_val;
        }

        string OffsetCalculator(string _Type)
        {
            string ret_val = "";
            switch (_Type)
            {
                case "Struct":
                    if (LastDataType.Contains("Int") || LastDataType.Contains("Bool"))
                    {
                        ret_val = (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0]))) ?
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".0" :
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 2).ToString() + ".0";
                    }
                    else
                    {
                        ret_val = LastOffset;
                    }
                    break;
                case "Int":
                    if (LastDataType.Contains("Int") || LastDataType.Contains("Bool"))
                    {
                        ret_val = (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0]))) ?
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".0" :
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 2).ToString() + ".0";
                    }
                    else
                    {
                        ret_val = LastOffset;
                    }
                    break;
                case "Bool":

                    if (LastDataType.Contains("Bool"))
                    {
                        ret_val = (int.Parse((LastOffset.Split('.'))[1]) == 7) ?
                            (int.Parse((LastOffset.Split('.'))[0]) + 1).ToString() + ".0" :
                            ((LastOffset.Split('.'))[0] + "." + (int.Parse((LastOffset.Split('.'))[1]) + 1).ToString());
                        
                    }
                    else
                    {
                        if (LastDataType.Contains("Struct"))
                        {
                            ret_val = LastOffset;
                        }
                        else
                        {
                            ret_val = (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0]))) ?
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".0" :
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 2).ToString() + ".0";
                        }
                    }
                   
                    break;
                default:
                    if (LastDataType.Contains("Int") || LastDataType.Contains("Bool"))
                    {
                        ret_val = (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0]))) ?
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".0" : 
                            (int.Parse((LastOffset.ToString().Split('.'))[0]) + 2).ToString() + ".0";
                    }
                    else
                    {
                        ret_val = LastOffset;
                    }
                    break;
            }
            LastOffset = ret_val;
            LastDataType = _Type;
            return ret_val;
        }

        List<string> getParsedData(SiemensStructure _ss, string structName)
        {
            List<string> ret_val = new List<string>();
            foreach (object ss in _ss.Data)
            {
                switch (ss.GetType().ToString())
                {
                    case "AlarmGenerator.SiemensStructure": ret_val.AddRange(getParsedData((SiemensStructure)ss, _ss.Name)); break;
                    case "AlarmGenerator.SiemensVariable":
                        ret_val.Add((((SiemensVariable)ss).SiemensVariableAddress + " " + DBName + " " + structName + " " +_ss.Name + " " + ((SiemensVariable)ss).Name).Replace("  "," "));
                        break;    
                }
            }

            return ret_val;

        }

        List<HMITag> generateHMITags(SiemensStructure _ss, string structName)
        {
            List<HMITag> ret_val = new List<HMITag>();

            int structNameCounter = 1;
            for (int i = 0; i < _ss.Data.Count; i++)
            {
                switch (_ss.Data[i].GetType().ToString())
                {
                    case "AlarmGenerator.SiemensStructure":
                        if (((SiemensStructure)_ss.Data[i]).Length == "2.0")
                        {
                            if (((SiemensStructure)_ss.Data[i]).GetType().Name == "SiemensStructure")
                              //  if ((((SiemensStructure)_ss.Data[i]).Data[0]).GetType().Name == "SiemensStructure")

                            {
                                    ret_val.AddRange(generateHMITags((SiemensStructure)_ss.Data[i], (structName + " " + ((SiemensStructure)_ss.Data[i]).Name).Replace(" Alarm", "")));
                            }
                            else 
                            {
                                List<string> vars = new List<string>();
                                for (int u = 0; u <= 15; u++)
                                {
                                    if (u < ((SiemensStructure)_ss.Data[i]).Data.Count)
                                    {
                                        if (((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Type == "Int")
                                        {
                                            vars.Add("-");
                                        }
                                        else
                                        {
                                            if (GetStructname(((SiemensStructure)_ss.Data[i]).Name) != "")
                                            {
                                                if (DBName.Replace(" Alarm", "").Replace(" " + GetStructname(((SiemensStructure)_ss.Data[i]).Name), "").Contains(" "))
                                                {
                                                    string [] t = DBName.Replace(" Alarm", "").Replace(" " + GetStructname(((SiemensStructure)_ss.Data[i]).Name), "").Split(' ');
                                                    vars.Add(t[0] + " : " + t[1] + " : " + GetStructname(((SiemensStructure)_ss.Data[i]).Name) + " " + ((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Name);

                                                }
                                                else 
                                                {
                                                    vars.Add(DBName.Replace(" Alarm", "").Replace(" " + GetStructname(((SiemensStructure)_ss.Data[i]).Name), "") + " : " + GetStructname(((SiemensStructure)_ss.Data[i]).Name) + " " + ((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Name);
                                                }

                                            }

                                            else
                                            {
                                                if (DBName.Replace(" Alarm", "").Contains(" "))
                                                {
                                                    string[] t = DBName.Replace(" Alarm", "").Split(' ');
                                                    if (((SiemensStructure)_ss.Data[i]).Name == "Message")
                                                    {
                                                        vars.Add(t[0] + " :" + GetStructname(((SiemensStructure)_ss.Data[i]).Name) + " " + ((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Name);


                                                    }
                                                    else 
                                                    {
                                                        vars.Add(t[0] + " : " + t[1] + " :" + GetStructname(((SiemensStructure)_ss.Data[i]).Name) + " " + ((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Name);

                                                    }


                                                }
                                                else
                                                {
                                                    vars.Add(DBName.Replace(" Alarm", "") + " :" + GetStructname(((SiemensStructure)_ss.Data[i]).Name) + " " + ((SiemensVariable)((SiemensStructure)_ss.Data[i]).Data[u]).Name);
                                                }


                                            }
                                        }
                                    }
                                    else
                                    {
                                        vars.Add("-");
                                    }
                                }
                                ret_val.Add(new HMITag(CPU, (structName + " " + ((SiemensStructure)_ss.Data[i]).Name), TagPath, Connection, ((SiemensStructure)_ss.Data[i]).StartAddress, vars));
                             //   ret_val.Add(new HMITag(CPU, (structName + " " + ((SiemensStructure)_ss.Data[i]).Name).Replace(" Alarm", ""), TagPath, Connection, ((SiemensStructure)_ss.Data[i]).StartAddress, vars));

                            }
                            break;
                        }
                        else
                        {
                            ret_val.AddRange(generateHMITags((SiemensStructure)_ss.Data[i], (structName + " " + ((SiemensStructure)_ss.Data[i]).Name)));
                           // ret_val.AddRange(generateHMITags((SiemensStructure)_ss.Data[i], (structName + " " + ((SiemensStructure)_ss.Data[i]).Name).Replace(" Alarm", "")));

                            break;
                        }
    
                    case "AlarmGenerator.SiemensVariable":
                        if (((SiemensVariable)_ss.Data[i]).Type == "Int")
                        {
                            List<string> vars = new List<string>();
                            for (int u = 0; u <= 15; u++)
                            {
                                vars.Add("-");

                            }
                            ret_val.Add(new HMITag(CPU, (structName + " " + ((SiemensVariable)_ss.Data[i]).Name), TagPath, Connection, ((SiemensVariable)_ss.Data[i]).SiemensVariableAddress,vars));
                        //    ret_val.Add(new HMITag(CPU, (structName + " " + ((SiemensVariable)_ss.Data[i]).Name).Replace(" Alarm", ""), TagPath, Connection, ((SiemensVariable)_ss.Data[i]).SiemensVariableAddress, vars));

                        }
                        else
                        {
                            int counter = 0;
                            int startBool = i;
                            while (_ss.Data[i].GetType().ToString().Contains("SiemensVariable"))
                            {
                                if (((SiemensVariable)_ss.Data[i]).Type == "Bool")
                                {
                                    counter++;
                                    if ( _ss.Data.Count-1 == i )
                                    {
                                        break;
                                    }
                                    i++;
                                    
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (i == _ss.Data.Count - 1)
                            {
                                if (_ss.Data[i].GetType().ToString().Contains("SiemensStructure"))
                                {
                                    i--;
                                }
                                else
                                {
                                    if (((SiemensVariable)_ss.Data[i]).Type == "Int")
                                    {
                                        i--;
                                    }
                                }
                            }
                            else
                            {
                                i--;
                            }
                           

                            
                            double result = counter / 16;
                            int words = 0;
                            if (counter % 16 == 0)
                            {
                                words = counter / 16;
                            }
                            else
                            {
                                words = counter / 16 + 1;
                            }
                            string address = "%DB" + DBNumber + ".DBW" + ((SiemensVariable)_ss.Data[startBool]).Offset.Split('.')[0]; 
                            for (int j = 0; j < words; j++)
                            {
                                List<string> vars = new List<string>();
                                for (int k = 16 * j; k <= ( j + 1 ) * 16 - 1; k++)
                                {
                                    if (k < counter )
                                    {
                                        if (DBName.Replace(" Alarm", "").Contains(" "))
                                        {
                                            string[] t = DBName.Split(' ');
                                          //  string[] t = DBName.Replace(" Alarm", "").Split(' ');
                                            vars.Add(t[0] + " : " + t[1] + " : " + GetStructname(_ss.Name) + " " + ((SiemensVariable)_ss.Data[startBool + k]).Name);

                                        }
                                        else
                                        {
                                            vars.Add(DBName + " : " + GetStructname(_ss.Name) + " " + ((SiemensVariable)_ss.Data[startBool + k]).Name);
                                           // vars.Add(DBName.Replace(" Alarm", "") + " : " + GetStructname(_ss.Name) + " " + ((SiemensVariable)_ss.Data[startBool + k]).Name);

                                        }


                                    }
                                    else
                                    {
                                        vars.Add("-");
                                    }
                                   
                                }

                                ret_val.Add(new HMITag(CPU, structName + " " + structNameCounter.ToString(), TagPath, Connection, address, vars));
                                structNameCounter++;
                                address = getNewAddress(address, 2);
                            }
                        }
                        break;
                }
            }

            return ret_val;
        }

        List<HMIAlarm> generateHMIAlarms(List<HMITag> _hmiTags, int _ID)
        {
            List < HMIAlarm > ret_val = new List<HMIAlarm>();

            LastId = _ID;

            foreach (HMITag hmitag in _hmiTags)
            {
                for (int i = 0; i <= 15; i++)
                {
                    ret_val.Add(new HMIAlarm(CPU, LastId.ToString(), Prefix, hmitag.Bits[i], hmitag.Name, TriggerBitGenerator(i)));
                    LastId++;
                }
            }

            return ret_val;
        }

        List<VisiWinVariable> generateVisiWinVariables(List<HMITag> hmiTags)
        {
            List<VisiWinVariable> ret_val = new List<VisiWinVariable>();

            foreach (HMITag hmitag in hmiTags)
            {
                ret_val.Add(new VisiWinVariable(Parent, hmitag.Name, AdjustAddress(hmitag.Address), hmitag.Bits));
            }

            return ret_val;
        }

        List<VisiWinAlarm> generateVisiWinAlarms(List<VisiWinVariable> vwVariables, int _ID)
        {
            List<VisiWinAlarm> ret_val = new List<VisiWinAlarm>();
            LastId = _ID;

            foreach (VisiWinVariable vwVariable in vwVariables)
            {
               
                for (int i = 0; i <= 15; i++)
                {
                    ret_val.Add(new VisiWinAlarm(LastId.ToString(), Prefix, Parent + "." + vwVariable.Alias + "{." + TriggerBitGenerator(i) + "}", vwVariable.Bits[i], vwVariable.ItemAccess != "" ? false : true));
                    LastId++;
                }
                
            }
            return ret_val;
        }

        private string AdjustAddress(string param)
        {
            string temp = param;
            temp = temp.Replace("%DB", "L1.DB");
            temp = temp.Replace("DBW", "INT");
            return temp;

        }

        string TriggerBitGenerator(int _i)
        {
            if (_i + 8 <= 15)
            {
                return (_i + 8).ToString();
            }
            else
            {
                return (_i - 8).ToString();
            }
        }

        string getNewAddress(string _a, int _o)
        {
            int ret_val = 0;
            string [] asx = _a.Split('.');
            asx[1] = asx[1].Replace("DBW", "");
            ret_val = int.Parse(asx[1]) + _o;
                return asx[0]+".DBW" + ret_val.ToString();

        }

        public override string ToString()
        {
            return "DB" + DBNumber + " - " + DBName;
        }

        public int CompareTo(SiemensDB that)
        {
            if (this.DBNumber < that.DBNumber) return -1;
            if (this.DBNumber == that.DBNumber) return 0;
            return 1;
        }

        private string GetStructname(string _a)
        {
            if (_a != "")
            {
                return _a.Replace("Warning", "").Replace("Error", "").Replace("Message", "").Replace("Predictive", "");
            }
            else 
            {
                return _a;
            } 
        }
    }
}
