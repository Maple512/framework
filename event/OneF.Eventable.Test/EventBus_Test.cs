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

namespace OneF.Eventable;

using System.Threading.Tasks;
using OneF.Eventable.Fakes;
using Shouldly;
using Xunit;

public class EventBus_Test : EventableTestBase
{
    private readonly IEventBus _eventBus;

    public EventBus_Test()
    {
        _eventBus = GetRequiredService<IEventBus>();
    }

    [Fact]
    public void Try_subscribe_event_data()
    {
        _eventBus.TrySubscribe<SendMessageEvent>().ShouldBeTrue();

        _eventBus.TrySubscribe<SendMessageEvent>().ShouldBeFalse();
    }

    [Fact]
    public void Trigger_send_message_event()
    {
        var msg = nameof(SendMessageEvent);

        Should.NotThrow(async () => await _eventBus.PublishAsync(new SendMessageEvent(msg)));
    }

    [Theory]
    [InlineData(100)]
    public async Task Trigger_event_callback(int num)
    {
        var eventData = new AutoincrementEvent
        {
            Number = num,
        };

        await foreach(var item in _eventBus.PublishAsync<AutoincrementEvent, int>(eventData))
        {
            item.ShouldBe(++num);
        }
    }
}
