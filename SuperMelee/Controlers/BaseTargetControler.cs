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
    public abstract class BaseTargetControler : BaseControler
    {
        protected IControlable target;
        protected TargetingInfo targetingInfo;
        public BaseTargetControler(TargetingInfo targetingInfo)
        {
            this.targetingInfo = targetingInfo;
            this.isFireAndForget = true;
        }
        protected BaseTargetControler(BaseTargetControler copy)
            : base(copy)
        {
            this.targetingInfo = copy.targetingInfo;
            //this.targetingTypes = copy.targetingTypes;
        }
        public IControlable Target
        {
            get { return target; }
            set { target = value; }
        }
        public TargetingInfo TargetingInfo
        {
            get { return targetingInfo; }
        }
        public bool IsCurrentTargetInvalid
        {
            get
            {
                return 
                    target == null ||
                    !target.IsTargetable ||
                    !targetingInfo.MeetsRequirements(FactionInfo.GetTargetingType(host, target));
            }
        }
        protected virtual bool CheckTarget()
        {
            if (IsCurrentTargetInvalid)
            {
                target = host.TargetRetriever.NextClosest(targetingInfo,this.host.Current.Position.Linear);
                host.Target = target;
                return IsCurrentTargetInvalid;
            }
            return false;
        }
        protected abstract float GetDesiredAngle();
        float GetAngleDifference()
        {
            return MathHelper.GetAngleDifference(GetDesiredAngle(), host.DirectionAngle);
        }
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {
            if (dt == 0||this.host==null||CheckTarget())
            {
                return original;
            }
            float Velocity = host.Current.Velocity.Angular;
            float AbsVelocity = MathHelper.Abs(Velocity);
            if (AbsVelocity > host.MovementInfo.MaxLinearVelocity)
            {
                return original;
            }
            else
            {
                float Accel = host.MovementInfo.MaxAngularAcceleration * dt;
                float DifferenceA = GetAngleDifference();
                float breakpoint = (AbsVelocity * AbsVelocity) / (2 * host.MovementInfo.MaxAngularAcceleration.Value);
                float AbsDifferenceA = MathHelper.Abs(DifferenceA);
                if (AbsDifferenceA > breakpoint)
                {
                    if (DifferenceA > 0)
                    {
                        original[InputAction.RotateLeft] = true;
                    }
                    else
                    {
                        original[InputAction.RotateRight] = true;
                    }
                    original.TorquePercent = 1;
                    original[InputAction.MoveForward] = true;
                    original.ThrustPercent = 1;
                    return original;
                }
                else
                {
                    float dv = Velocity;
                    original.TorquePercent = 1;
                    original[InputAction.MoveForward] = true;
                    original.ThrustPercent = .5f;
                    if (AbsVelocity < Accel)
                    {
                        original.TorquePercent = AbsVelocity / Accel;
                        original.ThrustPercent += (1 - original.TorquePercent) * .5f;
                    }
                    if (0 < -dv)
                    {
                        original[InputAction.RotateLeft] = true;
                    }
                    else
                    {
                        original[InputAction.RotateRight] = true;
                    }
                }
            }
            return original;
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            this.target = host.Target;
            base.OnCreation(gameResult,host);
            CheckTarget();
        }
    }
}