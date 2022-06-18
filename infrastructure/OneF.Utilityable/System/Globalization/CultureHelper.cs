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

namespace System.Globalization;

using System;
using OneF;

public static class CultureHelper
{
    public static IDisposable Use(string culture, string? uiCulture = null)
    {
        _ = Check.NotNull(culture);

        return Use(
            CultureInfo.GetCultureInfo(culture),
            uiCulture == null
                ? null
                : CultureInfo.GetCultureInfo(uiCulture)
        );
    }

    public static IDisposable Use(CultureInfo culture, CultureInfo? uiCulture = null)
    {
        _ = Check.NotNull(culture);

        if(CultureInfo.CurrentCulture == culture && CultureInfo.CurrentUICulture == uiCulture)
        {
            return DisposeAction.Nullable;
        }

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = uiCulture ?? culture;

        return new DisposeAction(
            () =>
            {
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentUiCulture;
            });
    }

    public static bool IsValidCultureCode(string cultureCode)
    {
        if(cultureCode.IsNullOrWhiteSpace())
        {
            return false;
        }

        try
        {
            _ = CultureInfo.GetCultureInfo(cultureCode);
            return true;
        }
        catch(CultureNotFoundException)
        {
            return false;
        }
    }

    public static CultureInfo GetParentCulture(string cultureName)
    {
        _ = Check.NotNullOrWhiteSpace(cultureName);

        var currentCulture = CultureInfo.GetCultureInfo(cultureName);

        return GetParentCulture(currentCulture);
    }

    public static CultureInfo GetParentCulture(CultureInfo culture)
    {
        _ = Check.NotNull(culture);

        var parentCulture = culture;

        do
        {
            if(!parentCulture.Parent.Name.IsNullOrEmpty())
            {
                parentCulture = parentCulture.Parent;
            }
            else
            {
                return parentCulture;
            }
        } while(true);
    }
}
