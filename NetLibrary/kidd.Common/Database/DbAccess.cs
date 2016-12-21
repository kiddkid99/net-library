using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace kidd.Common.DataBase
{
    /// <summary>
    /// 資料庫連線模組
    /// </summary>
    public class DbAccess
    {
        private ConnectionStringSettings _connectionSetting;

        private Dictionary<String, DbType> _dicDbType;
        private DbProviderFactory _factory;
        public Dictionary<String, DbType> DicDbType
        {
            get
            {
                return this._dicDbType;
            }
        }


        /// <summary>
        /// 實體化
        /// </summary>
        /// <param name="connectionString">資料庫連線字串 KEY Name</param>
        public DbAccess(string connectionString)
        {
            try
            {

                //取得 config 的連線字串。
                _connectionSetting = System.Configuration.ConfigurationManager.ConnectionStrings[connectionString];

                //產生對應的 DbProviderFactory 類別
                this._factory = DbProviderFactories.GetFactory(_connectionSetting.ProviderName);

                CreateDbTypeMapping();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void CreateDbTypeMapping()
        {
            //建立 Provider 對應的資料庫參數型態
            //create a parameter to let it do the mapping for us
            DbParameter providerParameter = _factory.CreateParameter();

            //get the type for reflecting
            Type parameterType = providerParameter.GetType();
            //find the provider specific DbType property
            PropertyInfo[] pis =
                parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            PropertyInfo providerDbTypeProperty = null;
            foreach (PropertyInfo pi in pis)
            {
                //ignore the "DbType" property, instead, get the other one
                if (pi.Name.IndexOf("DbType") > 0)
                {
                    providerDbTypeProperty = pi;
                    break;
                }
            }
            if (providerDbTypeProperty == null)
            {
                throw new Exception("couldn't find providers native DbType");
            }

            //get our metadata collection
            using (DbConnection dbconn = CreateConnection())
            {
                dbconn.Open();
                using (DataTable dt = dbconn.GetSchema(DbMetaDataCollectionNames.DataTypes))
                {
                    DbParameter parameter = providerParameter;
                    //use the column that provides the number for the value
                    DataColumn column = dt.Columns[DbMetaDataColumnNames.ProviderDbType];

                    //create mapping dictionary
                    _dicDbType = new Dictionary<string, DbType>();
                    foreach (DataRow row in dt.Rows)
                    {
                        //set a default
                        parameter.DbType = DbType.Object;
                        //get value
                        object value = row[column];
                        //set the property via reflection
                        providerDbTypeProperty.SetValue(providerParameter, value, null);
                        //get the NAME that the provider specifies
                        string name = Enum.GetName(providerDbTypeProperty.PropertyType, value);
                        try
                        {
                            //add that Name, and mapping over to my dictionary
                            _dicDbType.Add(name, parameter.DbType);
                        }
                        catch
                        {
                            //eat the errors of the duplicates;
                        }
                    }
                    //put a breakpoint here to inspect the dictionary

                    //return it if we want
                }
            }
        }

        /// <summary>
        /// 建立資料庫連線，使用完畢務必呼叫 Close()，或是使用 using 陳述式
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            DbConnection dbconn = this._factory.CreateConnection();
            dbconn.ConnectionString = _connectionSetting.ConnectionString;
            return dbconn;
        }

        /// <summary>
        /// 執行 DbCommand
        /// </summary>
        /// <param name="cmd">資料庫指令類別</param>
        /// <returns>bool</returns>
        public bool DoCommand(DbCommand cmd)
        {
            bool ret = false;

            try
            {
                //開啟資料庫連線
                using (DbConnection dbconn = CreateConnection())
                {
                    dbconn.Open();
                    cmd.Connection = dbconn;
                    cmd.ExecuteNonQuery();
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }

            return ret;
        }

        /// <summary>
        /// 使用交易機制執行指令集合
        /// </summary>
        /// <param name="commands">指令集合</param>
        /// <returns>bool</returns>
        public bool DoCommandsWithTransaction(List<DbCommand> commands)
        {
            using (DbConnection dbconn = CreateConnection())
            {
                dbconn.Open();

                using (DbTransaction transaction = dbconn.BeginTransaction())
                {
                    try
                    {
                        foreach (DbCommand cmd in commands)
                        {
                            cmd.Connection = dbconn;
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

        }

        /// <summary>
        /// 取得傳入指令碼的資料表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return GetDataTable<DbParameter>(sql, null);
        }

        /// <summary>
        /// 取得包含參數化指令碼的資料表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataTable GetDataTable<T>(string sql, List<T> param) where T : DbParameter
        {
            using (DbConnection conn = CreateConnection())
            {
                using (DbCommand cmd = CreateCommand(sql))
                {
                    cmd.Connection = conn;
                    conn.Open();

                    //建立參數化資料
                    if (param != null && param.Count > 0)
                    {
                        foreach (DbParameter p in param.ToArray())
                        {
                            cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                        }
                    }

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dtSchema = reader.GetSchemaTable();
                        // You can also use an ArrayList instead of List<>
                        List<DataColumn> listCols = new List<DataColumn>();

                        DataTable dt = new DataTable("result");
                        if (dtSchema != null)
                        {
                            foreach (DataRow row in dtSchema.Rows)
                            {
                                string columnName = Convert.ToString(row["ColumnName"]);
                                DataColumn column = new DataColumn(columnName, (Type)(row["DataType"]));
                                column.Unique = (bool)row["IsUnique"];
                                column.AllowDBNull = (bool)row["AllowDBNull"];
                                column.AutoIncrement = (bool)row["IsAutoIncrement"];
                                listCols.Add(column);
                                dt.Columns.Add(column);
                            }
                        }

                        //讀取資料
                        while (reader.Read())
                        {
                            //加入資料列
                            DataRow dataRow = dt.NewRow();
                            for (int i = 0; i < listCols.Count; i++)
                            {
                                dataRow[((DataColumn)listCols[i])] = reader[i];
                            }
                            dt.Rows.Add(dataRow);
                        }

                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 取得傳入指令碼的資料表結構
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetSchemaTable(string sql)
        {
            using (DbConnection dbconn = CreateConnection())
            {
                using (DbCommand cmd = CreateCommand(sql))
                {
                    cmd.Connection = dbconn;
                    dbconn.Open();

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = reader.GetSchemaTable();
                        return dt;
                    }

                }
            }
        }

        /// <summary>
        /// 取得分頁資料表（過時）
        /// </summary>
        /// <typeparam name="T">DbParameter 的衍生類別</typeparam>
        /// <param name="sqlString">資料庫指令碼</param>
        /// <param name="param">參數集合</param>
        /// <param name="pageNo">分頁頁數</param>
        /// <param name="pageSize">分頁大小</param>
        /// <param name="totalRecord">回傳總資料數</param>
        /// <param name="totalPage">回傳總頁數</param>
        /// <returns>DataTable</returns>
        [Obsolete("此方法已過時，取出所有資料後再做分頁判斷，對大資料的效能低落")]
        public DataTable GetPageDataTable<T>(string sqlString, List<T> param, int pageNo, int pageSize, out int totalRecord, out int totalPage)
            where T : DbParameter
        {
            totalPage = 0;
            totalRecord = 0;

            try
            {
                using (DbConnection dbconn = CreateConnection())
                {
                    using (DbCommand cmd = dbconn.CreateCommand())
                    {
                        dbconn.Open();
                        cmd.CommandText = sqlString;

                        //建立參數化資料
                        if (param != null && param.Count > 0)
                        {
                            foreach (DbParameter p in param.ToArray())
                            {
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                            }
                        }

                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            //跳過指定分頁數
                            for (int i = 0; i < ((pageNo - 1) * pageSize); i++)
                            {
                                totalRecord++;
                                if (!reader.Read())
                                {
                                    break;
                                }
                            }

                            DataTable dtSchema = reader.GetSchemaTable();
                            // You can also use an ArrayList instead of List<>
                            List<DataColumn> listCols = new List<DataColumn>();

                            DataTable dt = new DataTable("result");
                            if (dtSchema != null)
                            {
                                foreach (DataRow row in dtSchema.Rows)
                                {
                                    string columnName = Convert.ToString(row["ColumnName"]);
                                    DataColumn column = new DataColumn(columnName, (Type)(row["DataType"]));
                                    column.Unique = (bool)row["IsUnique"];
                                    column.AllowDBNull = (bool)row["AllowDBNull"];
                                    column.AutoIncrement = (bool)row["IsAutoIncrement"];
                                    listCols.Add(column);
                                    dt.Columns.Add(column);
                                }
                            }

                            //繼續讀取資料
                            int cnt = 0;
                            while (reader.Read())
                            {
                                totalRecord++;

                                if ((pageNo == 0) || (cnt < pageSize))
                                {
                                    //加入資料列
                                    DataRow dataRow = dt.NewRow();
                                    for (int i = 0; i < listCols.Count; i++)
                                    {
                                        dataRow[((DataColumn)listCols[i])] = reader[i];
                                    }
                                    dt.Rows.Add(dataRow);
                                    cnt++;
                                }
                            }

                            totalPage = (int)Math.Ceiling((double)(totalRecord) / pageSize);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPageToDataTable<T>(string sqlString, List<T> param, string sortString)
            where T : DbParameter
        {
            int total;
            int page;

            return GetPageToDataTable(sqlString, param, sortString, 0, 0, out total, out page);
        }

        /// <summary>
        /// 取得分頁資料表
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="param"></param>
        /// <param name="sortString">如果為空值則不進行分頁</param>
        /// <param name="pageNo">如果為0則不進行分頁</param>
        /// <param name="pageSize">如果為0則不進行分頁</param>
        /// <param name="totalRecord"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        //[Obsolete("此方法已過時，只能傳入不指定欄位 * 的 select 指令，才能正確執行分頁的指令碼")]
        public DataTable GetPageToDataTable<T>(string sqlString, List<T> param, string sortString, int pageNo, int pageSize, out int totalRecord, out int totalPage)
            where T : DbParameter
        {

            totalPage = 0;
            totalRecord = 0;
            try
            {

                //判斷是否分頁
                if (pageNo == 0 && pageSize == 0)
                {
                    //不使用分頁，加入排序指令碼
                    if (!String.IsNullOrWhiteSpace(sortString))
                    {
                        sqlString += String.Format(" ORDER BY {0} ", sortString);
                    }
                }
                else
                {
                    //分頁需要排序參數
                    if (String.IsNullOrWhiteSpace(sortString))
                    {
                        throw new ArgumentException("使用分頁時，sortString 參數必須指定值");
                    }
                    else if (pageNo == 0 || pageSize == 0)
                    {
                        throw new ArgumentException("使用分頁時，pageNo 及 pageSize 的值必須大於0");
                    }
                    else
                    {
                        //透過正規表達式，取出 SELECT 的欄位字串
                        //EX: SELECT [id], [name] FROM TABLE, 取出 { [id], [name] } 字串
                        Regex regex = new Regex(@"\s*select\s+(?:distinct\s+)?(?:top\s+\d*\s+)?(?'columns'.*?)from.*", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                        if (regex.IsMatch(sqlString))
                        {
                            //適用於 SQL Server 2005 版本以上的分頁語法，使用 ROW_NUMBER() 
                            //以第一個符合的字串當作參考
                            string select_columns = regex.Match(sqlString).Groups["columns"].Value;
                            string add_row_number_select_columns = String.Format("ROW_NUMBER() OVER (ORDER BY {0}) AS RowNum, {1}", sortString, select_columns);

                            //將搜尋欄位替換掉
                            sqlString = sqlString.Replace(select_columns, add_row_number_select_columns);

                            //分頁SQL語法
                            sqlString = String.Format(@"WITH DataResults AS ({0}) 
                                                        SELECT *,(Select MAX(RowNum) FROM DataResults) as TotalRows 
                                                        FROM DataResults 
                                                        WHERE 1=1 and RowNum BETWEEN @PageS and @PageE", sqlString);
                        }
                    }
                }


                using (DbConnection dbconn = CreateConnection())
                {
                    using (DbCommand cmd = dbconn.CreateCommand())
                    {
                        DataTable dt = new DataTable();

                        dbconn.Open();
                        cmd.CommandText = sqlString;

                        if (param != null && param.Count > 0)
                        {
                            foreach (DbParameter p in param.ToArray())
                            {
                                cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                            }
                        }
                        if (pageNo > 0 && pageSize > 0 && sortString != "") //要分頁
                        {
                            cmd.Parameters.Add(this.CreateParameter("@PageS", ((pageNo - 1) * pageSize) + 1, DbType.Int32));
                            cmd.Parameters.Add(this.CreateParameter("@PageE", (pageNo * pageSize), DbType.Int32));
                            DbDataAdapter sqlAdp = this._factory.CreateDataAdapter();
                            sqlAdp.SelectCommand = cmd;
                            sqlAdp.Fill(dt);
                            totalRecord = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalRows"]) : 0;
                            totalPage = (int)Math.Ceiling((double)(totalRecord) / pageSize);
                        }
                        else
                        {
                            DbDataAdapter sqlAdp = this._factory.CreateDataAdapter();
                            sqlAdp.SelectCommand = cmd;
                            sqlAdp.Fill(dt);
                            totalRecord = dt.Rows.Count;
                            totalPage = (int)Math.Ceiling((double)(totalRecord) / pageSize);
                        }

                        return dt;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 建立 指令參數 類別
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(String column, object value, DbType type)
        {
            DbParameter param = this._factory.CreateParameter();
            param.DbType = type;
            param.ParameterName = column;
            param.Value = value;

            return param;
        }


        /// <summary>
        /// 建立指令類別
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this._factory.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public T BindData<T>(DataRow row)
        {
            // Get all columns' name
            List<string> columns = new List<string>();
            foreach (DataColumn dc in row.Table.Columns)
            {
                columns.Add(dc.ColumnName.ToLower());
            }

            // Create object
            var ob = Activator.CreateInstance<T>();

            // Get all fields
            var fields = typeof(T).GetFields();
            foreach (var fieldInfo in fields)
            {
                if (columns.Contains(fieldInfo.Name))
                {
                    // Fill the data into the field
                    fieldInfo.SetValue(ob, row[fieldInfo.Name]);
                }
            }

            // Get all properties
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (columns.Contains(propertyInfo.Name.ToLower()))
                {
                    // Fill the data into the property
                    if (row[propertyInfo.Name] != DBNull.Value)
                    {
                        propertyInfo.SetValue(ob, row[propertyInfo.Name], null);
                    }
                }
            }

            return ob;
        }

        public List<T> BindDataList<T>(DataTable dt)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName.ToLower());
            }

            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                var ob = BindData<T>(dr);
                lst.Add(ob);
            }

            return lst;
        }


        public int GetIdentity(string table)
        {
            int ret = -1;
            string sql = String.Format("SELECT IDENT_CURRENT ('{0}') AS ID", table);
            using (var reader = GetDataTable(sql).CreateDataReader())
            {
                if (reader.Read())
                {
                    ret = (reader.IsDBNull(0)) ? 0 : Convert.ToInt32(reader.GetValue(0));
                }
                else
                {
                    ret = -1;
                }
                reader.Close();
            }

            return ret;
        }

    }
}
