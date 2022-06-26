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

namespace OneF.Shells;

using System;

public static class ShellHelper
{
    /*
     cmd 命令参数解析：https://www.cnblogs.com/mq0036/p/5244892.html
     */
    private const string _cmdPrefix = "/s /c";

    public static string CommandWrapper(string fileName, string args)
    {
        fileName = Check.NotNullOrWhiteSpace(fileName);

        if(UseCmd(fileName))
        {
            args = $"{_cmdPrefix} \"{ArgumentEscaper.EscapeAndConcatenateArgArrayForCmdProcessStart(args.Split(' '))}\"";
        }
        else
        {
            args = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args.Split(' '));
        }

        return args;
    }

    public static bool UseCmd(string fileName)
    {
        if(fileName.EndsWith("cmd", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return fileName.EndsWith("cmd.exe", StringComparison.OrdinalIgnoreCase);
    }
}
