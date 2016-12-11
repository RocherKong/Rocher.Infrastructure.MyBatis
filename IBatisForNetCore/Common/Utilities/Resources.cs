namespace IBatisNet.Common.Utilities
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Xml;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Xml;

    public class Resources
    {
        private static string _applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private static string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string PROTOCOL_SEPARATOR = "://";

        public static bool FileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }
            FileIOPermission permission = null;
            try
            {
                permission = new FileIOPermission(FileIOPermissionAccess.Read, filePath);
            }
            catch
            {
                return false;
            }
            try
            {
                permission.Demand();
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("iBATIS doesn't have the right to read the config file \"{0}\". Cause : {1}", filePath, exception.Message), exception);
            }
            return false;
        }

        public static XmlDocument GetAsXmlDocument(XmlNode node, NameValueCollection properties)
        {
            XmlDocument embeddedResourceAsXmlDocument = null;
            if (node.Attributes["resource"] != null)
            {
                return GetResourceAsXmlDocument(NodeUtils.ParsePropertyTokens(node.Attributes["resource"].Value, properties));
            }
            if (node.Attributes["url"] != null)
            {
                return GetUrlAsXmlDocument(NodeUtils.ParsePropertyTokens(node.Attributes["url"].Value, properties));
            }
            if (node.Attributes["embedded"] != null)
            {
                embeddedResourceAsXmlDocument = GetEmbeddedResourceAsXmlDocument(NodeUtils.ParsePropertyTokens(node.Attributes["embedded"].Value, properties));
            }
            return embeddedResourceAsXmlDocument;
        }

        public static XmlDocument GetConfigAsXmlDocument(string resourcePath)
        {
            XmlDocument document = new XmlDocument();
            XmlTextReader reader = null;
            resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);
            if (!FileExists(resourcePath))
            {
                resourcePath = Path.Combine(_baseDirectory, resourcePath);
            }
            try
            {
                reader = new XmlTextReader(resourcePath);
                document.Load(reader);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load config file \"{0}\". Cause : {1}", resourcePath, exception.Message), exception);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return document;
        }

        public static XmlDocument GetEmbeddedResourceAsXmlDocument(string resource)
        {
            XmlDocument document = new XmlDocument();
            bool flag = false;
            FileAssemblyInfo info = new FileAssemblyInfo(resource);
            if (info.IsAssemblyQualified)
            {
                Assembly assembly = null;
                assembly = Assembly.Load(info.AssemblyName);
                Stream manifestResourceStream = assembly.GetManifestResourceStream(info.ResourceFileName);
                if (manifestResourceStream == null)
                {
                    manifestResourceStream = assembly.GetManifestResourceStream(info.FileName);
                }
                if (manifestResourceStream == null)
                {
                    goto Label_00DC;
                }
                try
                {
                    document.Load(manifestResourceStream);
                    flag = true;
                    goto Label_00DC;
                }
                catch (Exception exception)
                {
                    throw new ConfigurationException(string.Format("Unable to load file \"{0}\" in embedded resource. Cause : {1}", resource, exception.Message), exception);
                }
            }
            foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
            {
                Stream inStream = assembly2.GetManifestResourceStream(info.FileName);
                if (inStream != null)
                {
                    try
                    {
                        document.Load(inStream);
                        flag = true;
                        break;
                    }
                    catch (Exception exception2)
                    {
                        throw new ConfigurationException(string.Format("Unable to load file \"{0}\" in embedded resource. Cause : ", resource, exception2.Message), exception2);
                    }
                }
            }
        Label_00DC:
            if (!flag)
            {
                _logger.Error("Could not load embedded resource from assembly");
                throw new ConfigurationException(string.Format("Unable to load embedded resource from assembly \"{0}\".", info.OriginalFileName));
            }
            return document;
        }

        public static FileInfo GetFileInfo(string resourcePath)
        {
            FileInfo info = null;
            resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);
            if (!FileExists(resourcePath))
            {
                resourcePath = Path.Combine(_applicationBase, resourcePath);
            }
            try
            {
                info = new FileInfo(resourcePath);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load file \"{0}\". Cause : \"{1}\"", resourcePath, exception.Message), exception);
            }
            return info;
        }

        public static XmlDocument GetFileInfoAsXmlDocument(FileInfo resource)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(resource.FullName);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load XmlDocument via FileInfo. Cause : {0}", exception.Message), exception);
            }
            return document;
        }

        public static string GetFileSystemResourceWithoutProtocol(string filePath)
        {
            int index = filePath.IndexOf("://");
            if (index == -1)
            {
                return filePath;
            }
            if (filePath.Length > (index + "://".Length))
            {
                while (filePath[++index] == '/')
                {
                }
            }
            return filePath.Substring(index);
        }

        public static XmlDocument GetResourceAsXmlDocument(string resource)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(Path.Combine(_applicationBase, resource));
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load file via resource \"{0}\" as resource. Cause : {1}", resource, exception.Message), exception);
            }
            return document;
        }

        public static XmlDocument GetStreamAsXmlDocument(Stream resource)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(resource);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load XmlDocument via stream. Cause : {0}", exception.Message), exception);
            }
            return document;
        }

        public static XmlDocument GetUriAsXmlDocument(Uri resource)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(resource.AbsoluteUri);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load XmlDocument via Uri. Cause : {0}", exception.Message), exception);
            }
            return document;
        }

        public static XmlDocument GetUrlAsXmlDocument(string url)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(url);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Unable to load file via url \"{0}\" as url. Cause : {1}", url, exception.Message), exception);
            }
            return document;
        }

        public static string GetValueOfNodeResourceUrl(XmlNode node, NameValueCollection properties)
        {
            string str = null;
            if (node.Attributes["resource"] != null)
            {
                string str2 = NodeUtils.ParsePropertyTokens(node.Attributes["resource"].Value, properties);
                return Path.Combine(_applicationBase, str2);
            }
            if (node.Attributes["url"] != null)
            {
                str = NodeUtils.ParsePropertyTokens(node.Attributes["url"].Value, properties);
            }
            return str;
        }

        [Obsolete("Use IBatisNet.Common.Utilities.TypeUtils")]
        public static Type TypeForName(string typeName)
        {
            return TypeUtils.ResolveType(typeName);
        }

        public static string ApplicationBase
        {
            get
            {
                return _applicationBase;
            }
        }

        public static string BaseDirectory
        {
            get
            {
                return _baseDirectory;
            }
        }

        internal class FileAssemblyInfo
        {
            private string _originalFileName = string.Empty;
            private string _unresolvedAssemblyName = string.Empty;
            private string _unresolvedFileName = string.Empty;
            public const string FileAssemblySeparator = ",";

            public FileAssemblyInfo(string unresolvedFileName)
            {
                this.SplitFileAndAssemblyNames(unresolvedFileName);
            }

            private void SplitFileAndAssemblyNames(string originalFileName)
            {
                this._originalFileName = originalFileName;
                int index = originalFileName.IndexOf(",");
                if (index < 0)
                {
                    this._unresolvedFileName = originalFileName.Trim();
                    this._unresolvedAssemblyName = null;
                }
                else
                {
                    this._unresolvedFileName = originalFileName.Substring(0, index).Trim();
                    this._unresolvedAssemblyName = originalFileName.Substring(index + 1).Trim();
                }
            }

            public string AssemblyName
            {
                get
                {
                    return this._unresolvedAssemblyName;
                }
            }

            public string FileName
            {
                get
                {
                    return this._unresolvedFileName;
                }
            }

            public bool IsAssemblyQualified
            {
                get
                {
                    return ((this.AssemblyName != null) && (this.AssemblyName.Trim().Length != 0));
                }
            }

            public string OriginalFileName
            {
                get
                {
                    return this._originalFileName;
                }
            }

            public string ResourceFileName
            {
                get
                {
                    return (this.AssemblyName + "." + this.FileName);
                }
            }
        }
    }
}

