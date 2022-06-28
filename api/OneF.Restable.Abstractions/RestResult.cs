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

namespace OneF.Restable;

using System;

[Serializable]
public class RestResult
{
    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public bool Success { get; init; }

    private static readonly RestResult _successed = new(true);

    public RestResult(bool success, string? errorCode = null, string? errorMessage = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Success = success;
    }

    public static RestResult Successed => _successed;

    public static RestResult Error(string? errorCode, string? errorMessage)
        => new(false, errorCode, errorMessage);
}

[Serializable]
public class RestResult<T> : RestResult
{
    public RestResult(T data, bool success, string? errorCode = null, string? errorMessage = null)
        : base(success, errorCode, errorMessage)
    {
        Data = data;
    }

    public T? Data { get; init; } = default;

    public static RestResult<T> WithSuccessed(T data)
    {
        return new(data, true);
    }
}
