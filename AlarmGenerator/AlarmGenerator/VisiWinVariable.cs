using System.Collections.Generic;

namespace AlarmGenerator
{
    public class VisiWinVariable
    {
        public string Parent { get; set; }
        public string Alias { get; set; }
        public string SendOnCancel { get; set; }
        public string DefaultValue { get; set; }
        public string AccessRights { get; set; }
        public string Comment { get; set; }
        public string DataType { get; set; }
        public string UnitClassProcess { get; set; }
        public string ItemAccess { get; set; }
        public string SendOnStartup { get; set; }
        public string DisableDisintegration { get; set; }
        public string SubstitutionValueMode { get; set; }
        public string WriteThru { get; set; }
        public string AutoBlockOptimization { get; set; }
        public string MaxValue { get; set; }
        public string FieldSize { get; set; }
        public string MinValueItem { get; set; }
        public string ClientIntern { get; set; }
        public string TextParams { get; set; }
        public string SubstitutionValue { get; set; }
        public string Group { get; set; }
        public string Enabled { get; set; }
        public string AccessPath { get; set; }
        public string UnitClassDisplay { get; set; }
        public string MinValue { get; set; }
        public string DenyPublicOPCAccess { get; set; }
        public string MaxValueItem { get; set; }
        public string SendOnExit { get; set; }
        public string LogChanges { get; set; }
        public string SendToOPCServer { get; set; }
        public string Text_1031 { get; set; }

        public List<string> Bits { get; set; }

        public VisiWinVariable(string _Parent, string _Alias, string _ItemAccess, List<string> _Bits)
        {
            Parent = _Parent;
            Alias = "Alarms." + (_Alias.Contains("Error") ? "Error" : _Alias.Contains("Warning") ? "Warning" : _Alias.Contains("Message") ? "Message" : _Alias.Contains("Predictive") ? "Predictive" : "Undefined") + "." + _Alias;
            SendOnCancel = "False";
            DefaultValue = "";
            AccessRights = "R/W";
            Comment = "";
            DataType = "VT_I2";
            UnitClassProcess = "(no Unit class)";
            ItemAccess = _Bits[0] != "" ? _ItemAccess : "";
            SendOnStartup = "False";
            DisableDisintegration = "False";
            SubstitutionValueMode = "0";
            WriteThru = "True";
            AutoBlockOptimization = "False";
            MaxValue = "0";
            FieldSize = "0";
            MinValueItem = "";
            ClientIntern = "False";
            TextParams = "False";
            SubstitutionValue = "False";
            Group = _Alias.Contains("Error") ? "Error" : _Alias.Contains("Warning") ? "Warning" : _Alias.Contains("Message") ? "Message" : _Alias.Contains("Predictive") ? "Predictive" : "Undefined";
            Enabled = "True";
            AccessPath = "";
            UnitClassDisplay = "(no Unit class)";
            MinValue = "0";
            DenyPublicOPCAccess = "False";
            MaxValueItem = "";
            SendOnExit = "False";
            LogChanges = "False";
            SendToOPCServer = "False";
            Text_1031 = "";
            Bits = _Bits;
    }

        public override string ToString()
        {
            return Alias;
        }

    }
}
