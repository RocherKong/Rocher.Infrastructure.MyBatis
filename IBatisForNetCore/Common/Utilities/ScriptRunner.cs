namespace IBatisNet.Common.Utilities
{
    using IBatisNet.Common;
    using IBatisNet.Common.Exceptions;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;

    public class ScriptRunner
    {
        private void ExecuteStatements(IDataSource dataSource, ArrayList sqlStatements)
        {
            IDbConnection connection = dataSource.DbProvider.CreateConnection();
            connection.ConnectionString = dataSource.ConnectionString;
            connection.Open();
            IDbTransaction transaction = connection.BeginTransaction();
            IDbCommand command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                foreach (string str in sqlStatements)
                {
                    command.CommandText = str;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
        }

        private ArrayList ParseScript(string script)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = new StringTokenizer(script, ";").GetEnumerator();
            while (enumerator.MoveNext())
            {
                string str = ((string) enumerator.Current).Replace("\r\n", " ").Trim();
                if (str != string.Empty)
                {
                    list.Add(str);
                }
            }
            return list;
        }

        public void RunScript(IDataSource dataSource, string sqlScriptPath)
        {
            this.RunScript(dataSource, sqlScriptPath, true);
        }

        public void RunScript(IDataSource dataSource, string sqlScriptPath, bool doParse)
        {
            FileInfo info = new FileInfo(sqlScriptPath);
            string str = info.OpenText().ReadToEnd();
            ArrayList sqlStatements = new ArrayList();
            if (!doParse)
            {
                switch (dataSource.DbProvider.Name)
                {
                    case "oracle9.2":
                    case "oracle10.1":
                    case "oracleClient1.0":
                    case "ByteFx":
                    case "MySql":
                        str = str.Replace("\r\n", " ").Replace("\t", " ");
                        sqlStatements.Add(str);
                        goto Label_022E;

                    case "OleDb1.1":
                        if (dataSource.ConnectionString.IndexOf("Microsoft.Jet.OLEDB") <= 0)
                        {
                            sqlStatements.Add(str);
                        }
                        else
                        {
                            str = str.Replace("\r\n", " ").Replace("\t", " ");
                            sqlStatements.Add(str);
                        }
                        goto Label_022E;
                }
                sqlStatements.Add(str);
            }
            else
            {
                switch (dataSource.DbProvider.Name)
                {
                    case "oracle9.2":
                    case "oracle10.1":
                    case "oracleClient1.0":
                    case "ByteFx":
                    case "MySql":
                        sqlStatements = this.ParseScript(str);
                        break;

                    case "OleDb1.1":
                        if (dataSource.ConnectionString.IndexOf("Microsoft.Jet.OLEDB") <= 0)
                        {
                            sqlStatements.Add(str);
                            break;
                        }
                        sqlStatements = this.ParseScript(str);
                        break;

                    default:
                        sqlStatements.Add(str);
                        break;
                }
            }
        Label_022E:
            try
            {
                this.ExecuteStatements(dataSource, sqlStatements);
            }
            catch (Exception exception)
            {
                throw new IBatisNetException("Unable to execute the sql: " + info.Name, exception);
            }
        }
    }
}

