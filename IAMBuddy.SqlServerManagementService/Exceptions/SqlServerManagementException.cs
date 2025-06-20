namespace IAMBuddy.SqlServerManagementService.Exceptions;

public class SqlServerManagementException : Exception
{
    public SqlServerManagementException(string message) : base(message)
    {
    }

    public SqlServerManagementException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerValidationException : SqlServerManagementException
{
    public SqlServerValidationException(string message) : base(message)
    {
    }

    public SqlServerValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerConnectionException : SqlServerManagementException
{
    public SqlServerConnectionException(string message) : base(message)
    {
    }

    public SqlServerConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerLoginException : SqlServerManagementException
{
    public SqlServerLoginException(string message) : base(message)
    {
    }

    public SqlServerLoginException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerRoleException : SqlServerManagementException
{
    public SqlServerRoleException(string message) : base(message)
    {
    }

    public SqlServerRoleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerUserException : SqlServerManagementException
{
    public SqlServerUserException(string message) : base(message)
    {
    }

    public SqlServerUserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class SqlServerPermissionException : SqlServerManagementException
{
    public SqlServerPermissionException(string message) : base(message)
    {
    }

    public SqlServerPermissionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}