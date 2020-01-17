using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppStarter.Parameters
{
    public class ParameterSet
    {
        [Option(
        longName: "ConfigFolderPath",
        Required = true,
        HelpText = "The folder path containing configuration files such as (app-config.xml and nlog.config)")]
        public string ConfigFolderPath { get; set; } = "";
    }
}
