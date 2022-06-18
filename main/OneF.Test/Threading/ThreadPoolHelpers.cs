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

namespace OneF.Threading;

using System;
using System.Threading;

public class ThreadPoolHelpers
{
    public static ThreadCountReset EnsureMinThreadsAtLeast(int minWorkerThreads)
    {
        ThreadPool.GetMinThreads(out var workerThreads, out var ioThreads);

        if(workerThreads < minWorkerThreads)
        {
            _ = ThreadPool.SetMinThreads(minWorkerThreads, ioThreads);

            return new ThreadCountReset(workerThreads, ioThreads);
        }

        return default;
    }

    public struct ThreadCountReset : IDisposable
    {
        private readonly bool _reset;
        private readonly int _worker, _io;

        internal ThreadCountReset(int worker, int io)
        {
            _reset = true;
            _worker = worker;
            _io = io;
        }

        public void Dispose()
        {
            if(_reset)
            {
                _ = ThreadPool.SetMinThreads(_worker, _io);
            }
        }
    }
}
