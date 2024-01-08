using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AlarmGenerator
{
    public class SiemensUDT
    {
        public string UDTName { get; set; }

        public SiemensVariable[] UDT { set; get; }

        public SiemensUDT(string _UDTName, string _UDTData)
        {
            UDTName = _UDTName;
            UDT = PrepareDB(_UDTData);
        }

        #region  - - - Prepare DB - - -

        SiemensVariable[] PrepareDB(string _Data)
        {
            return clearVariable(cutData(formatRows(removeComments(splitRows(_Data)))));
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

        string[] formatRows(string[] _DB)
        {
            for (int i = 0; i < _DB.Length; i++)
            {
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
                    while (i >= 0)
                    {
                        ret_val.RemoveAt(i);
                        i--;
                    }
                    break;
                }
            }

            for (int i = 0; i < ret_val.Count; i++)
            {
                if (ret_val[i].Contains("END_STRUCT"))
                {
                    while (i != ret_val.Count)
                    {
                        ret_val.RemoveAt(i);
                    }
                    break;
                }
            }
           
            return ret_val.ToArray();
        }

        SiemensVariable[] clearVariable(string[] _DB)
        {
            SiemensVariable[] ret_val = new SiemensVariable[_DB.Length];
            for (int i = 0; i < _DB.Length; i++)
            {
                if (ClearString((_DB[i].Split(':'))[1]) == "Bool")
                {
                    ret_val[i] = new SiemensVariable(ClearString((_DB[i].Split(':'))[0]), ClearString((_DB[i].Split(':'))[1]), "X", "X");
                }
                else 
                {
                    MessageBox.Show(UDTName +" has unsupported data type : " + ClearString((_DB[i].Split(':'))[1]) + Environment.NewLine+
                                       "You are allowed only to use bools in UDT.", "Not supported data Type");
                    UDTName = "Unsupported data type: " + UDTName + " - " + ClearString((_DB[i].Split(':'))[1]);
                }
            }
            return ret_val;
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

        #endregion

        public override string ToString()
        {
            return UDTName;
        }

    }
}
