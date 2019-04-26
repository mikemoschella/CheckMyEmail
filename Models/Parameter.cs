using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace CheckMyEmail.Models
{
    public class Parameter
    {
        public MySqlDbType DataType { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
    }
}