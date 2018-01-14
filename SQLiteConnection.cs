﻿using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace global.DatabaseHelper
{
    public class SQLiteConnection : DatabaseConnection
    {
        #region ClassDeclarations
        //protected string _strConnection;
        //protected string _strState;
        private const string _localDBName = "myApp_LocalPersistence.db3";
        private new Mono.Data.Sqlite.SqliteConnection _dbConn = new Mono.Data.Sqlite.SqliteConnection();
        #endregion
        #region ClassProperties
        public new string ConnectionString
        {
            get
            {
                return _dbConn.ConnectionString;
            }
        }
        public new string ConnectionState
        {
            get
            {
                return _dbConn.State.ToString();
            }
        }
        #endregion
        #region ClassConstructors
        public SQLiteConnection()
        {
            _strConnection = DBCreate();
        }
        public SQLiteConnection(string strConnectionString = null) : this()
        {
            NewConnection(strConnectionString);
        }
        #endregion
        #region ClassGetters
        #endregion
        #region ClassSetters
        #endregion
        #region ClassMethods

        public override void NewConnection()
        {
            NewConnection(null);
        }
        private string NewConnection(string strConnectionString)
        {
            //<summary><summary/>
            //<params><params/>
            //<output><output/>

            #region NewConnection_declarations
            string strDBPath = null;
            #endregion

            #region NewConnection_validaton
            if (string.IsNullOrWhiteSpace(strConnectionString))
            {
                strDBPath = _strConnection;
            }
            else
            {
                strDBPath = strConnectionString;
            }

            if (string.IsNullOrWhiteSpace(strDBPath))
            {
                throw new NullReferenceException(message: "Both strConnectionString and _strConnection are NULL.", innerException: new FileNotFoundException(message:"Default database doesn't exist.",fileName:Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + _localDBName));
            }

            if (ConnectionState == "Open")
            {
                _dbConn.Close();
            }
            #endregion

            #region NewConnection_procedure
            // Create connection to the database
            _dbConn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + strDBPath);
            _dbConn.Open();
            _strState = _dbConn.State.ToString();

            return strDBPath;
            #endregion
        }
        public override void CloseConnection()
        {
            ///<summary></summary>
            ///<params></params>
            ///<output></output>

            #region CloseConnection_declarations
            #endregion

            #region CloseConnection_validaton
            #endregion

            #region CloseConnection_procedure
            // close connection to the database
            if (this.ConnectionState == "Open")
            {
                _dbConn.Close();
            }
            _strState = _dbConn.State.ToString();

            #endregion
        }

        public override void OpenConnection()
        {
            ///<summary>
            /// uses already set connection string to re-open
            /// a closed connection
            /// </summary>
            /// <param>
            /// no public set params, uses _strConnection
            /// </param>


            #region OpenConnection_declarations
            #endregion

            #region OpenConnection_validaton
            #endregion

            #region OpenConnection_procedure
            // if connection not open then open
            if (_dbConn.State.ToString() != "Open")
            {
                _dbConn.ConnectionString = _strConnection;
                _dbConn.Open();
            }

            _strState = _dbConn.State.ToString();

            #endregion

        }

        #region Select Queries
        public override List<String> SelectQueryString(string strQuery)
        {
            ///<summary>
            /// same as SelectQuery but single parameter of string.  Allows for more complex select query.
            /// </summary>
            ///<params>
            /// </params>
            ///<output>
            /// </output>

            #region SelectQueryString_declarations
            int intNewConnectionCount = 0;
            var lstQueryResults = new List<string>();
            var lstReader = new List<string>();
            #endregion

            #region SelectQueryString_validaton
            //valiate strTable parameter
            while (this.ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }
            #endregion

            #region SelectQueryString_procedure
            // Execute query
            using (var dbCMD = _dbConn.CreateCommand())
            {
                // Create new command
                dbCMD.CommandText = strQuery;

                // Get the results from the database
                using (var dbRS = dbCMD.ExecuteReader())
                {
                    // Read the display field from the query
                    while (dbRS.Read())
                    {
                        // Read result from query
                        for (int i = 0; i < dbRS.FieldCount; i++)
                        {
                            lstReader.Add(dbRS[i].ToString());
                        }

                        lstQueryResults.Add(string.Join("|", lstReader.ToArray()));
                        lstReader.Clear();
                    }

                }
            }

            return lstQueryResults;
            #endregion
        }


        public override List<String> SelectQuery(List<string> lstColumns, string strTable, string strQuery)
        {
            ///<summary></summary>
            ///<params></params>
            ///<output></output>

            #region SelectQuery_declarations
            int intNewConnectionCount = 0;
            var lstQueryResults = new List<string>();
            var lstReader = new List<string>();
            var strQueryString = new StringBuilder();
            #endregion

            #region SelectQuery_validaton
            //valiate strTable parameter
            if (string.IsNullOrWhiteSpace(strTable))
            {
                throw new ArgumentNullException(paramName: nameof(strTable), message: "A table string must be provided.");
            }

            while (ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }
            #endregion

            #region SelectQuery_procedure
            // Create string
            strQueryString.Append("SELECT ");
            if (lstColumns.Count == 0)
            {
                strQueryString.Append("* ");
            }
            else
            {
                for (int i = 0; i < lstColumns.Count; i++)
                {
                    if (!lstColumns[i].Contains("["))
                    {
                        strQueryString.Append("[");
                    }
                    strQueryString.Append(lstColumns[i]);
                    if (!lstColumns[i].Contains("]"))
                    {
                        strQueryString.Append("]");
                    }
                    if (i < lstColumns.Count - 1)
                    {
                        strQueryString.Append(", ");
                    }
                }
            }

            strQueryString.Append(" FROM ");
            strQueryString.Append(strTable);
            strQueryString.Append(" ");
            strQueryString.Append(strQuery);

            // Execute query
            using (var dbCMD = _dbConn.CreateCommand())
            {
                // Create new command
                dbCMD.CommandText = strQueryString.ToString();

                // Get the results from the database
                using (var dbRS = dbCMD.ExecuteReader())
                {
                    // Read the display field from the query
                    while (dbRS.Read())
                    {
                        // Read result from query
                        for (int i = 0; i < dbRS.FieldCount; i++)
                        {
                            lstReader.Add(dbRS[i].ToString());
                        }

                        lstQueryResults.Add(string.Join("|", lstReader.ToArray()));
                        lstReader.Clear();
                    }

                }
            }

            return lstQueryResults;
            #endregion
        }
        #endregion

        public override bool DeleteQuery(string strTable, string strWhereClause)
        {
            #region DeleteQuery_Variables
            int intNewConnectionCount = 0;
            var strQuery = new StringBuilder();
            #endregion
            #region DeleteQuery_Validation
            if (string.IsNullOrWhiteSpace(strTable))
            {
                throw new ArgumentNullException(paramName: nameof(strTable), message: "Table cannot be blank.");
            }

            while (this.ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }

            #endregion
            #region DeleteQuery_Procedure
            //create update query
            strQuery.Append("delete from ");
            strQuery.Append(strTable);
            if (!strWhereClause.ToLower().Contains("where") && !string.IsNullOrWhiteSpace(strWhereClause))
            {
                strQuery.Append(" where ");
            }
            strQuery.Append(strWhereClause);

            try
            {
                ExecuteCommand(strQuery.ToString());
            }
            catch (Exception ex)
            {
                throw new SystemException(message: ex.Message);
            }

            return true;
            #endregion
        }
        public override bool UpdateQuery(string strUpdateString, string strTable, string strWhereClause)
        {
            #region UpdateQuery_Variables
            int intNewConnectionCount = 0;
            var strQuery = new StringBuilder();
            #endregion
            #region UpdateQuery_Validation
            if (string.IsNullOrWhiteSpace(strUpdateString))
            {
                throw new ArgumentNullException(paramName: nameof(strUpdateString), message: "Update string cannot be blank.");
            }

            if (string.IsNullOrWhiteSpace(strTable))
            {
                throw new ArgumentNullException(paramName: nameof(strTable), message: "Table cannot be blank.");
            }

            while (this.ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }

            #endregion
            #region UpdateQuery_Procedure
            //create update query
            strQuery.Append("update ");
            strQuery.Append(strTable);
            if (!strUpdateString.ToLower().Contains("set"))
            {
                strQuery.Append(" set ");
            }
            strQuery.Append(strUpdateString);
            if (!strWhereClause.ToLower().Contains("where") && !string.IsNullOrWhiteSpace(strWhereClause))
            {
                strQuery.Append(" where ");
            }
            strQuery.Append(strWhereClause);

            try
            {
                ExecuteCommand(strQuery.ToString());
            }
            catch (Exception ex)
            {
                throw new SystemException(message: ex.Message);
            }

            return true;
            #endregion
        }
        public override bool ExecuteCommand(string strCommand)
        {
            #region ExecuteCommand_Variables
            int intNewConnectionCount = 0;
            #endregion
            #region ExecuteCommand_Validation
            if (string.IsNullOrWhiteSpace(strCommand))
            {
                throw new ArgumentNullException(paramName: nameof(strCommand), message: "SQL Command Text is blank.");
            }

            while (this.ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }

            #endregion
            #region ExecuteCommand_Procedure

            using (var dbCMD = _dbConn.CreateCommand())
            {
                //dbCMD.CommandType = CommandType.Text;
                dbCMD.CommandText = strCommand;
                dbCMD.CommandTimeout = 3600;

                try
                {
                    dbCMD.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new SystemException(message: ex.Message);
                }
            }

            return true;
            #endregion
        }

        public override bool InsertQuery(string strInsertIntoString, string strInsertFromString)
        {
            #region InsertQuery_Variables
            int intNewConnectionCount = 0;
            var strQuery = new StringBuilder();
            #endregion
            #region InsertQuery_Validation
            if (string.IsNullOrWhiteSpace(strInsertIntoString))
            {
                throw new ArgumentNullException(paramName: nameof(strInsertIntoString), message: "Insert into string cannot be blank.");
            }
            if (string.IsNullOrWhiteSpace(strInsertFromString))
            {
                throw new ArgumentNullException(paramName: nameof(strInsertFromString), message: "Insert from string cannot be blank.");
            }

            while (this.ConnectionState != "Open")
            {
                if (intNewConnectionCount < 1)
                {
                    NewConnection();
                }
                else
                {
                    throw new InvalidOperationException("Unable to obtain active connection to server.");
                }

            }

            #endregion
            #region InsertQuery_Procedure
            //create update query
            strQuery.Append(strInsertIntoString);
            strQuery.Append(strInsertFromString);

            try
            {
                ExecuteCommand(strQuery.ToString());
            }
            catch (Exception ex)
            {
                throw new SystemException(message: ex.Message);
            }

            return true;
            #endregion
        }
        #endregion


        private string DBCreate()
        {
            //<summary><summary/>
            //<params><params/>
            //<output><output/>

            #region DBCreate_declarations
            string strPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            string strDBPath = strPath + "/" +_localDBName;
            var testStr = Path.GetPathRoot(strPath);
            #endregion

            #region DBCreate_validaton
            // Create the database if it doesn't already exist
            if (!File.Exists(strDBPath))
            {
                SqliteConnection.CreateFile(strDBPath);
            }
            else
            {
                return strDBPath; //db exists so exit now and return default path
            }
            #endregion

            #region DBCreate_procedure
            // Create connection to the database
            var sqliteConn = new Mono.Data.Sqlite.SqliteConnection("Data Source=" + strDBPath);

            // Set the structure of the database
            if (File.Exists(strDBPath))
            {
                var sqliteCMD = new[] { "CREATE TABLE LocalSettings (Type TEXT, StringValue1 TEXT, StringValue2 TEXT, StringValue3 TEXT, StringValue4 TEXT, StringValue5 TEXT)" };

                sqliteConn.Open();
                foreach (var cmd in sqliteCMD)
                {
                    using (var c = sqliteConn.CreateCommand())
                    {
                        c.CommandText = cmd;
                        c.CommandType = System.Data.CommandType.Text;
                        c.ExecuteNonQuery();
                    }
                }
            }

            sqliteConn.Close();

            return strDBPath;
            #endregion

        }
    }

    }

