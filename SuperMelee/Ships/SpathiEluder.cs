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
    public class SpathiEluderShipLoader : ShipLoader
    {
        public SpathiEluderShipLoader() : base("Spathi Eluder") { }
        protected override IShip CreateHardCodedShip()
        {
            return SpathiEluder.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class SpathiEluder : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(12, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(48)));
            DefaultState = new ShipState(new Bounded<float>(30),
                new Bounded<float>(10),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(10, 1)));

            DefaultActions.Add(SpathiEluderPrimary.Create());
            DefaultActions.Add(SpathiEluderSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("SpathiEluderDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> geometries = new List<IGeometry2D>();
            List<Coefficients> coefficients = new List<Coefficients>();

            float da = MathHelper.PI * 2 / 5;
            float podlength = 50;
            for (float angle = 0; angle < MathHelper.PI * 2; angle += da)
            {
                Vector2D direction = Vector2D.Rotate(angle - MathHelper.PI, Vector2D.XAxis);
                if (angle == 0)
                {
                    geometries.Add(new Polygon2D(new ALVector2D(angle - MathHelper.PI, direction * .3f * podlength), Polygon2D.FromRectangle(9, podlength * .6f)));
                    geometries.Add(new Polygon2D(new ALVector2D(0, direction * podlength * .6f), Polygon2D.FromNumberofSidesAndRadius(8, 10)));

                }
                else
                {
                    geometries.Add(new Polygon2D(new ALVector2D(angle - MathHelper.PI, direction * .5f * podlength), Polygon2D.FromRectangle(9, podlength)));
                    geometries.Add(new Polygon2D(new ALVector2D(0, direction * podlength), Polygon2D.FromNumberofSidesAndRadius(8, 10)));
                }
                coefficients.Add(DefaultCoefficients);
                coefficients.Add(DefaultCoefficients);
            }
            geometries.Add(new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 15)));
            coefficients.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(16, 1569.670392957858f, geometries.ToArray(), coefficients.ToArray());
            DefaultShape.BalanceBody();
            // ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }
            SpathiEluder returnvalue = new SpathiEluder(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Spathi Eluder", new CreateShipDelegate(Create));
        }*/
        protected SpathiEluder(PhysicsState state, FactionInfo factionInfo)
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
    public class SpathiEluderPrimary : GunAction
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
                new Bounded<float>(TimeWarp.ScaleVelocity(96)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(17, 96));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("Gun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 10),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 10)) },
                new Coefficients[] { coe });
            DefaultShape.BalanceBody();
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
        public static SpathiEluderPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            SpathiEluderPrimary rv= new SpathiEluderPrimary();
            rv.aIInfo = null;
            return rv;
        }
        SpathiEluderPrimary()
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
    public class SpathiEluderSecondary : GunAction
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
                new Bounded<float>(TimeWarp.ScaleTurning(1)),
                new Bounded<float>(8000),
                new Bounded<float>(TimeWarp.ScaleVelocity(45)));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 3, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(7));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(12, 45));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    TargetingInfo.All,
                    EffectTypes.None,
                    new EffectSounds("Boom23", null, null),
                    new ShipStateChange(-2, 0, 0, 0)));

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("Fart", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(10,10),
                new Vector2D(-10,5),
                new Vector2D(-10,-5),
                new Vector2D(10,-10),
                new Vector2D(15,0),
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.3f, 61.846841579974196f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
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
            w.AddControler(null,new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            return w;
        }
        public static SpathiEluderSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SpathiEluderSecondary();
        }

        SpathiEluderSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           MathHelper.PI,
           MathHelper.PI,
           CreateWeapon())
        { }
    }
}
#endif
