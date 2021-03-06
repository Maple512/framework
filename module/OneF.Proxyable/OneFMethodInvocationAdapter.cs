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

using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace OneF.Proxyable;

public class OneFMethodInvocationAdapter : OneFMethodInvocationAdapterBase
{
    public OneFMethodInvocationAdapter(
    IInvocation invocation,
    IInvocationProceedInfo proceedInfo,
    Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        : base(invocation)
    {
        ProceedInfo = proceedInfo;
        Proceed = proceed;
    }

    protected IInvocationProceedInfo ProceedInfo { get; }

    protected Func<IInvocation, IInvocationProceedInfo, Task> Proceed { get; }

    public override async Task ProceedAsync()
    {
        await Proceed(Invocation, ProceedInfo);
    }
}

public class OneFMethodInvocationAdapter<TResult> : OneFMethodInvocationAdapterBase
{
    public OneFMethodInvocationAdapter(
    IInvocation invocation,
    IInvocationProceedInfo proceedInfo,
    Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        : base(invocation)
    {
        ProceedInfo = proceedInfo;
        Proceed = proceed;
    }

    protected IInvocationProceedInfo ProceedInfo { get; }

    protected Func<IInvocation, IInvocationProceedInfo, Task<TResult>> Proceed { get; }

    public override async Task ProceedAsync()
    {
        SetReturnValue(await Proceed(Invocation, ProceedInfo));
    }
}
