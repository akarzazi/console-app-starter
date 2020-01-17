namespace ConsoleAppStarter.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlRoot("ListContainer")]
    public class ListContainer : IValidatableObject
    {
        [XmlElement("ListItem")]
        public List<ListItem> Items { get; set; } = new List<ListItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Items == null || Items?.Count == 0)
            {
                yield return new ValidationResult($"{ nameof(Items)} cannot be empty", new[] { nameof(Items) });
            }
        }
    }

    [XmlRoot("ListItem")]
    public class ListItem
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
