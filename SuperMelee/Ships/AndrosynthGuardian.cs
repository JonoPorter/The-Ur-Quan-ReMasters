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
    public class AndrosynthGuardianShipLoader : ShipLoader
    {
        public AndrosynthGuardianShipLoader() : base("Androsynth Guardian") { }
        protected override IShip CreateHardCodedShip()
        {
            return AndrosynthGuardian.Create(new PhysicsState(), null);
        }
    }
    [Serializable]
    public class AndrosynthGuardian
    {
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        static AndrosynthGuardian()
        {

        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            return AndrosynthGuardianNormal.Create(state, factionInfo);
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Androsynth Guardian", new CreateShipDelegate(AndrosynthGuardian.Create));
        }*/
        [Serializable]
        class AndrosynthGuardianNormal : Ship
        {
            public static ShipState DefaultState;
            public static RigidBodyTemplate DefaultShape;
            public static ShipMovementInfo DefaultMovementInfo;
            public static ActionList DefaultActions = new ActionList();
            public static ControlableSounds DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            public static ShipSounds DefaultShipSounds = new ShipSounds("AndrosynthGuardianDitty");
            static bool initialized = false;
            static void Initialize()
            {
                initialized = true;
                InitShape();
                DefaultMovementInfo = new ShipMovementInfo(
                    new Bounded<float>(TimeWarp.AngularAcceleration),
                    new Bounded<float>(TimeWarp.ScaleTurning(4)),
                    new Bounded<float>(TimeWarp.ScaleAcceleration(3, 1)),
                    new Bounded<float>(TimeWarp.ScaleVelocity(24)));

                DefaultState = new ShipState(new Bounded<float>(20),
                    new Bounded<float>(24),
                    new Bounded<float>(0),
                    new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(8, 1)));
                DefaultActions.Add(AndrosynthGuardianNormalPrimary.Creatw());
                DefaultActions.Add(AndrosynthGuardianSecondary.Creatw());
            }
            static void InitShape()
            {
                Coefficients DefaultCoefficients = TimeWarp.Coefficients;

                Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    //new Vector2D(60,0),
                    new Vector2D(60,8),
                    new Vector2D(0,15),
                    new Vector2D(-40,15),
                    new Vector2D(-40,-15),
                    new Vector2D(0,-15),
                    new Vector2D(60,-8)
                };
                List<Coefficients> ceos = new List<Coefficients>();
                List<IGeometry2D> goes = new List<IGeometry2D>();
                ceos.Add(DefaultCoefficients);
                goes.Add(new Polygon2D(ALVector2D.Zero, mainhullvertecies));
                float width = 70;
                for (int pos = -30; pos < 60; pos += 20)
                {
                    width -= 5;
                    ceos.Add(DefaultCoefficients);
                    goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(pos, 0)), Polygon2D.FromRectangle(width, 10)));
                }

                DefaultShape = new RigidBodyTemplate(9, 947.32712172484457f, goes.ToArray(), ceos.ToArray());
                DefaultShape.BalanceBody();
                ////DefaultShape.CalcInertiaMultiplier(.1f);
            }
            public static IShip Create(PhysicsState state, FactionInfo factionInfo)
            {
                if(!initialized)
                {
                    Initialize();
                }
                AndrosynthGuardianNormal returnvalue = new AndrosynthGuardianNormal(state, factionInfo);
                returnvalue.ControlHandler = new DefaultControlHandler();
                return returnvalue;
            }
            /*public static ShipInfo GetShipInfo()
            {
                return new ShipInfo("Mmrnmhrm XForm Laser", new CreateShipDelegate(Create));
            }*/
            AndrosynthGuardianNormal(PhysicsState state, FactionInfo factionInfo)
                : base(
                (LifeSpan)DefaultLifeTime.Clone(),
                state,
                DefaultBodyFlags,
                DefaultShape,
                new ShipMovementInfo(DefaultMovementInfo),
                new ShipState(DefaultState),
                DefaultControlableSounds,
                DefaultShipSounds,
                new WeaponsLogic(TargetingInfo.None, new EffectCollection()),
                new ActionList(DefaultActions),
                null)
            {
                this.controlableType = ControlableType.Ship;
            }
            protected AndrosynthGuardianNormal(AndrosynthGuardianNormal copy) : base(copy) { }
            public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
            {
                base.OnCreation(gameResult,factionInfo);
                this.ignoreInfo.AddGroupToIgnore(factionInfo.FactionNumber);
            }
            protected override ControlInput GetInput(float dt)
            {
                ControlInput input = base.GetInput(dt);
                if (input.ActiveActions.Length == 4)
                {
                    input[InputAction.MoveForward] = true;
                    if (shipState.Energy.IsEmpty)
                    {
                        input[InputAction.Action] = true;
                        input.ActiveActions[3] = true;
                    }
                }
                return input;
            }
            public override object Clone()
            {
                return new AndrosynthGuardianNormal(this);
            }
        }
        [Serializable]
        public class AndrosynthGuardianNormalPrimary : GunAction
        {
            public static RigidBodyTemplate DefaultShape;
            public static LifeSpan DefaultLifeTime;
            public static BodyFlags DefaultBodyFlags = BodyFlags.None;
            public static ShipMovementInfo DefaultMovementInfo;
            public static ShipState DefaultState;
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
                InitShape();
                DefaultMovementInfo = new ShipMovementInfo(
                    new Bounded<float>(TimeWarp.AngularAcceleration),
                    new Bounded<float>(TimeWarp.ScaleTurning(0)),
                    new Bounded<float>(100000),
                    new Bounded<float>(TimeWarp.ScaleVelocity(25)));

                DefaultState = new ShipState(new Bounded<float>(2), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
                DefaultCost = new Costs(new ShipStateChange(0, 3, 0, 0), null, null);
                DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
                DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(50, 25));
                DefaultEffectsWho = new TargetingInfo(TargetingTypes.Enemy | TargetingTypes.Debris, TargetingTypes.None, TargetingTypes.None);


                DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-2, 0, 0, 0)));

                DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
                DefaultActionSounds = new ActionSounds("BubblePop", null, null);
            }
            static void InitShape()
            {
                Coefficients coe = TimeWarp.Coefficients;

                IGeometry2D enginecone = new Polygon2D(new ALVector2D(0, new Vector2D(0, 0)), Polygon2D.FromNumberofSidesAndRadius(8, 8));
                DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.1f, 8), new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
                DefaultShape.BalanceBody();

            }
            static ISolidWeapon CreateWeapon()
            {

                ISolidWeapon w = new Controlable(
                    DefaultLifeTime,
                    new PhysicsState(),
                    DefaultBodyFlags,
                    DefaultShape,
                     new ShipMovementInfo(DefaultMovementInfo),
                new ShipState(DefaultState),
                new ControlableSounds(),
                new WeaponsLogic(
                DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)));
                w.ControlHandler = new DefaultControlHandler();
                w.AddControler(null,new BubbleController(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
                return w;
            }

            public static AndrosynthGuardianNormalPrimary Creatw()
            {
                if (!initialized)
                {
                    Initialize();
                }
                return new AndrosynthGuardianNormalPrimary();
            }

            AndrosynthGuardianNormalPrimary()
                : base(new Bounded<float>(DefaultDelay),
                DefaultTargetingTypes,
                new Costs(DefaultCost),
                false,
                DefaultActionSounds,
                0,
                0,
                CreateWeapon())
            {

            }
            public AndrosynthGuardianNormalPrimary(AndrosynthGuardianNormalPrimary copy) : base(copy) { }
            public override void OnSourceCreation(GameResult gameResult, IShip source)
            {
                base.OnSourceCreation(gameResult, source);
                this.Weapon.IgnoreInfo.JoinGroupToIgnore(source.FactionInfo.FactionNumber);
            }
            public override object Clone()
            {
                return new AndrosynthGuardianNormalPrimary(this);
            }
        }
        [Serializable]
        public class AndrosynthGuardianMeteor : Ship
        {
            public static EffectCollection DefaultEffectCollection = new EffectCollection();
            public static ShipState DefaultState;
            public static RigidBodyTemplate DefaultShape;
            public static ShipMovementInfo DefaultMovementInfo;
            public static ActionList DefaultActions = new ActionList();
            public static TargetingInfo DefaultTargetingTypes;
            public static ControlableSounds DefaultControlableSounds;
            public static ShipSounds DefaultShipSounds;
            static bool initialized = false;
            static void Initialize()
            {
                initialized = true;
                InitShape();
                DefaultMovementInfo = new ShipMovementInfo(
                    new Bounded<float>(100),
                    new Bounded<float>(TimeWarp.ScaleTurning(0.9f)),
                    new Bounded<float>(8000),
                    new Bounded<float>(TimeWarp.ScaleVelocity(60)));
                DefaultState = new ShipState(new Bounded<float>(20),
                                                new Bounded<float>(10),
                                                new Bounded<float>(0),
                                                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(8, -1), TimeWarp.RechargeRateToPerSeconds(8, -1), TimeWarp.RechargeRateToPerSeconds(8, -1), false));
                DefaultTargetingTypes = TargetingInfo.All;
                DefaultActions.Add(new NullAction());
                DefaultActions.Add(new NullAction());
                DefaultActions.Add(new NullAction());
                DefaultActions.Add(new NullTransformAction());
                DefaultActions[3].Costs = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
                DefaultActions[3].ActionSounds = new ActionSounds();
                DefaultEffectCollection.Effects.Add(new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-3, 0, 0, 0)));
                DefaultEffectCollection.AttachmentFlags = EffectAttachmentFlags.ClonedAttachment;
                DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
                DefaultShipSounds = new ShipSounds("AndrosynthGuardianDitty");
            }
            static void InitShape()
            {
                Coefficients DefaultCoefficients = TimeWarp.Coefficients;

                Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(80,0),
                    new Vector2D(40,40),
                    new Vector2D(0,30),
                    new Vector2D(-60,5),
                    new Vector2D(-60,-5),
                    new Vector2D(0,-20),
                    new Vector2D(40,-30)
                };

                IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);

                DefaultShape = new RigidBodyTemplate(11, 1236.3134770143224f, new IGeometry2D[] { mainhull }, new Coefficients[] { DefaultCoefficients });
                DefaultShape.BalanceBody();
                ////DefaultShape.CalcInertiaMultiplier(.1f);
            }
            public static IShip Create(PhysicsState state, FactionInfo factionInfo)
            {
                if (!initialized)
                {
                    Initialize();
                }

                AndrosynthGuardianMeteor returnvalue = new AndrosynthGuardianMeteor(state, factionInfo);
                returnvalue.ControlHandler = new DefaultControlHandler();
                return returnvalue;
            }
            /*public static ShipInfo GetShipInfo()
            {
                return new ShipInfo("Mmrnmhrm XForm Missile", new CreateShipDelegate(Create));
            }*/
            protected AndrosynthGuardianMeteor(PhysicsState state, FactionInfo factionInfo)
                : base(
                (LifeSpan)DefaultLifeTime.Clone(),
                state,
                DefaultBodyFlags,
                DefaultShape,
                new ShipMovementInfo(DefaultMovementInfo),
                new ShipState(DefaultState),
                DefaultControlableSounds,
                DefaultShipSounds,
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                new ActionList(DefaultActions),
                null)
            {
                this.controlableType = ControlableType.Ship;
            }
            public AndrosynthGuardianMeteor(AndrosynthGuardianMeteor copy)
                : base(copy)
            { }
            public override object Clone()
            {
                return new AndrosynthGuardianMeteor(this);
            }
        }
        [Serializable]
        public class AndrosynthGuardianSecondary : TransformAction
        {
            public static Costs DefaultCost;
            public static Bounded<float> DefaultDelay;
            public static TargetingInfo DefaultTargetingTypes;
            public static ActionSounds DefaultActionSounds;
            static bool initialized = false;
            static void Initialize()
            {
                initialized = true;

                DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
                DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
                DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Self);
                DefaultActionSounds = new ActionSounds("EchoBoom", null, null);

            }
            public static AndrosynthGuardianSecondary Creatw()
            {
                if (!initialized)
                {
                    Initialize();
                }
                return new AndrosynthGuardianSecondary();
            }
            AndrosynthGuardianSecondary()
                : base(new Bounded<float>(DefaultDelay),
                DefaultTargetingTypes,
                new Costs(DefaultCost),
                DefaultActionSounds,
                new ActionSounds(),
                new Costs(new ShipStateChange(), null, null),
                AndrosynthGuardianMeteor.Create(new PhysicsState(), null),
                true)
            {
                this.aIInfo = new SpecificRangeActionAIInfo(
                    new ShipStateChange(0, 5, 0, 0),
                    1000,
                    3000);
            }
            public AndrosynthGuardianSecondary(AndrosynthGuardianSecondary copy)
                : base(copy)
            { }
            public override object Clone()
            {
                return new AndrosynthGuardianSecondary(this);
            }
        }
    }
    [Serializable]
    public class BubbleController : MissileControler
    {
        static Random rand = new Random();
        public BubbleController(TargetingInfo targetingInfo)
            : base(targetingInfo)
        { }
        public BubbleController(BubbleController copy)
            : base(copy)
        { }
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {
            float rv = (float)rand.NextDouble() * 6;
            if (rv < 1 || rv > 5)
            {
                original.TorquePercent = (float)rand.NextDouble();
                if (rv < 1)
                {
                    original[InputAction.RotateRight] = true;
                }
                else
                {
                    original[InputAction.RotateLeft] = true;
                }
                int direction = rand.Next(0, 8);
                switch (direction)
                {
                    case 0:
                        original.ThrustPercent = (float)rand.NextDouble() * .5f;
                        original[InputAction.MoveLeft] = true;
                        break;
                    case 1:
                        original.ThrustPercent = (float)rand.NextDouble() * .5f;
                        original[InputAction.MoveRight] = true;
                        break;
                    default:
                        original.ThrustPercent = (float)rand.NextDouble();
                        original[InputAction.MoveForward] = true;
                        break;
                }
                return original;
            }
            return base.GetControlInput(dt, original);
        }
        public override object Clone()
        {
            return new BubbleController(this);
        }
    }
}
#endif
