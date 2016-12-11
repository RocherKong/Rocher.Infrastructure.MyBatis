namespace IBatisNet.Common.Logging
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging.Impl;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;

    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        private static readonly string ARGUMENT_ELEMENT = "arg";
        private static readonly string ARGUMENT_ELEMENT_KEY_ATTRIB = "key";
        private static readonly string ARGUMENT_ELEMENT_VALUE_ATTRIB = "value";
        private static readonly string LOGFACTORYADAPTER_ELEMENT = "logFactoryAdapter";
        private static readonly string LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB = "type";

        public object Create(object parent, object configContext, XmlNode section)
        {
            int count = section.SelectNodes(LOGFACTORYADAPTER_ELEMENT).Count;
            if (count > 1)
            {
                throw new IBatisNet.Common.Exceptions.ConfigurationException("Only one <logFactoryAdapter> element allowed");
            }
            if (count == 1)
            {
                return this.ReadConfiguration(section);
            }
            return null;
        }

        private LogSetting ReadConfiguration(XmlNode section)
        {
            XmlNode node = section.SelectSingleNode(LOGFACTORYADAPTER_ELEMENT);
            string strA = string.Empty;
            if (node.Attributes[LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB] != null)
            {
                strA = node.Attributes[LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB].Value;
            }
            if (strA == string.Empty)
            {
                throw new IBatisNet.Common.Exceptions.ConfigurationException("Required Attribute '" + LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB + "' not found in element '" + LOGFACTORYADAPTER_ELEMENT + "'");
            }
            Type factoryAdapterType = null;
            try
            {
                if (string.Compare(strA, "CONSOLE", true) == 0)
                {
                    factoryAdapterType = typeof(ConsoleOutLoggerFA);
                }
                else if (string.Compare(strA, "TRACE", true) == 0)
                {
                    factoryAdapterType = typeof(TraceLoggerFA);
                }
                else if (string.Compare(strA, "NOOP", true) == 0)
                {
                    factoryAdapterType = typeof(NoOpLoggerFA);
                }
                else
                {
                    factoryAdapterType = Type.GetType(strA, true, false);
                }
            }
            catch (Exception exception)
            {
                throw new IBatisNet.Common.Exceptions.ConfigurationException("Unable to create type '" + strA + "'", exception);
            }
            XmlNodeList list = node.SelectNodes(ARGUMENT_ELEMENT);
            NameValueCollection properties = null;
            properties = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            foreach (XmlNode node2 in list)
            {
                string name = string.Empty;
                string str3 = string.Empty;
                XmlAttribute attribute = node2.Attributes[ARGUMENT_ELEMENT_KEY_ATTRIB];
                XmlAttribute attribute2 = node2.Attributes[ARGUMENT_ELEMENT_VALUE_ATTRIB];
                if (attribute == null)
                {
                    throw new IBatisNet.Common.Exceptions.ConfigurationException("Required Attribute '" + ARGUMENT_ELEMENT_KEY_ATTRIB + "' not found in element '" + ARGUMENT_ELEMENT + "'");
                }
                name = attribute.Value;
                if (attribute2 != null)
                {
                    str3 = attribute2.Value;
                }
                properties.Add(name, str3);
            }
            return new LogSetting(factoryAdapterType, properties);
        }
    }
}

