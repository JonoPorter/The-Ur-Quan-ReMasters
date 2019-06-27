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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using SLWIC = System.Windows.Forms.ListView.SelectedListViewItemCollection;

namespace ReMasters.SuperMelee.GUI
{
    public partial class BattleSetup : Form
    {

        static readonly string DefaultConfigPath = SuperMeleePaths.ConfigDir + "BattleSetup.xml";
        static readonly string DefaultPlayer1Fleet = SuperMeleePaths.FleetDir + "Last Fleet of Player 1.xml";
        static readonly string DefaultPlayer2Fleet = SuperMeleePaths.FleetDir + "Last Fleet of Player 2.xml";
        ImageList imageList;

        public ImageList ImageList
        {
            get { return imageList; }
        }
        public bool Player1IsAI
        {
            get
            {
                return cbPlayer1Controler.Text == "AI";
            }
        }
        public bool Player2IsAI
        {
            get
            {
                return cbPlayer2Controler.Text == "AI";
            }
        }
        public int Player1WingmanCount
        {
            get
            {
                return (int)nudPlayer1AIWingman.Value;
            }
        }
        public int Player2WingmanCount
        {
            get
            {
                return (int)nudPlayer2AIWingman.Value;
            }
        }
        public List<IShip> Player1Ships
        {
            get
            {
                return lvPlayer1Ships.Ships;
            }
        }
        public List<IShip> Player2Ships
        {
            get
            {
                return lvPlayer2Ships.Ships;
            }
        }
        public BattleSetup()
        {
            InitializeComponent();
            List<ShipLoader> loaders = BaseDisplayDemo.GetShipLoaders();
            foreach (ShipLoader loader in loaders)
            {
                loader.Load();
            }
            imageList = lvGeneralShipLoaders.LargeImageList;
            lvGeneralShipLoaders.Setup(loaders);
            lvPlayer1Ships.Setup(loaders);
            lvPlayer2Ships.Setup(loaders);
            lvPlayer1Ships.Items.Clear();
            lvPlayer2Ships.Items.Clear();
            imageList = lvGeneralShipLoaders.LargeImageList;


            this.openFileDialog1.FileName = SuperMeleePaths.FleetDir + this.openFileDialog1.FileName;
            this.saveFileDialog1.FileName = SuperMeleePaths.FleetDir + this.saveFileDialog1.FileName;

            LoadFleet(DefaultPlayer1Fleet, lvPlayer1Ships);
            LoadFleet(DefaultPlayer2Fleet, lvPlayer2Ships);
            LoadConfig();
        }
        private void playerDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SLWIC)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void lvPlayer2Ships_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SLWIC)))
            {
                ShipLoaderListView lv = (ShipLoaderListView)sender;
                SLWIC cool = (SLWIC)e.Data.GetData(typeof(SLWIC));
                foreach (ListViewItem item in cool)
                {
                    lv.Items.Add((ListViewItem)item.Clone());
                }
                lv.Refresh();

                RecalcPlayer1();
                RecalcPlayer2();
            }
        }
        private void lvGeneralShipLoaders_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DragDropEffects effect = lvGeneralShipLoaders.DoDragDrop(lvGeneralShipLoaders.SelectedItems, DragDropEffects.Copy);
        }
        private void lvPlayer2Ships_KeyPress(object sender, KeyPressEventArgs e)
        {


        }
        private void commitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripTextBox tstb = this.toolStripTextBox1;
            int count;
            Control parent = this.cmsPlayers.Parent;
            if (int.TryParse(tstb.TextBox.Text, out count) && count > 1 && count < 100)
            {
                if (lvPlayer1Ships.Focused)
                {
                    foreach (ListViewItem item in lvPlayer1Ships.SelectedItems)
                    {
                        for (int pos = 1; pos < count; pos++)
                        {
                            lvPlayer1Ships.Items.Add((ListViewItem)item.Clone());
                        }
                    }
                }
                else if (lvPlayer2Ships.Focused)
                {
                    foreach (ListViewItem item in lvPlayer2Ships.SelectedItems)
                    {
                        for (int pos = 1; pos < count; pos++)
                        {
                            lvPlayer2Ships.Items.Add((ListViewItem)item.Clone());
                        }
                    }
                }
            }
            else
            {
                tstb.TextBox.Text = "2";
            }
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvPlayer1Ships.Focused)
            {
                foreach (ListViewItem item in lvPlayer1Ships.SelectedItems)
                {
                    lvPlayer1Ships.Items.Remove(item);
                }
            }
            else if (lvPlayer2Ships.Focused)
            {
                foreach (ListViewItem item in lvPlayer2Ships.SelectedItems)
                {
                    lvPlayer2Ships.Items.Remove(item);
                }
            }
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            CheckClose(true);
        }
        private bool CheckClose(bool doHide)
        {
            if (this.lvPlayer1Ships.Items.Count > 0 && this.lvPlayer2Ships.Items.Count > 0)
            {
                SaveFleet(DefaultPlayer1Fleet, lvPlayer1Ships);
                SaveFleet(DefaultPlayer2Fleet, lvPlayer2Ships);
                SaveConfig();
                if (doHide)
                {
                    this.Hide();
                }
                return false;
            }
            else
            {
                MessageBox.Show("Both Players need at least 1 ship");
                return true;
            }
        }


        private void BattleSetup_Activated(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.TopMost = false;
        }

        private void BattleSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = CheckClose(false);
        }

        private void lvPlayer1Ships_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ShipLoaderListView lv = (ShipLoaderListView)sender;

            DragDropEffects effect = lv.DoDragDrop(lv.SelectedItems, DragDropEffects.Copy);
            foreach (ListViewItem item in lv.SelectedItems)
            {
                lv.Items.Remove(item);
            }
        }

        private void lvGeneralShipLoaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvGeneralShipLoaders.SelectedItems.Count != 0)
            {
                ShipLoader loader = (ShipLoader)lvGeneralShipLoaders.SelectedItems[0].Tag;
                tbFullName.Text = loader.Info.FullName;
                tbPointValue.Text = loader.Info.PointValue.ToString();
                this.rtbDescription.Text = loader.Info.Description;
            }
        }
        private int GetTotalValue(ShipLoaderListView lv)
        {
            int total = 0;
            foreach (ListViewItem item in lv.Items)
            {
                ShipLoader loader = (ShipLoader)item.Tag;
                total += loader.Info.PointValue;
            }
            return total;
        }

        private void SetPointTotalValue(ShipLoaderListView lv, TextBox box)
        {
            box.Text = GetTotalValue(lv).ToString();
        }
        void RecalcPlayer1()
        {
            SetPointTotalValue(lvPlayer1Ships, tbPlayer1PointValue);

        }
        void RecalcPlayer2()
        {
            SetPointTotalValue(lvPlayer2Ships, tbPlayer2PointValue);
        }
        private void lvPlayer1Ships_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RecalcPlayer1();
            RecalcPlayer2();
        }

        private void lvPlayer2Ships_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RecalcPlayer2();
            RecalcPlayer1();
        }

        XmlSerializer shipCountArrayXmlSerializer = new XmlSerializer(typeof(ShipCount[]));
        XmlSerializer battleSetupConfigXmlSerializer = new XmlSerializer(typeof(BattleSetupConfig));
        void SaveConfig()
        {
            if (File.Exists(DefaultConfigPath))
            {
                File.Delete(DefaultConfigPath);
            }

            using (FileStream stream = File.Create(DefaultConfigPath))
            {
                battleSetupConfigXmlSerializer.Serialize(stream, 
                    new BattleSetupConfig(
                    this.cbPlayer1Controler.Text,
                    this.cbPlayer2Controler.Text,
                    this.nudPlayer1AIWingman.Value,
                    this.nudPlayer2AIWingman.Value));
            }
        }
        void LoadConfig()
        {
            if (File.Exists(DefaultConfigPath))
            {
                using (FileStream stream = File.OpenRead(DefaultConfigPath))
                {
                    BattleSetupConfig config = (BattleSetupConfig)battleSetupConfigXmlSerializer.Deserialize(stream);
                   this.cbPlayer1Controler.Text = config.Player1Controler;
                   this.cbPlayer2Controler.Text = config.Player2Controler;
                   this.nudPlayer1AIWingman.Value = config.Player1WingmanCount;
                   this.nudPlayer2AIWingman.Value = config.Player2WingmanCount;
                }
            }
        }
        void LoadFleet(string path, ShipLoaderListView lv)
        {
            if (!File.Exists(path))
            {
                return;
            }
            ShipCount[] array = null;
            using (FileStream stream = File.OpenRead(path))
            {
                array = (ShipCount[])shipCountArrayXmlSerializer.Deserialize(stream);
            }
            lv.Items.Clear();
            foreach (ShipCount shipcount in array)
            {
                foreach (ListViewItem storeItem in this.lvGeneralShipLoaders.Items)
                {
                    if (storeItem.Text == shipcount.ShipName)
                    {
                        for (int pos = 0; pos < shipcount.Count; ++pos)
                        {
                            lv.Items.Add((ListViewItem)storeItem.Clone());
                        }
                        break;
                    }
                }
            }
            RecalcPlayer2();
            RecalcPlayer1();
        }
        void LoadFleet(ShipLoaderListView lv)
        {
            if ((this.openFileDialog1.ShowDialog(this)& DialogResult.OK) == DialogResult.OK)
            {
                LoadFleet(this.openFileDialog1.FileName,lv);
                this.saveFileDialog1.FileName = this.openFileDialog1.FileName;
            }
        }
        void SaveFleet(string path, ShipLoaderListView lv)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Dictionary<string, int> shipListing = new Dictionary<string, int>();
            for (int pos = 0; pos < lv.Items.Count; ++pos)
            {
                string name = lv.Items[pos].Text;
                if (shipListing.ContainsKey(name))
                {
                    shipListing[name] += 1;
                }
                else 
                {
                    shipListing[name] = 1;
                }
            }
            ShipCount[] array = new ShipCount[shipListing.Count];
            int index = 0;
            foreach (KeyValuePair<string, int> pair in shipListing)
            {
                array[index++] = new ShipCount(pair.Key, pair.Value);
            }
            using (FileStream stream = File.Create(path))
            {
                shipCountArrayXmlSerializer.Serialize(stream, array);
            }
        }
        void SaveFleet(ShipLoaderListView lv)
        {
            if ((this.saveFileDialog1.ShowDialog(this) & DialogResult.OK) == DialogResult.OK)
            {
                SaveFleet(this.saveFileDialog1.FileName, lv);
                this.openFileDialog1.FileName = this.saveFileDialog1.FileName;
            }
        }


        public class ShipCount
        {
            [XmlAttribute]
            public string ShipName;
            [XmlAttribute]
            public int Count;
            public ShipCount() { }
            public ShipCount(string ShipName, int Count)
            {
                this.ShipName = ShipName;
                this.Count = Count;
            }
        }

        private void bPlayer1Load1_Click(object sender, EventArgs e)
        {
            LoadFleet(this.lvPlayer1Ships);
        }

        private void bPlayer1Save_Click(object sender, EventArgs e)
        {
            SaveFleet(this.lvPlayer1Ships);
        }

        private void bPlayer2Load_Click(object sender, EventArgs e)
        {

            LoadFleet(this.lvPlayer2Ships);
        }

        private void bPlayer2Save_Click(object sender, EventArgs e)
        {

            SaveFleet(this.lvPlayer2Ships);
        }


    }
    public class BattleSetupConfig
    {
        public string Player1Controler;
        public string Player2Controler;
        public Decimal Player1WingmanCount;
        public Decimal Player2WingmanCount;
        public BattleSetupConfig() { }
        public BattleSetupConfig(string Player1Controler, string Player2Controler,Decimal Player1WingmanCount,Decimal Player2WingmanCount)
        {
            this.Player1Controler = Player1Controler;
            this.Player2Controler = Player2Controler;
            this.Player1WingmanCount = Player1WingmanCount;
            this.Player2WingmanCount = Player2WingmanCount;
        }
    }
}