namespace IBatisNet.Common.Xml
{
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class NodeUtils
    {
        public static bool GetBooleanAttribute(NameValueCollection attributes, string name, bool def)
        {
            string s = attributes[name];
            if (s == null)
            {
                return def;
            }
            return XmlConvert.ToBoolean(s);
        }

        public static byte GetByteAttribute(NameValueCollection attributes, string name, byte def)
        {
            string s = attributes[name];
            if (s == null)
            {
                return def;
            }
            return XmlConvert.ToByte(s);
        }

        public static int GetIntAttribute(NameValueCollection attributes, string name, int def)
        {
            string s = attributes[name];
            if (s == null)
            {
                return def;
            }
            return XmlConvert.ToInt32(s);
        }

        public static string GetStringAttribute(NameValueCollection attributes, string name)
        {
            string str = attributes[name];
            if (str == null)
            {
                return string.Empty;
            }
            return str;
        }

        public static string GetStringAttribute(NameValueCollection attributes, string name, string def)
        {
            string str = attributes[name];
            if (str == null)
            {
                return def;
            }
            return str;
        }

        public static NameValueCollection ParseAttributes(XmlNode node)
        {
            return ParseAttributes(node, null);
        }

        public static NameValueCollection ParseAttributes(XmlNode node, NameValueCollection variables)
        {
            NameValueCollection values = new NameValueCollection();
            int count = node.Attributes.Count;
            for (int i = 0; i < count; i++)
            {
                XmlAttribute attribute = node.Attributes[i];
                string str = ParsePropertyTokens(attribute.Value, variables);
                values.Add(attribute.Name, str);
            }
            return values;
        }

        public static string ParsePropertyTokens(string str, NameValueCollection properties)
        {
            string str2 = "${";
            string str3 = "}";
            string str4 = str;
            if ((str4 != null) && (properties != null))
            {
                int index = str4.IndexOf(str2);
                for (int i = str4.IndexOf(str3); (index > -1) && (i > index); i = str4.IndexOf(str3))
                {
                    string str5 = str4.Substring(0, index);
                    string str6 = str4.Substring(i + str3.Length);
                    int startIndex = index + str2.Length;
                    string name = str4.Substring(startIndex, i - startIndex);
                    string str8 = properties.Get(name);
                    if (str8 == null)
                    {
                        str4 = str5 + name + str6;
                    }
                    else
                    {
                        str4 = str5 + str8 + str6;
                    }
                    index = str4.IndexOf(str2);
                }
            }
            return str4;
        }
    }
}

