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
    public class KohrAhMarauderShipLoader : ShipLoader
    {
        public KohrAhMarauderShipLoader() : base("Kohr-Ah Marauder") { }
        protected override IShip CreateHardCodedShip()
        {
            return KohrAhMarauder.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class KohrAhMarauder : Ship
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
                new Bounded<float>(TimeWarp.ScaleTurning(4)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 6)),
                new Bounded<float>(TimeWarp.ScaleVelocity(30)));
            DefaultState = new ShipState(new Bounded<float>(42),
                new Bounded<float>(42),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add(KohrAhMarauderPrimary.Create());
            DefaultActions.Add(KohrAhMarauderSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("KohrAhMarauderDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();




            Vector2D[] engineconevertecies = new Vector2D[]
            {
                new Vector2D(20,10),
                new Vector2D(-50,40),
                new Vector2D(-100,40),
                new Vector2D(-100,-40),
                new Vector2D(-50,-40),
                new Vector2D(20,-10)
            };
            Vector2D offset = Polygon2D.CalcCentroid(engineconevertecies);
            //engineconevertecies = Vector2D.Translate(-offset, engineconevertecies);

            OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(
                engineconevertecies,
                ref offset,
                engineconevertecies,
                Vector2D.Subtract);


            goes.Add(new Polygon2D(new ALVector2D(0, offset), engineconevertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(10, 0)), Polygon2D.FromNumberofSidesAndRadius(8, 40)));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(20, 1995.1433515249612f, goes.ToArray(), coes.ToArray());

            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {

            if (!initialized)
            {
                Initialize();
            }

            KohrAhMarauder returnvalue = new KohrAhMarauder(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Kohr-Ah Marauder", new CreateShipDelegate(Create));
        }*/
        protected KohrAhMarauder(PhysicsState state, FactionInfo factionInfo)
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
    public class KohrAhMarauderPrimary : GunAction
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

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultEffectsWho = new TargetingInfo(TargetingTypes.Enemy | TargetingTypes.Debris);
            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleAcceleration(4, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(64)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(6));
            DefaultLifeTime = new LifeSpan(20);
            DefaultState = new ShipState(new Bounded<float>(6), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 6, 0, 0), new ShipStateChange(), new ShipStateChange());

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-4, 0, 0, 0)));

            DefaultActionSounds = new ActionSounds("MechanicalGun", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(30,10),
                new Vector2D(-10,10),
                new Vector2D(0,-10),
                new Vector2D(10,-10),
            };
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();
            for (int pos = 0; pos < 4; ++pos)
            {
                goes.Add(new Polygon2D(
                    new ALVector2D(MathHelper.PI * .5f * pos, Vector2D.FromLengthAndAngle(10, MathHelper.PI * .5f * pos)),
                    bulletverticies));
                coes.Add(coe);
            }
            DefaultShape = new RigidBodyTemplate(.9f, 429.46676883367536f, goes.ToArray(), coes.ToArray());
            //DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon solidweapon = new Controlable(
                DefaultLifeTime,
                new PhysicsState(),
                DefaultBodyFlags,
                DefaultShape,
                 new ShipMovementInfo(DefaultMovementInfo),
            new ShipState(DefaultState),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
            return solidweapon;
        }
        public static KohrAhMarauderPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KohrAhMarauderPrimary();
        }

        KohrAhMarauderPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           0,
           0,
           CreateWeapon())
        {
            this.aIInfo = new RemoteControlGunActionAIInfo();
        }
        public KohrAhMarauderPrimary(KohrAhMarauderPrimary copy) : base(copy) { }
        protected override void OnWeaponCreation(ActionResult actionResult, float dt,int index)
        {
            CurrentWeapon.Current.Velocity.Angular = 5;
            CurrentWeapon.IgnoreInfo.JoinGroupToIgnore(CurrentWeapon.FactionInfo.FactionNumber);
        }
        protected override bool OnRunning(ActionResult actionResult, float dt)
        {
            return !(CurrentWeapon == null || CurrentWeapon.IsExpired);
        }
        protected override bool OnDeActivated(ActionResult actionResult, float dt)
        {
            if (CurrentWeapon == null || CurrentWeapon.IsExpired)
            {
                return false;
            }
            else
            {
                CurrentWeapon.ControlHandler = new TargetingControlHandler();
                CurrentWeapon.MovementInfo.MaxLinearVelocity.Value = TimeWarp.ScaleVelocity(4);
                CurrentWeapon.Current.Velocity.Linear = Vector2D.Zero;
                CurrentWeapon.AddControler(actionResult, new ProximityTargetingControler(600, this.targetableTypes));
                CurrentWeapon = null;
                return true;
            }
        }
        public override object Clone()
        {
            return new KohrAhMarauderPrimary(this);
        }
    }
    [Serializable]
    public class KohrAhMarauderSecondary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static ActionSounds DefaultActionSounds;
        public static float[] DefaultOffsetAngles;
        public static float[] DefaultVelocityAngles;



        static int anglecount = 20;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultEffectsWho = DefaultTargetingTypes;

            InitShape();
            DefaultState = new ShipState(new Bounded<float>(99), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(20)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(5, 20));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(9));
            DefaultCost = new Costs(new ShipStateChange(0, 21, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new ShipStateChange(-3, 0, 0, 0)));
            DefaultActionSounds = new ActionSounds("FlameThrower2", null, null);

            DefaultOffsetAngles = new float[anglecount];
            DefaultVelocityAngles = new float[anglecount];
            float da = (2 * MathHelper.PI) / (float)anglecount;
            int pos = 0;
            for (float angle = 0; angle < 2 * MathHelper.PI; angle += da, pos++)
            {
                DefaultOffsetAngles[pos] = angle;
                DefaultVelocityAngles[pos] = angle;
            }
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 20));
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.001f, 20), new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static ISolidWeapon[] CreateWeapons()
        {
            ISolidWeapon[] rv = new ISolidWeapon[anglecount];
            for (int pos = 0; pos < anglecount; ++pos)
            {
                rv[pos] = new ControlableWave(
                    DefaultLifeTime,
                    1,
                    new PhysicsState(),
                    20,
                    30,
                    TimeWarp.DefaultExposionColors,
                    TimeWarp.DefaultExplosionPrimaryColor,
                    new ShipMovementInfo(DefaultMovementInfo),
                    new ShipState(DefaultState),
                    new ControlableSounds(),
                    new WeaponsLogic(DefaultEffectsWho,
                        new EffectCollection(DefaultEffectCollection)));
            }
            return rv;
        }

        public static KohrAhMarauderSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KohrAhMarauderSecondary();
        }

        KohrAhMarauderSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           DefaultVelocityAngles,
           DefaultOffsetAngles,
           CreateWeapons())
        {
            this.aIInfo = new OffensiveShieldActionAIInfo(DefaultCost.ActivationCost,
                Weapons[0].LifeTime.TimeLeft * Weapons[0].MovementInfo.MaxLinearVelocity);
        }
        public KohrAhMarauderSecondary(KohrAhMarauderSecondary copy) : base(copy) { }
        protected override void OnWeaponCreation(ActionResult actionResult, float dt, int index)
        {
            this.CurrentWeapons[index].Flags &= ~BodyFlags.NoImpulse;
            this.CurrentWeapons[index].IgnoreInfo.JoinGroupToIgnore(this.CurrentWeapons[index].FactionInfo.FactionNumber);
        }
        public override object Clone()
        {
            return new KohrAhMarauderSecondary(this);
        }
    }
}
#endif
