using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CheckMyEmail.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace CheckMyEmail.Objects
{
    //Classs to Access MySql Database

    public class MySqlAccess
    {
        
            private readonly string _connectionString;

            public MySqlAccess(string ConnectionString)
            {
                    _connectionString = ConnectionString;

            }

            public MySqlAccess()
            {
                _connectionString = ConfigurationManager.ConnectionStrings["MySqlAccessConnection"].ConnectionString;

            }
            public DataSet MySql_DataAdapter(string SQLText)
            {
                DataSet ds = new DataSet();
                MySqlConnection MySql_Connection;
                MySql.Data.MySqlClient.MySqlDataAdapter MySql_DataAdapter;

                using (MySql_Connection = new MySqlConnection(_connectionString))
                {

                        MySql_DataAdapter = new MySqlDataAdapter(SQLText, MySql_Connection);

                        MySql_DataAdapter.Fill(ds, "dsResult");
                        return ds;
                }

            }





            public void MySql_CommandCall(CommandType CallType, string SQLText, List<Parameter> SQLParameters)
            {
                MySqlConnection MySql_Connection;
                MySql.Data.MySqlClient.MySqlCommand MySql_Command;

                using (MySql_Connection = new MySqlConnection(_connectionString))
                {

                        MySql_Connection.Open();

                        MySql_Command = MySql_Connection.CreateCommand();
                        MySql_Command.Connection = MySql_Connection;
                        MySql_Command.CommandType = CallType;
                        MySql_Command.CommandText = SQLText;



                        foreach (Parameter SQLParameter in SQLParameters)
                        {

                            switch (SQLParameter.DataType)
                            {
                                case MySqlDbType.Bit:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.Bit).Value = Boolean.Parse(SQLParameter.Value);
                                    break;
                                case MySqlDbType.VarString:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.VarString).Value = SQLParameter.Value;
                                    break;
                                case MySqlDbType.Int32:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.Int32).Value = Convert.ToInt32(SQLParameter.Value);
                                    break;
                                case MySqlDbType.DateTime:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.DateTime).Value = DateTime.Parse(SQLParameter.Value);
                                    break;
                                case MySqlDbType.Double:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.Double).Value = Convert.ToDouble(SQLParameter.Value);
                                    break;
                                case MySqlDbType.Float:
                                    MySql_Command.Parameters.Add(SQLParameter.ParameterName, MySqlDbType.Float).Value = Convert.ToDouble(SQLParameter.Value);
                                    break;
                            }

                        }
                        MySql_Command.ExecuteNonQuery();
                }

            }
        }
    }