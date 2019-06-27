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
using Physics2D.Joints;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class ChmmrAvatarShipLoader : ShipLoader
    {
        public ChmmrAvatarShipLoader() : base("Chmmr Avatar") { }
        protected override IShip CreateHardCodedShip()
        {
            return ChmmrAvatar.Create(new PhysicsState(), null);
        }
    }
    [Serializable]
    public class ChmmrAvatar : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static IShip[] DefaultSubShips;
        public static float DefaultSatelliteOrbitRadius;
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(3)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(7, 6)),
                new Bounded<float>(TimeWarp.ScaleVelocity(35)));
            DefaultState = new ShipState(
                new Bounded<float>(42),
                new Bounded<float>(42),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(1, 1)));

            DefaultActions.Add( ChmmrAvatarPrimary.Create());
            DefaultActions.Add( ChmmrAvatarSecondary.Create());

            InitSubShips();
            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("ChmmrAvatarDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();
            Vector2D[] leftmainhullvertecies = new Vector2D[]
                {
                    new Vector2D(50,2),
                    new Vector2D(-50,2),
                    new Vector2D(-50,-20),
                    new Vector2D(0,-25),
                    new Vector2D(70,-10)
                };
            Vector2D[] Rightmainhullvertecies = new Vector2D[]
                {
                    new Vector2D(50,-2),
                    new Vector2D(70,10),
                    new Vector2D(0,25),
                    new Vector2D(-50,20),
                    new Vector2D(-50,-2),

                };
            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(10,10),
                    new Vector2D(-30,10),
                    new Vector2D(-20,-50),
                    new Vector2D(0,-50)
                };
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(0,50),
                    new Vector2D(-20,50),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-10)
                };
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, -30)), leftWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, 30)), RightWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, -6)), leftmainhullvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, 6)), Rightmainhullvertecies));
            coes.Add(DefaultCoefficients);

            DefaultShape = new RigidBodyTemplate(22, 1891.9877824710443f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static void InitSubShips()
        {
            DefaultSatelliteOrbitRadius = TimeWarp.ScaleRange(4);
            DefaultSubShips = new IShip[3];
            PhysicsState state = new PhysicsState();
            FactionInfo factionInfo = new FactionInfo(0);
            DefaultSubShips[0] = ChmmrSatellite.Create(state, factionInfo);
            DefaultSubShips[1] = ChmmrSatellite.Create(state, factionInfo);
            DefaultSubShips[2] = ChmmrSatellite.Create(state, factionInfo);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ChmmrAvatar returnvalue = new ChmmrAvatar(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }

        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Chmmr Avatar", new CreateShipDelegate(Create));
        }*/
        protected static IShip[] CreateSubShips(PhysicsState physicsstate, FactionInfo factionInfo)
        {
            int SatelliteCount = DefaultSubShips.Length;
            IShip[] subShips = new IShip[SatelliteCount];
            int pos = 0;
            float da = MathHelper.PI * 2 / SatelliteCount;
            for (float angle = 0; angle < MathHelper.PI * 2; angle += da)
            {
                PhysicsState state = new PhysicsState(physicsstate);
                Vector2D direction = Vector2D.Rotate(angle, Vector2D.XAxis);
                state.Position.Linear += direction * (DefaultSatelliteOrbitRadius + 1);
                state.Position.Angular = angle - MathHelper.PI * .5f;
                state.Velocity.Linear = direction.RightHandNormal * 200;
                subShips[pos] = (IShip)DefaultSubShips[pos].Clone();
                subShips[pos].Current.Set(state);
                subShips[pos].SetAllPositions();
                //subShips[pos].FactionInfo = new FactionInfo(factionInfo);
                pos++;
            }
            return subShips;
        }
        protected ChmmrAvatar(PhysicsState state, FactionInfo factionInfo)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(), 
            state, 
            DefaultBodyFlags, DefaultShape, 
            new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            DefaultShipSounds, 
            new ActionList(DefaultActions), 
            CreateSubShips(state, factionInfo))
        { }
        public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            base.OnCreation(gameResult, factionInfo);
            int SatelliteCount = subShips.Length;
            for (int pos = 0; pos < SatelliteCount; ++pos)
            {
                subShips[pos].SetAllPositions();
            }
            for (int pos = 0; pos < SatelliteCount; ++pos)
            {
                int pos2 = (pos + 1) % SatelliteCount;
                gameResult.AddJoint(new PinJoint(new CollidablePair(subShips[pos], this), this.current.Position.Linear, 0, .1f));
                gameResult.AddJoint(new PinJoint(new CollidablePair(subShips[pos], subShips[pos2]), subShips[pos2].Current.Position.Linear, .1f, .1f));
            }
        }
        public override void RunControlInput(GameResult gameResult, float dt)
        {
            base.RunControlInput(gameResult,dt);
            foreach (IShip ship in subShips)
            {
                ship.CurrentControlInput = new ControlInput(1);
                ship.CurrentControlInput[InputAction.Action] = true;
                ship.CurrentControlInput.ActiveActions[0] = true;
                ship.RunControlInput(gameResult, dt);
            }
        }
        ChmmrAvatar(ChmmrAvatar copy) : base(copy) { }
        public override object Clone()
        {
            return new ChmmrAvatar(this);
        }
    }
    [Serializable]
    public class ChmmrAvatarPrimary : RayAction
    {
        public static LifeSpan DefaultLifeTime;
        public static Costs DefaultCost;
        public static float DefaultRange = TimeWarp.ScaleRange(15);
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            //DefaultTargetingTypes =  TargetingInfo.Enemy;
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Other);
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-.25f, 0, 0, 0)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RateToTime(0));
            DefaultActionSounds = new ActionSounds("Laser2", null, null);
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
        }
        static DirectedRayWeapon CreateWeapon()
        {
            return new DirectedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                new float[] { 100, 100, 100, 100 },
                new float[] { -.2f, -.1f, .1f, .2f },
                new float[] { 0, 0, 0, 0 },
                new float[] { DefaultRange, DefaultRange, DefaultRange, DefaultRange });
        }
        public static ChmmrAvatarPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChmmrAvatarPrimary();
        }

        ChmmrAvatarPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon())
        {
        }
    }
    [Serializable]
    public class ChmmrAvatarSecondary : RayAction
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(0);
        public static Costs DefaultCost;
        public static float DefaultRange;
        public static float DefaultImpulse;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultRange = TimeWarp.ScaleRange(120);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultImpulse = -25000;
            DefaultActionSounds = new ActionSounds("TractorBeam", null, null);
        }
        static TargetedRayWeapon CreateWeapon()
        {
            return new TargetedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                DefaultImpulse,
                DefaultRange
                );
        }
        public static ChmmrAvatarSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChmmrAvatarSecondary();
        }
        ChmmrAvatarSecondary()
            : base(new Bounded<float>(DefaultDelay),
                   DefaultTargetingTypes,
                   new Costs(DefaultCost),
                   true,
                   DefaultActionSounds,
                   CreateWeapon())
        {
            this.aIInfo = new SpecificRangeActionAIInfo(DefaultCost.ActivationCost, ChmmrAvatarPrimary.DefaultRange, DefaultRange);
        }
    }
    [Serializable]
    public class ChmmrSatellite : Ship
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
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));

            DefaultState = new ShipState(new Bounded<float>(10),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultActions.Add( ChmmrSatellitePrimary.Create());
            DefaultControlableSounds = new ControlableSounds(null, "Boom45");
            DefaultShipSounds = new ShipSounds();
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ChmmrSatellite returnvalue = new ChmmrSatellite(state, factionInfo);
            return returnvalue;
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 90), new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 20)) }, new Coefficients[] { DefaultCoefficients });
            //DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        protected ChmmrSatellite(PhysicsState state, FactionInfo factionInfo)
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
            this.controlableType = ControlableType.SubShip;
        }
        public ChmmrSatellite(ChmmrSatellite copy) : base(copy) { }
        protected override ControlInput GetInput(float dt)
        {
            ControlInput input = base.GetInput(dt);
            input[InputAction.Action] = true;
            input.ActiveActions[0] = true;
            return input;
        }
        public override object Clone()
        {
            return new ChmmrSatellite(this);
        }
    }
    [Serializable]
    public class ChmmrSatellitePrimary : RayPointDefence
    {
        public static LifeSpan DefaultLifeTime;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static float DefaultRadius;
        public static int DefaultMaxNumberofTargets;
        public static float DefaultImpulse = 20;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy);
            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultTargetingTypes, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(4));
            DefaultRadius = TimeWarp.ScaleRange(8);
            DefaultMaxNumberofTargets = 3;
            DefaultActionSounds = new ActionSounds();
            DefaultLifeTime = new LifeSpan(.03f);
        }
        static TargetedRayWeapon CreateWeapon()
        {
            return new TargetedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                DefaultImpulse,
                DefaultRadius);
        }
        public static ChmmrSatellitePrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ChmmrSatellitePrimary();
        }

        ChmmrSatellitePrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           DefaultActionSounds,
           DefaultRadius,
           DefaultMaxNumberofTargets,
            CreateWeapon())
        { }
    }
}
#endif
