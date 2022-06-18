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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace OneF.Proxyable;

public abstract class OneFMethodInvocationAdapterBase : IMethodInvocationContext
{
    private readonly Lazy<IReadOnlyDictionary<string, object>> _dictionary;

    protected OneFMethodInvocationAdapterBase(IInvocation invocation)
    {
        Invocation = invocation;
        _dictionary = new Lazy<IReadOnlyDictionary<string, object>>(GetArgumentDictionary());
    }

    protected IInvocation Invocation { get; }

    public object[] Arguments
    {
        get
        {
            return Invocation.Arguments;
        }
    }

    public IReadOnlyDictionary<string, object> ArgumentDictionay
    {
        get
        {
            return _dictionary.Value;
        }
    }

    public Type[] GeneicArguments
    {
        get
        {
            return Invocation.GenericArguments;
        }
    }

    public object TargetObject
    {
        get
        {
            return Invocation.InvocationTarget ?? Invocation.MethodInvocationTarget;
        }
    }

    public MethodInfo Method
    {
        get
        {
            return Invocation.MethodInvocationTarget ?? Invocation.Method;
        }
    }

    public object? ReturnValue { get; private set; }

    public abstract Task ProceedAsync();

    protected void SetReturnValue(object? value)
    {
        ReturnValue = value;
    }

    private IReadOnlyDictionary<string, object> GetArgumentDictionary()
    {
        return Method.GetParameters()
                     .Select(
                         (x, i) =>
                         {
                             return new KeyValuePair<string, object>(x.Name!, Invocation.Arguments[i]);
                         })
                     .ToDictionary(k => k.Key, v => v.Value);
    }
}
