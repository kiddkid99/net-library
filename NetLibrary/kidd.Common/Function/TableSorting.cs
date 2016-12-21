using kidd.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace kidd.Common.Function
{
    /// <summary>
    /// 資料排序模組功能。
    /// </summary>
    public class TableSorting
    {
        private DbAccess dbaccess = null;

        private string _table_name;
        /// <summary>
        /// 取得資料表名稱
        /// </summary>
        public string TableName
        {
            get { return _table_name; }
        }

        private string _col_name_4_sorting;
        /// <summary>
        /// 取得排序的欄位名稱
        /// </summary>
        public string ColName4Sorting
        {
            get { return _col_name_4_sorting; }
        }


        private string _col_name_4_group;
        /// <summary>
        /// 取得群組的欄位名稱
        /// </summary>
        public string ColName4Group
        {
            get { return _col_name_4_group; }
        }

        private string _col_name_4_pk;
        /// <summary>
        /// 取得主鍵欄位名稱
        /// </summary>
        public string ColName4Pk
        {
            get { return _col_name_4_pk; }
        }

        private string _pk_value;
        /// <summary>
        /// 取得及設定要調整資料的主鍵值
        /// </summary>
        public string PkValue
        {
            get
            {
                return _pk_value;
            }
            set
            {
                _pk_value = value;
            }
        }

        private int? _sorting_value;
        /// <summary>
        /// 取得及設定要調整資料的新排序值
        /// </summary>
        public int? SortingValue
        {
            get
            {
                return _sorting_value;
            }
            set
            {
                _sorting_value = value;
            }
        }


        private string _group_value;
        /// <summary>
        /// 取得要調整的資料群組的值，此值由功能自動抓取。
        /// </summary>
        public string GroupValue
        {
            get { return _group_value; }
        }

        /// <summary>
        /// 表單排序建構式
        /// </summary>
        /// <param name="dbaccess">資料庫連線模組</param>
        /// <param name="table_name">資料表名稱</param>
        /// <param name="col_name_4_sorting">要排序的欄位名稱</param>
        /// <param name="col_name_4_group">要群組的欄位名稱</param>
        /// <param name="col_name_4_pk">主鍵的欄位名稱</param>
        /// <param name="pk_value">要調整的資料主鍵值</param>
        /// <param name="new_sorting_value">要調整的資料新排序的值，可為空值</param>
        public TableSorting(DbAccess dbaccess, string table_name, string col_name_4_sorting, string col_name_4_group, string col_name_4_pk, string pk_value, int? sorting_value)
        {
            this.dbaccess = dbaccess;
            _table_name = table_name;
            _col_name_4_sorting = col_name_4_sorting;
            _col_name_4_group = col_name_4_group;
            _col_name_4_pk = col_name_4_pk;
            _pk_value = pk_value;
            _sorting_value = sorting_value;

            //取得群組的值
            _group_value = GetGroupValue();
        }

        /// <summary>
        /// 驗證基本參數
        /// </summary>
        /// <returns></returns>
        private bool ValideRequireParameter()
        {
            return !String.IsNullOrEmpty(this._table_name) && !String.IsNullOrEmpty(this._col_name_4_pk)
                && !String.IsNullOrEmpty(this._col_name_4_sorting) && !String.IsNullOrEmpty(this._pk_value);
        }

        /// <summary>
        /// 新增資料時的重新排序，請新增資料之後再呼叫此方法。
        /// </summary>
        /// <returns></returns>
        public bool ResortingForInsert()
        {
            bool result = false;
            //驗證基本參數
            if (!ValideRequireParameter() || this._sorting_value < 1)
            {
                result = false;
            }
            else
            {
                //取得此資料表
                var dt = GetRecord();

                if (dt.Rows.Count == 0)
                {
                    result = false;
                }
                else
                {
                    //取得該資料目前的 sorting 值
                    int? sorting_old = GetSortingOld(dt);

                    //當 sorting 有值，不處理排序
                    if (sorting_old.HasValue)
                    {
                        result = false;
                    }
                    else
                    {
                        //取得排序最大值
                        int? max_sorting = GetMaxSorting();

                        //設定排序值
                        if (this._sorting_value.HasValue)
                        {
                            if (!max_sorting.HasValue || max_sorting < 0)
                            {
                                //設定排序值，若無最大排序值，預設值為1
                                this._sorting_value = 1;
                            }
                            else if (max_sorting + 1 <= this._sorting_value)
                            {
                                //若超過最大值，設定為最大值+1，避免跳號。
                                this._sorting_value = max_sorting + 1;
                            }
                        }


                        //更新資料列的排序值
                        if (UpdateRecordSorting(this._sorting_value))
                        {
                            //當有設定排序，新增資料時，只需要更新排序值+1的情況
                            if (this._sorting_value.HasValue)
                            {
                                result = UpdateRecordSortingDown(this._sorting_value.Value, null);
                            }

                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// 更新資料時的重新排序，請更新資料之後再呼叫此方法。
        /// </summary>
        /// <param name="modify_group_value">是否異動群組欄位的值</param>
        /// <returns></returns>
        public bool ResortingForUpdate(bool modify_group_value)
        {

            //若有異動群組欄位的值，先把原始資料的排序值清空，然後作新增後的排序動作
            if (modify_group_value)
            {
                UpdateRecordSorting(null);
                return ResortingForInsert();
            }
            else
            {
                return ResortingForUpdate();
            }
        }

        /// <summary>
        /// 更新資料時的重新排序，請更新資料之後再呼叫此方法。
        /// </summary>
        /// <returns></returns>
        public bool ResortingForUpdate()
        {
            bool result = false;
            //驗證基本參數
            if (!ValideRequireParameter() || this._sorting_value < 1)
            {
                result = false;
            }
            else
            {
                //取得此資料表
                var dt = GetRecord();

                if (dt.Rows.Count == 0)
                {
                    result = false;
                }
                else
                {
                    //取得原本的 sorting 值
                    int? sorting_old = GetSortingOld(dt);

                    //取得排序最大值
                    int? max_sorting = GetMaxSorting();


                    //排序值未更動，不需更新
                    if ((!sorting_old.HasValue && !this._sorting_value.HasValue) || sorting_old == this._sorting_value)
                    {
                        result = true;
                    }
                    else
                    {
                        //有設定新的排序值時
                        if (this._sorting_value.HasValue)
                        {
                            if (!max_sorting.HasValue)
                            {
                                //若無最大值，預設值為 1
                                this._sorting_value = 1;
                            }
                            else if (max_sorting + 1 <= this._sorting_value)
                            {
                                //判斷是否超過最大值，避免跳號。
                                if (sorting_old.HasValue)
                                {
                                    this._sorting_value = max_sorting;
                                }
                                else
                                {
                                    this._sorting_value = max_sorting + 1;
                                }

                            }
                        }


                        //更新排序值
                        if (UpdateRecordSorting(this._sorting_value))
                        {
                            //舊排序有值
                            if (sorting_old.HasValue)
                            {
                                //新設定值小於舊設定值時，更新排序值+1
                                if (this._sorting_value < sorting_old)
                                {
                                    result = UpdateRecordSortingDown(this._sorting_value.Value, sorting_old);
                                }
                                else
                                {
                                    //更新排序值-1
                                    result = UpdateRecordSortingUp(this._sorting_value, sorting_old.Value);
                                }
                            }
                            else
                            {
                                //舊排序為空時，只需更新排序值 + 1 的情況
                                result = UpdateRecordSortingDown(this._sorting_value.Value, null);
                            }
                        }
                        else
                        {
                            result = false;
                        }

                    }
                }
            }


            return result;
        }

        /// <summary>
        /// 刪除資料時的重新排序，請刪除資料之前呼叫此方法。
        /// </summary>
        /// <returns></returns>
        public bool ResortingForDelete()
        {
            bool result = false;

            //驗證基本參數
            if (!ValideRequireParameter())
            {
                result = false;
            }
            else
            {
                //取得此資料表
                var dt = GetRecord();

                if (dt.Rows.Count == 0)
                {
                    result = false;
                }
                else
                {
                    //取得原本的 sorting 值
                    int? sorting_old = GetSortingOld(dt);

                    //舊排序值為空，不需處理
                    if (!sorting_old.HasValue)
                    {
                        result = true;
                    }
                    else
                    {
                        //將此筆資料的排序值設定為空
                        if (UpdateRecordSorting(null))
                        {
                            //更新排序值-1
                            result = UpdateRecordSortingUp(null, sorting_old.Value);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// 將符合條件的資料排序值-1
        /// </summary>
        /// <param name="new_sorting_value"></param>
        /// <param name="sorting_old"></param>
        /// <returns></returns>
        private bool UpdateRecordSortingUp(int? new_sorting_value, int sorting_old)
        {
            string sql = String.Format(@"UPDATE [{0}] SET [{1}] = [{1}] - 1
                                         WHERE [{1}] > @sorting_old 
                                         AND [{2}] != @pk_value ", this._table_name, this._col_name_4_sorting, this._col_name_4_pk);

            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("sorting_old", sorting_old));
            param.Add(new SqlParameter("pk_value", this._pk_value));

            //判斷是否加入新排序值的判斷
            if (new_sorting_value.HasValue)
            {
                sql += String.Format("AND [{0}] <= @sorting_value ", this._col_name_4_sorting);
                param.Add(new SqlParameter("sorting_value", new_sorting_value));
            }

            //判斷是否加入群組條件
            if (!String.IsNullOrEmpty(_col_name_4_group) && !String.IsNullOrEmpty(this._group_value))
            {
                sql += String.Format("AND [{0}] = @group_value ", this._col_name_4_group);
                param.Add(new SqlParameter("group_value", this._group_value));
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(param.ToArray());

            return dbaccess.DoCommand(cmd);
        }

        /// <summary>
        /// 將符合條件的資料排序值+1
        /// </summary>
        /// <param name="new_sorting_value"></param>
        /// <returns></returns>
        private bool UpdateRecordSortingDown(int new_sorting_value, int? sorting_old)
        {
            string sql = String.Format(@"UPDATE [{0}] SET [{1}] = [{1}] + 1 
                                         WHERE [{1}] >= @sorting_value 
                                         AND [{2}] != @pk_value ", this._table_name, this._col_name_4_sorting, this._col_name_4_pk);

            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("sorting_value", new_sorting_value));
            param.Add(new SqlParameter("pk_value", this._pk_value));

            //判斷是否加入舊排序值的判斷
            if (sorting_old.HasValue)
            {
                sql += String.Format("AND [{0}] < @sorting_old ", this._col_name_4_sorting);
                param.Add(new SqlParameter("sorting_old", sorting_old));
            }

            //判斷是否加入群組條件
            if (!String.IsNullOrEmpty(_col_name_4_group) && !String.IsNullOrEmpty(this._group_value))
            {
                sql += String.Format("AND [{0}] = @group_value ", this._col_name_4_group);
                param.Add(new SqlParameter("group_value", this._group_value));
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(param.ToArray());

            return dbaccess.DoCommand(cmd);
        }

        private int? GetMaxSorting()
        {
            int? result = null;

            string sql = String.Format(@"SELECT MAX([{0}]) AS [max_sorting] FROM [{1}] WHERE 1 = 1 ",
               this._col_name_4_sorting, this.TableName);

            List<SqlParameter> param = new List<SqlParameter>();

            //是否加入群組判斷
            if (!String.IsNullOrEmpty(_col_name_4_group) && !String.IsNullOrEmpty(this._group_value))
            {
                sql += String.Format("AND [{0}] = @group_value ", this._col_name_4_group);
                param.Add(new SqlParameter("group_value", this._group_value));
            }

            DataTable dt = dbaccess.GetDataTable(sql, param);
            if (dt.Rows.Count > 0)
            {
                string max_sorting = dt.Rows[0]["max_sorting"].ToString();

                if (!String.IsNullOrEmpty(max_sorting))
                {
                    result = Convert.ToInt32(max_sorting);
                }
            }

            return result;
        }

        /// <summary>
        ///取得群組的值，
        /// </summary>
        private string GetGroupValue()
        {
            string result = "";
            //當有設定群組的欄位時才會設定
            if (!String.IsNullOrEmpty(_col_name_4_group))
            {
                string sql = String.Format(@"SELECT [{0}] AS [group_value] FROM [{1}] WHERE [{2}] = @pk_value ",
                    this._col_name_4_group, this._table_name, this._col_name_4_pk);

                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("pk_value", this._pk_value));

                DataTable dt = dbaccess.GetDataTable(sql, param);

                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["group_value"].ToString();
                }
            }


            return result;
        }

        /// <summary>
        /// 取得原先的排序值
        /// </summary>
        /// <returns></returns>
        private int? GetSortingOld(DataTable dt)
        {
            int? result = null;

            if (dt.Rows.Count > 0)
            {
                string sorting_old = dt.Rows[0]["sorting_old"].ToString();

                if (!String.IsNullOrEmpty(sorting_old))
                {
                    result = Convert.ToInt32(sorting_old);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得目前的資料表
        /// </summary>
        /// <returns></returns>
        private DataTable GetRecord()
        {
            string sql = String.Format(@"SELECT [{0}] AS [sorting_old] FROM [{1}] WHERE [{2}] = @pk_value ",
                this._col_name_4_sorting, this.TableName, this._col_name_4_pk);

            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("pk_value", this._pk_value));

            //是否加入群組判斷
            if (!String.IsNullOrEmpty(_col_name_4_group) && !String.IsNullOrEmpty(this._group_value))
            {
                sql += String.Format("AND [{0}] = @group_value ", this._col_name_4_group);
                param.Add(new SqlParameter("group_value", this._group_value));
            }

            DataTable dt = dbaccess.GetDataTable(sql, param);

            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新資料排序值
        /// </summary>
        /// <param name="new_sorting_value"></param>
        /// <returns></returns>
        private bool UpdateRecordSorting(int? sorting_value)
        {
            SqlCommand cmd = new SqlCommand();
            string sql = String.Format(@"UPDATE [{0}] SET [{1}] = @sorting_value WHERE [{2}] = @pk_value ",
                this.TableName, this._col_name_4_sorting, this._col_name_4_pk);

            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("pk_value", this._pk_value));

            object value = sorting_value.HasValue ? (object)sorting_value.Value : (object)DBNull.Value;
            param.Add(new SqlParameter("sorting_value", value));

            //是否加入群組判斷
            if (!String.IsNullOrEmpty(_col_name_4_group) && !String.IsNullOrEmpty(this._group_value))
            {
                sql += String.Format("AND [{0}] = @group_value ", this._col_name_4_group);
                param.Add(new SqlParameter("group_value", this._group_value));
            }

            cmd.CommandText = sql;
            cmd.Parameters.AddRange(param.ToArray());

            return dbaccess.DoCommand(cmd);
        }
    }
}
