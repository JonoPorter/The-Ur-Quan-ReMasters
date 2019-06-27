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
using System.Collections.Generic;
using System.Text;
using Physics2D;
namespace ReMasters.SuperMelee
{
    [Serializable,Flags]
    public enum EffectTypes : int
    {
        None = 0,
        HealthSteal = 1 << 1,
        Health = 1 << 2,
        Energy = 1 << 3,
        HealthChangeRate = 1 << 4,
        EnergyChangeRate = 1 << 5,
        Position = 1 << 6,
        Velocity = 1 << 7,
        MaxAngularAcceleration = 1 << 8,
        MaxAngularVelocity = 1 << 9,
        MaxLinearAcceleration = 1 << 10,
        MaxLinearVelocity = 1 << 11,
        Controls = 1 << 12,
        MovementInfoMask = MaxAngularAcceleration | MaxAngularVelocity | MaxLinearAcceleration | MaxLinearVelocity,
        ShipStateMask = Health | Energy | HealthChangeRate | EnergyChangeRate,
    }
}