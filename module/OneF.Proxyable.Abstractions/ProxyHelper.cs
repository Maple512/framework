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
using System.Linq;
using System.Reflection;

namespace OneF.Proxyable;

public static class ProxyHelper
{
    private const string CastleProxyNamespace = "Castle.Proxies";
    private const string TargetFieldName = "_target";

    /// <summary>
    /// 获取代理对象的实例，如果不是代理对象，则返回给定对象本身
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object GetProxiedInstance(object obj)
    {
        var type = obj.GetType();
        if(type.Namespace != CastleProxyNamespace)
        {
            return obj;
        }

        var targetField = type
                          .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                          .FirstOrDefault(x => x.Name == TargetFieldName);

        if(targetField == null)
        {
            return obj;
        }

        return targetField.GetValue(obj)!;
    }

    /// <summary>
    /// 获取代理对象的类型，如果不是代理对象，则返回给定对象本身的类型
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Type GetProxiedInstanceType(object obj)
    {
        return GetProxiedInstance(obj).GetType();
    }
}
