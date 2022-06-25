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

namespace OneF.Interop;

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libc
    {
        [LibraryImport("libc.dll", EntryPoint = "kill", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int Kill(int pid, int sig);
    }
}
