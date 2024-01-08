namespace AlarmGenerator
{
    public class VisiWinAlarm
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public string LowActive { get; set; }
        public string Comment { get; set; }
        public string ItemStateBitNumber { get; set; }
        public string Enabled { get; set; }
        public string ItemState { get; set; }
        public string ItemEventBitNumber { get; set; }
        public string ItemAcknowledgeBitNumber { get; set; }
        public string TextParams { get; set; }
        public string Parameter1 { get; set; }
        public string Class { get; set; }
        public string Priority { get; set; }
        public string Type { get; set; }
        public string ItemAcknowledge { get; set; }
        public string Parameter2 { get; set; }
        public string ItemEvent { get; set; }
        public string Text_1031 { get; set; }

        public VisiWinAlarm(string _Name, string _Prefix, string _ItemEvent, string _Text_1031, bool _Reserve)
        {

            Parent = _ItemEvent.Contains("Error") ? "Error" : _ItemEvent.Contains("Warning") ? "Warning" : _ItemEvent.Contains("Message") ? "Message" : _ItemEvent.Contains("Predictive") ? "Predictive" : "Undefined";
            Name = _Name;
            LowActive = "False";
            Comment = "";
            ItemStateBitNumber = "0";
            Enabled = "True";
            ItemState = "";
            ItemEventBitNumber = "0";
            ItemAcknowledgeBitNumber = "0";
            TextParams = "";
            Parameter1 = "";
            Class = _ItemEvent.Contains("Error") ? "Error" : _ItemEvent.Contains("Warning") ? "Warning" : _ItemEvent.Contains("Message") ? "Message" : _ItemEvent.Contains("Predictive") ? "Predictive" : "Undefined";
            Priority = "0";
            Type = "(Bit)";
            ItemAcknowledge = "";
            Parameter2 = _Name;
            ItemEvent= _Reserve ? "" : _ItemEvent;
            Text_1031 = _Prefix + _Text_1031;
 
        }

        public override string ToString()
        {
            return Text_1031;
        }

    }
}
