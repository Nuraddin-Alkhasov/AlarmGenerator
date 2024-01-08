namespace AlarmGenerator
{
    public class HMIAlarm
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string AlarmText { get; set; }
        public string FieldInfo { get; set; }
        public string Class { get; set; }
        public string TriggerTag { get; set; }
        public string TriggerBit { get; set; }
        public string AcknowledgementTag { get; set; }
        public string AcknowledgementBit { get; set; }
        public string PLCAcknowledgementTag { get; set; }
        public string PLCAcknowledgementBit { get; set; }
        public string Group { get; set; }
        public string Report { get; set; }
        public string InfoText { get; set; }
        public string CPU { get; set; }
        public HMIAlarm(string _CPU, string _ID, string _Prefix, string _AlarmText, string _TriggerTag, string _TriggerBit)
        {
            CPU = _CPU;
            ID = _ID;
            Name = "Discrete alarm_" + _ID;
            AlarmText = _Prefix + _AlarmText;
            FieldInfo = "";
            Class = _TriggerTag.Contains("Error") ? "Error" : _TriggerTag.Contains("Warning") ? "Warning" : _TriggerTag.Contains("Message") ? "Message" : _TriggerTag.Contains("Predictive") ? "Predictive" : "Undefined";
            TriggerTag = _TriggerTag;
            TriggerBit = _TriggerBit;
            AcknowledgementTag = "<No value>";
            AcknowledgementBit = "0";
            PLCAcknowledgementTag = "<No value>";
            PLCAcknowledgementBit = "0";
            Group = "<No value>";
            Report = "False";
            InfoText = "<No value>";
        }

        public override string ToString()
        {
            return AlarmText;
        }
    }
}
