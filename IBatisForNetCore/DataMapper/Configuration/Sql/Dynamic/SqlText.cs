namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic
{
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using System;

    public sealed class SqlText : ISqlChild
    {
        private bool _isWhiteSpace;
        private ParameterProperty[] _parameters;
        private string _text = string.Empty;

        public bool IsWhiteSpace
        {
            get
            {
                return this._isWhiteSpace;
            }
        }

        public ParameterProperty[] Parameters
        {
            get
            {
                return this._parameters;
            }
            set
            {
                this._parameters = value;
            }
        }

        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
                this._isWhiteSpace = this._text.Trim().Length == 0;
            }
        }
    }
}

