namespace IBatisNet.Common.Utilities
{
    using System;
    using System.Collections;

    public class StringTokenizer : IEnumerable
    {
        private static readonly string _defaultDelim = " \t\n\r\f";
        private string _delimiters;
        private string _origin;
        private bool _returnDelimiters;

        public StringTokenizer(string str)
        {
            this._origin = string.Empty;
            this._delimiters = string.Empty;
            this._origin = str;
            this._delimiters = _defaultDelim;
            this._returnDelimiters = false;
        }

        public StringTokenizer(string str, string delimiters)
        {
            this._origin = string.Empty;
            this._delimiters = string.Empty;
            this._origin = str;
            this._delimiters = delimiters;
            this._returnDelimiters = false;
        }

        public StringTokenizer(string str, string delimiters, bool returnDelimiters)
        {
            this._origin = string.Empty;
            this._delimiters = string.Empty;
            this._origin = str;
            this._delimiters = delimiters;
            this._returnDelimiters = returnDelimiters;
        }

        public IEnumerator GetEnumerator()
        {
            return new StringTokenizerEnumerator(this);
        }

        public int TokenNumber
        {
            get
            {
                int num = 0;
                int num2 = 0;
                int length = this._origin.Length;
                while (num2 < length)
                {
                    while ((!this._returnDelimiters && (num2 < length)) && (this._delimiters.IndexOf(this._origin[num2]) >= 0))
                    {
                        num2++;
                    }
                    if (num2 >= length)
                    {
                        return num;
                    }
                    int num4 = num2;
                    while ((num2 < length) && (this._delimiters.IndexOf(this._origin[num2]) < 0))
                    {
                        num2++;
                    }
                    if ((this._returnDelimiters && (num4 == num2)) && (this._delimiters.IndexOf(this._origin[num2]) >= 0))
                    {
                        num2++;
                    }
                    num++;
                }
                return num;
            }
        }

        private class StringTokenizerEnumerator : IEnumerator
        {
            private int _cursor;
            private string _next;
            private StringTokenizer _stokenizer;

            public StringTokenizerEnumerator(StringTokenizer stok)
            {
                this._stokenizer = stok;
            }

            private string GetNext()
            {
                if (this._cursor >= this._stokenizer._origin.Length)
                {
                    return null;
                }
                char ch = this._stokenizer._origin[this._cursor];
                if (this._stokenizer._delimiters.IndexOf(ch) != -1)
                {
                    this._cursor++;
                    if (this._stokenizer._returnDelimiters)
                    {
                        return ch.ToString();
                    }
                    return this.GetNext();
                }
                int length = this._stokenizer._origin.IndexOfAny(this._stokenizer._delimiters.ToCharArray(), this._cursor);
                if (length == -1)
                {
                    length = this._stokenizer._origin.Length;
                }
                string str = this._stokenizer._origin.Substring(this._cursor, length - this._cursor);
                this._cursor = length;
                return str;
            }

            public bool MoveNext()
            {
                this._next = this.GetNext();
                return (this._next != null);
            }

            public void Reset()
            {
                this._cursor = 0;
            }

            public object Current
            {
                get
                {
                    return this._next;
                }
            }
        }
    }
}

