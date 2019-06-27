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

using System;
using Physics2D;
using Physics2D.CollidableBodies;
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using Color = System.Drawing.Color;
using ReMasters.SuperMelee;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;

namespace ReMasters.SuperMelee.Ships
{
    [Serializable]
    class Asteroid : Controlable
    {
        public static float TimeForPrimaryExplosion = .4f;
        public static float TimeForSecondaryExplosions = .2f;
        public static Random rand = new Random();
        public static Coefficients DefaultCoefficients;
        public static LifeSpan DefaultLifeTime;
        public static ShipMovementInfo DefaultShipMovementInfo;
        public static ShipState DefaultShipState;

        public static IEffect[] DefaultEffects;
        public static IProlongedEffect[] DefaultProlongedEffects;

        public static int[] DefaultColors = new int[] { Color.Gray.ToArgb(), Color.DimGray.ToArgb(), Color.LightGray.ToArgb() };
        public static int DefaultPrimaryColor = Color.Gray.ToArgb();

        public static IEffect[] GetClonedDefaultDefaultEffects()
        {
            int length = DefaultEffects.Length;
            IEffect[] effects = new IEffect[length];
            for (int pos = 0; pos < length; pos++)
            {
                effects[pos] = (IEffect)DefaultEffects[pos].Clone();
            }
            return effects;
        }
        public static IProlongedEffect[] GetClonedDefaultDefaultProlongedEffects()
        {
            int length = DefaultProlongedEffects.Length;
            IProlongedEffect[] effects = new IProlongedEffect[length];
            for (int pos = 0; pos < length; pos++)
            {
                effects[pos] = (IProlongedEffect)DefaultProlongedEffects[pos].Clone();
            }
            return effects;
        }
        public static float DefaultCollisonDamageToSelf = 1;
        public static FactionInfo AsteroidFaction;
        public static RigidBodyTemplate[] Templates;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        static Asteroid()
        {
            InitShapes();
            DefaultLifeTime = new LifeSpan(120);
            DefaultShipMovementInfo = new ShipMovementInfo(new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultShipState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultEffects = new IEffect[1];
            DefaultEffects[0] = new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(3, 0, 0, 0));
            DefaultProlongedEffects = new IProlongedEffect[0];
            AsteroidFaction = FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Debris");
            AsteroidFaction.FactionType = FactionType.Debris;
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(new TargetingInfo(TargetingTypes.None,TargetingTypes.None ,TargetingTypes.Debris), EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
        }
        static void InitShapes()
        {
            DefaultCoefficients = new Coefficients(1.1f, .2f, .2f);
            int length = 6;
            Templates = new RigidBodyTemplate[length];
            List<Vector2D> vertexes = new List<Vector2D>();
            for (int pos = 0; pos < length; ++pos)
            {
                int numberofSides = rand.Next(5, 10);
                float radius = 20 + (float)rand.NextDouble() * 20;
                Vector2D[] poly = Polygon2D.FromNumberofSidesAndRadius(numberofSides, radius);
                vertexes.AddRange(poly);
                for (int numbertoRemove = rand.Next(0, numberofSides - 4); numbertoRemove > 0; --numbertoRemove)
                {
                    vertexes.RemoveAt(rand.Next(0, vertexes.Count));
                }
                Polygon2D polly = new Polygon2D(ALVector2D.Zero, poly);
                poly = Polygon2D.MakeCentroidOrigin(vertexes.ToArray());
                MassInertia mi = MassInertia.FromSolidCylinder(1 + (float)rand.NextDouble() * 2, polly.BoundingRadius);
                Templates[pos] = new RigidBodyTemplate(mi, new IGeometry2D[] { polly }, new Coefficients[] { DefaultCoefficients });
                vertexes.Clear();
            }
        }
        Bounded<float> delay = new Bounded<float>(.5f);



        public static void CreateDebris(GameResult gameResult, IControlable wrecked)
        {
            PhysicsState state;
            int length = wrecked.CollidableParts.Length;
            float totalArea = 0;
            float[] areas = new float[length];
            for (int pos = 0; pos < length; ++pos)
            {
                totalArea += (areas[pos] = wrecked.CollidableParts[pos].BaseGeometry.Area);
            }
            float totalAreaInv = 1 / totalArea;
            if (wrecked is IShip)
            {
                float smallest = float.MaxValue;
                ControlableWave wave;
                for (int pos = 0; pos < length; ++pos)
                {

                    state = new PhysicsState(wrecked.Current);
                    state.Position.Linear = wrecked.CollidableParts[pos].Position.Linear + new Vector2D((float)((rand.NextDouble() - .5f) * 2) * wrecked.CollidableParts[pos].BaseGeometry.InnerRadius, (float)((rand.NextDouble() - .5f) * 2) * wrecked.CollidableParts[pos].BaseGeometry.InnerRadius);
                    smallest = MathHelper.Min(smallest, wrecked.CollidableParts[pos].BaseGeometry.InnerRadius);


                    wave = new ControlableWave(
                        new LifeSpan(TimeForSecondaryExplosions),
                        wrecked.MassInfo.Mass * totalAreaInv * areas[pos] * .5f,
                        state,
                        wrecked.CollidableParts[pos].BaseGeometry.InnerRadius,
                        (wrecked.BoundingRadius - wrecked.CollidableParts[pos].BaseGeometry.InnerRadius) / TimeForSecondaryExplosions,
                        TimeWarp.DefaultExposionColors,
                        TimeWarp.DefaultExplosionPrimaryColor,
                        new ShipMovementInfo(DefaultShipMovementInfo),
                        new ShipState(DefaultShipState),
                        AsteroidFaction,
                        new ControlableSounds());

                    wave.OnCreation(gameResult, new FactionInfo(AsteroidFaction));
                }
                state = new PhysicsState(wrecked.Current);
                wave = new ControlableWave(new LifeSpan(TimeForPrimaryExplosion),
                    wrecked.MassInfo.Mass * .5f,
                    state,
                    smallest,
                    (wrecked.BoundingRadius - smallest) / TimeForPrimaryExplosion,
                    TimeWarp.DefaultExposionColors,
                    TimeWarp.DefaultExplosionPrimaryColor,
                    new ShipMovementInfo(DefaultShipMovementInfo),
                    new ShipState(DefaultShipState),
                    AsteroidFaction,
                    new ControlableSounds());
                wave.OnCreation(gameResult, new FactionInfo(AsteroidFaction));
            }
            for (int pos = 0; pos < length; ++pos)
            {

                state = new PhysicsState(wrecked.Current);
                state.Position = wrecked.CollidableParts[pos].Position;
                MassInertia massInfo;
                float mass = wrecked.MassInfo.Mass * totalAreaInv * areas[pos];
                if (wrecked.CollidableParts[pos].UseCircleCollision)
                {
                    massInfo = MassInertia.FromSolidCylinder(mass, wrecked.CollidableParts[pos].BaseGeometry.BoundingRadius);
                }
                else
                {
                    wrecked.CollidableParts[pos].SetPosition(ALVector2D.Zero);
                    wrecked.CollidableParts[pos].CalcBoundingBox2D();
                    BoundingBox2D box = wrecked.CollidableParts[pos].BoundingBox2D;
                    massInfo = MassInertia.FromRectangle(mass, box.Upper.X - box.Lower.X, box.Upper.Y - box.Lower.Y);
                }
                //ColoredRigidBodyPart part = new ColoredRigidBodyPart(ALVector2D.Zero, state.Position, wrecked.CollidableParts[pos].BaseGeometry, DefaultCoefficients, DefaultColors, DefaultPrimaryColor);
                RigidBodyPart part = new RigidBodyPart(ALVector2D.Zero, state.Position, wrecked.CollidableParts[pos].BaseGeometry, DefaultCoefficients);
                Asteroid roid = new Asteroid(part, massInfo, state);
                roid.OnCreation(gameResult, new FactionInfo(AsteroidFaction));
            }
        }
        public static Asteroid CreateAsteroid(GameResult gameResult, PhysicsState state)
        {
            Asteroid roid = new Asteroid(Templates[rand.Next(Templates.Length)], state);
            roid.OnCreation(gameResult, new FactionInfo(AsteroidFaction));
            return roid;
        }
        protected Asteroid(ICollidableBodyPart part, MassInertia massInfo, PhysicsState physicsState)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(),
            massInfo,
            physicsState,
            BodyFlags.None,
            new ICollidableBodyPart[] { part },
            new ShipMovementInfo(DefaultShipMovementInfo),
            new ShipState(rand.Next(1, 6)),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.None,
                new EffectCollection(DefaultEffectCollection)))
        {
            this.factionInfo = new FactionInfo(AsteroidFaction);
            this.controlableType = ControlableType.Debris | ControlableType.Weapon;
            this.weaponInfo.EffectCollection.AttachmentFlags = EffectAttachmentFlags.ClonedAttachment;
            this.shipState.Health = new Bounded<float>(4);
        }
        protected Asteroid(RigidBodyTemplate template, PhysicsState physicsState)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(),
            physicsState,
            BodyFlags.None,
            template,
            new ShipMovementInfo(DefaultShipMovementInfo),
            new ShipState(rand.Next(1, 6)),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.None,
                new EffectCollection(DefaultEffectCollection)))
        {
            this.factionInfo = new FactionInfo(AsteroidFaction);
            this.controlableType = ControlableType.Debris | ControlableType.Weapon;
            this.weaponInfo.EffectCollection.AttachmentFlags = EffectAttachmentFlags.WeaponExpires;
        }
        public Asteroid(Asteroid copy) : base(copy) { }
        public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            base.OnCreation(gameResult, factionInfo);
            this.weaponInfo.OnCreation(gameResult, this, null, new NullAction());
        }
        public override bool IsThreatTo(IControlable other)
        {
            return true;
        }
        public override void OnCollision(GameResult gameResult, IControlable collider)
        {
            if (delay.IsFull)
            {
                delay.Empty();
                if (rand.NextDouble() > .9 && !(collider is ImpulseWave))
                {
                    this.ShipState.Health.Value -= DefaultCollisonDamageToSelf;
                }
                base.OnCollision(gameResult, collider);
            }
        }
        public override void Update(float dt)
        {
            delay.Value += dt;
            base.Update(dt);
        }
        public override object Clone()
        {
            return new Asteroid(this);
        }
    }
}
