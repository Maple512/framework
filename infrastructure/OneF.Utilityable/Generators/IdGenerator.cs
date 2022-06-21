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

/// <summary>
/// 雪花ID生成器
/// <para>雪花ID：第一位为未使用，接下来的41位为毫秒级时间(41位的长度可以使用69年)，然后是5位datacenterId和5位workerId(10位的长度最多支持部署1024个节点） ，最后12位是毫秒内的计数（12位的计数顺序号支持每个节点每毫秒产生4096个ID序号）</para>
/// <para>0 - 0000000000 0000000000 0000000000 0000000000 0 - 00000 - 00000 - 000000000000</para>
/// <para>时间漂移：服务器时间出现抖动，未能保持递增</para>
/// </summary>
public class IdGenerator
{
    /// <summary>
    /// js最大的安全整数（2^53 - 1）
    /// </summary>
    public const long JS_NUMBER_MAX_SAFE_INTEGER = 9_007_199_254_740_992;

    private static readonly IdGeneratorOptions _options;

    protected byte _timestampShift = 0;
    protected ushort _currentSeqNumber;
    protected long _lastTimeTick = 0; // -1L
    protected long _turnbackTimeTick = 0; // -1L;
    protected byte _turnbackIndex = 0;

    /// <summary>
    /// 是否漂移
    /// </summary>
    protected bool _isOverCost = false;
    protected int _overCostCountInOneTerm = 0;
    protected int _genCountInOneTerm = 0;
    protected int _termIndex = 0;

    public IdGenerator(IdGeneratorOptions options)
    {
        Check.Validate(options);

        _timestampShift = (byte)(_options.WorkerIdBitLength + _options.SequenceBitLength);

        _currentSeqNumber = options.SequenceMinNumber;
    }

    protected virtual long NextOverCostId()
    {
        // 当前时间差
        var currentTimeTick = GetCurrentTimeTick(_options.BaseTime);

        if(currentTimeTick > _lastTimeTick)
        {
            //EndOverCostAction(currentTimeTick);

            _lastTimeTick = currentTimeTick;
            _currentSeqNumber = _options.SequenceMinNumber;
            _isOverCost = false;
            _overCostCountInOneTerm = 0;
            _genCountInOneTerm = 0;

            return CalcId(_lastTimeTick);
        }

        if(_overCostCountInOneTerm >= _options.TopOverCostCount)
        {
            // EndOverCostAction(currentTimeTick);

            // TODO: 在漂移终止，等待时间对齐时，如果发生时间回拨较长，则此处可能等待较长时间。可优化为：在漂移终止时增加时间回拨应对逻辑。（该情况发生概率很低）

            _lastTimeTick = GetNextTimeTick();
            _currentSeqNumber = _options.SequenceMinNumber;
            _isOverCost = false;
            _overCostCountInOneTerm = 0;
            _genCountInOneTerm = 0;

            return CalcId(_lastTimeTick);
        }

        if(_currentSeqNumber > _options.SequenceMaxNumber)
        {
            _lastTimeTick++;
            _currentSeqNumber = _options.SequenceMinNumber;
            _isOverCost = true;
            _overCostCountInOneTerm++;
            _genCountInOneTerm++;

            return CalcId(_lastTimeTick);
        }

        _genCountInOneTerm++;

        return CalcId(_lastTimeTick);
    }

