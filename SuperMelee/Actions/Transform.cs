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
namespace ReMasters.SuperMelee.Actions
{
    [Serializable]
    public class NullTransformAction : NullAction
    {}
    [Serializable]
    public class TransformAction : BaseAction
    {
        IShip mirrorShip;
        ActionSounds mirrorActionSounds;
        IActionAIInfo mirrorAIInfo;
        Ship real;
        Ship other;
        Costs recosts;
        bool zeroVelocity;
        public TransformAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            ActionSounds mirrorActionSounds,
            Costs recosts,
            IShip mirrorShip,
            bool zeroVelocity)
            : base(delay, targetableTypes, costs, false, actionSounds)
        {
            this.mirrorActionSounds = mirrorActionSounds;
            this.mirrorShip = mirrorShip;
            this.recosts = recosts;
            this.zeroVelocity = zeroVelocity;
        }
        protected TransformAction(TransformAction copy)
            : base(copy)
        {
            this.mirrorActionSounds = copy.mirrorActionSounds; ;
            this.mirrorShip = (IShip)copy.mirrorShip; ;
            this.recosts = copy.recosts;
            this.zeroVelocity = copy.zeroVelocity;
        }
        public override void OnSourceCreation(GameResult gameResult, IShip source)
        {
            GameResult IgnoreResult = new GameResult();

            OnSourceCreation(gameResult, (Ship)source, (Ship)mirrorShip.Clone());
            other.OnCreation(IgnoreResult,real.FactionInfo);
            if (other.ControlHandler == null)
            {
                other.ControlHandler = real.ControlHandler;
            }
            other.ControlHandler.OnCreation(real);
            other.Actions.OnCreation(IgnoreResult, real);
            int actioncount = other.Actions.Count;
            for (int pos = 0; pos < actioncount; ++pos)
            {
                if (other.Actions[pos] is NullTransformAction)
                {
                    this.mirrorAIInfo = other.Actions[pos].AIInfo;
                    if (this.mirrorAIInfo != null)
                    {
                        this.mirrorAIInfo.OnSourceCreation(IgnoreResult, source, this);
                    }
                    other.Actions[pos] = this;
                    break;
                }
            }
            Functions.Swap<ActionSounds>(ref actionSounds, ref mirrorActionSounds);
        }
        public void OnSourceCreation(GameResult gameResult, Ship real, Ship other)
        {
            this.real = real;
            this.other = other;
            base.OnSourceCreation(gameResult,real);
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            Functions.Swap<Costs>(ref costs, ref recosts);
            Functions.Swap<ActionSounds>(ref actionSounds, ref mirrorActionSounds);
            Functions.Swap<IActionAIInfo>(ref aIInfo , ref mirrorAIInfo);
            real.Transform(other);
            if (zeroVelocity)
            {
                real.Current.Velocity.Linear = Vector2D.Zero;
            }
            return true;
        }
        public override object Clone()
        {
            return new TransformAction(this);
        }
    }
}