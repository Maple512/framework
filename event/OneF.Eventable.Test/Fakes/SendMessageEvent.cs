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

namespace OneF.Eventable.Fakes;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OneF.Moduleable.DependencyInjection;
using Shouldly;

public class SendMessageEvent : IEventData
{
    public SendMessageEvent(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

public class SendMessageHandler1 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override Task HandlerAsync(SendMessageEvent data, CancellationToken cancellationToken = default)
    {
        data.Message.ShouldNotBeNullOrEmpty();

        Debug.WriteLine($"1: {DateTime.Now:mm:ss.fffffff}");

        return Task.CompletedTask;
    }
}

public class SendMessageHandler2 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override Task HandlerAsync(SendMessageEvent data, CancellationToken cancellationToken = default)
    {
        data.Message.ShouldNotBeNullOrEmpty();

        Debug.WriteLine($"2: {DateTime.Now:mm:ss.fffffff}");

        return Task.CompletedTask;
    }
}

public class SendMessageHandler3 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override Task HandlerAsync(SendMessageEvent data, CancellationToken cancellationToken = default)
    {
        data.Message.ShouldNotBeNullOrEmpty();

        Debug.WriteLine($"3: {DateTime.Now:mm:ss.fffffff}");

        return Task.CompletedTask;
    }
}
