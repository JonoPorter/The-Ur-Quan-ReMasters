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
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public abstract class BaseControlHandler : BaseTimed,IControlHandler
    {
        protected IControlable host;
        public BaseControlHandler() { }
        public BaseControlHandler(BaseControlHandler copy) { }
        public abstract void HandleControlInput(float dt);
        public IControlable Host
        {
            get { return host; }
        }
        public virtual void OnCreation(IControlable host)
        {
            this.host = host;
            this.lifeTime = new LifeSpan(host.LifeTime);
        }
        public abstract object Clone();
    }
    [Serializable]
    public class DefaultControlHandler : BaseControlHandler
    {
        public static void UpdateAngularMovement(float dt, IControlable host)
        {
            if (dt == 0)
            {
                return;
            }
            ControlInput input = host.CurrentControlInput;
            
            float AV = host.Current.Velocity.Angular;
            float AAccel = host.MovementInfo.MaxAngularAcceleration.Value * dt;
            float AbsAV = MathHelper.Abs(AV);

            bool right = false;
            bool left = false;
            if (input != null)
            {
                right = input[InputAction.RotateRight];
                left = input[InputAction.RotateLeft];
            }
            bool thirdtest = (AbsAV <= host.MovementInfo.MaxAngularVelocity);
            if ((right ^ left) && thirdtest)
            {
                float newAV;
                AAccel *= input.TorquePercent;
                if (left)
                {
                    newAV = AV + AAccel;
                    if (newAV < host.MovementInfo.MaxAngularVelocity)
                    {
                        //host.current.Velocity.Angular = newAV;
                        ApplydAV(host, AAccel, dt);
                    }
                    else
                    {
                        //host.current.Velocity.Angular = host.MovementInfo.MaxAngularVelocity;
                        ApplydAV(host, host.MovementInfo.MaxAngularVelocity - AV, dt);
                    }
                }
                else
                {
                    newAV = AV - AAccel;
                    if (newAV > -host.MovementInfo.MaxAngularVelocity)
                    {
                        //host.current.Velocity.Angular = newAV;
                        ApplydAV(host, -AAccel, dt);
                    }
                    else
                    {
                        //host.current.Velocity.Angular = -host.MovementInfo.MaxAngularVelocity;
                        ApplydAV(host, -host.MovementInfo.MaxAngularVelocity - AV, dt);
                    }
                }
            }
            else
            {
                if (AbsAV <= AAccel)
                {
                    //host.current.Velocity.Angular = 0;
                    ApplydAV(host, 0 - AV, dt);
                }
                else
                {
                    //host.current.Velocity.Angular -= (float)Math.Sign(AV) * AAccel;
                    ApplydAV(host, -(float)Math.Sign(AV) * AAccel, dt);
                }
            }
        }
        public static void UpdateLinearMovement(float dt, IControlable host)
        {
            if (dt == 0)
            {
                return;
            }
            ControlInput input = host.CurrentControlInput;
            if (input == null)
            {
                return;
            }
            if (input[InputAction.MoveForward])
            {
                ApplyThrust(dt, host, host.DirectionVector, input.ThrustPercent);
            }
            else if (input[InputAction.MoveBackwards])
            {
                ApplyThrust(dt, host, -host.DirectionVector, input.ThrustPercent);
            }
            else if (input[InputAction.MoveLeft])
            {
                ApplyThrust(dt, host, host.DirectionVector.LeftHandNormal, input.ThrustPercent);
            }
            else if (input[InputAction.MoveRight])
            {
                ApplyThrust(dt, host, host.DirectionVector.RightHandNormal, input.ThrustPercent);
            }
        }
        public static void ApplyThrust(float dt, IControlable host, Vector2D direction, float ThrustPercent)
        {
            //Increase Velocity in direction.
            float LAccel = host.MovementInfo.MaxLinearAcceleration * dt * ThrustPercent;
            if ((host.UQMFlags & ContFlags.NoMaxSpeed) == ContFlags.NoMaxSpeed)
            {
                ApplydLV(host, LAccel * direction, dt);
            }
            else
            {
                float VelocityInDirection = host.Current.Velocity.Linear * direction;
                if (VelocityInDirection < host.MovementInfo.MaxLinearVelocity)
                {
                    float newLV = VelocityInDirection + LAccel;
                    if (newLV > host.MovementInfo.MaxLinearVelocity)
                    {
                        LAccel = host.MovementInfo.MaxLinearVelocity - VelocityInDirection;
                    }
                    ApplydLV(host, LAccel * direction, dt);
                }
            }

            // now reduce movement sideways to the direction.
            Vector2D tanget = direction.RightHandNormal;
            float SidewaysVelocity = host.Current.Velocity.Linear * tanget;
            if (MathHelper.Abs(SidewaysVelocity) < LAccel)
            {

                //host.current.Velocity.Linear -= SidewaysVelocity * tanget;
                ApplydLV(host, -SidewaysVelocity * tanget, dt);
            }
            else
            {
                if (SidewaysVelocity > 0)
                {
                    ApplydLV(host, -LAccel * tanget, dt);
                }
                else
                {
                    ApplydLV(host, LAccel * tanget, dt);
                }
            }
        }
        public static void ApplydLV(IControlable host, Vector2D dv, float dt)
        {
            Vector2D force = dv *(host.MassInfo.Mass / dt);
            host.Current.ForceAccumulator.Linear  += force;
            //host.Current.Velocity.Linear += dv;
        }
        public static void ApplydAV(IControlable host, float dv, float dt)
        {
            float torque = dv * (host.MassInfo.MomentofInertia / dt);
            host.Current.ForceAccumulator.Angular += torque;
            //host.Current.Velocity.Angular += dv;

        }
        public DefaultControlHandler()
        { }
        public DefaultControlHandler(DefaultControlHandler copy):base(copy)
        { }
        public override void HandleControlInput(float dt)
        {
            UpdateLinearMovement(dt, host);
            UpdateAngularMovement(dt, host);
        }

        public override object Clone()
        {
            return new DefaultControlHandler(this);
        }
    }
    [Serializable]
    public class ComplexShipControlHandler : BaseControlHandler
    {
        public static void SetSubShipsVelocity(IShip source, ALVector2D velocity)
        {
            foreach (IShip ship in source.SubShips)
            {
                if (ship.CurrentControlInput == null)
                {
                    ship.Current.Velocity = velocity;
                    ship.Current.ForceAccumulator.Linear = source.Current.ForceAccumulator.Linear * (ship.MassInfo.Mass * source.MassInfo.MassInv);
                    ship.Current.ForceAccumulator.Angular = source.Current.ForceAccumulator.Angular * (ship.MassInfo.MomentofInertia * source.MassInfo.MomentofInertiaInv);
                }
            }
        }
        protected IShip trueHost;
        public ComplexShipControlHandler()
        { }
        public ComplexShipControlHandler(ComplexShipControlHandler copy):base(copy)
        { }
        public IShip TrueSource
        {
            get { return trueHost; }
        }
        public override void HandleControlInput(float dt)
        {
            DefaultControlHandler.UpdateLinearMovement(dt, host);
            DefaultControlHandler.UpdateAngularMovement(dt, host);
            SetSubShipsVelocity(trueHost, host.Current.Velocity);
        }
        public override void OnCreation(IControlable host)
        {
            this.host = host;
            this.trueHost = (IShip)host;
            this.lifeTime = new LifeSpan(host.LifeTime);
        }
        public override object Clone()
        {
            return new ComplexShipControlHandler(this);
        }
    }
    [Serializable]
    public class InertialessControlHandler : BaseControlHandler
    {
        public static void UpdateLinearMovement(float dt, IControlable host)
        {
            if (dt == 0)
            {
                return;
            }
            ControlInput input = host.CurrentControlInput;
            if (input != null)
            {
                if (input[InputAction.MoveForward])
                {
                    DefaultControlHandler.ApplyThrust(dt, host, host.DirectionVector, input.ThrustPercent);
                }
                else if (input[InputAction.MoveBackwards])
                {
                    DefaultControlHandler.ApplyThrust(dt, host, -host.DirectionVector, input.ThrustPercent);
                }
                else if (input[InputAction.MoveLeft])
                {
                    DefaultControlHandler.ApplyThrust(dt, host, host.DirectionVector.LeftHandNormal, input.ThrustPercent);
                }
                else if (input[InputAction.MoveRight])
                {
                    DefaultControlHandler.ApplyThrust(dt, host, host.DirectionVector.RightHandNormal, input.ThrustPercent);
                }
                else
                {
                    StopLinearMovement(dt, host);
                }
            }
            else
            {
                StopLinearMovement(dt, host);
            }
        }
        public static void StopLinearMovement(float dt, IControlable host)
        {
            Vector2D velocity = host.Current.Velocity.Linear;
            float speed = velocity.MagnitudeSq;
            if (speed > 0)
            {
                speed = (float)MathHelper.Sqrt(speed);
                Vector2D direction = velocity * (1 / speed);
                float LAccel = host.MovementInfo.MaxLinearAcceleration.Value * dt;
                if (speed < LAccel)
                {
                    DefaultControlHandler.ApplydLV(host, -velocity, dt);
                    //host.Current.Velocity.Linear = Vector2D.Zero;
                }
                else
                {
                    DefaultControlHandler.ApplydLV(host, -direction * LAccel, dt);
                    //host.Current.Velocity.Linear -= direction * LAccel;
                }
            }
        }
        public InertialessControlHandler()
        { }
        public InertialessControlHandler(InertialessControlHandler copy):base(copy)
        { }
        public override void HandleControlInput(float dt)
        {
            UpdateLinearMovement(dt, host);
            DefaultControlHandler.UpdateAngularMovement(dt, host);
        }
        public override object Clone()
        {
            return new InertialessControlHandler(this);
        }
    }
    [Serializable]
    public class TargetingControlHandler : BaseControlHandler
    {
        public TargetingControlHandler()
        { }
        public TargetingControlHandler(TargetingControlHandler copy):base(copy)
        { }
        public override void HandleControlInput(float dt)
        {
            if (dt == 0)
            {
                return;
            }
            if (host.Target != null && host.IsTargetable)
            {
                Vector2D direction = (host.Target.Current.Position.Linear - host.Current.Position.Linear).Normalized;
                DefaultControlHandler.ApplyThrust(dt, host, direction, 1);
            }
            else
            {
                Vector2D velocity = host.Current.Velocity.Linear;
                float speed = velocity.MagnitudeSq;
                if (speed > 0)
                {
                    speed = (float)MathHelper.Sqrt(speed);
                    Vector2D direction = velocity * (1 / speed);
                    float LAccel = host.MovementInfo.MaxLinearAcceleration.Value * dt;
                    if (speed < LAccel)
                    {
                        DefaultControlHandler.ApplydLV(host, -velocity, dt);
                        //host.Current.Velocity.Linear = Vector2D.Zero;
                    }
                    else
                    {
                        DefaultControlHandler.ApplydLV(host, -direction * LAccel, dt);
                        //host.Current.Velocity.Linear -= direction * LAccel;
                    }
                }
            }
        }
        public override object Clone()
        {
            return new TargetingControlHandler(this);
        }
    }
}