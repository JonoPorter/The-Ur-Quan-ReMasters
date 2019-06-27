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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReMasters.SuperMelee.GUI
{
    public partial class ShipSelection : Form
    {
        public ShipSelection()
        {
            InitializeComponent();
            this.Activated += new EventHandler(SetLocation);
            this.Shown += new EventHandler(SetLocation);
            this.Validating += new CancelEventHandler(SetLocation);
            this.GotFocus += new EventHandler(SetLocation);
            this.Enter += new EventHandler(SetLocation);
            this.Invalidated += new InvalidateEventHandler(SetLocation);
            this.LocationChanged += new EventHandler(SetLocation);
        }

        void SetLocation(object sender, EventArgs e)
        {
            if (this.Location != Point.Empty)
            {
                this.Location = Point.Empty;
            }
        }

        public void Setup(string PlayerName,ImageList images, List<IShip> ships)
        {
            this.Text = PlayerName+" Ship Selection";
            this.lvShips.Setup(images, ships);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (alwaysRandom)
            {
                SelectRandom();
                if (this.lvShips.CurrentShip != null)
                {
                    this.Hide();
                }
            }
        }
        private void CheckForValidity(object sender, EventArgs e)
        {
            if (this.lvShips.CurrentShip != null)
            {
                this.Hide();
            }
        }
        List<IShip> ships;
        public IShip GetNextShip()
        {
            if (alwaysRandom)
            {
                if (ships == null)
                {
                    ships = new List<IShip>();
                    foreach (ListViewItem lvitem in lvShips.Items)
                    {
                        ships.Add((IShip)lvitem.Tag);
                    }
                }
                if (ships.Count == 0)
                {
                    return null;
                }

                int index = rand.Next(ships.Count);
                IShip rv = ships[index];
                ships.RemoveAt(index);
                return rv;
            }
            this.ShowDialog();
            return lvShips.RemoveCurrent();
        }
        public int ShipsLeft
        {
            get
            {
                if (ships != null)
                {
                    return ships.Count;
                }
                return lvShips.Items.Count;
            }
        }
        static Random rand = new Random();
        bool alwaysRandom = false;

        public bool AlwaysRandom
        {
            get { return alwaysRandom; }
            set { alwaysRandom = value; }
        }
        private void SelectRandom()
        {
            lock (lvShips)
            {
                lvShips.SelectedIndices.Clear();
                lvShips.SelectedIndices.Add(rand.Next(lvShips.Items.Count));
            }
        }
        private void bRandom_Click(object sender, EventArgs e)
        {
            SelectRandom();
            if (this.lvShips.CurrentShip != null)
            {
                this.Hide();
            }
        }
        private void bAlwaysRandom_Click(object sender, EventArgs e)
        {
            this.alwaysRandom = true;
            SelectRandom();
            if (this.lvShips.CurrentShip != null)
            {
                this.Hide();
            }
        }
    }
}