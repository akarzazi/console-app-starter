using System.Xml.Serialization;

namespace ConsoleAppStarter.Configuration
{
    [XmlRoot("ConfigElement")]
    public class ConfigElement
    {
        [XmlAttribute("MyEnum")]
        public MyEnum MyEnum { get; set; }
    }
}
