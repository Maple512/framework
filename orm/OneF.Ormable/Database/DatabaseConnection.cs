// Copyright 2021 Maple512 and Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace OneF.Ormable.Database;

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using OneF.Ormable.Infrastructure;

public abstract class DatabaseConnection : IDatabaseConnection, IResettable
{
    private int? _commandTimeout;
    private readonly int? _defaultCommandTimeout;
    private protected string? _connectionString;
    private DbConnection? _connection;

    /// <summary>
    /// 当前连接打开的次数
    /// </summary>
    private int _openedCount;

    /// <summary>
    /// 是否在本类内部打开的连接
    /// </summary>
    private bool _openedInternally;

    /// <summary>
    /// 是否属于本类内部的连接
    /// </summary>
    private bool _connectionOwned;

    protected DatabaseConnection(IConnectionOptions connectionInfo)
    {
        Id = Guid.NewGuid();
        _defaultCommandTimeout = _commandTimeout = connectionInfo.CommandTimeout;

        _connectionString = connectionInfo.ConnectionString.IsNullOrWhiteSpace()
            ? null
            : connectionInfo.ConnectionString;

        if(connectionInfo.DbConnection != null)
        {
            _connection = connectionInfo.DbConnection;
            _connectionOwned = false;

            if(_connectionString != null)
            {
                _connection.ConnectionString = _connectionString;
            }
        }
        else
        {
            _connectionOwned = true;
        }
    }

    public Guid Id { get; }

    public virtual string? ConnectionString
    {
        get => _connectionString ?? _connection?.ConnectionString;
        set
        {
            if(_connection != null && _connection.ConnectionString != value)
            {
                // Let ADO.NET throw if this is not valid for the state of the connection.
                _connection.ConnectionString = value;
            }

            _connectionString = value;
        }
    }

    [AllowNull]
    public DbConnection Connection
    {
        get
        {
            if(_connection != null)
            {
                return _connection;
            }

            return _connection = CreateDbConnection();
        }
        set
        {
            if(!ReferenceEquals(_connection, value))
            {
                if(_openedCount > 0)
                {
                    throw new InvalidOperationException("The instance of DbConnection is currently in use. The connection can only be changed when the existing connection is not being used.");
                }

                Dispose();

                _connection = value;
                _connectionString = null;
                _connectionOwned = false;
            }
        }
    }

    public int? CommandTimeout
    {
        get => _commandTimeout;
        set
        {
            if(value.HasValue
                && value < 0)
            {
                throw new ArgumentException($"{nameof(CommandTimeout)} can't be less than zero.");
            }

            _commandTimeout = value;
        }
    }

    public virtual bool Open()
    {
        if(Connection.State == ConnectionState.Broken)
        {
            CloseConnection();
        }

        var wasOpened = false;
        if(Connection.State != ConnectionState.Open)
        {
            OpenInternal();

            wasOpened = true;
        }

        _openedCount++;

        return wasOpened;
    }

    public virtual async Task<bool> OpenAsync(CancellationToken cancellationToken)
    {
        if(Connection.State == ConnectionState.Broken)
        {
            await CloseConnectionAsync();
        }

        var wasOpened = false;
        if(Connection.State != ConnectionState.Open)
        {
            await OpenInternalAsync(cancellationToken);

            wasOpened = true;
        }

        _openedCount++;

        return wasOpened;
    }

    public virtual bool Close()
    {
        var wasClosed = false;

        if(ShouldClose())
        {
            _openedCount--;

            if(Connection.State != ConnectionState.Closed)
            {
                CloseConnection();

                wasClosed = true;
            }

            _openedInternally = false;
        }

        return wasClosed;
    }

    public virtual async Task<bool> CloseAsync()
    {
        var wasClosed = false;

        if(ShouldClose())
        {
            _openedCount--;

            if(Connection.State != ConnectionState.Closed)
            {
                await CloseConnectionAsync();

                wasClosed = true;
            }

            _openedInternally = false;
        }

        return wasClosed;
    }

    void IResettable.Reset() => Reset();

    Task IResettable.ResetAsync(CancellationToken cancellationToken) => ResetAsync().AsTask();

    public virtual void Dispose()
    {
        Reset(true);

        GC.SuppressFinalize(this);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await ResetAsync(true);

        GC.SuppressFinalize(this);
    }

    protected abstract DbConnection CreateDbConnection();

    protected virtual void CloseConnection() => Connection.Close();

    protected virtual Task CloseConnectionAsync() => Connection.CloseAsync();

    protected void OpenInternal()
    {
        OpenConnection();

        if(_openedCount == 0)
        {
            _openedInternally = true;
        }
    }

    protected async Task OpenInternalAsync(CancellationToken cancellationToken = default)
    {
        await OpenConnectionAsync(cancellationToken);

        if(_openedCount == 0)
        {
            _openedInternally = true;
        }
    }

    protected virtual void OpenConnection() => Connection.Open();

    protected virtual Task OpenConnectionAsync(CancellationToken cancellationToken = default) => Connection.OpenAsync(cancellationToken);

    protected virtual void DisposeDbConnection() => Connection.Dispose();

    protected virtual async ValueTask DisposeDbConnectionAsync() => await Connection.DisposeAsync();

    protected virtual void Reset(bool disposeConnection = false)
    {
        _commandTimeout = _defaultCommandTimeout;

        _openedCount = 0;
        _openedInternally = false;

        if(disposeConnection
            && _connectionOwned
            && _connection is null)
        {
            DisposeDbConnection();

            _connection = null;

            _openedCount = 0;

            _openedInternally = false;
        }
    }

    protected virtual async ValueTask ResetAsync(bool disposeConnection = false)
    {
        _commandTimeout = _defaultCommandTimeout;

        _openedCount = 0;
        _openedInternally = false;

        if(disposeConnection
            && _connectionOwned
            && _connection is null)
        {
            await DisposeDbConnectionAsync();

            _connection = null;

            _openedCount = 0;

            _openedInternally = false;
        }
    }

    protected virtual string GetValidatedConnectionString() => ConnectionString!;

    /// <summary>
    /// 是否应该关闭连接（只有在本类内部打开的连接才能关闭）
    /// </summary>
    /// <returns></returns>
    private bool ShouldClose()
        => _openedInternally
            && (_openedCount == 0
                || _openedCount == 1);
}
