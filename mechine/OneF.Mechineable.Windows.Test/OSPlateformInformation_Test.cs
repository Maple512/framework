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

namespace OneF.Mechineable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json;
using Shouldly;
using Xunit;

public class OSPlateformInformation_test
{
    [Fact]
    public void Get_information()
    {
        // 启动时间
        var startTime = OSPlateformInfomation.StartedTick;

        // 设备名称
        var mechineName = Environment.MachineName;

        //  Microsoft Windows NT 10.0.19044.0
        var os = Environment.OSVersion.ToString();

        // 操作系统 win10-x64
        var identifier = RuntimeInformation.RuntimeIdentifier;

        // 系统架构 X64
        var osArchitecture = RuntimeInformation.OSArchitecture;

        // NET框架 .NET 6.0.6
        var framework = RuntimeInformation.FrameworkDescription;

        // 处理器架构 X64
        var process = RuntimeInformation.ProcessArchitecture;
    }

    [Fact]
    public void Get_mac_address()
    {
        var result = GetNetworkInterface();

        result.ShouldNotBeNullOrWhiteSpace();
    }

    /// <summary>  
    /// 获取本机的物理地址  
    /// </summary>  
    /// <returns></returns>  
    [Fact]
    public static string GetNetworkInterface()
    {
        var adapters = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up
                                                && x.NetworkInterfaceType == NetworkInterfaceType.Ethernet);

        foreach(var item in adapters)
        {
            var id = item.Id;

            var type = item.NetworkInterfaceType;

            var mac = item.GetPhysicalAddress().GetAddressBytes()
                .Select(x => x.ToString("X"))
                .JoinAsString(':');

            var speed = item.Speed / 1000 / 1000;

            var status = item.OperationalStatus;

            var ipProperties = item.GetIPProperties();

            // dhcp 地址
            var dhcpServerAddress = ipProperties.DhcpServerAddresses[0];

            // 网关
            var gatewayAddress = ipProperties.GatewayAddresses[0];

            // dns
            var dnsAddress = ipProperties.DnsAddresses[0];

            var ipv4 = ipProperties.UnicastAddresses.FirstOrDefault(x => x.PrefixLength == 24);

            var ipv4Address = ipv4?.Address;

            var ipv4Mask = ipv4?.IPv4Mask;

            var ipv6 = ipProperties.UnicastAddresses.FirstOrDefault(x => x.PrefixLength == 64);

            var ipv6Address = ipv6?.Address;

            var ipv6Mask = ipv6?.IPv4Mask;

        }

        return null;
    }
}
