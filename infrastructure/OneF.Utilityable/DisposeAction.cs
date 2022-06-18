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

namespace OneF;

using System;
using System.Threading.Tasks;

public readonly struct DisposeAction : IDisposable, IAsyncDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}
