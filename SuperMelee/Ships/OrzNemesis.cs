#region GPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a other of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
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
using ReMasters.SuperMelee.Weapons;
using ReMasters.SuperMelee.Controlers;
using ReMasters.SuperMelee.Effects;
using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class OrzNemesisShipLoader : ShipLoader
    {
        public OrzNemesisShipLoader() : base("Orz Nemesis") { }
        protected override IShip CreateHardCodedShip()
        {
            return OrzNemesis.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class OrzNemesis : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();

        public static IShip[] DefaultSubShips;

        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(1)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(5, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(35)));

            DefaultState = new ShipState(new Bounded<float>(16),
                new Bounded<float>(20),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 1)));

            DefaultActions.Add(OrzNemesisPrimary.Create());
            DefaultActions.Add(OrzNemesisSecondary.Create());
            DefaultActions.Add(OrzNemesisTernary.Create());

            DefaultSubShips = new IShip[1];
            DefaultSubShips[0] = OrzTurret.Create(new PhysicsState(), new FactionInfo(0));


            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("OrzNemesisDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(10,15),
                    new Vector2D(-40,20),
                    new Vector2D(-40,-20),
                    new Vector2D(10,-15)
                };
            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(40,10),
                    new Vector2D(-30,2),
                    new Vector2D(-50,-30),
                    new Vector2D(-40,-30)
                };
            int length = leftWingvertecies.Length;
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(-40,30),
                    new Vector2D(-50,30),
                    new Vector2D(-30,-2),
                    new Vector2D(40,-10)
                };
            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);
            IGeometry2D leftWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, -20)), leftWingvertecies);
            IGeometry2D RightWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, 20)), RightWingvertecies);
            DefaultShape = new RigidBodyTemplate(13, 869.439914791763f, new IGeometry2D[] { mainhull, RightWing, leftWing }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            OrzNemesis returnvalue = new OrzNemesis(state, factionInfo);
            returnvalue.ControlHandler = new ComplexShipControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Orz Nemesis", new CreateShipDelegate(Create));
        }*/
        protected static IShip[] GetSubShips(PhysicsState state, FactionInfo factionInfo)
        {
            IShip[] returnvalue = new IShip[1];
            returnvalue[0] = (IShip)DefaultSubShips[0].Clone();
            returnvalue[0].IgnoreInfo.IsCollidable = false;
            returnvalue[0].Current.Set(state);
            returnvalue[0].SetAllPositions();
            //returnvalue[0].FactionInfo = new FactionInfo(factionInfo);
            return returnvalue;
        }
        protected OrzNemesis(PhysicsState state, FactionInfo factionInfo)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(), 
            state, 
            DefaultBodyFlags, DefaultShape, 
            new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            DefaultShipSounds, 
            new ActionList(DefaultActions), 
            GetSubShips(state, factionInfo))
        { }
        OrzNemesis(OrzNemesis copy) : base(copy) { }
        public override void OnCreation(GameResult gameResult,FactionInfo factionInfo)
        {
            base.OnCreation(gameResult,factionInfo);
            subShips[0].Current.Position = this.current.Position;
            subShips[0].SetAllPositions();
            gameResult.AddJoint(new PinJoint(new CollidablePair(this, subShips[0]), this.current.Position.Linear, 0, .1f));
            subShips[0].IgnoreInfo.IsCollidable = false;
            subShips[0].ShipState = this.shipState;
        }
        public override object Clone()
        {
            return new OrzNemesis(this);
        }
    }
    [Serializable]
    public class OrzNemesisPrimary : BaseAction ,IGunAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(0);
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds();
        }
        public static OrzNemesisPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new OrzNemesisPrimary();
        }

        OrzNemesisPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds)
        {
            this.aIInfo = new GunActionAIInfo(costs.ActivationCost);
        }
        OrzNemesisPrimary(OrzNemesisPrimary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            source.SubShips[0].CurrentControlInput = new ControlInput(1);
            source.SubShips[0].CurrentControlInput[InputAction.Action] = true;
            source.SubShips[0].CurrentControlInput.ActiveActions[0] = true;
            source.SubShips[0].RunControlInput(actionResult, dt);
            source.SubShips[0].CurrentControlInput = null;
            return true;
        }
        public override object Clone()
        {
            return new OrzNemesisPrimary(this);
        }

        #region IGunAction Members

        public ISolidWeapon Weapon
        {
            get { return ((IGunAction)source.SubShips[0].Actions[0]).Weapon; }
        }

        public float VelocityAngle
        {
            get { return ((IGunAction)source.SubShips[0].Actions[0]).VelocityAngle - source.SubShips[0] .DirectionAngle- source.DirectionAngle; }
        }

        public float RandomVelocityAngle
        {
            get { return ((IGunAction)source.SubShips[0].Actions[0]).RandomVelocityAngle; }
        }

        #endregion
    }
    [Serializable]
    public class OrzNemesisSecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(0);
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds();
        }
        public static OrzNemesisSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new OrzNemesisSecondary();
        }

        OrzNemesisSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds)
        { }
        OrzNemesisSecondary(OrzNemesisSecondary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            source.SubShips[0].CurrentControlInput = new ControlInput(0);
            source.SubShips[0].CurrentControlInput[InputAction.RotateRight] = source.CurrentControlInput[InputAction.RotateRight];
            source.SubShips[0].CurrentControlInput[InputAction.RotateLeft] = source.CurrentControlInput[InputAction.RotateLeft];
            source.SubShips[0].RunControlInput(actionResult,dt);
            source.CurrentControlInput[InputAction.RotateRight] = false;
            source.CurrentControlInput[InputAction.RotateLeft] = false;
            source.CurrentControlInput.TorquePercent = 0;
            return true;
        }

        public override object Clone()
        {
            return new OrzNemesisSecondary(this);
        }
    }
    [Serializable]
    public class OrzNemesisTernary : GunAction
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
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(3)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(40, 4)),
                new Bounded<float>(TimeWarp.ScaleVelocity(40)));
            DefaultState = new ShipState(new Bounded<float>(3), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(1, 0, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(12));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(600, 80));
            DefaultEffectCollection.Effects.Add(new MedKitEffect(new TargetingInfo(TargetingTypes.None, TargetingTypes.Ally | TargetingTypes.Ship, TargetingTypes.None), EffectTypes.None, new EffectSounds(), 1));

            DefaultEffectCollection.ProlongedEffects.Add(new BordingPartyEffect(
                new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None),
                EffectTypes.Health,
                new EffectSounds("Intruder", "Zap", "Argh"),
                new LifeSpan(),
                new Bounded<float>(TimeWarp.RateToTime(7)), 1));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("Go", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            Vector2D[] hullverticies = new Vector2D[]
            {
                new Vector2D(5,5),
                new Vector2D(-5,5),
                new Vector2D(-5,-5),
                new Vector2D(5,-5)
            };
            IGeometry2D hull = new Polygon2D(ALVector2D.Zero, hullverticies);
            DefaultShape = new RigidBodyTemplate(MassInertia.FromRectangle(.1f, 10, 10), new IGeometry2D[] { hull }, new Coefficients[] { coe });
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
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null, new MannedMissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            w.UQMFlags |= ContFlags.CanDoGravityWhip;
            return w;
        }
        public static OrzNemesisTernary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new OrzNemesisTernary();
        }

        OrzNemesisTernary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           true,
           DefaultActionSounds,
           MathHelper.PI,
           MathHelper.PI,
           CreateWeapon())
        { }
    }
    [Serializable]
    public class OrzTurret : Ship
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
                new Bounded<float>(100),
                new Bounded<float>(TimeWarp.ScaleTurning(3)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(2, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(20)));
            DefaultState = new ShipState(new Bounded<float>(14),
                new Bounded<float>(32),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(50, 1)));
            DefaultActions.Add(OrzTurretPrimary.Create());

            DefaultControlableSounds = new ControlableSounds();
            DefaultShipSounds = new ShipSounds();
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            IGeometry2D mainhull = new Polygon2D(new ALVector2D(0, new Vector2D(40, 0)), Polygon2D.FromRectangle(10, 50));
            IGeometry2D engine = new Circle2D(20, new Vector2D(0, 0));
            DefaultShape = new RigidBodyTemplate(2, 1935.8061672698966f, new IGeometry2D[] { engine, mainhull }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients });
            //DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            OrzTurret returnvalue = new OrzTurret(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }

        protected OrzTurret(PhysicsState state, FactionInfo factionInfo)
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
        { }
    }
    [Serializable]
    public class OrzTurretPrimary : GunAction
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
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(4));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(20, 120));
            DefaultState = new ShipState(new Bounded<float>(3), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 6, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    TargetingInfo.All,
                    EffectTypes.None,
                    new EffectSounds("Boom23", null, null),
                    new ShipStateChange(-3, 0, 0, 0)));

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("Gun2", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(10,2.5f),
                new Vector2D(-10,5),
                new Vector2D(-10,-5),
                new Vector2D(10,-2.5f)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.5f, 37.644296109656715f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static ISolidWeapon CreateWeapon()
        {
            return new Controlable(
                DefaultLifeTime,
                new PhysicsState(),
                DefaultBodyFlags,
                DefaultShape,
                 new ShipMovementInfo(DefaultMovementInfo),
            new ShipState(DefaultState),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static OrzTurretPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new OrzTurretPrimary();
        }

        OrzTurretPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           0,
           0,
           CreateWeapon())
        { }
    }
}
#endif
