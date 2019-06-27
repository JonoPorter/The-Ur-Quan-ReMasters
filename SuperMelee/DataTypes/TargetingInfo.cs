#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public struct TargetingInfo : IEquatable<TargetingInfo>
    {


        public static readonly TargetingInfo None = new TargetingInfo();
        public static readonly TargetingInfo All = new TargetingInfo(TargetingTypes.All, TargetingTypes.None, TargetingTypes.None);
        public static readonly TargetingInfo Self = new TargetingInfo(TargetingTypes.Self, TargetingTypes.Self, TargetingTypes.None);
        public static readonly TargetingInfo Enemy = new TargetingInfo(TargetingTypes.Enemy, TargetingTypes.None, TargetingTypes.None);

        public static TargetingInfo operator &(TargetingInfo first, TargetingInfo second)
        {
            TargetingInfo returnvalue = new TargetingInfo();
            returnvalue.RequireAll = first.RequireAll & second.RequireAll;
            returnvalue.ExcludeAny = first.ExcludeAny & second.ExcludeAny;
            returnvalue.RequireAny = first.RequireAny & second.RequireAny;
            return returnvalue;
        }
        public static TargetingInfo operator |(TargetingInfo first, TargetingInfo second)
        {
            TargetingInfo returnvalue = new TargetingInfo();
            returnvalue.RequireAll = first.RequireAll | second.RequireAll;
            returnvalue.ExcludeAny = first.ExcludeAny | second.ExcludeAny;
            returnvalue.RequireAny = first.RequireAny | second.RequireAny;
            return returnvalue;
        }
        public static TargetingInfo Merge(TargetingInfo first, TargetingInfo second)
        {
            TargetingInfo returnvalue = new TargetingInfo();
            if (first.isValid && second.isValid)
            {
                returnvalue.isValid = true;
                returnvalue.RequireAll = first.RequireAll & second.RequireAll;
                returnvalue.RequireAny = first.RequireAny | second.RequireAny | first.RequireAll | second.RequireAll;
                returnvalue.ExcludeAny = (first.ExcludeAny | second.ExcludeAny) & ~returnvalue.RequireAny;
            }
            else if (first.isValid)
            {
                returnvalue = first;
            }
            else
            {
                returnvalue = second;
            }
            return returnvalue;
        }

        public static TargetingInfo FromRequireAll(TargetingTypes RequireAll)
        {
            TargetingInfo rv = new TargetingInfo();
            rv.RequireAll = RequireAll;
            return rv;
        }
        public static TargetingInfo FromRequireAny(TargetingTypes RequireAny)
        {
            TargetingInfo rv = new TargetingInfo();
            rv.RequireAny = RequireAny;
            return rv;
        }
        public static TargetingInfo FromExcludeAny(TargetingTypes ExcludeAny)
        {
            TargetingInfo rv = new TargetingInfo();
            rv.ExcludeAny = ExcludeAny;
            return rv;
        }
        
        public TargetingTypes RequireAll;
        public TargetingTypes ExcludeAny;
        public TargetingTypes RequireAny;
        bool isValid;
        public TargetingInfo(TargetingTypes RequireAny, TargetingTypes RequireAll, TargetingTypes ExcludeAny)
        {
            this.RequireAny = RequireAny;
            this.RequireAll = RequireAll;
            this.ExcludeAny = ExcludeAny;
            this.isValid = true;
        }
        public TargetingInfo(TargetingTypes RequireAny)
        {
            this.RequireAny = RequireAny;
            this.RequireAll = TargetingTypes.None;
            this.ExcludeAny = TargetingTypes.None;
            this.isValid = true;
        }
        public bool MeetsRequirements(TargetingTypes targetingTypes)
        {
            return
                ((ExcludeAny & targetingTypes) == TargetingTypes.None) &&
                ((RequireAll & targetingTypes) == RequireAll) &&
                ((RequireAny == TargetingTypes.None) ||
                ((RequireAny & targetingTypes) != TargetingTypes.None));
        }
        public bool IsAll
        {
            get
            {
                return this == All;
            }
        }
        public bool IsSelf
        {
            get
            {
                return ((RequireAny & TargetingTypes.Self) == TargetingTypes.Self) ||
                    RequireAll == TargetingTypes.Self;
            }
        }


        public bool Equals(TargetingInfo other)
        {
            return this == other;
        }
        public static bool operator ==(TargetingInfo left,TargetingInfo right)
        {
            return 
                left.ExcludeAny == right.ExcludeAny&&
                left.RequireAll == right.RequireAll&&
                left.RequireAny == right.RequireAny;
        }
        public static bool operator !=(TargetingInfo left, TargetingInfo right)
        {
            return !(left == right);
        }
    }
}