using Helpers.Validation.DataAnnotationsValidator;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ConsoleAppStarter.Configuration
{
    public class ConfigurationParser
    {
        ILogger<ConfigurationParser> _logger;

        public ConfigurationParser(ILogger<ConfigurationParser> logger)
        {
            _logger = logger;
        }

        public ConfigurationRoot Parse(string configPath)
        {
            ValidateSchema(configPath);

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationRoot));
            ConfigurationRoot config;
            using (Stream reader = new FileStream(configPath, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                config = (ConfigurationRoot)serializer.Deserialize(reader);
            }

            var validationResults = new List<ValidationResult>();
            DataAnnotationsValidator.TryValidateObjectRecursive(config, validationResults);
            if (validationResults.Count > 0)
            {
                var errorsStr = string.Join("\n", validationResults.Select(FormatValidationResult));
                throw new Exception("Invalid configuration file, Validation errors : \n" + errorsStr);
            }

            return config;
        }

        private string FormatValidationResult(ValidationResult validationResult)
        {
            var str = validationResult.ErrorMessage;
            str += string.Join("\n", validationResult.MemberNames);
            return str;
        }

        private void ValidateSchema(string xmlPath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);
            var schemaAttribute = xml.DocumentElement.Attributes["xsi:noNamespaceSchemaLocation"];
            if (schemaAttribute == null)
            {
                _logger.LogWarning("No XSD schema found, validation skipped");
                // do not valide, let user remove schema if too restrictive
                return;
            }

            var directory = Path.GetDirectoryName(xmlPath);
            xml.Schemas.Add(null, Path.Combine(directory, schemaAttribute.Value));
            try
            {
                xml.Validate(null);
            }
            catch (XmlSchemaValidationException ex)
            {
                if (ex.SourceObject is XmlNode errorNode)
                {
                    var message = $@"{ex.Message} near \n{errorNode.OuterXml} owener element \n{errorNode.ParentNode?.OuterXml}";
                    throw new XmlSchemaValidationException(message, ex);
                }
                throw ex;
            }
        }

        private static void validationEventHandler(object sender, ValidationEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