    protected virtual long NextNormalId()
    {
        var currentTimeTick = GetCurrentTimeTick(_options.BaseTime);

        if(currentTimeTick < _lastTimeTick)
        {
            if(_turnbackTimeTick < 1)
            {
                _turnbackTimeTick = _lastTimeTick - 1;
                _turnbackIndex++;

                // 每毫秒序列数的前5位是预留位，0用于手工新值，1-4是时间回拨次序
                // 支持4次回拨次序（避免回拨重叠导致ID重复），可无限次回拨（次序循环使用）。
                if(_turnbackIndex > 4)
                {
                    _turnbackIndex = 1;
                }

                //BeginTurnBackAction(_TurnBackTimeTick);
            }

            //Thread.Sleep(1);
            return CalcTurnBackId(_turnbackTimeTick);
        }

        // 时间追平时，_TurnBackTimeTick清零
        if(_turnbackTimeTick > 0)
        {
            //EndTurnBackAction(_TurnBackTimeTick);
            _turnbackTimeTick = 0;
        }

        if(currentTimeTick > _lastTimeTick)
        {
            _lastTimeTick = currentTimeTick;
            _currentSeqNumber = _options.SequenceMinNumber;

            return CalcId(_lastTimeTick);
        }

        if(_currentSeqNumber > _options.SequenceMaxNumber)
        {
            //BeginOverCostAction(currentTimeTick);

            _termIndex++;
            _lastTimeTick++;
            _currentSeqNumber = _options.SequenceMinNumber;
            _isOverCost = true;
            _overCostCountInOneTerm = 1;
            _genCountInOneTerm = 1;

            return CalcId(_lastTimeTick);
        }

        return CalcId(_lastTimeTick);
    }

    protected virtual long CalcId(in long useTimeTick)
    {
        var result = (useTimeTick << _timestampShift)
            + ((long)_options.WorkerId << _options.SequenceBitLength)
            + _currentSeqNumber;

        _currentSeqNumber++;
        return result;
    }

    protected virtual long CalcTurnBackId(in long useTimeTick)
    {
        var result = (useTimeTick << _timestampShift)
            + ((long)_options.WorkerId << _options.SequenceBitLength)
            + _turnbackIndex;

        _turnbackTimeTick--;

        return result;
    }

    private static long GetCurrentTimeTick(DateTime baseTime)
    {
        return (DateTime.UtcNow - baseTime).Ticks / TimeSpan.TicksPerMillisecond;
    }

    protected virtual long GetNextTimeTick()
    {
        var tempTimeTicker = GetCurrentTimeTick(_options.BaseTime);

        while(tempTimeTicker <= _lastTimeTick)
        {
            tempTimeTicker = GetCurrentTimeTick(_options.BaseTime);
        }

        return tempTimeTicker;
    }

    private static readonly object _lock = new();

    public virtual long NextId()
    {
        lock(_lock)
        {
            return _isOverCost ? NextOverCostId() : NextNormalId();
        }
    }

    //private static long _StartTimeTick = 0;
    //private static long _BaseTimeTick = 0;

    //public Action<OverCostActionArg> GenAction { get; set; }

    //private void DoGenIdAction(OverCostActionArg arg)
    //{
    //    Task.Run(() =>
    //    {
    //        GenAction(arg);
    //    });
    //}

    //private void BeginOverCostAction(in long useTimeTick)
    //{
    //    return;

    //    if(GenAction == null)
    //    {
    //        return;
    //    }

    //    DoGenIdAction(new OverCostActionArg(
    //        WorkerId,
    //        useTimeTick,
    //        1,
    //        _OverCostCountInOneTerm,
    //        _GenCountInOneTerm,
    //        _TermIndex));
    //}

    //private void EndOverCostAction(in long useTimeTick)
    //{
    //    if(_TermIndex > 10000)
    //    {
    //        _TermIndex = 0;
    //    }
    //    return;

    //    if(GenAction == null)
    //    {
    //        return;
    //    }

    //    DoGenIdAction(new OverCostActionArg(
    //        WorkerId,
    //        useTimeTick,
    //        2,
    //        _OverCostCountInOneTerm,
    //        _GenCountInOneTerm,
    //        _TermIndex));
    //}

    //private void BeginTurnBackAction(in long useTimeTick)
    //{
    //    return;

    //    if(GenAction == null)
    //    {
    //        return;
    //    }

    //    DoGenIdAction(new OverCostActionArg(
    //    WorkerId,
    //    useTimeTick,
    //    8,
    //    0,
    //    0,
    //    _TurnBackIndex));
    //}

    //private void EndTurnBackAction(in long useTimeTick)
    //{
    //    return;

    //    if(GenAction == null)
    //    {
    //        return;
    //    }

    //    DoGenIdAction(new OverCostActionArg(
    //    WorkerId,
    //    useTimeTick,
    //    9,
    //    0,
    //    0,
    //    _TurnBackIndex));
    //}
}
