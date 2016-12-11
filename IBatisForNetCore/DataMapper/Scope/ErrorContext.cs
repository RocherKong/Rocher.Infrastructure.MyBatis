namespace IBatisNet.DataMapper.Scope
{
    using System;
    using System.Text;

    public class ErrorContext
    {
        private string _activity = string.Empty;
        private string _moreInfo = string.Empty;
        private string _objectId = string.Empty;
        private string _resource = string.Empty;

        public void Reset()
        {
            this._resource = string.Empty;
            this._activity = string.Empty;
            this._objectId = string.Empty;
            this._moreInfo = string.Empty;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if ((this._activity != null) && (this._activity.Length > 0))
            {
                builder.Append(Environment.NewLine);
                builder.Append("- The error occurred while ");
                builder.Append(this._activity);
                builder.Append(".");
            }
            if ((this._moreInfo != null) && (this._moreInfo.Length > 0))
            {
                builder.Append(Environment.NewLine);
                builder.Append("- ");
                builder.Append(this._moreInfo);
            }
            if ((this._resource != null) && (this._resource.Length > 0))
            {
                builder.Append(Environment.NewLine);
                builder.Append("- The error occurred in ");
                builder.Append(this._resource);
                builder.Append(".");
            }
            if ((this._objectId != null) && (this._objectId.Length > 0))
            {
                builder.Append("  ");
                builder.Append(Environment.NewLine);
                builder.Append("- Check the ");
                builder.Append(this._objectId);
                builder.Append(".");
            }
            return builder.ToString();
        }

        public string Activity
        {
            get
            {
                return this._activity;
            }
            set
            {
                this._activity = value;
            }
        }

        public string MoreInfo
        {
            get
            {
                return this._moreInfo;
            }
            set
            {
                this._moreInfo = value;
            }
        }

        public string ObjectId
        {
            get
            {
                return this._objectId;
            }
            set
            {
                this._objectId = value;
            }
        }

        public string Resource
        {
            get
            {
                return this._resource;
            }
            set
            {
                this._resource = value;
            }
        }
    }
}

