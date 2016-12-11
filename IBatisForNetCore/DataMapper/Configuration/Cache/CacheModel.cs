namespace IBatisNet.DataMapper.Configuration.Cache
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("cacheModel", Namespace="http://ibatis.apache.org/mapping")]
    public class CacheModel
    {
        [NonSerialized]
        private ICacheController _controller;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.Cache.FlushInterval _flushInterval;
        [NonSerialized]
        private int _hits;
        [NonSerialized]
        private string _id = string.Empty;
        [NonSerialized]
        private string _implementation = string.Empty;
        [NonSerialized]
        private bool _isReadOnly = true;
        [NonSerialized]
        private bool _isSerializable;
        [NonSerialized]
        private long _lastFlush = DateTime.Now.Ticks;
        private static IDictionary _lockMap = new HybridDictionary();
        [NonSerialized]
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [NonSerialized]
        private HybridDictionary _properties = new HybridDictionary();
        [NonSerialized]
        private int _requests;
        [NonSerialized]
        private object _statLock = new object();
        [NonSerialized]
        public const long NO_FLUSH_INTERVAL = -99999L;
        [NonSerialized]
        public static readonly object NULL_OBJECT = new object();

        public void AddProperty(string name, string value)
        {
            this._properties.Add(name, value);
        }

        public void Flush()
        {
            this._lastFlush = DateTime.Now.Ticks;
            this._controller.Flush();
        }

        private void FlushHandler(object sender, ExecuteEventArgs e)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Flush cacheModel named " + this._id + " for statement '" + e.StatementName + "'");
            }
            this.Flush();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object GetLock(CacheKey key)
        {
            int identityHashCode = HashCodeProvider.GetIdentityHashCode(this._controller);
            int hashCode = key.GetHashCode();
            int num3 = (0x1d * identityHashCode) + hashCode;
            object obj2 = _lockMap[num3];
            if (obj2 == null)
            {
                obj2 = num3;
                _lockMap[num3] = obj2;
            }
            return obj2;
        }

        public void Initialize()
        {
            this._flushInterval.Initialize();
            try
            {
                if (this._implementation == null)
                {
                    throw new DataMapperException("Error instantiating cache controller for cache named '" + this._id + "'. Cause: The class for name '" + this._implementation + "' could not be found.");
                }
                Type type = TypeUtils.ResolveType(this._implementation);
                object[] args = new object[0];
                this._controller = (ICacheController) Activator.CreateInstance(type, args);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException("Error instantiating cache controller for cache named '" + this._id + ". Cause: " + exception.Message, exception);
            }
            try
            {
                this._controller.Configure(this._properties);
            }
            catch (Exception exception2)
            {
                throw new ConfigurationException("Error configuring controller named '" + this._id + "'. Cause: " + exception2.Message, exception2);
            }
        }

        public void RegisterTriggerStatement(IMappedStatement mappedStatement)
        {
            mappedStatement.Execute += new ExecuteEventHandler(this.FlushHandler);
        }

        public ICacheController CacheController
        {
            set
            {
                this._controller = value;
            }
        }

        [XmlElement("flushInterval", typeof(IBatisNet.DataMapper.Configuration.Cache.FlushInterval))]
        public IBatisNet.DataMapper.Configuration.Cache.FlushInterval FlushInterval
        {
            get
            {
                return this._flushInterval;
            }
            set
            {
                this._flushInterval = value;
            }
        }

        public double HitRatio
        {
            get
            {
                if (this._requests != 0)
                {
                    return (((double) this._hits) / ((double) this._requests));
                }
                return 0.0;
            }
        }

        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The id attribute is mandatory in a cacheModel tag.");
                }
                this._id = value;
            }
        }

        [XmlAttribute("implementation")]
        public string Implementation
        {
            get
            {
                return this._implementation;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The implementation attribute is mandatory in a cacheModel tag.");
                }
                this._implementation = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this._isReadOnly;
            }
            set
            {
                this._isReadOnly = value;
            }
        }

        public bool IsSerializable
        {
            get
            {
                return this._isSerializable;
            }
            set
            {
                this._isSerializable = value;
            }
        }

        public object this[CacheKey key]
        {
            get
            {
                lock (this)
                {
                    if ((this._lastFlush != -99999L) && ((DateTime.Now.Ticks - this._lastFlush) > this._flushInterval.Interval))
                    {
                        this.Flush();
                    }
                }
                object obj2 = null;
                lock (this.GetLock(key))
                {
                    obj2 = this._controller[key];
                }
                if ((this._isSerializable && !this._isReadOnly) && ((obj2 != NULL_OBJECT) && (obj2 != null)))
                {
                    try
                    {
                        MemoryStream serializationStream = new MemoryStream((byte[]) obj2) {
                            Position = 0L
                        };
                        obj2 = new BinaryFormatter().Deserialize(serializationStream);
                    }
                    catch (Exception exception)
                    {
                        throw new IBatisNetException("Error caching serializable object.  Be sure you're not attempting to use a serialized cache for an object that may be taking advantage of lazy loading.  Cause: " + exception.Message, exception);
                    }
                }
                lock (this._statLock)
                {
                    this._requests++;
                    if (obj2 != null)
                    {
                        this._hits++;
                    }
                }
                if (_logger.IsDebugEnabled)
                {
                    if (obj2 != null)
                    {
                        _logger.Debug(string.Format("Retrieved cached object '{0}' using key '{1}' ", obj2, key));
                        return obj2;
                    }
                    _logger.Debug(string.Format("Cache miss using key '{0}' ", key));
                }
                return obj2;
            }
            set
            {
                if (value == null)
                {
                    value = NULL_OBJECT;
                }
                if ((this._isSerializable && !this._isReadOnly) && (value != NULL_OBJECT))
                {
                    try
                    {
                        MemoryStream serializationStream = new MemoryStream();
                        new BinaryFormatter().Serialize(serializationStream, value);
                        value = serializationStream.ToArray();
                    }
                    catch (Exception exception)
                    {
                        throw new IBatisNetException("Error caching serializable object. Cause: " + exception.Message, exception);
                    }
                }
                this._controller[key] = value;
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug(string.Format("Cache object '{0}' using key '{1}' ", value, key));
                }
            }
        }
    }
}

