namespace AlarmGenerator
{
    public class NeedUDT
    {
        public NeedUDT(string _Type, string _DBName, string _DBNumber, string _line)
        {
            Type = _Type;
            DBName = _DBName;
            DBNumber = _DBNumber;
            line = _line;
            TypeName = getTypeName(_line);
            Supported = getSupported();
            isUDT = getIsUDT();
        }
        public string Type;
        public string DBName;
        public string DBNumber;
        string line;
        public string TypeName;
        public bool Supported;
        public bool isUDT;
        string getTypeName(string _line)
        {
            string[] temp = _line.Split(':');
            return ClearString(temp[0]);
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

        bool getSupported() 
        {
            if (Type.Contains("Array"))
            {
                return false;
            }
            else 
            {
                switch (Type)
                {
                    case "Byte": return false;
                    case "String": return false;
                    case "WString": return false;
                    case "Char": return false;
                    case "WChar": return false;
                    case "DInt": return false;
                    case "LInt": return false;
                    case "SInt": return false;
                    case "UDInt": return false;
                    case "UInt": return false;
                    case "ULInt": return false;
                    case "USInt": return false;
                    case "Word": return false;
                    case "DWord": return false;
                    case "LWord": return false;
                    case "Real": return false;
                    case "LReal": return false;
                    case "Bool": return true;
                    case "Int": return false;
                    default: return true;
                }
            }
          
        
        }

        bool getIsUDT()
        {
            if (Type.Contains("Array"))
            {
                return false;
            }
            else
            {
                switch (Type)
                {
                    case "Byte": return false;
                    case "String": return false;
                    case "WString": return false;
                    case "Char": return false;
                    case "WChar": return false;
                    case "DInt": return false;
                    case "LInt": return false;
                    case "SInt": return false;
                    case "UDInt": return false;
                    case "UInt": return false;
                    case "ULInt": return false;
                    case "USInt": return false;
                    case "Word": return false;
                    case "DWord": return false;
                    case "LWord": return false;
                    case "Real": return false;
                    case "LReal": return false;
                    case "Bool": return false;
                    case "Int": return false;
                    default: return true;
                }
            }


        }

        public override string ToString()
        {
            return Type;
        }
    }
}
