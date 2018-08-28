using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRSystem
{
    public class DatabaseManager
    {
        DataSet ds;
        SqlDataAdapter da;
        public static SqlConnection connect()
        {
            //Reading the connection string from web.config    
            string Name = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            //Passing the string in sqlconnection.    
            SqlConnection conn = new SqlConnection(Name);
            //Check wheather the connection is close or not if open close it else open it    
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();

            }
            else
            {

                conn.Open();
            }
            return conn;

        }
        public DataTable MyMethod(string Query)
        {
            ds = new DataSet();
            DataTable dt = new DataTable();
            da = new SqlDataAdapter(Query, DatabaseManager.connect());

            da.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            return dt;

        }
    }
}