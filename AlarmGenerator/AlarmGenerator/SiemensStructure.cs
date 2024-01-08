using System.Collections.Generic;

namespace AlarmGenerator
{
    public class SiemensStructure
    {
        public List<object> Data = new List<object>();
        public string Name { get; set; }
        public string Type { get; set; }
        public string StartAddress { get; set; }
        public string FirstOffset { get; set; }
        public string LastOffset { get; set; }
        public string Length { get; set; }
        public string LastDataType { get; set; }

        public SiemensStructure(string _Name, string _Type, string _DBNumber, string _Offset)
        {
            Name = _Name;
            Type = _Type;
            LastOffset = (int.Parse((_Offset.Split('.'))[0]) + 1).ToString() + ".7"; ;
            FirstOffset = _Offset;
            LastDataType = "";
            StartAddress = "%" + _DBNumber + ".DBW" + (FirstOffset.Split('.'))[0];
            Length = "2.0";
        }

        public SiemensStructure(string _Name, string _Type, string _DBNumber, string _Offset, SiemensUDT _UDT, string _LastDataType)
        {
            Name = _Name;
            Type = _Type;
            FirstOffset = LastOffset = _Offset;
            Length = "2.0";
            LastDataType = "UDT";
            foreach (SiemensVariable v in _UDT.UDT)
            {
                SiemensVariable temp = new SiemensVariable(v.Name, v.Type, _DBNumber, _Offset);
                string newOffset = OffsetCalculator(v.Type);
                temp.SiemensVariableAddress = "%" + _DBNumber + ".DBX" + newOffset;
                temp.Offset = newOffset;
                Data.Add(temp);
                switch (v.Type)
                {
                    case "Bool":
                        if (int.Parse((Length.Split('.'))[1]) == 7)
                        {
                            Length = (int.Parse((Length.Split('.'))[0]) + 1).ToString() + ".0";
                        }
                        else
                        {
                            Length = (Length.Split('.'))[0] + "." + (int.Parse((Length.Split('.'))[1]) + 1).ToString();
                        }
                        break;

                    case "Int":
                        Length = (int.Parse((Length.Split('.'))[0]) + 2) + (Length.Split('.'))[1];
                        break;

                }
            }
            LastDataType = "UDT";
            StartAddress = "%" + _DBNumber + ".DBW" + (_Offset.Split('.'))[0];
         //   FirstOffset = _Offset;
            if (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0])))
            {
                LastOffset = (int.Parse((LastOffset.ToString().Split('.'))[0])).ToString() + ".7";
            }
            else
            {
                LastOffset = (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".7";
            }
            Length = (int.Parse((LastOffset.ToString().Split('.'))[0]) - (int.Parse((FirstOffset.ToString().Split('.'))[0])) + 1).ToString() + ".0";
        }

        public void Add(object _V)
        {
            Data.Add(_V);
            switch (_V.GetType().ToString())
            {
                case "AlarmGenerator.SiemensStructure" :
                    LastOffset = ((SiemensStructure)_V).LastOffset;
                    break;
                case "AlarmGenerator.SiemensVariable" :
                    LastOffset = ((SiemensVariable)_V).Offset;
                    break;
            }

            if (IsOdd(int.Parse((LastOffset.ToString().Split('.'))[0])))
            {
                LastOffset = (int.Parse((LastOffset.ToString().Split('.'))[0])).ToString() + ".7";
            }
            else
            {
                LastOffset = (int.Parse((LastOffset.ToString().Split('.'))[0]) + 1).ToString() + ".7";
            }
            Length = (int.Parse((LastOffset.ToString().Split('.'))[0]) - (int.Parse((FirstOffset.ToString().Split('.'))[0])) + 1).ToString() + ".0";
        }

        string OffsetCalculator(string _Type)
        {
            string ret_val = "";
            switch (_Type)
            {

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
                        if (LastDataType.Contains("UDT"))
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
                //case "Byte":
                //    ret_val = LastOffset;   
                //    break;
            }
            LastOffset = ret_val;
            LastDataType = _Type;
            return ret_val;
        }

        public static bool IsOdd(double value)
        {
            return value % 2 != 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
