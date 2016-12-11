namespace IBatisNet.DataMapper.Configuration.Cache
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("flushInterval")]
    public class FlushInterval
    {
        private int _hours;
        private long _interval = -99999L;
        private int _milliseconds;
        private int _minutes;
        private int _seconds;

        public void Initialize()
        {
            long num = 0L;
            if (this._milliseconds != 0)
            {
                num += this._milliseconds * 0x2710L;
            }
            if (this._seconds != 0)
            {
                num += this._seconds * 0x989680L;
            }
            if (this._minutes != 0)
            {
                num += this._minutes * 0x23c34600L;
            }
            if (this._hours != 0)
            {
                num += this._hours * 0x861c46800L;
            }
            if (num == 0L)
            {
                num = -99999L;
            }
            this._interval = num;
        }

        [XmlAttribute("hours")]
        public int Hours
        {
            get
            {
                return this._hours;
            }
            set
            {
                this._hours = value;
            }
        }

        [XmlIgnore]
        public long Interval
        {
            get
            {
                return this._interval;
            }
        }

        [XmlAttribute("milliseconds")]
        public int Milliseconds
        {
            get
            {
                return this._milliseconds;
            }
            set
            {
                this._milliseconds = value;
            }
        }

        [XmlAttribute("minutes")]
        public int Minutes
        {
            get
            {
                return this._minutes;
            }
            set
            {
                this._minutes = value;
            }
        }

        [XmlAttribute("seconds")]
        public int Seconds
        {
            get
            {
                return this._seconds;
            }
            set
            {
                this._seconds = value;
            }
        }
    }
}

