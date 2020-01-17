using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ConsoleAppStarter.Configuration
{
    [XmlRoot("Configuration")]
    public class ConfigurationRoot
    {
        [Required]
        [XmlElement("Logging")]
        public Logging Logging { get; set; }

        [Required]
        [XmlElement("ConfigElement")]
        public ConfigElement ConfigElement { get; set; }

        [Required]
        [XmlElement("ConfigWithOptions")]
        public ConfigWithOptions ConfigWithOptions { get; set; }

        [Required]
        [XmlElement("ListContainer")]
        public ListContainer ListContainer { get; set; }

    }

    [XmlRoot("Logging")]
    public class Logging
    {
        [Required]
        [XmlAttribute("LogPath")]
        public string LogPath { get; set; }
    }
}
