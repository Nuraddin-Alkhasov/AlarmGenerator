namespace AlarmGenerator
{
    public class SiemensVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string SiemensVariableAddress { get; set; }
        public string Offset { get; set; }
        public string Length { get; set; }
        public SiemensVariable(string _Name, string _Type, string _DBNumber, string _Offset)
        {
            Name = _Name;
            Type = _Type;
            SiemensVariableAddress = "%" + _DBNumber + ".DBX" + _Offset;
            Offset = _Offset;
            switch (Type)
            {
                case "Bool":
                    SiemensVariableAddress = "%" + _DBNumber + ".DBX" + _Offset;
                    Length = "0.1";
                    break;
                case "Int":
                    SiemensVariableAddress = "%" + _DBNumber + ".DBW" + _Offset.Split('.')[0];
                    Length = "2.0";
                    break;
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
