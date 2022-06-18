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

namespace OneF.Http;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

public static class OneFHttpContextExtensions
{
    private const string _xforwardedfor = "X-Forwarded-For";
    private const string _remoteAddr = "REMOTE_ADDR";

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tryUseXForwardedHeader"></param>
    /// <returns></returns>
    public static string? GetIPAddress(this HttpContext context, bool tryUseXForwardedHeader = true)
    {
        string? ip = null;

        if(tryUseXForwardedHeader && context.Request.Headers.TryGetValue(_xforwardedfor, out var xforwardedfor))
        {
            ip = xforwardedfor.ToString().Split(',').FirstOrDefault();
        }

        if(ip.IsNullOrWhiteSpace())
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }

        if(ip.IsNullOrWhiteSpace() && context.Request.Headers.TryGetValue(_remoteAddr, out var remoteAddr))
        {
            ip = remoteAddr;
        }

        return ip;
    }
}
