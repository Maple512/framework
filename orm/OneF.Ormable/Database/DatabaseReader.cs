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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

public class DatabaseReader : IDatabaseReader
{
    private bool _disposed;

    private readonly DbDataReader _reader;

    public DatabaseReader(DbDataReader reader)
    {
        _reader = reader;
    }

    public object this[int i] => _reader[i];

    public object this[string name] => _reader[name];

    public int Depth => _reader.Depth;
    public bool IsClosed => _reader.IsClosed;
    public int RecordsAffected => _reader.RecordsAffected;
    public int FieldCount => _reader.FieldCount;

    public void Close() => _reader.Close();

    public void Dispose()
    {
        if(!_disposed)
        {
            try
            {
                _reader.Close();
            }
            finally
            {
                _disposed = true;
                _reader.Dispose();
            }
        }

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if(!_disposed)
        {
            try
            {
                await _reader.CloseAsync();
            }
            finally
            {
                _disposed = true;
                await _reader.DisposeAsync();
            }
        }

        GC.SuppressFinalize(this);
    }

    public bool GetBoolean(int i) => _reader.GetBoolean(i);

    public byte GetByte(int i) => _reader.GetByte(i);

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        => _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    public char GetChar(int i)
        => _reader.GetChar(i);

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        => _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    public IDataReader GetData(int i)
        => _reader.GetData(i);

    public string GetDataTypeName(int i)
        => _reader.GetDataTypeName(i);

    public DateTime GetDateTime(int i)
        => _reader.GetDateTime(i);

    public decimal GetDecimal(int i)
        => _reader.GetDecimal(i);

    public double GetDouble(int i)
        => _reader.GetDouble(i);

    public IEnumerator GetEnumerator()
        => _reader.GetEnumerator();

    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
    public Type GetFieldType(int i)
        => _reader.GetFieldType(i);

    public float GetFloat(int i)
        => _reader.GetFloat(i);

    public Guid GetGuid(int i)
        => _reader.GetGuid(i);

    public short GetInt16(int i)
        => _reader.GetInt16(i);

    public int GetInt32(int i)
        => _reader.GetInt32(i);

    public long GetInt64(int i)
        => _reader.GetInt64(i);

    public string GetName(int i)
        => _reader.GetName(i);

    public int GetOrdinal(string name)
        => _reader.GetOrdinal(name);

    public DataTable? GetSchemaTable()
        => _reader.GetSchemaTable();

    public string GetString(int i)
        => _reader.GetString(i);

    public object GetValue(int i)
        => _reader.GetValue(i);

    public int GetValues(object[] values)
        => _reader.GetValues(values);

    public bool IsDBNull(int i)
        => _reader.IsDBNull(i);

    public bool NextResult()
        => _reader.NextResult();

    public bool Read()
        => _reader.Read();

    public Task<bool> ReadAsync(CancellationToken cancellationToken = default)
        => _reader.ReadAsync(cancellationToken);
}
