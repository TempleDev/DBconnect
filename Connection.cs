﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempleDev
{
    public class DBConnect
    {
        String SqlConnectString = "";
        SqlConnection myConnectionSql;
        SqlCommand objCmd;
        SqlDataReader objDataReader;
        DataSet ds;

        public DBConnect()
        {
            myConnectionSql = new SqlConnection(SqlConnectString);
        }

        public DataSet GetDataSet(String SqlSelect)
        {
            // Input parameter is a SELECT SQL statement. Return is the Dataset
            // Note: The Dataset is also stored as a class variable for use
            // in the GetField function
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(SqlSelect, myConnectionSql);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            ds = myDataSet;
            return myDataSet;
        }

        public DataSet GetDataSet(String SqlSelect, out int theRecordCount)
        {
            // Input parameter is a SELECT SQL statement.
            // Output parameter is the number of rows in the returned dataset.
            // Return is a Dataset
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(SqlSelect, myConnectionSql);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            ds = myDataSet;
            theRecordCount = ds.Tables[0].Rows.Count;
            return myDataSet;
        }

        public DataSet GetDataSet(String SqlSelect, out int theRecordCount, out String theErrorMessage)
        {
            // Input parameter is a SELECT SQL statement.
            // Output parameter (1) is the number of rows in the returned dataset.
            // Output parameter (2) is the error message when an exception occurs.
            // Return is a Dataset
            try
            {

                SqlDataAdapter myDataAdapter = new SqlDataAdapter(SqlSelect, myConnectionSql);
                DataSet myDataSet = new DataSet();
                myDataAdapter.Fill(myDataSet);
                ds = myDataSet;
                theRecordCount = ds.Tables[0].Rows.Count;
                theErrorMessage = "";
                return myDataSet;
            }
            catch (Exception ex)
            {
                theRecordCount = 0;
                theErrorMessage = ex.Message;
                return null;
            }
        }

        //public SqlDataReader GetDataReader(String SqlSelect)
        //{
        //    // Input parameter is a SELECT SQL statement.
        //    objCmd = new SqlCommand(SqlSelect, myConnectionSql);
        //    return objCmd.ExecuteReader();
        //}


        public int DoUpdate(String SqlManipulate)
        {
            // Input parameter is a SQL manipulate statement (INSERT, UPDATE, DELETE).
            // Returns the number of rows affected by the update.
            // Returns -1 when an exsception occurs.
            objCmd = new SqlCommand(SqlManipulate, myConnectionSql);
            try
            {
                myConnectionSql.Open();
                int ret = objCmd.ExecuteNonQuery();
                myConnectionSql.Close();
                return ret;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void DoUpdateWithDSCmdOjb(DataSet passedds, string DBTableDestination)
        {
          // Parameters in are the dataset being passed and the table you are inserting into.
          // The dataset MUST have the SAME COLUMN NAMES as the SQL table to which it is being inserted into.

          try
          {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(myConnectionSql))
            {
              bulkCopy.DestinationTableName = DBTableDestination;

              bulkCopy.WriteToServer(passedds.Tables[0]);

              myConnectionSql.Close();
            }
          }
          catch (Exception ex)
          {
            myConnectionSql.Close();
          }
        }

        public int DoUpdateUsingCmdObj(SqlCommand theCommandObject)
        {
            // Input parameter is a Command object containing a SQL manipulate statement (Insert, Update, Delete).
            // Returns the number of rows affected by the update.
            // Returns -1 when an exsception occurs.
            // This method is used for passing parameters to a Stored Procedure
            try
            {
                theCommandObject.Connection = myConnectionSql;
                theCommandObject.Connection.Open();
                int ret = theCommandObject.ExecuteNonQuery();
                theCommandObject.Connection.Close();
                return ret;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public DataSet GetDataSetUsingCmdObj(SqlCommand theCommand)
        {
            // This method is used for Stored Procedures (SELECT statement only) with Parameters
            theCommand.Connection = myConnectionSql;
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(theCommand);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            ds = myDataSet;

            return myDataSet;

        }

        public DataRow GetRow(DataSet theDataSet, int theRow)
        {
            DataTable objTable = ds.Tables[0];
            DataRow objRow = objTable.Rows[theRow];
            return objRow;
        }

        public Array GetRows(String theCondition)
        {
            // Input parameters are (1) a DataSet and (2) the zero based row of the
            // table in the DataSet to be returned.  Returns a row.
            DataRow[] objRow;
            DataTable objTable = ds.Tables[0];
            objRow = objTable.Select(theCondition);
            return objRow;
        }

        public Object GetField(String theFieldName, int theRow)
        {
            // Input parameterss are (1) a Field (Column) name as a string and
            // (2) the zero based row of the table in the DataSet
            // from which the field is to be extracted. Returns the value
            // in the field as a variant type.
            // This function assumes that one of the getDataSet functions
            // had been called, thus producing a ds at the class level.
            DataTable objTable = ds.Tables[0];
            DataRow objRow = objTable.Rows[theRow];
            return objRow[theFieldName];
        }

        public void CommitDataSet(DataSet theDataSet)
        {
            // Input parameter is a DataSet. This function is used to Commit
            // the Dataset to the Data Source when updating a disconnected ds.

            SqlDataAdapter myDataAdapter = new SqlDataAdapter();
            myDataAdapter.Update(theDataSet);
        }

        public Object ExecuteScalarFunction(SqlCommand theCommand)
        {
            // Input parameter is a Command object containing a Select statement
            // that returns a single scalar value. Returns the single scalar value.
            // Returns  scalar value as a Variant Type.
            theCommand.Connection = myConnectionSql;
            return theCommand.ExecuteScalar();
        }

        public SqlConnection GetConnection()
        {
            // NOTE: .NET has implemented its Stored User Defined Functions only
            // with the Managed Provider for SQL Server,
            // not the OLEDB provider.
            return myConnectionSql;
        }

        public void CloseConnection()
        {
            try
            {
                myConnectionSql.Close();
            }
            catch (Exception ex)
            {
                // Catch exception created when trying to close a closed connection.
            }
        }

        public void ResetConnection()
        {
            try
            {
                myConnectionSql.Close();
                myConnectionSql.Open();
            }
            catch (Exception ex)
            {
                // Catch exception created when trying to close a closed connection.
            }
        }

        // The Deconstructor
        ~DBConnect()
        {
            // Close any open connections to the database before the objects of this class
            // are garbage collected.
            try
            {
                myConnectionSql.Close();
            }
            catch (Exception ex)
            {
                // Catch exception created when trying to close a closed connection.
            }
        }

    }   // end class
}   // end namespace
