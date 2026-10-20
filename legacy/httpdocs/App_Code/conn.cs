using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;

/// <summary>
/// Summary description for conn
/// </summary>
public static class conn
{
    public static SqlConnection connection = new SqlConnection("server=REDACTED;database=TRCekimDB;uid=REDACTED;pwd=REDACTED;");

    //public static SqlConnection connection = new SqlConnection("server=REDACTED;database=TRCekimDB;uid=REDACTED;pwd=REDACTED;");
}