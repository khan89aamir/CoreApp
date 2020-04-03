using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CoreApp
{
    /// <summary>
    /// Database Access Layer.
    /// </summary>
    public class clsConnection_DAL
    {
        #region Variables and Other Objects

        bool _IsEncrypted; // Connection string is encrypted or decrypted.
        string _CommandText;
        bool IsRollBack = false;
        int Counter = 0;
        string strColumns;
        string strValues;
        // List for storing sql parameter.
        char[] c1 = new char[2];

        SqlConnection Objcon;
        clsCommon ObjCommon = new clsCommon();
        clsUtility ObjUtil = new clsUtility();
        SqlTransaction ObjTrans;
        SqlDataAdapter ObjDA;
        List<SqlParameter> lstSQLParameter = new List<SqlParameter>();

        /// <summary>
        /// Set strConnectionString. 
        /// </summary>
        public static string strConnectionString { get; set; }

        #endregion

        /// <summary>
        /// Get or Set SQL Connection Object.
        /// </summary>
        public SqlConnection ConnectionObject
        {
            get { return Objcon; }
            set { Objcon = value; }
        }
        /// <summary>
        /// Return the command text of currently executed statement.
        /// </summary>
        public string CommandText
        {
            get
            {
                return _CommandText;
            }
        }
        /// <summary>
        /// Initialize the new instance of sqlconnection.
        /// If you are not passing any argument in the constructor then you must set  static string strConnectionString property of clsConnection_DAL class.
        /// </summary>
        public clsConnection_DAL()
        {
            try
            {
                if (clsConnection_DAL.strConnectionString == null)
                {
                    Objcon = new SqlConnection();
                }
                else
                {
                    if (clsConnection_DAL.strConnectionString.Trim().Length != 0)
                    {
                        Objcon = new SqlConnection(clsConnection_DAL.strConnectionString);
                    }
                }
                c1[0] = '[';
                c1[1] = ']';
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, " clsConnection_DAL() <BR><BR><FONT FACE='Courier New'> <b>Connection String : None </b>" + GetConnectionString() + "</Font><BR><BR>");
            }
        }

        /// <summary>
        /// Initialize new instance of Data Access Layer.
        /// It will automatically initialize the sql connection object.
        /// </summary>
        /// <param name="IsEncrypted">Pass true if you are using encrypted connection string else pass false.</param>
        public clsConnection_DAL(bool IsEncrypted)
        {
            try
            {
                _IsEncrypted = IsEncrypted;
                Objcon = new SqlConnection(GetConnectionString());
                c1[0] = '[';
                c1[1] = ']';
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, " clsConnection_DAL(bool IsEncrypted) (Constructor) <BR><BR><FONT FACE='Courier New'> <b>Connection String : </b>" + GetConnectionString() + "</Font><BR><BR>");
            }
        }
        /// <summary>
        /// Initialize new instance of Data Access Layer.
        /// Use this constructor if you want to set the connection string in your own way.
        /// Just pass you Sql connection object in the parameter.
        /// </summary>
        /// <param name="ObjSQLCon">Sql connection object.</param>
        public clsConnection_DAL(SqlConnection ObjSQLCon)
        {
            try
            {
                this.ConnectionObject = ObjSQLCon;
                c1[0] = '[';
                c1[1] = ']';
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, " clsConnection_DAL(SqlConnection ObjSQLCon) (Constructor) <BR><BR><FONT FACE='Courier New'> <b>Connection String : </b>" + GetConnectionString() + "</Font><BR><BR>");
            }
        }

        /// <summary>
        /// Clear and reset all the used variables and columns
        /// </summary>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool ResetData()
        {
            try
            {
                // IMP Note : Never make transaction object Null over here.
                Counter = 0;
                strColumns = string.Empty;
                strValues = string.Empty;
                lstSQLParameter.Clear();
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "ResetData() <br><br> Error while resetting the data.  ");
                return false;
            }
        }

        /// <summary>
        /// Delete the data from a table by passing condition. (without where clause.)
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Pass the table name</param>
        /// <param name="strCondition">Pass the condition without where clause.</param>
        /// <returns></returns>
        public int DeleteData(string strTableName, string strCondition)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }

            int result = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                DataTable dt = new DataTable();
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }

                cmd.Connection = Objcon;

                //Replace the where keyword by empty string.
                if (strCondition != null)
                {
                    strCondition = strCondition.Replace("Where", " ");
                }

                cmd.CommandText = "DELETE " + strTableName + " WHERE " + strCondition;
                _CommandText = cmd.CommandText;
                result = cmd.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                CloseConnection();
                clsCommon.ShowError(ex, SetError("DeleteData(string strTableName, string strCondition)", cmd.CommandText));
                return -1;
            }
            return result;
        }

        private string SetError(string strMethod, string cmdText)
        {
            return " " + strMethod + " <BR><BR><FONT FACE='Courier New'> <b>CommandText : </b> " + cmdText + "</Font><BR><BR>";
        }

        /// <summary>
        /// Execute SQL INSERT statement.
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Pass the table name where you want to insert the data.Its a good practice to pass the name of the table with database name.<br>For example : [DatabaseName].[dbo].[TableName]</br></param>
        /// <param name="ReturnIdentity">True returns the identity value of the inserted record. False returns the number of rows affected.</param>
        /// <returns>Returns the number of rows affected OR Identity value.</returns>
        public int InsertData(string strTableName, bool ReturnIdentity)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }
            int result = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }

                SqlParameter[] p = lstSQLParameter.ToArray();

                if (ReturnIdentity)
                {
                    cmd.CommandText = "INSERT INTO " + strTableName + "(" + strColumns + ") VALUES(" + strValues + "); SELECT SCOPE_IDENTITY()";
                }
                else
                {
                    cmd.CommandText = "INSERT INTO " + strTableName + "(" + strColumns + ") VALUES(" + strValues + ")";
                }
                _CommandText = cmd.CommandText;
                cmd.Parameters.AddRange(p);
                cmd.Connection = Objcon;

                // if user want identity value then get the identity value.
                if (ReturnIdentity)
                {
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    result = cmd.ExecuteNonQuery();
                }
                CloseConnection();
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                string strMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                CloseConnection();
                clsCommon.ShowError(ex, SetError("InsertData(string strTableName, bool ReturnIdentity)", cmd.CommandText));
                ResetData();
                return -1;
            }
            ResetData();
            return result;
        }

        /// <summary>
        ///  Execute SQL command to updated the data.
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Table Name</param>
        /// <param name="strCondition">Write condition without where clause. Example: ID=101</param>
        /// <returns>Returns the number of rows affected</returns>
        public int UpdateData(string strTableName, string strCondition)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }
            int result = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }

                SqlParameter[] p = lstSQLParameter.ToArray();

                strCondition = strCondition.Replace("where", " ");
                cmd.CommandText = "UPDATE " + strTableName + " SET " + strColumns + " WHERE " + strCondition;
                _CommandText = cmd.CommandText;
                cmd.Parameters.AddRange(p);
                cmd.Connection = Objcon;
                result = cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                CloseConnection();
                clsCommon.ShowError(ex, SetError("UpdateData(string strTableName, string strCondition)", cmd.CommandText));
                ResetData();
                return -1;
            }
            ResetData();
            return result;
        }

        /// <summary>
        /// Open the connection and begin the transaction.
        /// </summary>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool BeginTransaction()
        {
            try
            {
                ResetData();
                IsRollBack = false;
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                ObjTrans = Objcon.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "BeginTransaction()");
                return false;
            }
        }

        /// <summary>
        /// Commit the Transaction and Close the connection.
        /// </summary>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool CommitTransaction()
        {
            try
            {
                // if transaction is not rollback then commit it else set it to null.
                // and if connection has been closed then no need to commit it.
                if (!IsRollBack && ObjTrans.Connection != null)
                {
                    ObjTrans.Commit();
                    Objcon.Close();
                    ObjTrans = null;
                }
                else
                {
                    ObjTrans = null;
                    IsRollBack = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ObjTrans = null;
                clsCommon.ShowError(ex, "CommitTransaction()");
                return false;
            }
        }

        /// <summary>
        /// Roll Back the Transaction and Close the connection.
        /// </summary>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool RollbackTransaction()
        {
            try
            {
                if (ObjTrans.Connection != null)
                {
                    ObjTrans.Rollback();
                    Objcon.Close();
                    ObjTrans = null;
                }
                else
                {
                    ObjTrans = null;
                    IsRollBack = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ObjTrans = null;
                clsCommon.ShowError(ex, "RollbackTransaction()");
                return false;
            }
        }

        /// <summary>
        /// Set Column Data. Assign the data to the specified column of the table.
        /// if there is a space between column name then write the column name as [First Name]
        /// </summary>
        /// <param name="strColumnnName">Column Name</param>
        /// <param name="DataType">Data Type of that column</param>
        /// <param name="Value">Value to be passed to the column</param>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool SetColumnData(string strColumnnName, SqlDbType DataType, object Value)
        {
            try
            {
                if (Counter == 0)
                {
                    strColumns = strColumnnName;
                    strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strValues = "@" + strColumnnName;
                }
                else
                {
                    strColumns += "," + strColumnnName;
                    strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strValues += ",@" + strColumnnName;
                }
                SqlParameter p = new SqlParameter("@" + strColumnnName.Trim(c1).Replace(" ", string.Empty), DataType);
                p.Value = Value;
                lstSQLParameter.Add(p);
                Counter++;
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "SetColumnData(string strColumnnName, SqlDbType DataType, object Value)");
                ResetData();
                return false;
            }
        }

        /// <summary>
        /// Update Column Data. Assign the data to the specified column of the table.
        /// </summary>
        /// <param name="strColumnnName">Column Name</param>
        /// <param name="DataType">Data Type of that column</param>
        /// <param name="Value">Value to be passed to the column</param>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool UpdateColumnData(string strColumnnName, SqlDbType DataType, object Value)
        {
            try
            {
                if (Counter == 0)
                {
                    strColumns = strColumnnName + "=@" + strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strValues = "@" + strColumnnName;
                }
                else
                {
                    strColumns += "," + strColumnnName + "=@" + strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                    strValues += ",@" + strColumnnName;
                }
                SqlParameter p = new SqlParameter("@" + strColumnnName, DataType);
                p.Value = Value;
                lstSQLParameter.Add(p);
                Counter++;
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "UpdateColumnData(string strColumnnName, SqlDbType DataType, object Value)");
                ResetData();
                return false;
            }
        }

        /// <summary>
        /// Get the complete table by passing just a table name.
        ///  Its a good practice to pass the name of the table with database name
        ///  For example : [DatabaseName].[dbo].[TableName]
        ///  Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Enter table name.</param>
        /// <returns>DataTable</returns>
        public DataTable GetData(string strTableName)
        {
            if (clsCommon.IsPoupGridCellClick)
            {
                return null;
            }
            else
            {
                //Clear Dictionary list for Pop-up
                clsUtility.ObjPopupControl.Clear();
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = "SELECT * FROM " + strTableName + " WITH(NOLOCK)";
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetData(string strTableName)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the complete table by passing table name and orderbyclasuse.
        /// Its a good practice to pass the name of the table with database name 
        ///For example : [DatabaseName].[dbo].[TableName] 
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Enter table name</param>
        /// <param name="OrderByclause">Set the order by clause 
        /// For example : DateTime desc (DateTime is a column name and desc is sorting order.)
        /// You can pass Null if you don't want to user the order by clause.
        /// </param>
        /// <returns>DataTable</returns>
        public DataTable GetData(string strTableName, string OrderByclause)
        {
            if (clsCommon.IsPoupGridCellClick)
            {
                return null;
            }
            else
            {
                //Clear Dictionary list for Pop-up
                clsUtility.ObjPopupControl.Clear();
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                ObjDA = new SqlDataAdapter();
                if (OrderByclause != null)
                {
                    OrderByclause = OrderByclause.Replace("order by", " ");
                    cmd.CommandText = "SELECT * FROM " + strTableName + " WITH(NOLOCK) order by " + OrderByclause;
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM " + strTableName + " WITH(NOLOCK)";
                }
                ObjDA.SelectCommand = cmd;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                _CommandText = cmd.CommandText;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetData(string strTableName, string OrderByclause)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the complete table by passing the table name, condition  and order by clause
        ///Its a good practice to pass the name of the table with database name
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        ///Condition Example : EmployeeName='Jhone' AND age='41'
        /// </summary>
        /// <param name="strTableName">Pass the table name</param>
        /// <param name="strCondition">Pass the condition without where clause.</param>
        /// <param name="OrderByclause">Set the order by clause 
        /// For example : DateTime desc (DateTime is a column name and desc is sorting order.)
        /// You can pass Null if you don't want to user the order by clause.
        /// </param>
        /// <returns>Data Table</returns>
        public DataTable GetData(string strTableName, string strCondition, string OrderByclause)
        {
            if (clsCommon.IsPoupGridCellClick)
            {
                return null;
            }
            else
            {
                //Clear Dictionary list for Pop-up
                clsUtility.ObjPopupControl.Clear();
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();

                //Replace the where keyword by empty string.
                if (strCondition != null)
                {
                    strCondition = strCondition.Replace("Where", " ");
                }
                if (OrderByclause != null)
                {
                    OrderByclause = OrderByclause.Replace("order by", " ");
                    cmd.CommandText = "SELECT * FROM " + strTableName + " WITH(NOLOCK) WHERE " + strCondition + " ORDER BY " + OrderByclause;
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM " + strTableName + " WITH(NOLOCK) WHERE " + strCondition;
                }
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetData(string strTableName, string strCondition, string OrderByclause)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the complete table by passing the table name, ColumnNames (Col,Col2,Col3 etc) and order by clause.
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strTableName">Pass the table name</param>
        /// <param name="strColumns">Pass the column names</param>
        /// <param name="OrderByclause">Set the order by clause </param>
        /// For example : DateTime desc (DateTime is a column name and desc is sorting order.)
        /// You can pass Null if you don't want to user the order by clause.
        /// <returns>Data Table</returns>
        public DataTable GetDataCol(string strTableName, string strColumns, string OrderByclause)
        {
            if (clsCommon.IsPoupGridCellClick)
            {
                return null;
            }
            else
            {
                //Clear Dictionary list for Pop-up
                clsUtility.ObjPopupControl.Clear();
            }
            SqlCommand cmd = new SqlCommand();

            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                if (OrderByclause != null)
                {
                    OrderByclause = OrderByclause.Replace("order by", " ");
                    cmd.CommandText = "SELECT " + strColumns + " FROM " + strTableName + " WITH(NOLOCK) ORDER BY " + OrderByclause;
                }
                else
                {
                    cmd.CommandText = "SELECT " + strColumns + " FROM " + strTableName + " WITH(NOLOCK)";
                }
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetDataCol(string strTableName, string strColumns, string OrderByclause)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the table by passing Table Name, ColumnNames (Col,Col2,Col3 etc), and condition.
        ///Its a good practice to pass the name of the table with database name
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        ///Condition Example : EmployeeName='Jhone' AND age='41'
        /// </summary>
        /// <param name="strTableName">Pass the table name.</param>
        /// <param name="strColumns">Set the column. For example (Col1,Col2,Col2 etc)</param>
        /// <param name="strCondition">Pass the condition without where clause. Example : EmployeeName='Jhone' AND age='41'</param>
        /// <param name="OrderByclause">Set the order by clause 
        /// For example : DateTime desc (DateTime is a column name and desc is sorting order.)
        /// You can pass Null if you dont want to user the ordery by clause.
        /// </param>
        /// <returns></returns>
        public DataTable GetDataCol(string strTableName, string strColumns, string strCondition, string OrderByclause)
        {
            if (clsCommon.IsPoupGridCellClick)
            {
                return null;
            }
            else
            {
                //Clear Dictionary list for Pop-up
                clsUtility.ObjPopupControl.Clear();
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();

                //Replace the where keyword by empty string.
                if (strCondition != null)
                {
                    strCondition = strCondition.Replace("Where", " ");
                }
                if (OrderByclause != null)
                {
                    OrderByclause = OrderByclause.Replace("order by", " ");
                    cmd.CommandText = "SELECT " + strColumns + " FROM " + strTableName + " WITH(NOLOCK) WHERE " + strCondition + " ORDER BY " + OrderByclause;
                }
                else
                {
                    cmd.CommandText = "SELECT " + strColumns + " FROM " + strTableName + " WITH(NOLOCK) WHERE " + strCondition;
                }
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetDataCol(string strTableName, string strColumns, string strCondition, string OrderByclause)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Create a default connection string.
        /// </summary>
        /// <returns></returns>
        private string CreateConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ".";
            builder.InitialCatalog = "master";
            builder.IntegratedSecurity = true;
            // if user is using encrypted connection string then encrypt it and generate.
            if (_IsEncrypted)
            {
                // Encrypt the connection string.
                return ObjUtil.Encrypt(builder.ConnectionString, true);
            }
            else
            {
                // Encrypt the connection string.
                return builder.ConnectionString;
            }
        }

        /// <summary>
        /// Get the connection string in decrypted format from the ServerConfig.sc file.
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            try
            {
                string str = string.Empty;
                if (File.Exists("AppConfig/ServerConfig.sc"))
                {
                    StreamReader sw = new StreamReader("AppConfig/ServerConfig.sc");
                    // if user is using encrypted connection string then decrypt it else just read it.
                    if (_IsEncrypted)
                    {
                        // Decrypt the encrypted connection string.
                        str = ObjUtil.Decrypt(sw.ReadLine(), true);
                        sw.Close();
                    }
                    else
                    {
                        // Decrypt the encrypted connection string.
                        str = sw.ReadLine();
                        sw.Close();
                    }
                }
                else
                {
                    if (Directory.Exists("AppConfig"))
                    {
                        CreateConnectionString();
                        StreamWriter sw = new StreamWriter("AppConfig/ServerConfig.sc");
                        sw.WriteLine(CreateConnectionString());
                        sw.Close();
                    }
                    else
                    {
                        Directory.CreateDirectory("AppConfig");
                        StreamWriter sw = new StreamWriter("AppConfig/ServerConfig.sc");
                        sw.WriteLine(CreateConnectionString());
                        sw.Close();
                    }
                    return null;
                }
                return str;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "GetConnectionString()");
                return null;
            }
        }

        /// <summary>
        /// Execute the sql select statement and returns the data table
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public DataTable ExecuteSelectStatement(string strQuery)
        {
            // This code is for data grid view popup window.
            //When user click on the data cell. the textchange event will fire and this will again fetch the same data which is not needed.
            if (clsUtility.dgvPoup != null && clsUtility.dgvPoup.SelectedRows.Count > 0)
            {
                return null;
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                //if (strQuery.ToLower().Contains("select") == false)
                //{
                //    clsUtility.ShowErrorMessage("Select keyword is missing in the ExecuteSelectStatemen()." +
                //                             "\nExecuteSelectStatemen() Executes the sql select statement and returns the data table." +
                //                             "\nDon't write INSERT, UPDATED query in this method.\n Query :" + strQuery, "CoreApp");
                //    return null;
                //}
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = strQuery;
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteSelectStatement(string strQuery)", cmd.CommandText));
            }
            return null;
        }

        private DataTable ExeSp(string strQuery, string Database)
        {
            // This code is for data grid view popup window.
            //When user click on the data cell. the textchange event will fire and this will again fetch the same data which is not needed.
            if (clsUtility.dgvPoup != null && clsUtility.dgvPoup.SelectedRows.Count > 0)
            {
                return null;
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                Objcon.ChangeDatabase(Database);
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = strQuery;
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExeSp(string strQuery, string Database)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the information of identity column
        /// </summary>
        /// <param name="TableName">Table name</param>
        /// <param name="Database">database name</param>
        /// <returns>returns the table</returns>

        public DataTable IdentityColInfo(string TableName, string Database)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                Objcon.ChangeDatabase(Database);
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = "select is_identity from sys.columns as c where c.is_identity=1 and object_id=(select object_id from sys.tables where name='" + TableName + "')";
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("IdentityColInfo(string TableName, string Database)", cmd.CommandText));
            }
            return null;
        }
        /// <summary>
        /// Execute the sql select statement and returns the data table
        ///Its a good practice to pass the name of the table with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strQuery">pass the query</param>
        /// <param name="Database"> pass the database name</param>
        /// <returns></returns>
        public DataTable ExecuteSelectStatement(string strQuery, string Database)
        {
            // This code is for data grid view popup window.
            //When user click on the data cell. the textchange event will fire and this will again fetch the same data which is not needed.
            if (clsUtility.dgvPoup != null && clsUtility.dgvPoup.SelectedRows.Count > 0)
            {
                return null;
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (strQuery.ToLower().Contains("select") == false)
                {
                    clsUtility.ShowErrorMessage("Select keyword is missing in the ExecuteSelectStatemen()." +
                                             "\nExecuteSelectStatemen() Executes the sql select statement and returns the data table." +
                                             "\nDon't write INSERT, UPDATED query in this method.\n Query :" + strQuery, "CoreApp");
                    return null;
                }
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                Objcon.ChangeDatabase(Database);
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = strQuery;
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteSelectStatement(string strQuery)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        ///  Executes a Transact-SQL statement against the connection and returns the
        ///     number of rows affected.
        /// </summary>
        /// <param name="strQuery">SQL Query to be executed.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strQuery)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }
            int result = 0;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                cmd.CommandText = strQuery;
                cmd.Connection = Objcon;
                _CommandText = cmd.CommandText;
                result = cmd.ExecuteNonQuery();
                CloseConnection();
                // if connection is closed after executing the query then it means query has been executed properly.
                // in this case if result it -1 then set it to 0
                if (result == -1)
                {
                    result = 0;
                }
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteNonQuery(string strQuery)", cmd.CommandText));
                return -1;
            }
            return result;
        }
        /// <summary>
        ///  Executes the query, and returns the first column of the first row in the
        ///    result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="strQuery">SQL Query to be executed.</param>
        /// <returns>The first column of the first row in the result set, or a null reference
        ///</returns>
        public object ExecuteScalar(string strQuery)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }
            SqlCommand cmd = new SqlCommand();
            object result = null;
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                cmd.CommandText = strQuery;
                cmd.Connection = Objcon;
                _CommandText = cmd.CommandText;
                result = cmd.ExecuteScalar();
                CloseConnection();

            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteScalar(string strQuery)", cmd.CommandText));
            }
            return result;
        }

        /// <summary>
        ///  Executes the query, and returns the first column of the first row in the
        ///    result set returned by the query. Additional columns or rows are ignored.
        ///    Note that this method only returns integer value.
        /// </summary>
        /// <param name="strQuery">SQL Query to be executed.</param>
        /// <returns>The first column of the first row in the result set.
        ///</returns>
        public int ExecuteScalarInt(string strQuery)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return -1;
            }
            //int result = 0;
            int result = -1; //for checking LAN connectivity is there or not?
            SqlCommand cmd = new SqlCommand();
            object o = null;
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                cmd.CommandText = strQuery;
                _CommandText = cmd.CommandText;
                cmd.Connection = Objcon;
                o = cmd.ExecuteScalar();
                if (o != null)
                {
                    result = Convert.ToInt32(o);
                }
                else
                {
                    return 0;
                }
                CloseConnection();
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    result = -1;
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteScalarInt(string strQuery)", cmd.CommandText));
            }
            return result;
        }

        /// <summary>
        /// Count the total number of rows as per the given condition.
        /// Where condition Example : EmployeeName='Jhone' AND age='41'
        /// </summary>
        /// <param name="strTableName">Table name</param>
        /// <param name="strCondition">Condition without where clause.Example : EmployeeName='Jhone' AND age='41'</param>
        /// <returns></returns>
        public int CountRecords(string strTableName, string strCondition)
        {
            object result = null;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                //Replace the where keyword by empty string.
                if (strCondition != null)
                {
                    strCondition = strCondition.Replace("Where", " ");
                }
                cmd.CommandText = "SELECT COUNT(1) FROM " + strTableName + " WITH(NOLOCK) WHERE " + strCondition;
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                _CommandText = cmd.CommandText;
                result = cmd.ExecuteScalar();
                CloseConnection();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("CountRecords(string strTableName, string strCondition)", cmd.CommandText));
            }
            return 0;
        }

        /// <summary>
        /// Count the total number of rows from table.
        /// </summary>
        /// <param name="strTableName">Table name</param>
        /// <returns></returns>
        public int CountRecords(string strTableName)
        {
            object result = null;
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                cmd.CommandText = "SELECT COUNT(1) FROM " + strTableName + " WITH(NOLOCK)";
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                _CommandText = cmd.CommandText;
                result = cmd.ExecuteScalar();
                CloseConnection();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("CountRecords(string strTableName)", cmd.CommandText));
            }
            return 0;
        }

        /// <summary>
        /// This method allows you to take database backup.
        /// </summary>
        /// <param name="strDatabaseName">Database Name</param>
        /// <param name="strPath">Path where the backup file will be stored. (Ex. C:/MyBackup/CoreApp.bak)</param>
        /// <param name="strBackupFileName">Backup File Name.</param>
        /// <returns>Returns True if backup operation is successfully completed  else returns false.</returns>
        public bool BackupDatabase(string strDatabaseName, string strPath, string strBackupFileName)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                cmd.CommandText = "BACKUP DATABASE " + strDatabaseName + " TO DISK='" + strPath + @"\" + strBackupFileName.Replace(".bak", "") + ".bak'";
                _CommandText = cmd.CommandText;
                cmd.Connection = Objcon;
                int result = cmd.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("BackupDatabase(string strDatabaseName, string strPath, string strBackupFileName)", cmd.CommandText));
                return false;
            }
        }

        /// <summary>
        /// This method will restore the database.
        /// </summary>
        /// <param name="strDatabaseName">Database Name</param>
        /// <param name="strPath">Path of the database file. (.bak file). Example : C:/CoreApp/Data.bak</param>
        /// <returns>Returns true if backup operation is successful else returns false.</returns>
        public bool RestoreDataBase(string strDatabaseName, string strPath)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                cmd.CommandText = "RESTORE DATABASE " + strDatabaseName + " FROM DISK='" + strPath + "' WITH REPLACE";
                _CommandText = cmd.CommandText;
                cmd.Connection = Objcon;
                int result = cmd.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("RestoreDataBase(string strDatabaseName, string strPath)", cmd.CommandText));
                return false;
            }
        }

        /// <summary>
        /// Get the list of databases from SQL server. This method will not show system database like master,model, msdb, tempdb etc.
        /// </summary>
        /// <param name="BasicInfo">Set true if you want only basic info about the database. Set false if you want full info</param>
        /// <returns>DataTable containing database information.</returns>
        public DataTable GetDatabases(bool BasicInfo)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                if (!BasicInfo)
                {
                    cmd.CommandText = "select * from sys.databases WHERE name not in('master','model','msdb','tempdb','ReportServer','ReportServerTempDB')";
                }
                else
                {
                    cmd.CommandText = "select name,database_id,create_date,collation_name,user_access,is_read_only,state,recovery_model,page_verify_option from sys.databases WHERE name not in('master','model','msdb','tempdb','ReportServer','ReportServerTempDB')";
                }
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetDatabases(bool BasicInfo)", cmd.CommandText));
            }

            return null;
        }

        /// <summary>
        /// Get the list of tables from the specified database.
        /// </summary>
        /// <param name="strDatabseName">Pass the database name.</param>
        /// <returns>DataTable containing Table information.</returns>
        public DataTable GetTables(string strDatabseName)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = "select * from " + strDatabseName + ".sys.tables";
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetTables(string strDatabseName)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Get the detail information about the Columns of the specified database and table.
        /// </summary>
        /// <param name="strDatabase">Pass the database name</param>
        /// <param name="strTable">Pass the table name. (without .dbo or any other schema)</param>
        /// <returns>DataTable containing Column information</returns>
        public DataTable GetColumns(string strDatabase, string strTable)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                DataTable dt = new DataTable();
                cmd.Connection = Objcon;
                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                ObjDA = new SqlDataAdapter();
                cmd.CommandText = "select * from " + strDatabase + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + strTable + "'";
                _CommandText = cmd.CommandText;
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(dt);
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("GetColumns(string strDatabase, string strTable)", cmd.CommandText));
            }
            return null;
        }

        /// <summary>
        /// Creates the SQL server database in the server.
        /// </summary>
        /// <returns></returns>
        public bool CreateDatabase(string dbName)
        {
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Objcon;
                cmd.CommandText = "Create database " + dbName;
                _CommandText = cmd.CommandText;
                int result = cmd.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                CloseConnection();
                clsCommon.ShowError(ex, SetError("CreateDatabase()", _CommandText));
                return false;
            }
        }
        /// <summary>
        /// Check if the database is there in the server or not.
        /// </summary>
        /// <param name="dbName">Name of the database</param>
        /// <returns>returns true if database exists else returns false</returns>
        public bool IsDatabaseExist(string dbName)
        {
            try
            {
                dbName = dbName.Trim(c1);
                int result = ExecuteScalarInt("SELECT count(1) FROM master.dbo.sysdatabases where name='" + dbName + "'");
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, SetError("IsDatabaseExist()", "SELECT * FROM master.dbo.sysdatabases where name='" + dbName + "'"));
                return false;
            }
        }

        /// <summary>
        /// This immediately disconnects all users, closes any open connections, rolls back any transactions in progress, and drops the database.
        /// </summary>
        /// <param name="dbName">name of the database.</param>
        /// <returns></returns>
        public bool DeleteDatabase(string dbName)
        {
            try
            {
                int result = ExecuteNonQuery("ALTER DATABASE " + dbName + "  SET SINGLE_USER   WITH ROLLBACK IMMEDIATE DROP DATABASE " + dbName + "");
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, SetError("DeleteDatabase()", "SELECT * FROM master.dbo.sysdatabases where name='" + dbName + "'"));
                return false;
            }
        }
        /// <summary>
        /// Close the Connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                // if transaction is in running mode then DO NOT close the connection.
                if (ObjTrans != null)
                {
                    return;
                }

                if (Objcon.State == ConnectionState.Open || Objcon.State == ConnectionState.Broken)
                {
                    Objcon.Close();
                }
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "CloseConnection()");
            }
        }

        /// <summary>
        /// Read the connection string from specified file. Connection string must be there in that file. The file must contains only connection string.
        /// </summary>
        /// <param name="strPath">path of the file from where you are going to get the connection string.</param>
        /// <param name="isEnrypted">Pass true if connection string is encrypted or decrypted</param>
        /// <returns>returns the connection string.</returns>
        public string ReadConStringFromFile(string strPath, bool isEnrypted)
        {
            if (File.Exists(strPath))
            {
                CloseConnection();

                if (isEnrypted)
                {
                    return ObjUtil.Decrypt(ObjUtil.ReadFile(strPath), true);
                }
                else
                {
                    return ObjUtil.ReadFile(strPath);
                }
            }
            else
            {
                clsUtility.ShowErrorMessage("File not found at path :" + strPath + "\nPlease check the path.", "File not found");
                return string.Empty;
            }
        }
        /// <summary>
        /// set the connection string
        /// </summary>
        /// <param name="strConString"></param>
        public void SetConnectionString(string strConString)
        {
            CloseConnection();
            clsConnection_DAL.strConnectionString = strConString;
            Objcon.ConnectionString = strConString;
        }
        /// <summary>
        /// Test or Check the SQL connection. This method just open the sql connection and close it.
        /// It is useful to check that if you can connect to the database or not.
        /// </summary>
        /// <returns>If connection is ok then returns true else returns false.</returns>
        public bool TestConnection()
        {
            bool status = false;
            try
            {
                Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                Objcon.Open();
                status = true;
                CloseConnection();
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "TestConnection");
                status = false;
            }
            return status;
        }

        /// <summary>
        /// Get the Help text of the database object. 
        /// </summary>
        /// <param name="objectName">Pass the database object name such as table,view procedure etc</param>
        /// <param name="database">Pass the database name.</param>
        /// <returns>returns the help text in a string formate.</returns>
        public string GetHelpText(string objectName, string database)
        {
            string strHelpText = string.Empty;
            DataTable dt = ExeSp("exec SP_HELPTEXT " + objectName, database);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strHelpText += Environment.NewLine + dt.Rows[i][0].ToString();
                }
            }
            return strHelpText;
        }

        /// <summary>
        /// Set the currently use database.
        /// </summary>
        /// <param name="dbName">database name</param>
        public void ChangeDatabase(string dbName)
        {
            try
            {
                // if Connection object doest have the connection string then set the static connection string .
                if (Objcon.ConnectionString.Trim().Length == 0)
                {
                    Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                }
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    Objcon.Open();
                }
                dbName = dbName.Trim(c1);
                Objcon.ChangeDatabase(dbName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Set Store Procedure Data. Assign the data to the specified paramter of the Store procedure.
        /// </summary>
        /// <param name="strColumnnName">Column Name</param>
        /// <param name="DataType">Data Type of that column</param>
        /// <param name="Value">Value to be passed to the column</param>
        /// <returns>Returns true after successful execution else returns false.</returns>
        public bool SetStoreProcedureData(string strParamterName, SqlDbType DataType, object Value)
        {
            try
            {
                if (Counter == 0)
                {
                    strColumns = strParamterName;
                    strParamterName = strParamterName.Trim(c1).Replace(" ", string.Empty);
                    strValues = "@" + strParamterName;
                }
                else
                {
                    strColumns += "," + strParamterName;
                    strParamterName = strParamterName.Trim(c1).Replace(" ", string.Empty);
                    strValues += ",@" + strParamterName;
                }
                SqlParameter p = new SqlParameter("@" + strParamterName.Trim(c1).Replace(" ", string.Empty), DataType);
                p.Value = Value;
                lstSQLParameter.Add(p);
                Counter++;
                return true;
            }
            catch (Exception ex)
            {
                clsCommon.ShowError(ex, "SetStoreProcedureData(string strParamterName, SqlDbType DataType, object Value)");
                ResetData();
                return false;
            }
        }

        /// <summary>
        /// Execute Store Procedure statement.
        ///Its a good practice to pass the name of the Store Procedure with database name.
        ///For example : [DatabaseName].[dbo].[TableName]
        ///Example : EmpDb.dbo.Employee;
        /// </summary>
        /// <param name="strStoreProcedureName">Pass the Store Procedure name where you want to call.Its a good practice to pass the name of the Store Procedure with database name.<br>For example : [DatabaseName].[dbo].[StoreProcedure]</br></param>
        /// <param name="ReturnOutput">True returns the output value of the inserted record. False returns the number of rows affected.</param>
        /// <returns>Returns the number of rows affected OR Identity value.</returns>
        public DataTable ExecuteStoreProcedureStatement(string strStoreProcedureName, string strOutPutPrameterName)
        {
            // If Transaction is rollback in first attempt then don't fire any other queries.
            if (IsRollBack)
            {
                ResetData();
                return null;
            }
            int result = 0;
            //string strOutPutPrameterName = string.Empty;
            string strOutPutPrameterValue = string.Empty;
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            try
            {
                if (Objcon.State == ConnectionState.Closed || Objcon.State == ConnectionState.Broken)
                {
                    // if Connection object doest have the connection string then set the static connection string .
                    if (Objcon.ConnectionString.Trim().Length == 0)
                    {
                        Objcon.ConnectionString = clsConnection_DAL.strConnectionString;
                    }
                    Objcon.Open();
                }

                // Transaction is in progress.
                if (ObjTrans != null)
                {
                    cmd.Transaction = ObjTrans;
                }
                
                
                ObjDA = new SqlDataAdapter();

                SqlParameter[] p = lstSQLParameter.ToArray();
                cmd.CommandText = strStoreProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@ID", 1);
                cmd.Parameters.AddRange(p);
                cmd.Connection = Objcon;
                if (strOutPutPrameterName != null)
                {
                    cmd.Parameters.Add("@" + strOutPutPrameterName, SqlDbType.VarChar, 20);
                    cmd.Parameters["@" + strOutPutPrameterName].Direction = ParameterDirection.Output;
                }
                
                // if user want return output value.
                if (strOutPutPrameterName != null)
                {
                    result = cmd.ExecuteNonQuery();
                    strOutPutPrameterValue = Convert.ToString(cmd.Parameters["@" + strOutPutPrameterName].Value);
                }
                else
                {

                    ObjDA.SelectCommand = cmd;
                    //_CommandText = cmd.CommandText;
                    ObjDA.Fill(dt);
                    strOutPutPrameterValue = null;
                }
                CloseConnection();
            }
            catch (Exception ex)
            {
                if (ObjTrans != null)
                {
                    IsRollBack = true;
                    ObjTrans.Rollback();
                }
                string strMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
                CloseConnection();
                clsCommon.ShowError(ex, SetError("ExecuteStoreProcedureStatement(string strStoreProcedureName, bool ReturnOutput)", cmd.CommandText));
                ResetData();
                return null;
            }
            ResetData();
            //return strOutPutPrameterValue;
            return dt;
        }

        /// <summary>
        /// Return the Current DataBase name.
        /// </summary>        
        /// <returns>Returns string.</returns>

        public string GetCurrentDBName(bool b)
        {
            string a = ReadConStringFromFile("AppConfig/ServerConfig.sc", b);
            string[] arr = a.Split(new char[] { ';', '=' });
            return arr[3].ToString();
        }
    }
}