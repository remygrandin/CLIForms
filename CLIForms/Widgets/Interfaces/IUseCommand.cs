using System.Xml.Serialization;

namespace CLIForms.Widgets.Interfaces
{
    interface IUseCommand
    {
        [XmlAttribute]
        string Command { get; set; }
    }
}
