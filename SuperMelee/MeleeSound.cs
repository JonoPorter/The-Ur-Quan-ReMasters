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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SdlDotNet;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
using System.Xml.Serialization;
using Tao.OpenGl;

namespace ReMasters.SuperMelee
{
    [Serializable]
    public sealed class ActionSounds
    {
        public MeleeSound Activated;
        public MeleeSound Running;
        public MeleeSound DeActivated;
        public ActionSounds()
        {
            this.Activated = new MeleeSound();
            this.Running = new MeleeSound();
            this.DeActivated = new MeleeSound();
        }
        public ActionSounds(string ActivatedName, string RunningName, string DeActivatedName)
        {
            this.Activated = new MeleeSound(ActivatedName);
            this.Running = new MeleeSound(RunningName);
            this.DeActivated = new MeleeSound(DeActivatedName);
        }
        public ActionSounds(MeleeSound Activated, MeleeSound Running, MeleeSound DeActivated)
        {
            this.Activated = Activated;
            this.Running = Running;
            this.DeActivated = DeActivated;
            if (Activated == null)
            {
                this.Activated = new MeleeSound();
            }
            if (Running == null)
            {
                this.Running = new MeleeSound();
            }
            if (DeActivated == null)
            {
                this.DeActivated = new MeleeSound();
            }
        }
    }
    [Serializable]
    public sealed class ControlableSounds
    {
        public MeleeSound Created;
        public MeleeSound Death;
        public ControlableSounds()
        {
            this.Created = new MeleeSound();
            this.Death = new MeleeSound();
        }
        public ControlableSounds(string CreatedName, string DeathName)
        {
            this.Created = new MeleeSound(CreatedName);
            this.Death = new MeleeSound(DeathName);
        }
    }
    [Serializable]
    public sealed class ShipSounds
    {
        public MeleeMusic Victory;
        public ShipSounds()
        {
            this.Victory = new MeleeMusic();
        }
        public ShipSounds(string VictoryName)
        {
            this.Victory = new MeleeMusic(VictoryName);
        }
    }
    [Serializable]
    public sealed class WeaponSounds
    {
        public MeleeSound Collision;
        public WeaponSounds()
        {
            this.Collision = new MeleeSound();
        }
        public WeaponSounds(string CollisionName)
        {
            this.Collision = new MeleeSound(CollisionName);
        }
    }
    [Serializable]
    public sealed class EffectSounds
    {
        public MeleeSound Attached;
        public MeleeSound Applied;
        public MeleeSound Exhausted;
        public EffectSounds()
        {
            this.Attached = new MeleeSound();
            this.Applied = new MeleeSound();
            this.Exhausted = new MeleeSound();
        }
        public EffectSounds(string AttachedName, string AppliedName, string ExhaustedName)
        {
            this.Attached = new MeleeSound(AttachedName);
            this.Applied = new MeleeSound(AppliedName);
            this.Exhausted = new MeleeSound(ExhaustedName);
        }
    }
    public static class MeleeDamageSounds
    {
        static string DamageSoundsFile = SuperMeleePaths.ConfigDir + "DamageSounds.XML";
        static List<DamageSound> sounds;
        public static void Load()
        {
            XmlSerializer s = new XmlSerializer(typeof(List<DamageSound>));
            if (File.Exists(DamageSoundsFile))
            {
                using (FileStream stream = File.OpenRead(DamageSoundsFile))
                {
                    sounds = (List<DamageSound>)s.Deserialize(stream);
                }
            }
            else
            {
                sounds = new List<DamageSound>();
                sounds.Add(new DamageSound(1, 2, "Boom1"));
                sounds.Add(new DamageSound(2, 4, "Boom23"));
                sounds.Add(new DamageSound(4, 6, "Boom45"));
                sounds.Add(new DamageSound(6, 20, "Boom67"));
                using (FileStream stream = File.Create(DamageSoundsFile))
                {
                    s.Serialize(stream, sounds);
                }
            }
        }
        public static void PlayDamage(float damage)
        {
            foreach (DamageSound sound in sounds)
            {
                if (sound.IsInRange(damage))
                {
                    sound.Sound.Play();
                    break;
                }
            }
        }
    }
    public class DamageSound
    {
        float minDamage;
        float maxDamage;
        MeleeSound sound;
        public DamageSound() { }
        public DamageSound(float minDamage, float maxDamage, string sound)
        {
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
            this.sound = new MeleeSound(sound);
        }
        public float MinDamage
        {
            get { return minDamage; }
            set { minDamage = value; }
        }
        public float MaxDamage
        {
            get { return maxDamage; }
            set { maxDamage = value; }
        }
        public string SoundName
        {
            get { return sound.Name; }
            set { this.sound = new MeleeSound(value); }
        }
        public MeleeSound Sound
        {
            get
            {
                return sound;
            }
        }
        public bool IsInRange(float damage)
        {
            return damage >= minDamage && damage < maxDamage;
        }
    }
    [Serializable]
    public sealed partial class MeleeSound
    {
        static Random rand = new Random();
        Bounded<float> playDelay;

