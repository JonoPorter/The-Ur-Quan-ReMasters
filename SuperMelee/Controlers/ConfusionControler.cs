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
using Physics2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Controlers
{
    [Serializable]
    public class ConfusionControler : BaseControler
    {
        static Random rand = new Random();
        protected InputAction direction;
        public ConfusionControler(LifeSpan lifeTime)
        {
            this.lifeTime = lifeTime;
            if ((rand.NextDouble() - .5f) > 0)
            {
                direction = InputAction.RotateRight;
            }
            else
            {
                direction = InputAction.RotateLeft;
            }
        }
        protected ConfusionControler(ConfusionControler copy)
            : base(copy)
        {
            this.lifeTime = (LifeSpan)copy.lifeTime.Clone();
            this.direction = copy.direction;
        }
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {
            original[InputAction.RotateRight] = false;
            original[InputAction.RotateLeft] = false;
            original[direction] = true;
            original.TorquePercent = 1;
            return original;
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            this.host = host;
        }
        public override object Clone()
        {
            return new ConfusionControler(this);
        }
    }
}