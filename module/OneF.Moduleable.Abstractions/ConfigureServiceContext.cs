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

namespace OneF.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 配置服务容器上下文
/// </summary>
public readonly struct ConfigureServiceContext
{
    public ConfigureServiceContext(
        IServiceCollection services,
        IConfiguration configuration)
    {
        Services = Check.NotNull(services);
        Configuration = Check.NotNull(configuration);
    }

    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Services, Configuration);
    }

    public override string ToString()
    {
        return $"ConfigureServiceContext: {Services}, {Configuration}";
    }
}
