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
    public class PkunkFuryShipLoader : ShipLoader
    {
        public PkunkFuryShipLoader() : base("Pkunk Fury") { }
        protected override IShip CreateHardCodedShip()
        {
            return PkunkFury.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class PkunkFury : Ship
    {
        static Random rand = new Random();
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;

        public static float DefaultRespawnChance;
        public static MeleeSound DefaultRespawnSound;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(0)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(16, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(64)));
            DefaultState = new ShipState(new Bounded<float>(8),
                new Bounded<float>(12),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(50, 1)));

            DefaultActions.Add(PkunkFuryPrimary.Create());
            DefaultActions.Add(PkunkFurySecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("PkunkFuryDitty");
            DefaultRespawnSound = new MeleeSound("Hallelujah");
            DefaultRespawnChance = .5f;
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();


            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(20,-10),
                    new Vector2D(5,5),
                    new Vector2D(-10,5),
                    new Vector2D(20,-30)
                };
            int length = leftWingvertecies.Length;
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(20,10),
                    new Vector2D(20,30),
                    new Vector2D(-10,-5),
                    new Vector2D(5,-5)
                };
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(15, -5)), leftWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(15, 5)), RightWingvertecies));
            coes.Add(DefaultCoefficients);


            Vector2D[] RightWingvertecies2 = (Vector2D[])RightWingvertecies.Clone();
            Vector2D[] leftWingvertecies2 = (Vector2D[])leftWingvertecies.Clone();
            for (int pos = RightWingvertecies2.Length - 1; pos > -1; --pos)
            {
                RightWingvertecies2[pos] *= .7f;
            }
            for (int pos = leftWingvertecies2.Length - 1; pos > -1; --pos)
            {
                leftWingvertecies2[pos] *= .7f;
            }
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-15, 5)), leftWingvertecies2));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-15, -5)), RightWingvertecies2));
            coes.Add(DefaultCoefficients);


            Vector2D[] RightWingvertecies3 = new Vector2D[]
                {
                    new Vector2D(5,5),
                    new Vector2D(0,7),
                    new Vector2D(-15,7),
                    new Vector2D(-20,5),
                    new Vector2D(-10,-40),
                    new Vector2D(0,-40)
                };
            Vector2D[] leftWingvertecies3 = new Vector2D[]
                {
                    new Vector2D(0,40),
                    new Vector2D(-10,40),
                    new Vector2D(-20,-5),
                    new Vector2D(-15,-7),
                    new Vector2D(0,-7),
                    new Vector2D(5,-5)
                };

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-8, 40)), leftWingvertecies3));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-8, -40)), RightWingvertecies3));
            coes.Add(DefaultCoefficients);





            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, 0)), Polygon2D.FromRectangle(7, 50)));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(7, 809.028931f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            PkunkFury returnvalue = new PkunkFury(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Pkunk Fury", new CreateShipDelegate(Create));
        }*/

        float respawnChance;
        MeleeSound respawnSound;
        protected PkunkFury(PhysicsState state, FactionInfo factionInfo)
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
            this.respawnChance = DefaultRespawnChance;
            this.respawnSound = DefaultRespawnSound;
        }
        protected PkunkFury(PkunkFury copy)
            : base(copy)
        {
            this.respawnChance = copy.respawnChance;
            this.respawnSound = copy.respawnSound;
        }
        public override void Kill(GameResult gameResult)
        {
            if ((float)rand.NextDouble() <= respawnChance)
            {
                this.current.Velocity = ALVector2D.Zero;
                this.current.Position.Linear += Vector2D.FromLengthAndAngle(1200, (float)rand.NextDouble() * MathHelper.TWO_PI);
                this.shipState.Health.Fill();
                this.shipState.Energy.Fill();
                this.movementInfo.MaxAngularAcceleration.Fill();
                this.movementInfo.MaxAngularVelocity.Fill();
                this.movementInfo.MaxLinearAcceleration.Fill();
                this.movementInfo.MaxLinearVelocity.Fill();
                this.attachedEffectCollection.Clear();
                BlockingSheild sheild = new BlockingSheild(TargetingInfo.Self, EffectTypes.None, new EffectSounds(), new LifeSpan(0), EffectTypes.ShipStateMask | EffectTypes.MovementInfoMask);
                this.attachedEffectCollection.ProlongedEffects.Add(sheild);
                respawnSound.Play();
            }
            else
            {
                base.Kill(gameResult);
            }
        }
        public override object Clone()
        {
            return new PkunkFury(this);
        }
    }
    [Serializable]
    public class PkunkFuryPrimary : GunAction
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

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(96)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(5.5f, 96));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultEffectsWho = TargetingInfo.All;
            DefaultActionSounds = new ActionSounds("Gun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 7),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 7)) },
                new Coefficients[] { coe });
            //DefaultShape.BalanceBody();
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
            new WeaponsLogic(DefaultEffectsWho,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static PkunkFuryPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new PkunkFuryPrimary();
        }

        PkunkFuryPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           new float[] { -MathHelper.PI / 2, 0, MathHelper.PI / 2 },
           new float[] { -MathHelper.PI / 2, 0, MathHelper.PI / 2 },
           new ISolidWeapon[] { CreateWeapon(), CreateWeapon(), CreateWeapon() })
        { }
    }
    [Serializable]
    public class PkunkFurySecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, -2, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(16));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds();
            DefaultActionSounds.Activated.Names = new List<string>(14);
            for (int pos = 1; pos < 15; ++pos)
            {
                DefaultActionSounds.Activated.Names.Add("Insult" + pos.ToString().PadLeft(2, '0'));
            }
        }
        public static PkunkFurySecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new PkunkFurySecondary();
        }

        PkunkFurySecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds)
        {
            this.aIInfo = new RechargeActionAIInfo(DefaultCost.ActivationCost);
        }
        PkunkFurySecondary(PkunkFurySecondary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            return true;
        }
        public override object Clone()
        {
            return new PkunkFurySecondary(this);
        }
    }
}
#endif
