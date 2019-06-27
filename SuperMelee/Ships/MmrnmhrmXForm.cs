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


    public class MmrnmhrmXFormShipLoader : ShipLoader
    {
        public MmrnmhrmXFormShipLoader() : base("Mmrnmhrm X-Form") { }
        protected override IShip CreateHardCodedShip()
        {
            return MmrnmhrmXForm.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class MmrnmhrmXForm
    {
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        static MmrnmhrmXForm()
        {

        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            return MmrnmhrmXFormLaser.Create(state, factionInfo);
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Mmrnmhrm X-Form", new CreateShipDelegate(MmrnmhrmXForm.Create));
        }*/
        [Serializable]
        class MmrnmhrmXFormLaser : Ship
        {
            public static ShipState DefaultState;
            public static RigidBodyTemplate DefaultShape;
            public static ShipMovementInfo DefaultMovementInfo;
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
                    new Bounded<float>(TimeWarp.ScaleTurning(2)),
                    new Bounded<float>(TimeWarp.ScaleAcceleration(5, 1)),
                    new Bounded<float>(TimeWarp.ScaleVelocity(20)));

                DefaultState = new ShipState(new Bounded<float>(20),
                    new Bounded<float>(10),
                    new Bounded<float>(0),
                    new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 2)));
                DefaultActions.Add(MmrnmhrmXFormLaserPrimary.Create());
                DefaultActions.Add(MmrnmhrmXFormSecondary.Create());
                DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
                DefaultShipSounds = new ShipSounds("MmrnmhrmXFormDitty");
            }
            static void InitShape()
            {
                Coefficients DefaultCoefficients = TimeWarp.Coefficients;

                Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(60,0),
                    new Vector2D(40,8),
                    new Vector2D(0,15),
                    new Vector2D(-40,10),
                    new Vector2D(-40,-10),
                    new Vector2D(0,-15),
                    new Vector2D(40,-8)
                };
                Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(10,10),
                    new Vector2D(-30,10),
                    new Vector2D(-30,-50),
                    new Vector2D(-20,-50)
                };
                int length = leftWingvertecies.Length;
                Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(-20,50),
                    new Vector2D(-30,50),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-10)
                };
                IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);
                IGeometry2D leftWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, -20)), leftWingvertecies);
                IGeometry2D RightWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, 20)), RightWingvertecies);
                DefaultShape = new RigidBodyTemplate(11, 1350.9334372293902f, new IGeometry2D[] { leftWing, RightWing, mainhull }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
                DefaultShape.BalanceBody();
                //////DefaultShape.CalcInertiaMultiplier(.1f);
            }
            public static IShip Create(PhysicsState state, FactionInfo factionInfo)
            {
                if (!initialized)
                {
                    Initialize();
                }

                MmrnmhrmXFormLaser returnvalue = new MmrnmhrmXFormLaser(state, factionInfo);
                returnvalue.ControlHandler = new DefaultControlHandler();
                return returnvalue;
            }
            /*public static ShipInfo GetShipInfo()
            {
                return new ShipInfo("Mmrnmhrm XForm Laser", new CreateShipDelegate(Create));
            }*/
            MmrnmhrmXFormLaser(PhysicsState state, FactionInfo factionInfo)
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
        class MmrnmhrmXFormLaserPrimary : RayAction
        {
            public static LifeSpan DefaultLifeTime;
            public static Costs DefaultCost;
            public static float DefaultRange;
            public static EffectCollection DefaultEffectCollection = new EffectCollection();

            public static Bounded<float> DefaultDelay;
            public static TargetingInfo DefaultTargetingTypes;
            public static ActionSounds DefaultActionSounds;

            static bool initialized = false;
            static void Initialize()
            {
                initialized = true;

                DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Other);
                DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

                DefaultRange = TimeWarp.ScaleRange(8);
                DefaultLifeTime = new LifeSpan(TimeWarp.RateToTime(0));
                DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultTargetingTypes, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));

                DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
                DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
                DefaultActionSounds = new ActionSounds("Laser4", null, null);
            }
            static DirectedRayWeapon CreateWeapon()
            {
                float angle = .22f;
                return new DirectedRayWeapon(
                    (LifeSpan)DefaultLifeTime.Clone(),
                    new WeaponsLogic(DefaultTargetingTypes,
                    new EffectCollection(DefaultEffectCollection)),
                    new float[] { 500, 500 },
                    new float[] { MathHelper.PI * .6f, MathHelper.PI * -.6f },
                    new float[] { -angle, angle },
                    new float[] { DefaultRange, DefaultRange });
            }
            public static MmrnmhrmXFormLaserPrimary Create()
            {
                if (!initialized)
                {
                    Initialize();
                }
                return new MmrnmhrmXFormLaserPrimary();
            }

            MmrnmhrmXFormLaserPrimary()
                : base(new Bounded<float>(DefaultDelay),
               DefaultTargetingTypes,
               new Costs(DefaultCost),
               false,
               DefaultActionSounds,
               CreateWeapon())
            { }

        }
        [Serializable]
        public class MmrnmhrmXFormMissile : Ship
        {
            public static ShipState DefaultState;
            public static RigidBodyTemplate DefaultShape;
            public static ShipMovementInfo DefaultMovementInfo;
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
                    new Bounded<float>(TimeWarp.ScaleTurning(13)),
                    new Bounded<float>(TimeWarp.ScaleAcceleration(10, 0)),
                    new Bounded<float>(TimeWarp.ScaleVelocity(50)));
                DefaultState = new ShipState(new Bounded<float>(20),
                                                new Bounded<float>(10),
                                                new Bounded<float>(0),
                                                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 1)));

                DefaultActions.Add(MmrnmhrmXFormMissilePrimary.Create());

                NullTransformAction t = new NullTransformAction();
                t.AIInfo = new SpecificRangeActionAIInfo(
                           MmrnmhrmXFormSecondary.DefaultCost.ActivationCost,
                           0,
                           800);
                DefaultActions.Add(t);

                DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
                DefaultShipSounds = new ShipSounds("MmrnmhrmXFormDitty");
            }
            static void InitShape()
            {
                Coefficients DefaultCoefficients = TimeWarp.Coefficients;

                Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(60,0),
                    new Vector2D(40,8),
                    new Vector2D(0,15),
                    new Vector2D(-40,10),
                    new Vector2D(-40,-10),
                    new Vector2D(0,-15),
                    new Vector2D(40,-8)
                };
                Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(10,10),
                    new Vector2D(-30,10),
                    new Vector2D(-50,-30),
                    new Vector2D(-40,-30)
                };
                int length = leftWingvertecies.Length;
                Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(-40,30),
                    new Vector2D(-50,30),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-10)
                };
                IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);
                IGeometry2D leftWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, -20)), leftWingvertecies);
                IGeometry2D RightWing = new Polygon2D(new ALVector2D(0, new Vector2D(-10, 20)), RightWingvertecies);
                DefaultShape = new RigidBodyTemplate(11, 1129.1553811562187f, new IGeometry2D[] { leftWing, RightWing, mainhull }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
                DefaultShape.BalanceBody();
                //////DefaultShape.CalcInertiaMultiplier(.1f);
            }
            public static IShip Create(PhysicsState state, FactionInfo factionInfo)
            {
                if (!initialized)
                {
                    Initialize();
                }

                MmrnmhrmXFormMissile returnvalue = new MmrnmhrmXFormMissile(state, factionInfo);
                returnvalue.ControlHandler = new DefaultControlHandler();
                return returnvalue;
            }
            /*public static ShipInfo GetShipInfo()
            {
                return new ShipInfo("Mmrnmhrm XForm Missile", new CreateShipDelegate(Create));
            }*/
            protected MmrnmhrmXFormMissile(PhysicsState state, FactionInfo factionInfo)
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
        public class MmrnmhrmXFormMissilePrimary : GunAction
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
                    new Bounded<float>(TimeWarp.ScaleTurning(9)),
                    new Bounded<float>(10000),
                    new Bounded<float>(TimeWarp.ScaleVelocity(80)));

                DefaultState = new ShipState(new Bounded<float>(1),
                    new Bounded<float>(0),
                    new Bounded<float>(0),
                    new Bounded<float>(0));
                DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
                DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(20));
                DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(50, 80));

                DefaultEffectCollection.Effects.Add(
                    new ShipStateEffect(
                    TargetingInfo.All,
                    EffectTypes.None,
                    new EffectSounds("Boom1", null, null),
                    new ShipStateChange(-1, 0, 0, 0)));
                DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Other | TargetingTypes.Ship, TargetingTypes.None);
                DefaultEffectsWho = TargetingInfo.All;
                DefaultActionSounds = new ActionSounds("MissileLaunch2", null, null);
            }
            static void InitShape()
            {
                Coefficients coe = TimeWarp.Coefficients;

                Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(15,2.5f),
                new Vector2D(-15,2.5f),
                new Vector2D(-15,-2.5f),
                new Vector2D(15,-2.5f)
            };
                IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
                DefaultShape = new RigidBodyTemplate(.01f, 77.585000000013721f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
                DefaultShape.BalanceBody();
                ////DefaultShape.CalcInertiaMultiplier(.1f);
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
                new WeaponsLogic(DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)));
                w.ControlHandler = new DefaultControlHandler();
                w.AddControler(null,new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
                return w;
            }
            public static MmrnmhrmXFormMissilePrimary Create()
            {
                if (!initialized)
                {
                    Initialize();
                }
                return new MmrnmhrmXFormMissilePrimary();
            }

            MmrnmhrmXFormMissilePrimary()
                : base(new Bounded<float>(DefaultDelay),
               DefaultTargetingTypes,
               new Costs(DefaultCost),
               true,
               DefaultActionSounds,
               new float[] { 0, 0 } ,
               new float[] { (MathHelper.PI / 2 + .3f), -(MathHelper.PI / 2 + .3f) } ,
               new ISolidWeapon[] { CreateWeapon(), CreateWeapon() })
            { }
        }
        [Serializable]
        public class MmrnmhrmXFormSecondary : TransformAction
        {
            public static Costs DefaultCost = new Costs(new ShipStateChange(0, 10, 0, 0), null, null);
            public static Bounded<float> DefaultDelay;
            public static TargetingInfo DefaultTargetingTypes;
            public static ActionSounds DefaultActionSounds;

            static bool initialized = false;
            static void Initialize()
            {
                initialized = true;

                DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
                DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Self);
                DefaultActionSounds = new ActionSounds("Transform", null, null);
            }
            public static MmrnmhrmXFormSecondary Create()
            {
                if (!initialized)
                {
                    Initialize();
                }
                return new MmrnmhrmXFormSecondary();
            }

            MmrnmhrmXFormSecondary()
                : base(new Bounded<float>(DefaultDelay),
               DefaultTargetingTypes,
               new Costs(DefaultCost),
               DefaultActionSounds,
               DefaultActionSounds,
               new Costs(DefaultCost),
               MmrnmhrmXFormMissile.Create(new PhysicsState(), null),
               false)
            {
                this.aIInfo = new SpecificRangeActionAIInfo(
                       DefaultCost.ActivationCost,
                       1000,
                       3000);
            }
        }
    }
}
#endif
