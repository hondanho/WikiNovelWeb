
using System;


namespace WowSQL
{
  public class SqlHelperException : Exception
  {
    public SqlHelperException(SQLHelperStatusCode statusCode, Exception exception)
    {
      this.Exception = exception;
      this.StatusCode = statusCode;
    }

    public SqlHelperException(SQLHelperStatusCode statusCode, string str_exception)
      : this(statusCode, new Exception(str_exception))
    {
    }

    public Exception Exception { set; get; }

    public SQLHelperStatusCode StatusCode { set; get; }

    public override string ToString()
    {
      return "[" + ((int) this.StatusCode).ToString() + "] " + this.Exception.Message;
    }
  }
}
