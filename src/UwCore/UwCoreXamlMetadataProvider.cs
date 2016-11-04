using System;
using Windows.UI.Xaml.Markup;

namespace UwCore
{
    public sealed class UwCoreXamlMetadataProvider : IXamlMetadataProvider
    {
        public IXamlType GetXamlType(Type type)
        {
            return null;
        }

        public IXamlType GetXamlType(string fullName)
        {
            return null;
        }

        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return new[]
            {
                new XmlnsDefinition
                {
                    Namespace = "uwCoreBehaviors",
                    XmlNamespace = "http://schemas.xemio.net/uwcore/behaviors"
                }
            };
        }
    }
}