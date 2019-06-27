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
    public class UmgahDroneShipLoader : ShipLoader
    {
        public UmgahDroneShipLoader() : base("Umgah Drone") { }
        protected override IShip CreateHardCodedShip()
        {
            return UmgahDrone.Create(new PhysicsState(), null);
        }
    }



    [Serializable]
    public class UmgahDrone : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;
        public static Bounded<float> DefaultRechargeDelay;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(4)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 3)),
                new Bounded<float>(TimeWarp.ScaleVelocity(18)));
            DefaultState = new ShipState(new Bounded<float>(10),
                new Bounded<float>(30),
                new Bounded<float>(0),
                new Bounded<float>(0));//TimeWarp.RechargeRateToPerSeconds(150, 30)));
            DefaultRechargeDelay = new Bounded<float>(TimeWarp.RateToTime(150));
            DefaultActions.Add( UmgahDronePrimary.Create());
            DefaultActions.Add( UmgahDroneSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("UmgahDroneDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();
            Vector2D[] hullvertecies = new Vector2D[]
            {
                new Vector2D(30,7),
                new Vector2D(0,20),
                new Vector2D(-30,20),
                new Vector2D(-30,-20),
                new Vector2D(0,-20),
                new Vector2D(30,-7)
            };
            goes.Add(new Polygon2D(ALVector2D.Zero, hullvertecies));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(8, 366.504272f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            UmgahDrone returnvalue = new UmgahDrone(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Umgah Drone", new CreateShipDelegate(Create));
        }*/
        protected Bounded<float> rechargeDelay;
        protected UmgahDrone(PhysicsState state, FactionInfo factionInfo)
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
            rechargeDelay = new Bounded<float>(DefaultRechargeDelay);
        }
        public UmgahDrone(UmgahDrone copy)
            : base(copy)
        {
            this.rechargeDelay = new Bounded<float> ( copy.rechargeDelay);
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            rechargeDelay.Value += dt;
            if (rechargeDelay.IsFull)
            {
                this.shipState.Energy.Fill();
            }
            if (this.shipState.Energy.IsFull)
            {
                rechargeDelay.Empty();
            }
        }
        public override object Clone()
        {
            return new UmgahDrone(this);
        }
    }
    [Serializable]
    public class UmgahDronePrimary : RayAction
    {
        public static LifeSpan DefaultLifeTime;
        public static Costs DefaultCost;
        public static float DefaultRange;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        public static float[] DefaultWeaponImpulses;
        public static float[] DefaultWeaponPositions;
        public static float[] DefaultWeaponDirections;
        public static float[] DefaultWeaponRanges;

        public static int DefaultWeaponCount = 40;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultRange = TimeWarp.ScaleRange(8);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-1f / (float)DefaultWeaponCount, 0, 0, 0)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RateToTime(0));
            DefaultActionSounds = new ActionSounds("EnergyField1", null, null);
            InitWeaponSettings();
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
        }
        static void InitWeaponSettings()
        {
            float positionRange = .3f;
            float directionRange = .5f;

            float inc;
            float value;

            DefaultWeaponImpulses = new float[DefaultWeaponCount];
            DefaultWeaponPositions = new float[DefaultWeaponCount];
            DefaultWeaponDirections = new float[DefaultWeaponCount];
            DefaultWeaponRanges = new float[DefaultWeaponCount];
            value = 300 / (float)DefaultWeaponCount;
            for (int pos = 0; pos < DefaultWeaponCount; ++pos)
            {
                DefaultWeaponRanges[pos] = DefaultRange;
                DefaultWeaponImpulses[pos] = value;
            }
            inc = positionRange / (float)DefaultWeaponCount;
            value = -positionRange / 2;
            for (int pos = 0; pos < DefaultWeaponCount; ++pos)
            {
                DefaultWeaponPositions[pos] = value;
                value += inc;
            }
            inc = directionRange / (float)DefaultWeaponCount;
            value = -directionRange / 2;
            for (int pos = 0; pos < DefaultWeaponCount; ++pos)
            {
                DefaultWeaponDirections[pos] = value;
                value += inc;
            }
        }
        static DirectedRayWeapon CreateWeapon()
        {
            return new DirectedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                DefaultWeaponImpulses,
                DefaultWeaponPositions,
                DefaultWeaponDirections,
                DefaultWeaponRanges);
        }
        public static UmgahDronePrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new UmgahDronePrimary();
        }

        UmgahDronePrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon())
        { }
    }
    [Serializable]
    public class UmgahDroneSecondary : InstantAction
    {
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = TargetingInfo.Self;
            DefaultEffectsWho = DefaultTargetingTypes;
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultEffectCollection.Effects.Add(
                new BackwardsTeleport(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    200));
            DefaultActionSounds = new ActionSounds("Laser7", null, null);
        }
         static IWeapon CreateWeapon()
        {
            return new InstantWeapon(
                new WeaponsLogic(DefaultEffectsWho,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static UmgahDroneSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new UmgahDroneSecondary();
        }
         UmgahDroneSecondary()
            : base(new Bounded<float>(DefaultDelay),
        DefaultTargetingTypes,
        new Costs(DefaultCost),
        DefaultActionSounds,
        CreateWeapon())
        {
            this.aIInfo = new ShieldActionAIInfo();
        }
    }
    [Serializable]
    public class BackwardsTeleport : PhysicsStateEffect
    {
        static Random rand = new Random();
        float distance;
        PhysicsState diff;
        public BackwardsTeleport(TargetingInfo effectWho,
            EffectTypes harmfulEffectTypes,
            EffectSounds effectSounds, float distance)
            : base(effectWho, harmfulEffectTypes, effectSounds)
        {
            this.distance = distance;
        }
        public BackwardsTeleport(BackwardsTeleport copy)
            : base(copy)
        {
            this.distance = copy.distance;
        }
        public override PhysicsState PhysicsStateChange
        {
            get
            {
                return diff;
            }
            set
            {
                diff = value;
            }
        }
        public override void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie)
        {
            diff = new PhysicsState();
            diff.Position.Linear = attachie.DirectionVector * -distance; ;
            diff.Velocity.Linear = -attachie.Current.Velocity.Linear;
            base.OnTargetAttachment(attachmentResult, attachie);
        }
        public override object Clone()
        {
            return new BackwardsTeleport(this);
        }
    }
}
#endif
