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
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

public class DatabaseTransaction : IDatabaseTransaction
{
    private bool disposedValue;

    public DatabaseTransaction(Guid transactionId, DbTransaction dbTransaction)
    {
        Id = transactionId;
        DbTransaction = dbTransaction;
    }

    public Guid Id { get; }

    public DbTransaction DbTransaction { get; }

    public void Commit() => DbTransaction.Commit();

    public Task CommitAsync(CancellationToken cancellationToken = default) => DbTransaction.CommitAsync(cancellationToken);

    public void Rollback() => DbTransaction.Rollback();

    public Task RollbackAsync(CancellationToken cancellationToken = default) => DbTransaction.RollbackAsync(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!disposedValue)
        {
            if(disposing)
            {
                DbTransaction.Dispose();
            }

            disposedValue = true;
        }
    }
}
