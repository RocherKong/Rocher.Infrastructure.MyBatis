namespace IBatisNet.DataMapper
{
    using System;

    public class ExecuteEventArgs : EventArgs
    {
        private string _statementName = string.Empty;

        public string StatementName
        {
            get
            {
                return this._statementName;
            }
            set
            {
                this._statementName = value;
            }
        }
    }
}

