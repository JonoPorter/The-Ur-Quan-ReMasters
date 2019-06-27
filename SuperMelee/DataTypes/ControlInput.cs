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
using AdvanceMath; using AdvanceSystem;

namespace ReMasters.SuperMelee
{
    [Serializable]
    public enum InputAction : int
    {
        None = 0,
        MoveForward = 1,
        MoveBackwards = 2,
        MoveLeft = 3,
        MoveRight = 4,
        RotateRight = 5,
        RotateLeft = 6,
        Action = 7,
    }
    [Serializable]
    public sealed class ControlInput
    {
        bool[] activeActions;
        public float ThrustPercent = 1;
        public float TorquePercent = 1;
        BitArray controls = new BitArray(8);
        public ControlInput(int numberofactions)
        {
            activeActions = new bool[numberofactions];
        }
        public bool this[InputAction action]
        {
            get
            {
                return controls[(int)action];
            }
            set
            {
                controls[(int)action] = value;
            }
        }
        public bool[] ActiveActions
        {
            get
            {
                return activeActions;
            }
            set
            {
                activeActions = value;
            }
        }
    }
}