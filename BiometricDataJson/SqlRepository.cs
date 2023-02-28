using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace RTP.SQLWriter
{
    public interface ISqlConnectionStringProvider
    {
        string SqlServerName { get; set; }
        string SqlInstanceName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Database { get; set; }
        string Table { get; set; }
        string ConnectionString { get; }
    }

    public class ConnectionStringProvider : ISqlConnectionStringProvider
    {
        public string SqlServerName { get; set; }
        public string SqlInstanceName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Table { get; set; }
       
        public string ConnectionString => connectionStringBuilder;
        private string connectionStringBuilder {
            get {
                return String.Format("Data Source = {0}\\{1}; Initial Catalog = {2}; User ID = {3}; Password = {4}; MultipleActiveResultSets = True", SqlServerName, SqlInstanceName, Database, UserName, Password);
            }
        }

        public ConnectionStringProvider(string SqlServerName, string SqlInstanceName, string userName, string password, string database, string tableName = null)
        {
            this.SqlServerName = SqlServerName;
            this.SqlInstanceName = SqlInstanceName;
            this.UserName = userName;
            this.Password = password;
            this.Database = database;
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }

    public class SqlRepository<T> : IDisposable where T : class
    {
        private readonly SqlConnection _connection;
        private readonly string _tableName;
        private readonly PropertyInfo[] _properties;
      

        public SqlRepository(ISqlConnectionStringProvider sqlConnectionStringProvider, string tableName = null)
        {
            
            if (tableName != null)
                _tableName = tableName;
            else
                _tableName = typeof(T).GetCustomAttribute<TableNameAttribute>().Name;

            _connection = new SqlConnection(sqlConnectionStringProvider.ConnectionString);
            _properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ColumnNameAttribute>() != null)
                .ToArray();
        }

        public void Insert(T entity)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = _connection;
                command.CommandType = CommandType.Text;

                string columns = string.Join(", ", _properties.Select(p => p.GetCustomAttribute<ColumnNameAttribute>().Name));
                string parameters = string.Join(", ", _properties.Select(p => "@" + p.Name));

                command.CommandText = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";

                foreach (var property in _properties)
                {
                    command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                }

                _connection.Open();
                command.ExecuteNonQuery();
                _connection.Close();
            }
        }

        public void Update(T entity)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = _connection;
                command.CommandType = CommandType.Text;

                string set = string.Join(", ", _properties.Select(p => p.GetCustomAttribute<ColumnNameAttribute>().Name + " = @" + p.Name));
                string idColumn = _properties.First(p => p.Name == "Id").GetCustomAttribute<ColumnNameAttribute>().Name;

                command.CommandText = $"UPDATE {_tableName} SET {set} WHERE {idColumn} = @Id";

                foreach (var property in _properties)
                {
                    command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                }

                _connection.Open();
                command.ExecuteNonQuery();
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = _connection;
                command.CommandType = CommandType.Text;

                string idColumn = _properties.First(p => p.Name == "Id").GetCustomAttribute<ColumnNameAttribute>().Name;

                command.CommandText = $"DELETE FROM {_tableName} WHERE {idColumn} = @Id";
                command.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                command.ExecuteNonQuery();
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}