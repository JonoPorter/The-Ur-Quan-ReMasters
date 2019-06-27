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
using System.Collections;
using System.Collections.Generic;
using Physics2D;
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public sealed class GeneralChange
    {
        public ShipMovementInfoChange MovementInfoChange;
        public PhysicsState PhysicsChange;
        public ShipStateChange ShipStateChange;
        public void Merge(GeneralChange other)
        {
            if (other != null)
            {
                if (other.MovementInfoChange != null)
                {
                    if (MovementInfoChange == null)
                    {
                        MovementInfoChange = other.MovementInfoChange;
                    }
                    else
                    {
                        MovementInfoChange.Merge(other.MovementInfoChange);
                    }
                }
                if (other.ShipStateChange != null)
                {
                    if (ShipStateChange == null)
                    {
                        ShipStateChange = other.ShipStateChange;
                    }
                    else
                    {
                        ShipStateChange.Merge(other.ShipStateChange);
                    }
                }
                if (other.PhysicsChange != null)
                {
                    if (PhysicsChange == null)
                    {
                        PhysicsChange = other.PhysicsChange;
                    }
                    else
                    {
                        PhysicsChange.Velocity += other.PhysicsChange.Velocity;
                        PhysicsChange.Position += other.PhysicsChange.Position;
                        PhysicsChange.ForceAccumulator += other.PhysicsChange.ForceAccumulator;
                        PhysicsChange.Acceleration += other.PhysicsChange.Acceleration;
                    }
                }
            }
        }
    }


}