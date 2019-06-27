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
    public abstract class BaseActionAIInfo : IActionAIInfo
    {
        protected float effectiveRange = -1;
        protected IShip source;
        protected float aimAngle;
        protected bool shouldAim;
        protected bool shouldActivate;
        protected ShipStateChange cost;
        protected BaseActionAIInfo(ShipStateChange cost)
        {
            if (cost != null)
            {
                this.cost = new ShipStateChange(cost);
                this.cost.Health *= 3;
            }
        }
        protected BaseActionAIInfo(BaseActionAIInfo copy) { this.cost = copy.cost; }
        public bool ShouldAim { get { return shouldAim; } }
        public bool ShouldActivate { get { return shouldActivate; } }
        public float AimAngle { get { return aimAngle; } }
        public float EffectiveRange { get { return effectiveRange; } }
        public virtual void OnSourceCreation(GameResult gameResult, IShip source, IAction action)
        {
            this.source = source;
        }
        protected abstract void UpdateInternal(AIStateInfo aIState);
        public void Update(AIStateInfo aIState)
        {
            this.shouldActivate = false;
            this.shouldAim = false;
            bool test = this.cost == null || this.source.ShipState.MeetsCost(-this.cost);
            if (test)
            {
                UpdateInternal(aIState);
            }
        }
        public abstract bool IsThreatTo(IControlable other);
        public abstract object Clone();
    }
    [Serializable]
    public class ShieldActionAIInfo : BaseActionAIInfo
    {
        public ShieldActionAIInfo() : base((ShipStateChange)null) { }
        protected ShieldActionAIInfo(ShieldActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            for (int pos = 0; pos < aIState.Threats.Length; ++pos)
            {
                if (aIState.Threats[pos])
                {
                    this.shouldActivate = true;
                    return;
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            return false;
        }
        public override object Clone()
        {
            return new ShieldActionAIInfo(this);
        }


    }
    [Serializable]
    public class OffensiveShieldActionAIInfo : BaseActionAIInfo
    {
        float range;
        public OffensiveShieldActionAIInfo(ShipStateChange cost,float range)
            : base(cost)
        {
            this.range = range;
        }
        protected OffensiveShieldActionAIInfo(OffensiveShieldActionAIInfo copy) : base(copy)
        {
            this.range = copy.range;
        }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            if (aIState.VectorToTarget.Magnitude < range)
            {
                this.shouldActivate = true;
                return;
            }
            for (int pos = 0; pos < aIState.Threats.Length; ++pos)
            {
                if (aIState.Threats[pos])
                {
                    this.shouldActivate = true;
                    return;
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost))
            {
                return false;
            }
            return source.ShipState.MeetsCost(-cost) && ((other.Current.Position.Linear - source.Current.Position.Linear).Magnitude < range);
        }
        public override object Clone()
        {
            return new OffensiveShieldActionAIInfo(this);
        }


    }
    [Serializable]
    public class RechargeActionAIInfo : BaseActionAIInfo
    {
        public RechargeActionAIInfo(ShipStateChange cost) : base(cost) { }
        protected RechargeActionAIInfo(RechargeActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            if (cost != null)
            {
                if (cost.Health > 0)
                {
                    this.shouldActivate = source.ShipState.Energy.HasRoomFor(-cost.Energy * 1.2f);
                }
                else
                {
                    this.shouldActivate = true;
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            return false;
        }

        public override object Clone()
        {
            return new RechargeActionAIInfo(this);
        }
    }
    [Serializable]
    public abstract class BaseActionAIInfo<TAction> : BaseActionAIInfo
        where TAction : IAction
    {
        protected TAction action;
        protected BaseActionAIInfo(ShipStateChange cost) : base(cost) { }
        protected BaseActionAIInfo(BaseActionAIInfo<TAction> copy) : base(copy) { }
        public override void OnSourceCreation(GameResult gameResult, IShip source, IAction action)
        {
            SetAction((TAction)action);
            base.OnSourceCreation(gameResult, source, action);
        }
        protected virtual void SetAction(TAction action)
        {
            this.action = action;
        }
    }
    [Serializable]
    public class GunActionAIInfo : BaseActionAIInfo<IGunAction>
    {
        public static float AimTolerance = .1f;
        public GunActionAIInfo(ShipStateChange cost)
            : base(cost)
        { }
        protected GunActionAIInfo(GunActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            float distance = aIState.VectorToTarget.Magnitude;

            float range = action.Weapon.LifeTime.TimeLeft *
                (action.Weapon.MovementInfo.MaxLinearVelocity + aIState.VectorToTarget.Normalized * source.Current.Velocity.Linear) +
                (source.BoundingRadius + aIState.Target.BoundingRadius + action.Weapon.BoundingRadius);
            if (range > distance)
            {
                if (action.Weapon.IsFireAndForget)
                {
                    this.aimAngle = aIState.VectorToTarget.Angle + action.VelocityAngle;
                    float diff = MathHelper.GetAngleDifference(this.aimAngle,
                          source.DirectionAngle);
                    this.shouldActivate = MathHelper.Abs(diff) < MathHelper.HALF_PI;
                    this.shouldAim = !this.shouldActivate;
                }
                else if (Logic.TrySolveInterceptAngle(
                    source.Current.Position.Linear,
                    aIState.Target.Current.Position.Linear,
                    aIState.Target.Current.Velocity.Linear - source.Current.Velocity.Linear,
                    action.Weapon.MovementInfo.MaxLinearVelocity,
                    out this.aimAngle))
                {
                    this.aimAngle += action.VelocityAngle;
                    this.shouldAim = true;
                    float angle = aIState.VectorToTarget.Angle;
                    this.shouldActivate = Logic.InArc(source.DirectionAngle + action.VelocityAngle,
                        aimAngle,
                        distance,
                        aIState.Target.BoundingRadius,
                        GunActionAIInfo.AimTolerance);
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost) || !action.Weapon.WeaponInfo.CanEffect(other))
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float currentAngle = source.DirectionAngle + action.VelocityAngle;
            float weaponrange = action.Weapon.LifeTime.TimeLeft * action.Weapon.MovementInfo.MaxLinearVelocity + other.BoundingRadius + source.BoundingRadius;
            float distance = diff.Magnitude;
            return distance < weaponrange &&
                    Logic.InArc(currentAngle,
                        diff.Angle,
                        distance,
                        other.BoundingRadius,
                        GunActionAIInfo.AimTolerance+action.RandomVelocityAngle);
        }
        public override object Clone()
        {
            return new GunActionAIInfo(this);
        }
    }
    /*[Serializable]
    public class RandomGunActionAIInfo : BaseActionAIInfo<RandomGunAction>
    {
        public RandomGunActionAIInfo(ShipStateChange cost)
            : base(cost)
        { }
        protected RandomGunActionAIInfo(RandomGunActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            float distance = aIState.VectorToTarget.Magnitude;

            float range = action.Weapon.LifeTime.TimeLeft *
                (action.Weapon.MovementInfo.MaxLinearVelocity + aIState.VectorToTarget.Normalized * source.Current.Velocity.Linear) +
                (source.BoundingRadius + aIState.Target.BoundingRadius + action.Weapon.BoundingRadius);
            if (range > distance)
            {
                if (action.Weapon.IsFireAndForget)
                {
                    this.aimAngle = aIState.VectorToTarget.Angle + action.VelocityAngle;
                    float diff = MathHelper.GetAngleDifference(this.aimAngle,
                          source.DirectionAngle);
                    this.shouldActivate = MathHelper.Abs(diff) < MathHelper.HALF_PI;
                    this.shouldAim = !this.shouldActivate;
                }
                else if (Logic.TrySolveInterceptAngle(
                    source.Current.Position.Linear,
                    aIState.Target.Current.Position.Linear,
                    aIState.Target.Current.Velocity.Linear - source.Current.Velocity.Linear,
                    action.Weapon.MovementInfo.MaxLinearVelocity,
                    out this.aimAngle))
                {
                    this.aimAngle += action.VelocityAngle;
                    this.shouldAim = true;
                    this.shouldActivate = Logic.InArc(aimAngle,
                        aIState.VectorToTarget.Angle,
                        distance,
                        aIState.Target.BoundingRadius,
                        GunActionAIInfo.AimTolerance + action.RandomVelocityAngle);
                }
            }
        }

        public override object Clone()
        {
            return new RandomGunActionAIInfo(this);
        }

        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost) || !action.Weapon.WeaponInfo.CanEffect(other))
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float currentAngle = source.DirectionAngle + action.VelocityAngle;
            float weaponrange = action.Weapon.LifeTime.TimeLeft * action.Weapon.MovementInfo.MaxLinearVelocity + other.BoundingRadius + source.BoundingRadius;
            float distance = diff.Magnitude;
            return distance < weaponrange &&
                    Logic.InArc(currentAngle,
                        diff.Angle,
                        distance,
                        other.BoundingRadius,
                        GunActionAIInfo.AimTolerance+action.RandomVelocityAngle);
        }
    }*/
    [Serializable]
    public class MultiGunActionAIInfo : BaseActionAIInfo<IMultiGunAction>
    {
        public MultiGunActionAIInfo(ShipStateChange cost)
            : base(cost)
        { }
        protected MultiGunActionAIInfo(MultiGunActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            float distance = aIState.VectorToTarget.Magnitude;
            float aimAngle = 0;
            float angle = aIState.VectorToTarget.Angle;
            int length = action.Weapons.Length;
            for (int pos = 0; pos < length; ++pos)
            {
                float range = action.Weapons[pos].LifeTime.TimeLeft *
                    (action.Weapons[pos].MovementInfo.MaxLinearVelocity + aIState.VectorToTarget.Normalized * source.Current.Velocity.Linear) +
                    (source.BoundingRadius + aIState.Target.BoundingRadius + action.Weapons[pos].BoundingRadius);
                if (range > distance)
                {
                    if (action.Weapons[pos].IsFireAndForget)
                    {
                        this.aimAngle = angle - action.VelocityAngles[pos];
                        float diff = MathHelper.GetAngleDifference(this.aimAngle,
                              source.DirectionAngle);
                        this.shouldActivate = MathHelper.Abs(diff) < MathHelper.HALF_PI;
                        this.shouldAim = !this.shouldActivate;
                        return;
                    }
                    else
                    {
                        if (Logic.TrySolveInterceptAngle(
                              source.Current.Position.Linear,
                              aIState.Target.Current.Position.Linear,
                              aIState.Target.Current.Velocity.Linear - source.Current.Velocity.Linear,
                              action.Weapons[pos].MovementInfo.MaxLinearVelocity,
                              out aimAngle))

                            aimAngle += action.VelocityAngles[pos];


                        if (MathHelper.Abs(MathHelper.GetAngleDifference(angle, aimAngle)) <
                                MathHelper.Abs(MathHelper.GetAngleDifference(angle, this.aimAngle)))
                        {
                            this.shouldAim = true;
                            this.aimAngle = aimAngle;
                        }
                        if (Logic.InArc(source.DirectionAngle + action.VelocityAngles[pos],
                                aimAngle,
                                distance,
                                aIState.Target.BoundingRadius,
                                GunActionAIInfo.AimTolerance + GetRandomVelocityAngle(pos)))
                        {
                            this.shouldActivate = true;
                            return;

                        }
                    }
                }
            }
        }
        float GetRandomVelocityAngle(int index)
        {
            if (action.RandomVelocityAngles != null)
            {
                return action.RandomVelocityAngles[index];
            }
            else
            {
                return 0;
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost!=null&&!source.ShipState.MeetsCost(-cost))
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float distance = diff.Magnitude;
            float angle = diff.Angle;
            for (int pos = 0; pos < action.VelocityAngles.Length; ++pos)
            {
                float currentAngle = source.DirectionAngle + action.VelocityAngles[pos];
                float weaponrange = action.Weapons[pos].LifeTime.TimeLeft * action.Weapons[pos].MovementInfo.MaxLinearVelocity + other.BoundingRadius + source.BoundingRadius;
                if (distance < weaponrange &&
                        Logic.InArc(currentAngle,
                            angle,
                            distance,
                            other.BoundingRadius,
                            GunActionAIInfo.AimTolerance + GetRandomVelocityAngle(pos)))
                {
                    return true;
                }
            }

            return false;
        }
        public override object Clone()
        {
            return new MultiGunActionAIInfo(this);
        }
    }
    [Serializable]
    public class ProximityActionAIInfo : BaseActionAIInfo<BaseProximityAction>
    {
        public ProximityActionAIInfo(ShipStateChange cost) : base(cost) { }
        protected ProximityActionAIInfo(ProximityActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            if (action.Radius > aIState.VectorToTarget.Magnitude)
            {
                this.shouldActivate = true;
                return;
            }
            for (int pos = 0; pos < aIState.Threats.Length; ++pos)
            {
                if (aIState.Threats[pos])
                {
                    this.shouldActivate = true;
                    break;
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost) || !action.CanTarget(other))
            {
                return false;
            }
            return  ((other.Current.Position.Linear - source.Current.Position.Linear).Magnitude < action.Radius);
        }
        public override object Clone()
        {
            return new ProximityActionAIInfo(this);
        }
    }
    [Serializable]
    public class RayActionAIInfo : BaseActionAIInfo<RayAction>
    {
        float[] directions = null;
        float[] distances = null;
        public RayActionAIInfo(ShipStateChange cost) : base(cost) { }
        protected RayActionAIInfo(RayActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {


            if (distances != null)
            {
                int length = directions.Length;
                float distance = aIState.VectorToTarget.Magnitude - (aIState.Target.BoundingRadius + source.BoundingRadius);
                float angle = aIState.VectorToTarget.Angle;
                this.aimAngle = MathHelper.PI + angle;
                float aimAngle;
                for (int pos = 0; pos < length; ++pos)
                {
                    if (distance < distances[pos] * 2)
                    {
                        aimAngle = angle - directions[pos];
                        if (MathHelper.Abs(MathHelper.GetAngleDifference(angle, aimAngle)) <
                            MathHelper.Abs(MathHelper.GetAngleDifference(angle, this.aimAngle)))
                        {
                            this.shouldAim = true;
                            this.aimAngle = aimAngle;
                        }
                        if (distance < distances[pos] &&
                            Logic.InArc(source.DirectionAngle - directions[pos],
                            angle,
                            distance,
                            aIState.Target.BoundingRadius,
                            .1f))
                        {
                            this.shouldActivate = true;
                            return;
                        }
                    }
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost) || !action.CanTarget(other))
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float distance = diff.Magnitude;
            float angle = diff.Angle;
            if (directions != null)
            {
                for (int pos = 0; pos < directions.Length; ++pos)
                {
                    float currentAngle = source.DirectionAngle + directions[pos];
                    float weaponrange = distances[pos] + other.BoundingRadius + source.BoundingRadius;
                    if (distance < weaponrange &&
                            Logic.InArc(currentAngle,
                                angle,
                                distance,
                                other.BoundingRadius,
                                GunActionAIInfo.AimTolerance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void OnSourceCreation(GameResult gameResult, IShip source, IAction action)
        {
            base.OnSourceCreation(gameResult,source, action);
            if (this.action.Weapon is IDirectedRayWeapon)
            {
                IDirectedRayWeapon w = (IDirectedRayWeapon)this.action.Weapon;
                directions = w.Directions;
                distances = w.Distances;
            }
        }
        public override object Clone()
        {
            return new RayActionAIInfo(this);
        }
    }
    [Serializable]
    public class RemoteControlGunActionAIInfo : BaseActionAIInfo<GunAction>
    {
        public RemoteControlGunActionAIInfo()
            : base((ShipStateChange)null)
        { }
        protected RemoteControlGunActionAIInfo(RemoteControlGunActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            if (action.CurrentWeapon == null)
            {
                float distance = aIState.VectorToTarget.Magnitude;

                float range = action.Weapon.LifeTime.TimeLeft *
                    (action.Weapon.MovementInfo.MaxLinearVelocity + aIState.VectorToTarget.Normalized * source.Current.Velocity.Linear) +
                    (source.BoundingRadius + aIState.Target.BoundingRadius + action.Weapon.BoundingRadius);
                if (range > distance)
                {
                    if (action.Weapon.IsFireAndForget)
                    {
                        this.aimAngle = aIState.VectorToTarget.Angle + action.VelocityAngle;
                        float diff = MathHelper.GetAngleDifference(this.aimAngle,
                              source.DirectionAngle);
                        this.shouldActivate = MathHelper.Abs(diff) < MathHelper.HALF_PI;
                        this.shouldAim = !this.shouldActivate;
                    }
                    else if (Logic.TrySolveInterceptAngle(
                        source.Current.Position.Linear,
                        aIState.Target.Current.Position.Linear,
                        aIState.Target.Current.Velocity.Linear - source.Current.Velocity.Linear,
                        action.Weapon.MovementInfo.MaxLinearVelocity,
                        out this.aimAngle))
                    {
                        this.aimAngle += action.VelocityAngle;
                        this.shouldAim = true;
                        float angle = aIState.VectorToTarget.Angle;
                        this.shouldActivate = Logic.InArc(source.DirectionAngle + action.VelocityAngle,
                            aimAngle,
                            distance,
                            aIState.Target.BoundingRadius,
                            GunActionAIInfo.AimTolerance);
                    }
                }
            }
            else
            {
                Vector2D fromcw = aIState.Target.Current.Position.Linear - action.CurrentWeapon.Current.Position.Linear;
                if (fromcw * action.CurrentWeapon.Current.Velocity.Linear  > 0)
                {
                    this.shouldActivate = true;
                }
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (cost != null && !source.ShipState.MeetsCost(-cost) || action.CurrentWeapon != null)
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float currentAngle = source.DirectionAngle + action.VelocityAngle;
            float weaponrange = action.Weapon.LifeTime.TimeLeft * action.Weapon.MovementInfo.MaxLinearVelocity + other.BoundingRadius + source.BoundingRadius;
            float distance = diff.Magnitude;
            return distance < weaponrange &&
                    Logic.InArc(currentAngle,
                        diff.Angle,
                        distance,
                        other.BoundingRadius,
                        GunActionAIInfo.AimTolerance);

        }
        public override object Clone()
        {
            return new RemoteControlGunActionAIInfo(this);
        }
    }
    [Serializable]
    public class SpecificRangeActionAIInfo : BaseActionAIInfo
    {
        float minRange;
        float maxRange;
        public SpecificRangeActionAIInfo(ShipStateChange cost,float minRange,float maxRange) : base(cost)
        {
            this.minRange = minRange;
            this.maxRange = maxRange;
        }
        protected SpecificRangeActionAIInfo(SpecificRangeActionAIInfo copy)
            : base(copy) 
        {
            this.minRange = copy.minRange;
            this.maxRange = copy.maxRange;
        }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            float distance = aIState.VectorToTarget.Magnitude;
            this.shouldActivate = distance > minRange && distance < maxRange;
        }
        public override bool IsThreatTo(IControlable other)
        {
            return false;
        }

        public override object Clone()
        {
            return new SpecificRangeActionAIInfo(this);
        }
    }
    [Serializable]
    public class BuildUpGunActionAIInfo : BaseActionAIInfo<BuildUpGunAction>
    {
        public BuildUpGunActionAIInfo(ShipStateChange cost)
            : base(cost)
        { }
        protected BuildUpGunActionAIInfo(BuildUpGunActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            if (action.CurrentWeapon == null)
            {
                this.shouldActivate = source.ShipState.MeetsCost(this.action.Costs.ActivationCost * -1.5f) ;
            }
            else
            {
                float distance = aIState.VectorToTarget.Magnitude;

                float range = action.Weapon.LifeTime.TimeLeft *
                    (action.Weapon.MovementInfo.MaxLinearVelocity + aIState.VectorToTarget.Normalized * source.Current.Velocity.Linear) +
                    (source.BoundingRadius + aIState.Target.BoundingRadius + action.Weapon.BoundingRadius);


                if (range > distance)
                {
                    if (action.Weapon.IsFireAndForget)
                    {
                        this.aimAngle = aIState.VectorToTarget.Angle + action.VelocityAngle;
                        float diff = MathHelper.GetAngleDifference(this.aimAngle,
                              source.DirectionAngle);
                        this.shouldActivate = MathHelper.Abs(diff) < MathHelper.HALF_PI;
                        this.shouldAim = !this.shouldActivate;
                    }
                    else if (Logic.TrySolveInterceptAngle(
                        source.Current.Position.Linear,
                        aIState.Target.Current.Position.Linear,
                        aIState.Target.Current.Velocity.Linear - source.Current.Velocity.Linear,
                        action.Weapon.MovementInfo.MaxLinearVelocity,
                        out this.aimAngle))
                    {
                        this.aimAngle += action.VelocityAngle;
                        this.shouldAim = true;
                        float angle = aIState.VectorToTarget.Angle;
                        this.shouldActivate = Logic.InArc(source.DirectionAngle + action.VelocityAngle,
                            aimAngle,
                            distance,
                            aIState.Target.BoundingRadius,
                            0);
                    }
                }
                this.shouldActivate = !this.shouldActivate;
            }
        }
        public override bool IsThreatTo(IControlable other)
        {
            if (!action.Weapon.WeaponInfo.CanEffect(other) || action.CurrentWeapon == null)
            {
                return false;
            }
            Vector2D diff = other.Current.Position.Linear - source.Current.Position.Linear;
            float currentAngle = source.DirectionAngle + action.VelocityAngle;
            float weaponrange = action.Weapon.LifeTime.TimeLeft * action.Weapon.MovementInfo.MaxLinearVelocity + other.BoundingRadius + source.BoundingRadius;
            float distance = diff.Magnitude;
            return distance < weaponrange &&
                    Logic.InArc(currentAngle,
                        diff.Angle,
                        distance,
                        other.BoundingRadius,
                        GunActionAIInfo.AimTolerance);

        }
        public override object Clone()
        {
            return new BuildUpGunActionAIInfo(this);
        }
    }
    [Serializable]
    public class CloakingActionAIInfo : BaseActionAIInfo
    {
        public CloakingActionAIInfo(ShipStateChange cost)
            : base(cost)
        { }
        protected CloakingActionAIInfo(CloakingActionAIInfo copy) : base(copy) { }
        protected override void UpdateInternal(AIStateInfo aIState)
        {
            this.shouldActivate = !source.IsInvisible;
        }
        public override bool IsThreatTo(IControlable other)
        {
            return false;
        }

        public override object Clone()
        {
            return new CloakingActionAIInfo(this);
        }
    }
}