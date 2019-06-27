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
    [Serializable, Flags]
    public enum TargetingTypes : int
    {
        None = 0,
        Neutral = 1 << 0,
        Enemy = 1 << 1,
        Ally = 1 << 2,
        Self = 1 << 3,
        Other = 1 << 4,
        Debris = 1 << 5,
        Planets = 1 << 6,
        Weapon = 1 << 7,
        Ship = 1 << 8,
        SubShip = 1 << 9,
        All = Self | Other,
    }
}