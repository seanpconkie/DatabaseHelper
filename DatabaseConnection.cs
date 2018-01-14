using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace global.DatabaseHelper
{
    public abstract class DatabaseConnection
    {
        #region ClassDeclarations
        protected Mono.Data.Sqlite.SqliteConnection _dbConn = new Mono.Data.Sqlite.SqliteConnection();
        protected string _strConnection = null;
        protected string _strState = null;
        #endregion
        #region ClassProperties
        public string ConnectionString
        {
            get
            {
                return _dbConn.ConnectionString;
            }
        }
        public string ConnectionState
        {
            get
            {
                return _dbConn.State.ToString();
            }
        }
        #endregion
        #region ClassConstructors
        public DatabaseConnection()
        {

        }
        public DatabaseConnection(string strConnectionString) : this()
        {

        }
        #endregion
        #region ClassGetters
        #endregion
        #region ClassSetters
        #endregion
        #region AbstractClassMethods
        public abstract void NewConnection();
        public abstract void OpenConnection();
        public abstract void CloseConnection();
        public abstract bool InsertQuery(string strInsertIntoString, string strInsertFromStrin);
        public abstract bool DeleteQuery(string strTable, string strWhereClause);
        public abstract bool UpdateQuery(string strUpdateString, string strTable, string strWhereClause);
        public abstract bool ExecuteCommand(string strCommand);
        public abstract List<String> SelectQueryString(string strQuery);
        public abstract List<String> SelectQuery(List<string> lstColumns, string strTable, string strQuery);
        #endregion
        #region PublicClassMethods
        #endregion
    }
}
