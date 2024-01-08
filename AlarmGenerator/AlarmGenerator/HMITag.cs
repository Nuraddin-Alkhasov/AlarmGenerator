using System.Collections.Generic;

namespace AlarmGenerator
{
    public class HMITag
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Connection { get; set; }
        public string PLCtag { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public string Coding { get; set; }
        public string AccessMethod { get; set; }
        public string Address { get; set; }
        public string IndirectAddressing { get; set; }
        public string IndexTag { get; set; }
        public string StartValue { get; set; }
        public string IDTag { get; set; }
        public string DisplayName { get; set; }
        public string Comment { get; set; }
        public string AcquisitionMode { get; set; }
        public string AcquisitionCycle { get; set; }
        public string LimitUpper2Type { get; set; }
        public string LimitUpper2 { get; set; }
        public string LimitUpper1Type { get; set; }
        public string LimitUpper1 { get; set; }
        public string LimitLower2Type { get; set; }
        public string LimitLower2 { get; set; }
        public string LimitLower1Type { get; set; }
        public string LimitLower1 { get; set; }
        public string LinearScaling { get; set; }
        public string EndValuePLC { get; set; }
        public string StartValuePLC { get; set; }
        public string EndValueHMI { get; set; }
        public string StartValueHMI { get; set; }
        public string GmpRelevant { get; set; }
        public string ConfirmationType { get; set; }
        public string MandatoryCommenting { get; set; }
        public string CPU { get; set; }
        public List<string> Bits { get; set; }

        public HMITag(string _CPU, string _Name, string _Path, string _Connection, string _Address, List<string> _Bits)
        {
            CPU = _CPU;
            Name = _Name;
            Path = _Path + (_Name.Contains("Error") ? "Error" : _Name.Contains("Warning") ? "Warning" : _Name.Contains("Message") ? "Message" : _Name.Contains("Predictive") ? "Predictive" : "Undefined");
            Connection = _Bits[0] != "" ? _Connection : "<No Value>";
            PLCtag = "<No Value>";
            DataType = "Int";
            Length = "2";
            Coding = "Binary";
            AccessMethod = _Bits[0] != "" ? "Absolute access" : "<No Value>";
            Address =_Bits[0] != "" ? _Address : "<No Value>";
            IndirectAddressing = "False";
            IndexTag = "<No Value>";
            StartValue = "<No Value>";
            IDTag = "0";
            DisplayName = "<No Value>";
            Comment = "<No Value>";
            AcquisitionMode = "Continuous";
            AcquisitionCycle = "1 s";
            LimitUpper2Type = "None";
            LimitUpper2 = "<No Value>";
            LimitUpper1Type = "None";
            LimitUpper1 = "<No Value>";
            LimitLower2Type = "None";
            LimitLower2 = "<No Value>";
            LimitLower1Type = "None";
            LimitLower1 = "<No Value>";
            LinearScaling = "False";
            EndValuePLC = "10";
            StartValuePLC = "0";
            EndValueHMI = "100";
            StartValueHMI = "0";
            GmpRelevant = "False";
            ConfirmationType = "None";
            MandatoryCommenting = "False";

            Bits = _Bits;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