        public List<string> Names;
        [XmlIgnore]
        public string Name
        {
            get
            {
                if (Names != null)
                {
                    return Names[rand.Next(Names.Count)];
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (Names == null)
                    {
                        Names = new List<string>();
                    }
                    else
                    {
                        Names.Clear();
                    }
                    Names.Add(value);
                }
            }
        }
        public float PlayDelay
        {
            get
            {
                return playDelay.Binder.Upper;
            }
            set
            {
                playDelay = new Bounded<float>(value);
            }
        }
        public MeleeSound() { }
        public MeleeSound(string Name)
        {
            this.Name = Name;
        }
        public MeleeSound(string Name, float PlayDelay)
        {
            this.Name = Name;
            this.PlayDelay = PlayDelay;
        }
        public MeleeSound(List<string> Names)
        {
            this.Names = Names;
        }
        public MeleeSound(List<string> Names, float PlayDelay)
        {
            this.Names = Names;
            this.PlayDelay = PlayDelay;
        }
        public void Play()
        {
            Play(0);
        }
        public void Play(float dt)
        {
            if (Name != null)
            {
                if (playDelay == null)
                {
                    MeleeSound.QueueSound(Name);
                }
                else
                {
                    playDelay.Value += dt;
                    if (playDelay.IsFull)
                    {
                        playDelay.Empty();
                        MeleeSound.QueueSound(Name);
                    }
                }
            }
        }
    }
    public sealed partial class MeleeSound
    {
        /// <summary>
        /// A dictionary that holds the sounds that have already been played or need to be played. 
        /// </summary>
        static Dictionary<string, SoundInfo> sounds = new Dictionary<string, SoundInfo>();
        static MeleeSound()
        { }
        public static void Load()
        {
#if Release
		            try
            {  
	#endif
                Mixer.ChannelsAllocated = 32;
                DirectoryInfo soundDir = new DirectoryInfo(SuperMeleePaths.SoundDir);
                foreach (FileInfo info in soundDir.GetFiles("*.ogg"))
                {
                    string name = Path.GetFileNameWithoutExtension(info.Name);
                    Sound sound = new Sound(info.FullName);
                    sound.Volume = sound.Volume / 2;
                    sounds.Add(name, new SoundInfo(false, sound));
                }
#if Release
            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            } 
#endif
        }
        /// <summary>
        /// Called to ask for a sound to be played on the next tick.
        /// </summary>
        /// <param name="name">The file name of the sound.</param>
        public static void QueueSound(string name)
        {

            SoundInfo soundInfo;
            if (sounds.TryGetValue(name, out soundInfo))
            {
                soundInfo.ShouldPlay = true;
            }
            /*else
            {
                if (File.Exists(name))
                {
                    Sound sound = new Sound(name);
                    sound.Volume = sound.Volume / 2;
                    sounds.Add(name, new SoundInfo(true, sound));
                }
            }*/
        }
        /// <summary>
        /// Called by the OnTick event handler. To play the sounds that were queued.
        /// </summary>
        public static void PlaySounds()
        {
            try
            {
                foreach (KeyValuePair<string, SoundInfo> info in sounds)
                {
                    if (info.Value.ShouldPlay)
                    {
                        info.Value.ShouldPlay = false;

                        info.Value.Sound.Play();
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// Holds information about the Sound.
        /// </summary>
        class SoundInfo
        {
            /// <summary>
            /// States if on the next Tick if it should be played.
            /// </summary>
            public bool ShouldPlay;
            /// <summary>
            /// The sound object.
            /// </summary>
            public Sound Sound;
            public SoundInfo(bool ShouldPlay, Sound Sound)
            {
                this.ShouldPlay = ShouldPlay;
                this.Sound = Sound;
            }
        }
        public static void Close()
        {

            foreach (SoundInfo info in sounds.Values)
            {
                info.Sound.Close();
            }
            sounds.Clear();
        }
    }
    [Serializable]
    public sealed partial class MeleeMusic
    {
        static Dictionary<string, Music> musics = new Dictionary<string, Music>();
        static string defaultMusic;

        static MeleeMusic() { }
        public static void Load()
        {
#if Release
		try
            {  
#endif

            //Music.Volume = (3 / 4) * Music.Volume;
            DirectoryInfo soundDir = new DirectoryInfo(SuperMeleePaths.MusicDir);
            foreach (FileInfo info in soundDir.GetFiles("*.ogg"))
            {
                string name = Path.GetFileNameWithoutExtension(info.Name);
                Music music = new Music(info.FullName);
                musics.Add(name, music);
            }
            foreach (FileInfo info in soundDir.GetFiles("*.mod"))
            {
                string name = Path.GetFileNameWithoutExtension(info.Name);
                Music music = new Music(info.FullName);
                musics.Add(name, music);
            }
            DefaultMusic = "Battle";
            Music.EnableMusicFinishedCallback();

#if Release
            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            } 
#endif
        }
        public static string DefaultMusic
        {
            get { return MeleeMusic.defaultMusic; }
            set
            {
                if (MeleeMusic.defaultMusic != value && value != null)
                {
                    MeleeMusic.defaultMusic = value;
                    Music music;
                    if (musics.TryGetValue(value, out music))
                    {
                        music.QueuedMusic = music;
                    }
                }
            }
        }
        /// <summary>
        /// Called to ask for a sound to be played on the next tick.
        /// </summary>
        /// <param name="name">The file name of the sound.</param>
        public static bool PlayMusic(string name)
        {
            Music music;
            if (musics.TryGetValue(name, out music))
            {
                if (Music.CurrentMusic != music)
                {
                    music.Play();
                    return true;
                }
            }
            return false;
        }
        public static void PlayDefault()
        {
            if (PlayMusic(defaultMusic))
            {
                Music.CurrentMusic.QueuedMusic = Music.CurrentMusic;
            }
        }
        public static void Close()
        {
            Music.Stop();
            foreach (Music music in musics.Values)
            {
                music.Dispose();
            }
            musics.Clear();
        }
    }
    public sealed partial class MeleeMusic
    {
        public string Name;
        public MeleeMusic() { }
        public MeleeMusic(string Name)
        {
            this.Name = Name;
        }
        public void Play()
        {
            if (Name != null)
            {
                MeleeMusic.PlayMusic(Name);
            }
        }
    }
}