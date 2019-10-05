namespace JsonToAzureAppSettings
{
    public class Setting
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool SlotSetting { get; set; }
    }
}
