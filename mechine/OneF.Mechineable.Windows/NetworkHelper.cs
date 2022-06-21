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
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

public static class NetworkHelper
{
    public static IEnumerable<NetworkInformation> GetNetworks()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up
                                                && x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            .Where(x => !x.Description.Contains("Virtual", StringComparison.OrdinalIgnoreCase))
            .Select(x =>
            {
                return new NetworkInformation(x);
            });
    }
}

/// <summary>
/// 网卡信息
/// </summary>
[Serializable]
public class NetworkInformation
{
    public NetworkInformation(NetworkInterface data)
    {
        Id = data.Id;

        Type = data.NetworkInterfaceType;

        MACAddress = data.GetPhysicalAddress().GetAddressBytes()
            .Select(x => x.ToString("X2"))
            .JoinAsString(':');

        MaxSpeed = $"{data.Speed / 1000 / 1000}Mbps";

        Status = data.OperationalStatus;

        Description = data.Description;

        var ipProperties = data.GetIPProperties();

        if(ipProperties.DhcpServerAddresses.Any())
        {
            // dhcp 地址
            DHCPServer = ipProperties.DhcpServerAddresses[0].ToString();
        }

        if(ipProperties.GatewayAddresses.Any())
        {
            // 网关
            IPGateway = ipProperties.GatewayAddresses[0].Address.ToString();
        }

        if(ipProperties.DnsAddresses.Any())
        {
            // dns
            DNS = ipProperties.DnsAddresses[0].ToString();
        }

        var ipv4 = ipProperties.UnicastAddresses
            .FirstOrDefault(x => x.PrefixLength == 24);

        IPv4 = ipv4?.Address.ToString();

        IPv4Mask = ipv4?.IPv4Mask.ToString();

        var ipv6 = ipProperties.UnicastAddresses
            .FirstOrDefault(x => x.PrefixLength == 64);

        IPv6 = ipv6?.Address.ToString();
    }

    public string? Id { get; }

    /// <summary>
    /// 网卡类型
    /// </summary>
    public NetworkInterfaceType Type { get; }

    /// <summary>
    /// 最大速度
    /// </summary>
    public string MaxSpeed { get; }

    /// <summary>
    /// 状态
    /// </summary>
    public OperationalStatus Status { get; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// MAC 地址
    /// </summary>
    public string? MACAddress { get; }

    /// <summary>
    /// IPv4 DHCP 服务地址
    /// </summary>
    public string? DHCPServer { get; }

    /// <summary>
    /// IPv4
    /// </summary>
    public string? IPv4 { get; }

    /// <summary>
    /// IPv4 子网掩码
    /// </summary>
    public string? IPv4Mask { get; }

    /// <summary>
    /// 默认网关
    /// </summary>
    public string? IPGateway { get; }

    /// <summary>
    /// IPv6
    /// </summary>
    public string? IPv6 { get; }

    /// <summary>
    /// IPv4 DNS 地址
    /// </summary>
    public string? DNS { get; }
}
