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

namespace OneF.Ormable.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyModel;

/// <summary>
/// Finds and loads SpatiaLite.
/// </summary>
/// <remarks>See Spatial data, and Accessing SQLite databases with EF Core for more information.</remarks>
internal static class SpatialiteLoader
{
    private static readonly string? _sharedLibraryExtension;

    private static readonly string? _pathVariableName;

    private static bool _looked;

    static SpatialiteLoader()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _sharedLibraryExtension = ".dll";
            _pathVariableName = "PATH";
        }
        else
        {
            _looked = true;
        }
    }

    /// <summary>
    /// Tries to load the mod_spatialite extension into the specified connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns>true if the extension was loaded; otherwise, false.</returns>
    public static bool TryLoad(DbConnection connection)
    {
        _ = Check.NotNull(connection);
        var flag = false;
        if(connection.State != ConnectionState.Open)
        {
            connection.Open();
            flag = true;
        }

        try
        {
            Load(connection);

            return true;
        }
        catch(SqliteException ex) when(ex.SqliteErrorCode == 1)
        {
            return false;
        }
        finally
        {
            if(flag)
            {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// Loads the mod_spatialite extension into the specified connection.
    /// The extension will be loaded from native NuGet assets when available.
    /// </summary>
    /// <param name="connection"></param>
    public static void Load(DbConnection connection)
    {
        _ = Check.NotNull(connection);

        FindExtension();

        if(connection is SqliteConnection sqliteConnection)
        {
            sqliteConnection.LoadExtension("mod_spatialite");
            return;
        }

        using var dbCommand = connection.CreateCommand();

        dbCommand.CommandText = "SELECT load_extension('mod_spatialite');";

        _ = dbCommand.ExecuteNonQuery();
    }

    private static void FindExtension()
    {
        if(_looked)
        {
            return;
        }

        bool flag;
        try
        {
            flag = DependencyContext.Default != null;
        }
        catch(Exception)
        {
            flag = false;
        }

        if(flag)
        {
            var dependencyContext = DependencyContext.Default!;

            var dictionary = new Dictionary<(string, string), int>();

            var rid = RuntimeInformation.RuntimeIdentifier;

            var list = dependencyContext.RuntimeGraph.FirstOrDefault((RuntimeFallbacks g) => g.Runtime == rid)?.Fallbacks.ToList() ?? new List<string>();

            list.Insert(0, rid);

            foreach(var runtimeLibrary in dependencyContext.RuntimeLibraries)
            {
                foreach(var nativeLibraryGroup in runtimeLibrary.NativeLibraryGroups)
                {
                    foreach(var runtimeFile in nativeLibraryGroup.RuntimeFiles)
                    {
                        if(string.Equals(Path.GetFileName(runtimeFile.Path), "mod_spatialite" + _sharedLibraryExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            var num = list.IndexOf(nativeLibraryGroup.Runtime);
                            if(num != -1)
                            {
                                dictionary.Add((runtimeLibrary.Path, runtimeFile.Path), num);
                            }
                        }
                    }
                }
            }

            var tuple = (from p in dictionary
                         orderby p.Value
                         select p.Key).FirstOrDefault();

            var tuple2 = tuple;

            if(tuple2.Item1 != null || tuple2.Item2 != null)
            {
                string? assetDirectory = null;
                if(File.Exists(Path.Combine(AppContext.BaseDirectory, tuple.Item2)))
                {
                    assetDirectory = Path.Combine(AppContext.BaseDirectory, Path.GetDirectoryName(tuple.Item2.Replace('/', Path.DirectorySeparatorChar))!);
                }
                else
                {
                    string? path = null;
                    var array = ((string)AppDomain.CurrentDomain.GetData("PROBING_DIRECTORIES")!).Split(Path.PathSeparator);
                    for(var i = 0; i < array.Length; i++)
                    {
                        var text = Path.Combine(array[i], (tuple.Item1 + "/" + tuple.Item2).Replace('/', Path.DirectorySeparatorChar));
                        if(File.Exists(text))
                        {
                            path = text;
                        }
                    }

                    assetDirectory = Path.GetDirectoryName(path);
                }

                var num2 = 1;
                var environmentVariable = Environment.GetEnvironmentVariable(_pathVariableName!);
                while(environmentVariable == null && num2 < 1000)
                {
                    Thread.Sleep(num2);
                    num2 *= 2;
                    environmentVariable = Environment.GetEnvironmentVariable(_pathVariableName!);
                }

                if(environmentVariable == null || !environmentVariable.Split(Path.PathSeparator).Any((string p) => string.Equals(p.TrimEnd(Path.DirectorySeparatorChar), assetDirectory, StringComparison.OrdinalIgnoreCase)))
                {
                    Environment.SetEnvironmentVariable(_pathVariableName!, assetDirectory + Path.PathSeparator + environmentVariable);
                }
            }
        }

        _looked = true;
    }
}
