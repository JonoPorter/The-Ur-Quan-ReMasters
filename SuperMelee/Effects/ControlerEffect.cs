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
using System.Collections.Generic;
using System.Text;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;

namespace ReMasters.SuperMelee.Effects
{

    [Serializable]
    public class ControlerEffect : BaseProlongedEffect
    {
        protected IControler controler;
        public ControlerEffect(TargetingInfo effectsWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds, LifeSpan lifeTime, IControler controler)
            : base(effectsWho, harmfulEffectTypes | EffectTypes.Controls, effectSounds, lifeTime)
        {
            this.controler = controler;
        }
        protected ControlerEffect(ControlerEffect copy)
            : base(copy)
        {
            this.controler = (IControler)copy.controler.Clone();
        }
        public override void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie)
        {
            base.OnTargetAttachment(attachmentResult, attachie);
            attachie.AddControler(attachmentResult, controler);
            this.isTargetAttached = true;
        }
        public override void OnExpired()
        {
            controler.IsExpired = true;
            base.OnExpired();
        }
        public override void ApplyEffect(GameResult gameResult, float dt) { }
        public override void OnTargetDeath(GameResult gameResult) { this.IsExpired = true; }
        public override object Clone()
        {
            return new ControlerEffect(this);
        }
    }
}