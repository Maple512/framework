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
using System.Diagnostics;

// source: https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs
public struct StopwatchValue
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private readonly long _startTimestamp;

    public bool IsActive
    {
        get
        {
            return _startTimestamp != 0;
        }
    }

    private StopwatchValue(long startTimestamp)
    {
        _startTimestamp = startTimestamp;
    }

    public static StopwatchValue StartNew()
    {
        return new StopwatchValue(Stopwatch.GetTimestamp());
    }

    public TimeSpan GetElapsedTime()
    {
        // Start timestamp can't be zero in an initialized StopwatchValue. It would have to be
        // literally the first thing executed when the machine boots to be 0. So it being 0 is a
        // clear indication of default(StopwatchValue)
        if(!IsActive)
        {
            throw new InvalidOperationException($"An uninitialized, or 'default', {nameof(StopwatchValue)} cannot be used to get elapsed time.");
        }

        var end = Stopwatch.GetTimestamp();

        var timestampDelta = end - _startTimestamp;

        var ticks = _timestampToTicks * timestampDelta;

        return new TimeSpan(ticks);
    }
}
