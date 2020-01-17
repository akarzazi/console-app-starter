using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ConsoleAppStarter.Configuration
{
    [XmlRoot("ConfigWithOptions")]
    public class ConfigWithOptions : IValidatableObject
    {
        public Option1 Option1 { get; set; }

        public Option2 Option2 { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            var either = (Option1 == null) != (Option2 == null);
            if (!either)
            {
                yield return new ValidationResult(
                    $"Either of {nameof(Option1)} or {nameof(Option2)} should be provided. \n",
                    new[] { nameof(Option1), nameof(Option2) });
            }
        }
    }

    [XmlRoot("Option1")]
    public class Option1
    {
    }

    [XmlRoot("Option2")]
    public class Option2
    {
        [XmlAttribute("RequiredAttribute")]
        public string RequiredAttribute { get; set; }

        [XmlAttribute("OptionalAttribute")]
        public string OptionalAttribute { get; set; }
    }
}
