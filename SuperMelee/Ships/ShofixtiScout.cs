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
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class ShofixtiScoutShipLoader : ShipLoader
    {
        public ShofixtiScoutShipLoader() : base("Shofixti Scout") { }
        protected override IShip CreateHardCodedShip()
        {
            return ShofixtiScout.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class ShofixtiScout : Ship
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
                new Bounded<float>(TimeWarp.ScaleTurning(1)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(5, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(35)));
            DefaultState = new ShipState(
                new Bounded<float>(6),
                new Bounded<float>(4),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(9, 1)));

            DefaultActions.Add(ShofixtiScoutPrimary.Create());
            DefaultActions.Add(ShofixtiScoutSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("ShofixtiScoutDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            Vector2D[] scoutverticies = new Vector2D[]
            {
                new Vector2D(40,0),
                new Vector2D(-20,23),
                new Vector2D(-40,0),
                new Vector2D(-20,-23)
            };

            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, scoutverticies);
            DefaultShape = new RigidBodyTemplate(1, 377.05059301584328f, new IGeometry2D[] { mainhull }, new Coefficients[] { DefaultCoefficients, });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ShofixtiScout returnvalue = new ShofixtiScout(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Shofixti Scout", new CreateShipDelegate(Create));
        }*/
        protected ShofixtiScout(PhysicsState state, FactionInfo factionInfo)
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
    public class ShofixtiScoutPrimary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(TimeWarp.ScaleVelocity(96)));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(14, 96));
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(3));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("Gun1", null, null);
        }
        public static ShofixtiScoutPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ShofixtiScoutPrimary();
        }

        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(5,2),
                new Vector2D(-5,3),
                new Vector2D(-5,-3),
                new Vector2D(5,-2)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.001f, 10.549201161945275f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
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
        ShofixtiScoutPrimary()
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
    [Serializable]
    public class ShofixtiScoutSecondary : TransporterAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static float DefaultInitialRadius;
        public static float DefaultExpansionRate;
        public static float DefaultMass;
        public static LifeSpan DefaultLifeTime;
        public static int[] DefaultExposionColors = new int[] { Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Yellow.ToArgb() };
        public static int DefaultExplosionPrimaryColor = Color.Orange.ToArgb();
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static int DefaultLeverPulls;
        public static Bounded<float> DefaultDamageDelay;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), new ShipStateChange(0, 0, 0, 0), null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultDamageDelay = new Bounded<float>(.08f);
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultInitialRadius = ShofixtiScout.DefaultShape.BoundingRadius;
            DefaultExpansionRate = ShofixtiScout.DefaultShape.BoundingRadius + TimeWarp.ScaleRange(13);
            DefaultMass = 2;
            DefaultLeverPulls = 3;
            DefaultLifeTime = new LifeSpan(1);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-2, 0, 0, 0)));
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
            DefaultActionSounds = new ActionSounds();
        }
        public static ShofixtiScoutSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ShofixtiScoutSecondary();
        }

        static ISolidWeapon CreateWeapon()
        {
            return new ControlableWave(
            (LifeSpan)DefaultLifeTime.Clone(),
            DefaultMass,
            new PhysicsState(), DefaultInitialRadius,
            DefaultExpansionRate,
            DefaultExposionColors,
            DefaultExplosionPrimaryColor,
            new ShipMovementInfo(),
            new ShipState(100),
            new ControlableSounds("ScreamingDeath", null),
            new MultipleHitWeaponsLogic(
            TargetingInfo.Self,
            new EffectCollection(DefaultEffectCollection),
            new Bounded<float>(DefaultDamageDelay)));
        }
        int leverPulls;
        ShofixtiScoutSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon()
           )
        {
            this.leverPulls = DefaultLeverPulls;
            this.aIInfo = new SpecificRangeActionAIInfo(null, 0, 400);
        }
        ShofixtiScoutSecondary(ShofixtiScoutSecondary copy)
            : base(copy)
        {
            this.leverPulls = copy.leverPulls;
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            leverPulls--;
            if (leverPulls == 0)
            {
                source.Kill(actionResult);
                base.OnActivated(actionResult,dt);
            }
            return true;
        }
        protected override bool OnRunning(ActionResult actionResult, float dt)
        {
            return base.OnRunning(actionResult, dt);
        }
        public override object Clone()
        {
            return new ShofixtiScoutSecondary(this);
        }
    }
}
#endif
