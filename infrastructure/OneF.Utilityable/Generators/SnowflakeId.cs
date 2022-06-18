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

namespace OneF.Generators;

using System;
using System.Net.NetworkInformation;
using System.Threading;

public class SnowflakeId
{
    /// <summary>
    /// Start time 2010-11-04 09:42:54
    /// </summary>
    public const long Twepoch = 1288834974657L;

    /// <summary>
    /// The number of bits occupied by workerId
    /// </summary>
    private const int WorkerIdBits = 10;

    /// <summary>
    /// The number of bits occupied by timestamp
    /// </summary>
    private const int TimestampBits = 41;

    /// <summary>
    /// The number of bits occupied by sequence
    /// </summary>
    private const int SequenceBits = 12;

    /// <summary>
    /// Maximum supported machine id, the result is 1023
    /// </summary>
    private const int MaxWorkerId = ~(-1 << WorkerIdBits);

    /// <summary>
    /// mask that help to extract timestamp and sequence from a long
    /// </summary>
    private const long TimestampAndSequenceMask = ~(-1L << (TimestampBits + SequenceBits));

    /// <summary>
    /// business meaning: machine ID (0 ~ 1023)
    /// actual layout in memory:
    /// highest 1 bit: 0
    /// middle 10 bit: workerId
    /// lowest 53 bit: all 0
    /// </summary>
    private long WorkerId { get; set; }

    /// <summary>
    /// timestamp and sequence mix in one Long
    /// highest 11 bit: not used
    /// middle  41 bit: timestamp
    /// lowest  12 bit: sequence
    /// </summary>
    private long _timestampAndSequence;

    private static SnowflakeId? _snowflakeId;

    private static readonly object SLock = new();

    private readonly object _lock = new();

    public SnowflakeId(long workerId)
    {
        InitTimestampAndSequence();

        // sanity check for workerId
        if(workerId is > MaxWorkerId or < 0)
        {
            throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");
        }

        WorkerId = workerId << (TimestampBits + SequenceBits);
    }

    public static SnowflakeId Default()
    {
        if(_snowflakeId != null)
        {
            return _snowflakeId;
        }

        lock(SLock)
        {
            if(_snowflakeId != null)
            {
                return _snowflakeId;
            }

            if(!long.TryParse(Environment.GetEnvironmentVariable("CAP_WORKERID", EnvironmentVariableTarget.Machine), out var workerId))
            {
                workerId = Util.GenerateWorkerId(MaxWorkerId);
            }

            return _snowflakeId = new SnowflakeId(workerId);
        }
    }

    public virtual long NextId()
    {
        lock(_lock)
        {
            WaitIfNecessary();

            var timestampWithSequence = _timestampAndSequence & TimestampAndSequenceMask;

            return WorkerId | timestampWithSequence;
        }
    }

    /// <summary>
    /// init first timestamp and sequence immediately
    /// </summary>
    private void InitTimestampAndSequence()
    {
        var timestamp = GetNewestTimestamp();

        var timestampWithSequence = timestamp << SequenceBits;

        _timestampAndSequence = timestampWithSequence;
    }

    /// <summary>
    /// block current thread if the QPS of acquiring UUID is too high
    /// that current sequence space is exhausted
    /// </summary>
    private void WaitIfNecessary()
    {
        var currentWithSequence = ++_timestampAndSequence;

        var current = currentWithSequence >> SequenceBits;

        var newest = GetNewestTimestamp();

        if(current >= newest)
        {
            Thread.Sleep(5);
        }
    }

    /// <summary>
    /// get newest timestamp relative to twepoch
    /// </summary>
    /// <returns></returns>
    private static long GetNewestTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Twepoch;
    }
}

internal static class Util
{
    /// <summary>
    /// auto generate workerId, try using mac first, if failed, then randomly generate one
    /// </summary>
    /// <returns>workerId</returns>
    public static long GenerateWorkerId(int maxWorkerId)
    {
        try
        {
            return GenerateWorkerIdBaseOnMac();
        }
        catch
        {
            return GenerateRandomWorkerId(maxWorkerId);
        }
    }

    /// <summary>
    /// use lowest 10 bit of available MAC as workerId
    /// </summary>
    /// <returns>workerId</returns>
    private static long GenerateWorkerIdBaseOnMac()
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces();

        if(nics == null || nics.Length < 1)
        {
            throw new Exception("no available mac found");
        }

        var adapter = nics[0];
        var address = adapter.GetPhysicalAddress();
        var mac = address.GetAddressBytes();

        return ((mac[4] & 3) << 8) | (mac[5] & 255);
    }

    /// <summary>
    /// randomly generate one as workerId
    /// </summary>
    /// <returns></returns>
    private static long GenerateRandomWorkerId(int maxWorkerId)
    {
        return new Random().Next(maxWorkerId + 1);
    }
}
