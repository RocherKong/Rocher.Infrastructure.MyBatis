namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using System;
    using System.Text;

    public sealed class SqlGenerator
    {
        private static string BuildDeleteQuery(IStatement statement)
        {
            StringBuilder builder = new StringBuilder();
            Delete delete = (Delete) statement;
            string[] strArray = delete.Generate.By.Split(new char[] { ',' });
            builder.Append("DELETE FROM");
            builder.Append("\t" + delete.Generate.Table + "");
            builder.Append(" WHERE ");
            int length = strArray.Length;
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Trim();
                if (i > 0)
                {
                    builder.Append("\tAND " + str + " = ?");
                }
                else
                {
                    builder.Append("\t " + str + " = ?");
                }
            }
            return builder.ToString();
        }

        private static string BuildInsertQuery(IStatement statement)
        {
            StringBuilder builder = new StringBuilder();
            Insert insert = (Insert) statement;
            int count = statement.ParameterMap.PropertiesList.Count;
            builder.Append("INSERT INTO " + insert.Generate.Table + " (");
            for (int i = 0; i < count; i++)
            {
                ParameterProperty property = statement.ParameterMap.PropertiesList[i];
                if (i < (count - 1))
                {
                    builder.Append("\t" + property.ColumnName + ",");
                }
                else
                {
                    builder.Append("\t" + property.ColumnName + "");
                }
            }
            builder.Append(") VALUES (");
            for (int j = 0; j < count; j++)
            {
                ParameterProperty property1 = statement.ParameterMap.PropertiesList[j];
                if (j < (count - 1))
                {
                    builder.Append("\t?,");
                }
                else
                {
                    builder.Append("\t?");
                }
            }
            builder.Append(")");
            return builder.ToString();
        }

        public static string BuildQuery(IStatement statement)
        {
            string str = string.Empty;
            if (statement is Select)
            {
                return BuildSelectQuery(statement);
            }
            if (statement is Insert)
            {
                return BuildInsertQuery(statement);
            }
            if (statement is Update)
            {
                return BuildUpdateQuery(statement);
            }
            if (statement is Delete)
            {
                str = BuildDeleteQuery(statement);
            }
            return str;
        }

        private static string BuildSelectQuery(IStatement statement)
        {
            StringBuilder builder = new StringBuilder();
            Select select = (Select) statement;
            int count = statement.ParameterMap.PropertiesList.Count;
            builder.Append("SELECT ");
            for (int i = 0; i < count; i++)
            {
                ParameterProperty property = statement.ParameterMap.PropertiesList[i];
                if (i < (count - 1))
                {
                    builder.Append("\t" + property.ColumnName + " as " + property.PropertyName + ",");
                }
                else
                {
                    builder.Append("\t" + property.ColumnName + " as " + property.PropertyName);
                }
            }
            builder.Append(" FROM ");
            builder.Append("\t" + select.Generate.Table + "");
            string[] strArray = select.Generate.By.Split(new char[] { ',' });
            if ((strArray.Length > 0) && (select.Generate.By.Length > 0))
            {
                builder.Append(" WHERE ");
                int length = strArray.Length;
                for (int j = 0; j < length; j++)
                {
                    string str = strArray[j];
                    if (j > 0)
                    {
                        builder.Append("\tAND " + str + " = ?");
                    }
                    else
                    {
                        builder.Append("\t" + str + " = ?");
                    }
                }
            }
            if (statement.ParameterClass == null)
            {
                statement.ParameterMap = null;
            }
            return builder.ToString();
        }

        private static string BuildUpdateQuery(IStatement statement)
        {
            StringBuilder builder = new StringBuilder();
            Update update = (Update) statement;
            int count = statement.ParameterMap.PropertiesList.Count;
            string[] strArray = update.Generate.By.Split(new char[] { ',' });
            builder.Append("UPDATE ");
            builder.Append("\t" + update.Generate.Table + " ");
            builder.Append("SET ");
            for (int i = 0; i < count; i++)
            {
                ParameterProperty property = statement.ParameterMap.PropertiesList[i];
                if (update.Generate.By.IndexOf(property.ColumnName) < 0)
                {
                    if (i < ((count - strArray.Length) - 1))
                    {
                        builder.Append("\t" + property.ColumnName + " = ?,");
                    }
                    else
                    {
                        builder.Append("\t" + property.ColumnName + " = ? ");
                    }
                }
            }
            builder.Append(" WHERE ");
            int length = strArray.Length;
            for (int j = 0; j < length; j++)
            {
                string str = strArray[j];
                if (j > 0)
                {
                    builder.Append("\tAND " + str + " = ?");
                }
                else
                {
                    builder.Append("\t " + str + " = ?");
                }
            }
            return builder.ToString();
        }
    }
}

