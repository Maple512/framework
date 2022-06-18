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

namespace OneF.Ormable.Database.Command;

using System.Collections.Generic;
using System.Data;

public class ParameterInfoCollection : List<ParameterInfo>
{
    public ParameterInfoCollection()
    {
    }

    public ParameterInfoCollection(int capacity) : base(capacity)
    {
    }

    public ParameterInfoCollection(IEnumerable<ParameterInfo> collection) : base(collection)
    {
    }
}

public static class ParameterInfoCollectionExtensions
{
    public static ParameterInfoCollection Add<T>(
        this ParameterInfoCollection parameters,
        string name,
        T? value)
    {
        parameters.Add(new ParameterInfo(name, value));

        return parameters;
    }

    public static ParameterInfoCollection Add<T>(
        this ParameterInfoCollection parameters,
        string name,
        T? value,
        byte? precision = null,
        byte? scale = null,
        DbType? dbType = null,
        int? size = null)
    {
        parameters.Add(new ParameterInfo(name, value, precision, scale, dbType, size));

        return parameters;
    }
}
