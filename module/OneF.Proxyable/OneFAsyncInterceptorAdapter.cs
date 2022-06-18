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

public class OneFAsyncInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
    where TInterceptor : IOneFInterceptor
{
    private readonly TInterceptor _interceptor;

    public OneFAsyncInterceptorAdapter(TInterceptor interceptor)
    {
        _interceptor = interceptor;
    }

    protected override async Task InterceptAsync(
    IInvocation invocation,
    IInvocationProceedInfo proceedInfo,
    Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        var adapter = new OneFMethodInvocationAdapter(invocation, proceedInfo, proceed);

        await _interceptor.InterceptAsync(adapter);
    }

    protected override async Task<TResult> InterceptAsync<TResult>(
    IInvocation invocation,
    IInvocationProceedInfo proceedInfo,
    Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        var adapter = new OneFMethodInvocationAdapter<TResult>(invocation, proceedInfo, proceed);

        await _interceptor.InterceptAsync(adapter);

        return (TResult)adapter.ReturnValue!;
    }
}
