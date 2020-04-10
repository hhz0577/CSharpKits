using System;
using System.Collections.Generic;
using System.Text;

namespace HhzOrm.DataSource
{
    using Oracle.DataAccess.Client;//oracle
    using System.Data.SqlClient;//sql server
    using System.Data.SQLite;//sqlite 
    using MySql.Data.MySqlClient;//mysql
    using System.Data.OleDb;//access
    using System.Collections;
    using System.Data;
    using util;
    using System.Security.Cryptography;

    class DBConnectionPool
    {
        private static Hashtable mapdb = new Hashtable();
        public static void initConnection(object dbconn, DaoStruct daoStruct)
        {
            if (!mapdb.ContainsKey(daoStruct.ConStr))
            {
                switch (daoStruct.DbType)
                {
                    case "oracle":
                        if (dbconn == null)
                            dbconn = new OracleConnection(daoStruct.ConStr);
                        if (((OracleConnection)dbconn).State != ConnectionState.Open)
                            ((OracleConnection)dbconn).Open();
                        break;
                    case "mysql":
                        if (dbconn == null)
                            dbconn = new MySqlConnection(daoStruct.ConStr);
                        if (((MySqlConnection)dbconn).State != ConnectionState.Open)
                            ((MySqlConnection)dbconn).Open();
                        break;
                    case "sql":
                        if (dbconn == null)
                            dbconn = new SqlConnection(daoStruct.ConStr);
                        if (((SqlConnection)dbconn).State != ConnectionState.Open)
                            ((SqlConnection)dbconn).Open();
                        break;
                    case "access":
                        if (dbconn == null)
                            dbconn = new OleDbConnection(daoStruct.ConStr);
                        if (((OleDbConnection)dbconn).State != ConnectionState.Open)
                            ((OleDbConnection)dbconn).Open();
                        break;
                    case "sqllite":
                        if (dbconn == null)
                            dbconn = new SQLiteConnection(daoStruct.ConStr);
                        if (((SQLiteConnection)dbconn).State != ConnectionState.Open)
                        {
                            //((SQLiteConnection)dbconn).SetPassword("123");
                            //string ss = ((SQLiteConnection)dbconn).ConnectionString;
                            ((SQLiteConnection)dbconn).Open();
                            
                            //ss = ((SQLiteConnection)dbconn).ConnectionString;
                            //((SQLiteConnection)dbconn).Close();
                            //string ss  = ((SQLiteConnection)dbconn);

                        }
                            
                        break;
                }
                mapdb.Add(daoStruct.ConStr, dbconn);
            }
        }
        public static object GetConnection(DaoStruct daoStruct)
        {
            object dbconn = null;
            foreach (DictionaryEntry de in mapdb)
            {
                if (de.Key.ToString() == daoStruct.ConStr)
                {
                    switch (daoStruct.DbType)
                    {
                        case "oracle":
                            dbconn = (OracleConnection)de.Value;
                            break;
                        case "mysql":
                            dbconn = (MySqlConnection)de.Value;
                            break;
                        case "sql":
                            dbconn = (SqlConnection)de.Value;
                            break;
                        case "access":
                            dbconn = (OleDbConnection)de.Value;
                            break;
                        case "sqllite":
                            dbconn = (SQLiteConnection)de.Value;
                            break;
                    }
                    break;
                }
            }
            return dbconn;
        }
        public static void disposeAllConn()
        {
            foreach (DictionaryEntry de in mapdb)
            {
                object o = de.Value;
                if (o is OracleConnection)
                {
                    if (((OracleConnection)o).State != ConnectionState.Closed)
                    {
                        ((OracleConnection)o).Close();
                    }
                    ((OracleConnection)o).Dispose();
                    o = null;
                }
                else if (o is MySqlConnection)
                {
                    if (((MySqlConnection)o).State != ConnectionState.Closed)
                    {
                        ((MySqlConnection)o).Close();
                    }
                    ((MySqlConnection)o).Dispose();
                    o = null;
                }
                else if (o is SqlConnection)
                {
                    if (((SqlConnection)o).State != ConnectionState.Closed)
                    {
                        ((SqlConnection)o).Close();
                    }
                    ((SqlConnection)o).Dispose();
                    o = null;
                }
                else if (o is OleDbConnection)
                {
                    if (((OleDbConnection)o).State != ConnectionState.Closed)
                    {
                        ((OleDbConnection)o).Close();
                    }
                    ((OleDbConnection)o).Dispose();
                    o = null;
                }
                else if (o is SQLiteConnection)
                {
                    if (((SQLiteConnection)o).State != ConnectionState.Closed)
                    {
                        ((SQLiteConnection)o).Close();
                    }
                    ((SQLiteConnection)o).Dispose();
                    o = null;
                }
            }
            mapdb.Clear();
        }
        public static void disposeConnection(DaoStruct daoStruct)
        {
            object dbconn = null;
            foreach (DictionaryEntry de in mapdb)
            {
                if (de.Key.ToString() == daoStruct.ConStr)
                {
                    dbconn = de.Value;
                    break;
                }
            }
            switch (daoStruct.DbType)
            {
                case "oracle":
                    if (dbconn != null)
                    {
                        if (((OracleConnection)dbconn).State != ConnectionState.Closed)
                        {
                            ((OracleConnection)dbconn).Close();
                        }
                        ((OracleConnection)dbconn).Dispose();
                        dbconn = null;
                        mapdb.Remove(daoStruct.ConStr);
                    }
                    break;
                case "mysql":
                    if (dbconn != null)
                    {
                        if (((MySqlConnection)dbconn).State != ConnectionState.Closed)
                        {
                            ((MySqlConnection)dbconn).Close();
                        }
                        ((MySqlConnection)dbconn).Dispose();
                        dbconn = null;
                        mapdb.Remove(daoStruct.ConStr);
                    }
                    break;
                case "sql":
                    if (dbconn != null)
                    {
                        if (((SqlConnection)dbconn).State != ConnectionState.Closed)
                        {
                            ((SqlConnection)dbconn).Close();
                        }
                        ((SqlConnection)dbconn).Dispose();
                        dbconn = null;
                        mapdb.Remove(daoStruct.ConStr);
                    }
                    break;
                case "access":
                    if (dbconn != null)
                    {
                        if (((OleDbConnection)dbconn).State != ConnectionState.Closed)
                        {
                            ((OleDbConnection)dbconn).Close();
                        }
                        ((OleDbConnection)dbconn).Dispose();
                        dbconn = null;
                        mapdb.Remove(daoStruct.ConStr);
                    }
                    break;
                case "sqllite":
                    if (dbconn != null)
                    {
                        if (((SQLiteConnection)dbconn).State != ConnectionState.Closed)
                        {
                            ((SQLiteConnection)dbconn).Close();
                        }
                        ((SQLiteConnection)dbconn).Dispose();
                        dbconn = null;
                        mapdb.Remove(daoStruct.ConStr);
                    }
                    break;
            }
        }
        public static string StringToSHA1Hash(string inputString)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] encryptedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
