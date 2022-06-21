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
using System.Management;
using Xunit;

public class CPUHelper_Test
{
    /// <summary>
    /// 内核
    /// </summary>
    [Fact]
    public void Win32_processor()
    {
        using var manage = new ManagementObjectSearcher("Select * from Win32_Processor").Get();

        foreach(var item in manage)
        {
            Debug.WriteLine($"******************************************************");

            foreach(var property in item.Properties)
            {
                if(property.IsArray
                    && property.Value is not null)
                {
                    Debug.WriteLine($"{property.Name}: {(property.Value as IEnumerable)!.JoinAsString()}, {property.Type}");
                }
                else
                {
                    Debug.WriteLine($"{property.Name}: {property.Value}, {property.Type}");
                }
            }

            Debug.WriteLine($"******************************************************");
        }

        var enumerator = manage.GetEnumerator();
        enumerator.MoveNext();

        var processor = enumerator.Current;

        Debug.WriteLine($"名称：{processor["Caption"]}");
        Debug.WriteLine($"版本：{processor["Name"]}");
        Debug.WriteLine($"设备ID：{processor["DeviceID"]}");
        Debug.WriteLine($"制造商：{processor["Manufacturer"]}");
        Debug.WriteLine($"插座型号：{processor["SocketDesignation"]}");
        Debug.WriteLine($"基准速度：{Convert.ToUInt32(processor["MaxClockSpeed"]) / 1000d:N2}GHz");
        Debug.WriteLine($"当前速度：{Convert.ToUInt32(processor["CurrentClockSpeed"]) / 1000d:N2}GHz");
        Debug.WriteLine($"处理器ID：{processor["ProcessorId"]}");
        Debug.WriteLine($"处理器类型：{processor["ProcessorType"]}");
        Debug.WriteLine($"L2缓存：{Convert.ToUInt32(processor["L2CacheSize"]) / 1024d:N2}MB");
        Debug.WriteLine($"L3缓存：{Convert.ToUInt32(processor["L3CacheSize"]) / 1024d:N2}MB");
        Debug.WriteLine($"核心数：{processor["NumberOfCores"]}");
        Debug.WriteLine($"启用核心数：{processor["NumberOfEnabledCore"]}");
        Debug.WriteLine($"逻辑处理器：{processor["NumberOfLogicalProcessors"]}");
        Debug.WriteLine($"线程数：{Convert.ToUInt32(processor["ThreadCount"])}");
    }

    /// <summary>
    /// 物理
    /// </summary>
    [Fact]
    public void Win32_computer()
    {
        using var manage = new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get();

        foreach(var item in manage)
        {
            Debug.WriteLine($"******************************************************");

            foreach(var property in item.Properties)
            {
                if(property.IsArray
                    && property.Value is not null)
                {
                    Debug.WriteLine($"{property.Name}: {(property.Value as IEnumerable)!.JoinAsString()}, {property.Type}");
                }
                else
                {
                    Debug.WriteLine($"{property.Name}: {property.Value}, {property.Type}");
                }
            }

            Debug.WriteLine($"******************************************************");
        }

        //var enumerator = manage.GetEnumerator();
        //enumerator.MoveNext();

        //var processor = enumerator.Current;

        //Debug.WriteLine($"名称：{processor["Caption"]}");
        //Debug.WriteLine($"版本：{processor["Name"]}");
        //Debug.WriteLine($"设备ID：{processor["DeviceID"]}");
        //Debug.WriteLine($"制造商：{processor["Manufacturer"]}");
        //Debug.WriteLine($"插座型号：{processor["SocketDesignation"]}");
        //Debug.WriteLine($"基准速度：{Convert.ToUInt32(processor["MaxClockSpeed"]) / 1000d:N2}GHz");
        //Debug.WriteLine($"当前速度：{Convert.ToUInt32(processor["CurrentClockSpeed"]) / 1000d:N2}GHz");
        //Debug.WriteLine($"处理器ID：{processor["ProcessorId"]}");
        //Debug.WriteLine($"处理器类型：{processor["ProcessorType"]}");
        //Debug.WriteLine($"L2缓存：{Convert.ToUInt32(processor["L2CacheSize"]) / 1024d:N2}MB");
        //Debug.WriteLine($"L3缓存：{Convert.ToUInt32(processor["L3CacheSize"]) / 1024d:N2}MB");
        //Debug.WriteLine($"核心数：{processor["NumberOfCores"]}");
        //Debug.WriteLine($"启用核心数：{processor["NumberOfEnabledCore"]}");
        //Debug.WriteLine($"逻辑处理器：{processor["NumberOfLogicalProcessors"]}");
        //Debug.WriteLine($"线程数：{Convert.ToUInt32(processor["ThreadCount"])}");
    }
}
