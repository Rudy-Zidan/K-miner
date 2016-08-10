using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace K_miner.Core.DataBase
{

    class DataBase
    {
        SqlConnection con;
        public DataBase()
        {
            con = new SqlConnection("Data Source=RUDY-PC\\SQLEXPRESS;Initial Catalog=WebDB;Integrated Security=true;");
        }
        public DataTable Select(string SQL, List<SqlQueryParameter> param, bool StoreProcedure)
        {
            DataTable result = null;
            try
            {
                con.Open();
                SqlCommand cmd = BuildSQlCommand(SQL, param, StoreProcedure);
                if (cmd != null)
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    result = new DataTable();
                    sda.Fill(result);
                }
            }
            catch (SqlException sql)
            {
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public bool Manipulate(string SQL, List<SqlQueryParameter> param, bool StoreProcedure)
        {
            bool flag = false;
            try
            {
                con.Open();
                SqlCommand cmd = BuildSQlCommand(SQL, param, StoreProcedure);
                if (cmd != null)
                {
                    int i = cmd.ExecuteNonQuery();
                    if (i !=0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            catch (SqlException sql)
            {
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return flag;
        }
        private SqlCommand BuildSQlCommand(string SQL, List<SqlQueryParameter> param, bool StoreProcedure)
        {
            SqlCommand cmd = new SqlCommand() ;
            cmd.Connection = con;
            if (StoreProcedure == true)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                cmd.CommandType = CommandType.Text;
            }
            cmd.CommandText = SQL;
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++)
                {
                    cmd.Parameters.Add(param[i].Parameter, param[i].Value);
                }
            }
            return cmd;
        }

        ~DataBase()
        {
        }
    }
}
