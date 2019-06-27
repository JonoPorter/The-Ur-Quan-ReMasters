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
#if !Release
using System;
using System.Collections.Generic;
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class SlylandroProbeShipLoader : ShipLoader
    {
        public SlylandroProbeShipLoader() : base("Slylandro Probe") { }
        protected override IShip CreateHardCodedShip()
        {
            return SlylandroProbe.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class SlylandroProbe : Ship 
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(0)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(60, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(60)));
            DefaultState = new ShipState(new Bounded<float>(12),
                new Bounded<float>(20), 
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(0,0)));

            DefaultActions.Add( SlylandroProbePrimary.Create());
            DefaultActions.Add( SlylandroProbeSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            DefaultShipSounds = new ShipSounds("SlylandroProbeDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, Polygon2D.FromRectangle(10, 120));

            IGeometry2D sphereone = new Polygon2D(new ALVector2D(0, new Vector2D(-60, 0)), Polygon2D.FromNumberofSidesAndRadius(10, 25));
            IGeometry2D spheretwo = new Polygon2D(new ALVector2D(0, new Vector2D(60, 0)), Polygon2D.FromNumberofSidesAndRadius(10, 25));
            IGeometry2D subsphereone = new Polygon2D(new ALVector2D(0, new Vector2D(0, 30)), Polygon2D.FromNumberofSidesAndRadius(10, 10));
            IGeometry2D subspheretwo = new Polygon2D(new ALVector2D(0, new Vector2D(0, -30)), Polygon2D.FromNumberofSidesAndRadius(10, 10));

            DefaultShape = new RigidBodyTemplate(
                8,
                2635.9963064342355f,
                new IGeometry2D[] { sphereone, spheretwo, subsphereone, subspheretwo, mainhull },
                new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            SlylandroProbe returnvalue = new SlylandroProbe(state, factionInfo);
            returnvalue.ControlHandler = new ProbeControlHandler();
            return returnvalue;
        }
        protected SlylandroProbe(PhysicsState state, FactionInfo factionInfo)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(), 
            state, 
            DefaultBodyFlags, DefaultShape, 
            new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            DefaultShipSounds, 
            new ActionList(DefaultActions), 
            null)
        {
           /* this.attachedEffectCollection.ProlongedEffects.Add(
              new BlockingSheild(
                TargetingInfo.Self,
                EffectTypes.None,
                new EffectSounds(),
                new LifeSpan(),
                EffectTypes.HealthSteal));*/
            this.attachedEffectCollection.ProlongedEffects.Add(
                new EffectBlockEffect<JumpShipEffect>(new EffectSounds(), new LifeSpan()));
        }

        protected SlylandroProbe(SlylandroProbe copy)
            : base(copy)
        {
        }
        public override float DirectionAngle
        {
            get
            {
                return ((ProbeControlHandler)this.controlHandler).DirectionAngle;
            }
        }
        public override void UpdatePosition(float dt)
        {
            base.UpdatePosition(dt);
            direction = ((ProbeControlHandler)this.controlHandler).DirectionVector;
        }
        public override object Clone()
        {
            return new SlylandroProbe(this);
        }
    }
    [Serializable]
    public class SlylandroProbePrimary : RayPointDefence
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(.4f);
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;

        public static float DefaultRadius = 600;
        public static int DefaultMaxNumberofTargets = 10;
        public static float DefaultImpulse = 2;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultEffectsWho = new TargetingInfo(TargetingTypes.Enemy);
            DefaultTargetingTypes = DefaultEffectsWho;
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(2));
            DefaultActionSounds = new ActionSounds("Laser6", null, null);
        }
        public static SlylandroProbePrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SlylandroProbePrimary();
        }
        static TargetedRayWeapon CreateWeapon()
        {
            return new LightningRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(), 
                new WeaponsLogic(DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)), 
                DefaultImpulse, 
                DefaultRadius, 300, 70);
        }
         SlylandroProbePrimary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            DefaultActionSounds,
            DefaultRadius,
            DefaultMaxNumberofTargets,
             CreateWeapon())
        {}
         SlylandroProbePrimary(SlylandroProbePrimary copy)
            : base(copy)
        { }
        public override object Clone()
        {
            return new SlylandroProbePrimary(this);
        }
    }
    [Serializable]
    public class SlylandroProbeSecondary : RayPointDefence
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(.4f);
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;

        public static float DefaultRadius = 600;
        public static int DefaultMaxNumberofTargets = 10;
        public static float DefaultImpulse = 2;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Debris);
            DefaultEffectsWho = DefaultTargetingTypes;
            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(2));
            DefaultActionSounds = new ActionSounds("Energize", null, null);
        }
        static TargetedRayWeapon CreateWeapon()
        {
            return new LightningRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(), 
                new WeaponsLogic(DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)), 
                DefaultImpulse, 
                DefaultRadius, 600, 20);
        }
        public static SlylandroProbeSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SlylandroProbeSecondary();
        }
        SlylandroProbeSecondary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            DefaultActionSounds,
            DefaultRadius,
            DefaultMaxNumberofTargets,
             CreateWeapon())
        {
            this.aIInfo = new RechargeActionAIInfo(DefaultCost.ActivationCost);
        }
        public SlylandroProbeSecondary(SlylandroProbeSecondary copy)
            : base(copy)
        { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            bool rv = base.OnActivated(actionResult,dt);
            if (rv)
            {
                source.ShipState.Energy.Fill();
            }
            return rv;
        }
        public override object Clone()
        {
            return new SlylandroProbeSecondary(this);
        }
    }
    [Serializable]
    public class ProbeControlHandler : BaseControlHandler
    {
        float angle = 0;
        float angleVelocity = 0;
        Bounded<float> delay = new Bounded<float>(.5f);
        void UpdateLinearMovement(float dt, IControlable host)
        {
            if (dt == 0)
            {
                return;
            }
            ControlInput input = host.CurrentControlInput;
            if (input[InputAction.MoveForward] && delay.IsFull)
            {
                delay.Empty();
                angle += MathHelper.PI;
            }
            DefaultControlHandler.ApplyThrust(dt, host, Vector2D.FromLengthAndAngle(1, angle), 1);
        }
        void UpdateAngularMovement(float dt, IControlable host)
        {
            if (dt == 0)
            {
                return;
            }
            ControlInput input = host.CurrentControlInput;
            float AV = angleVelocity;
            float AAccel = host.MovementInfo.MaxAngularAcceleration.Value * dt;
            float AbsAV = MathHelper.Abs(AV);
            bool right = input[InputAction.RotateRight];
            bool left = input[InputAction.RotateLeft];
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
        void ApplydAV(IControlable host, float dv, float dt)
        {
            angleVelocity += dv;
        }
        public ProbeControlHandler()
        { }
        public ProbeControlHandler(ProbeControlHandler copy)
        { }
        public Vector2D DirectionVector
        {
            get
            {
                return Vector2D.FromLengthAndAngle(1, angle);
            }
        }
        public float DirectionAngle
        {
            get
            {
                return  angle;
            }
        }
        public override void HandleControlInput(float dt)
        {
            UpdateLinearMovement(dt, host);
            UpdateAngularMovement(dt, host);
            host.Current.Velocity.Angular = host.MovementInfo.MaxAngularVelocity / 3;
        }
        public override void Update(float dt)
        {
            angle += angleVelocity * dt;
            delay.Value += dt;
            base.Update(dt);
        }
        public override object Clone()
        {
            return new ProbeControlHandler(this);
        }
    }
}
#endif
