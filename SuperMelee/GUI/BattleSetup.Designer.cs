namespace ReMasters.SuperMelee.GUI
{
    partial class BattleSetup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbPlayer1 = new System.Windows.Forms.GroupBox();
            this.lPlayer1Controler = new System.Windows.Forms.Label();
            this.lPlayer1Fleet = new System.Windows.Forms.Label();
            this.bPlayer1Save = new System.Windows.Forms.Button();
            this.lPlayer1AIWingman = new System.Windows.Forms.Label();
            this.bPlayer1Load1 = new System.Windows.Forms.Button();
            this.cbPlayer1Controler = new System.Windows.Forms.ComboBox();
            this.lPlayer1PointValue = new System.Windows.Forms.Label();
            this.nudPlayer1AIWingman = new System.Windows.Forms.NumericUpDown();
            this.tbPlayer1PointValue = new System.Windows.Forms.TextBox();
            this.lvPlayer1Ships = new ReMasters.SuperMelee.GUI.ShipLoaderListView();
            this.cmsPlayers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.multiplyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbPlayer2 = new System.Windows.Forms.GroupBox();
            this.lPlayer2Controler = new System.Windows.Forms.Label();
            this.lPlayer2Fleet = new System.Windows.Forms.Label();
            this.bPlayer2Save = new System.Windows.Forms.Button();
            this.lPlayer2AIWingman = new System.Windows.Forms.Label();
            this.bPlayer2Load = new System.Windows.Forms.Button();
            this.cbPlayer2Controler = new System.Windows.Forms.ComboBox();
            this.lPlayer2PointValue = new System.Windows.Forms.Label();
            this.nudPlayer2AIWingman = new System.Windows.Forms.NumericUpDown();
            this.tbPlayer2PointValue = new System.Windows.Forms.TextBox();
            this.lvPlayer2Ships = new ReMasters.SuperMelee.GUI.ShipLoaderListView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lvGeneralShipLoaders = new ReMasters.SuperMelee.GUI.ShipLoaderListView();
            this.rtbDescription = new System.Windows.Forms.RichTextBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.tbPointValue = new System.Windows.Forms.TextBox();
            this.LPointValue = new System.Windows.Forms.Label();
            this.tbFullName = new System.Windows.Forms.TextBox();
            this.lFullName = new System.Windows.Forms.Label();
            this.bStart = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbPlayer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayer1AIWingman)).BeginInit();
            this.cmsPlayers.SuspendLayout();
            this.gbPlayer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayer2AIWingman)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbPlayer1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbPlayer2);
            this.splitContainer1.Size = new System.Drawing.Size(495, 388);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.TabIndex = 4;
            // 
            // gbPlayer1
            // 
            this.gbPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer1.Controls.Add(this.lPlayer1Controler);
            this.gbPlayer1.Controls.Add(this.lPlayer1Fleet);
            this.gbPlayer1.Controls.Add(this.bPlayer1Save);
            this.gbPlayer1.Controls.Add(this.lPlayer1AIWingman);
            this.gbPlayer1.Controls.Add(this.bPlayer1Load1);
            this.gbPlayer1.Controls.Add(this.cbPlayer1Controler);
            this.gbPlayer1.Controls.Add(this.lPlayer1PointValue);
            this.gbPlayer1.Controls.Add(this.nudPlayer1AIWingman);
            this.gbPlayer1.Controls.Add(this.tbPlayer1PointValue);
            this.gbPlayer1.Controls.Add(this.lvPlayer1Ships);
            this.gbPlayer1.Location = new System.Drawing.Point(3, 3);
            this.gbPlayer1.Name = "gbPlayer1";
            this.gbPlayer1.Size = new System.Drawing.Size(488, 183);
            this.gbPlayer1.TabIndex = 13;
            this.gbPlayer1.TabStop = false;
            this.gbPlayer1.Text = "Player 1";
            // 
            // lPlayer1Controler
            // 
            this.lPlayer1Controler.AutoSize = true;
            this.lPlayer1Controler.Location = new System.Drawing.Point(6, 16);
            this.lPlayer1Controler.Name = "lPlayer1Controler";
            this.lPlayer1Controler.Size = new System.Drawing.Size(52, 13);
            this.lPlayer1Controler.TabIndex = 10;
            this.lPlayer1Controler.Text = "Controler:";
            // 
            // lPlayer1Fleet
            // 
            this.lPlayer1Fleet.AutoSize = true;
            this.lPlayer1Fleet.Location = new System.Drawing.Point(110, 13);
            this.lPlayer1Fleet.Name = "lPlayer1Fleet";
            this.lPlayer1Fleet.Size = new System.Drawing.Size(30, 13);
            this.lPlayer1Fleet.TabIndex = 12;
            this.lPlayer1Fleet.Text = "Fleet";
            // 
            // bPlayer1Save
            // 
            this.bPlayer1Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPlayer1Save.Location = new System.Drawing.Point(268, 13);
            this.bPlayer1Save.Name = "bPlayer1Save";
            this.bPlayer1Save.Size = new System.Drawing.Size(75, 23);
            this.bPlayer1Save.TabIndex = 9;
            this.bPlayer1Save.Text = "Save";
            this.bPlayer1Save.UseVisualStyleBackColor = true;
            this.bPlayer1Save.Click += new System.EventHandler(this.bPlayer1Save_Click);
            // 
            // lPlayer1AIWingman
            // 
            this.lPlayer1AIWingman.AutoSize = true;
            this.lPlayer1AIWingman.Location = new System.Drawing.Point(6, 58);
            this.lPlayer1AIWingman.Name = "lPlayer1AIWingman";
            this.lPlayer1AIWingman.Size = new System.Drawing.Size(99, 13);
            this.lPlayer1AIWingman.TabIndex = 12;
            this.lPlayer1AIWingman.Text = "AI Wingman Count:";
            // 
            // bPlayer1Load1
            // 
            this.bPlayer1Load1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPlayer1Load1.Location = new System.Drawing.Point(187, 13);
            this.bPlayer1Load1.Name = "bPlayer1Load1";
            this.bPlayer1Load1.Size = new System.Drawing.Size(75, 23);
            this.bPlayer1Load1.TabIndex = 8;
            this.bPlayer1Load1.Text = "Load";
            this.bPlayer1Load1.UseVisualStyleBackColor = true;
            this.bPlayer1Load1.Click += new System.EventHandler(this.bPlayer1Load1_Click);
            // 
            // cbPlayer1Controler
            // 
            //this.cbPlayer1Controler.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbPlayer1Controler.FormattingEnabled = true;
            this.cbPlayer1Controler.Items.AddRange(new object[] {
            "Human",
            "AI"});
            this.cbPlayer1Controler.Location = new System.Drawing.Point(9, 34);
            this.cbPlayer1Controler.Name = "cbPlayer1Controler";
            this.cbPlayer1Controler.Size = new System.Drawing.Size(101, 21);
            this.cbPlayer1Controler.TabIndex = 11;
            this.cbPlayer1Controler.Text = "Human";
            // 
            // lPlayer1PointValue
            // 
            this.lPlayer1PointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lPlayer1PointValue.AutoSize = true;
            this.lPlayer1PointValue.Location = new System.Drawing.Point(349, 19);
            this.lPlayer1PointValue.Name = "lPlayer1PointValue";
            this.lPlayer1PointValue.Size = new System.Drawing.Size(64, 13);
            this.lPlayer1PointValue.TabIndex = 7;
            this.lPlayer1PointValue.Text = "Point Value:";
            // 
            // nudPlayer1AIWingman
            // 
            this.nudPlayer1AIWingman.Location = new System.Drawing.Point(9, 76);
            this.nudPlayer1AIWingman.Name = "nudPlayer1AIWingman";
            this.nudPlayer1AIWingman.Size = new System.Drawing.Size(101, 20);
            this.nudPlayer1AIWingman.TabIndex = 11;
            // 
            // tbPlayer1PointValue
            // 
            this.tbPlayer1PointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPlayer1PointValue.Location = new System.Drawing.Point(419, 19);
            this.tbPlayer1PointValue.Name = "tbPlayer1PointValue";
            this.tbPlayer1PointValue.Size = new System.Drawing.Size(66, 20);
            this.tbPlayer1PointValue.TabIndex = 6;
            // 
            // lvPlayer1Ships
            // 
            this.lvPlayer1Ships.AllowDrop = true;
            this.lvPlayer1Ships.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPlayer1Ships .ContextMenuStrip = this.cmsPlayers;
            this.lvPlayer1Ships.Location = new System.Drawing.Point(113, 42);
            this.lvPlayer1Ships.Name = "lvPlayer1Ships";
            this.lvPlayer1Ships.Size = new System.Drawing.Size(369, 135);
            this.lvPlayer1Ships.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvPlayer1Ships.TabIndex = 2;
            this.lvPlayer1Ships.UseCompatibleStateImageBehavior = false;
            this.lvPlayer1Ships.DragEnter += new System.Windows.Forms.DragEventHandler(this.playerDragEnter);
            this.lvPlayer1Ships.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvPlayer2Ships_DragDrop);
            this.lvPlayer1Ships.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvPlayer1Ships_ItemSelectionChanged);
            this.lvPlayer1Ships.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lvPlayer2Ships_KeyPress);
            this.lvPlayer1Ships.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvPlayer1Ships_ItemDrag);
            // 
            // cmsPlayers
            // 
            this.cmsPlayers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multiplyToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cmsPlayers.Name = "cmsPlayers";
            this.cmsPlayers.Size = new System.Drawing.Size(122, 48);
            // 
            // multiplyToolStripMenuItem
            // 
            this.multiplyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1,
            this.commitToolStripMenuItem});
            this.multiplyToolStripMenuItem.Name = "multiplyToolStripMenuItem";
            this.multiplyToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.multiplyToolStripMenuItem.Text = "Multiply";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBox1.Text = "2";
            // 
            // commitToolStripMenuItem
            // 
            this.commitToolStripMenuItem.Name = "commitToolStripMenuItem";
            this.commitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.commitToolStripMenuItem.Text = "Commit";
            this.commitToolStripMenuItem.Click += new System.EventHandler(this.commitToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // gbPlayer2
            // 
            this.gbPlayer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer2.Controls.Add(this.lPlayer2Controler);
            this.gbPlayer2.Controls.Add(this.lPlayer2Fleet);
            this.gbPlayer2.Controls.Add(this.bPlayer2Save);
            this.gbPlayer2.Controls.Add(this.lPlayer2AIWingman);
            this.gbPlayer2.Controls.Add(this.bPlayer2Load);
            this.gbPlayer2.Controls.Add(this.cbPlayer2Controler);
            this.gbPlayer2.Controls.Add(this.lPlayer2PointValue);
            this.gbPlayer2.Controls.Add(this.nudPlayer2AIWingman);
            this.gbPlayer2.Controls.Add(this.tbPlayer2PointValue);
            this.gbPlayer2.Controls.Add(this.lvPlayer2Ships);
            this.gbPlayer2.Location = new System.Drawing.Point(3, 3);
            this.gbPlayer2.Name = "gbPlayer2";
            this.gbPlayer2.Size = new System.Drawing.Size(488, 191);
            this.gbPlayer2.TabIndex = 11;
            this.gbPlayer2.TabStop = false;
            this.gbPlayer2.Text = "Player 2";
            // 
            // lPlayer2Controler
            // 
            this.lPlayer2Controler.AutoSize = true;
            this.lPlayer2Controler.Location = new System.Drawing.Point(6, 16);
            this.lPlayer2Controler.Name = "lPlayer2Controler";
            this.lPlayer2Controler.Size = new System.Drawing.Size(52, 13);
            this.lPlayer2Controler.TabIndex = 6;
            this.lPlayer2Controler.Text = "Controler:";
            // 
            // lPlayer2Fleet
            // 
            this.lPlayer2Fleet.AutoSize = true;
            this.lPlayer2Fleet.Location = new System.Drawing.Point(110, 13);
            this.lPlayer2Fleet.Name = "lPlayer2Fleet";
            this.lPlayer2Fleet.Size = new System.Drawing.Size(30, 13);
            this.lPlayer2Fleet.TabIndex = 8;
            this.lPlayer2Fleet.Text = "Fleet";
            // 
            // bPlayer2Save
            // 
            this.bPlayer2Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPlayer2Save.Location = new System.Drawing.Point(267, 13);
            this.bPlayer2Save.Name = "bPlayer2Save";
            this.bPlayer2Save.Size = new System.Drawing.Size(75, 23);
            this.bPlayer2Save.TabIndex = 5;
            this.bPlayer2Save.Text = "Save";
            this.bPlayer2Save.UseVisualStyleBackColor = true;
            this.bPlayer2Save.Click += new System.EventHandler(this.bPlayer2Save_Click);
            // 
            // lPlayer2AIWingman
            // 
            this.lPlayer2AIWingman.AutoSize = true;
            this.lPlayer2AIWingman.Location = new System.Drawing.Point(6, 58);
            this.lPlayer2AIWingman.Name = "lPlayer2AIWingman";
            this.lPlayer2AIWingman.Size = new System.Drawing.Size(99, 13);
            this.lPlayer2AIWingman.TabIndex = 10;
            this.lPlayer2AIWingman.Text = "AI Wingman Count:";
            // 
            // bPlayer2Load
            // 
            this.bPlayer2Load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bPlayer2Load.Location = new System.Drawing.Point(186, 13);
            this.bPlayer2Load.Name = "bPlayer2Load";
            this.bPlayer2Load.Size = new System.Drawing.Size(75, 23);
            this.bPlayer2Load.TabIndex = 4;
            this.bPlayer2Load.Text = "Load";
            this.bPlayer2Load.UseVisualStyleBackColor = true;
            this.bPlayer2Load.Click += new System.EventHandler(this.bPlayer2Load_Click);
            // 
            // cbPlayer2Controler
            // 
           // this.cbPlayer2Controler.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbPlayer2Controler.FormattingEnabled = true;
            this.cbPlayer2Controler.Items.AddRange(new object[] {
            "Human",
            "AI"});
            this.cbPlayer2Controler.Location = new System.Drawing.Point(9, 34);
            this.cbPlayer2Controler.Name = "cbPlayer2Controler";
            this.cbPlayer2Controler.Size = new System.Drawing.Size(101, 21);
            this.cbPlayer2Controler.TabIndex = 7;
            this.cbPlayer2Controler.Text = "Human";
            // 
            // lPlayer2PointValue
            // 
            this.lPlayer2PointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lPlayer2PointValue.AutoSize = true;
            this.lPlayer2PointValue.Location = new System.Drawing.Point(349, 16);
            this.lPlayer2PointValue.Name = "lPlayer2PointValue";
            this.lPlayer2PointValue.Size = new System.Drawing.Size(64, 13);
            this.lPlayer2PointValue.TabIndex = 3;
            this.lPlayer2PointValue.Text = "Point Value:";
            // 
            // nudPlayer2AIWingman
            // 
            this.nudPlayer2AIWingman.Location = new System.Drawing.Point(9, 76);
            this.nudPlayer2AIWingman.Name = "nudPlayer2AIWingman";
            this.nudPlayer2AIWingman.Size = new System.Drawing.Size(101, 20);
            this.nudPlayer2AIWingman.TabIndex = 9;
            // 
            // tbPlayer2PointValue
            // 
            this.tbPlayer2PointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPlayer2PointValue.Location = new System.Drawing.Point(418, 16);
            this.tbPlayer2PointValue.Name = "tbPlayer2PointValue";
            this.tbPlayer2PointValue.Size = new System.Drawing.Size(64, 20);
            this.tbPlayer2PointValue.TabIndex = 2;
            // 
            // lvPlayer2Ships
            // 
            this.lvPlayer2Ships.AllowDrop = true;
            this.lvPlayer2Ships.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPlayer2Ships.ContextMenuStrip = this.cmsPlayers;
            this.lvPlayer2Ships.Location = new System.Drawing.Point(113, 39);
            this.lvPlayer2Ships.Name = "lvPlayer2Ships";
            this.lvPlayer2Ships.Size = new System.Drawing.Size(369, 146);
            this.lvPlayer2Ships.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvPlayer2Ships.TabIndex = 1;
            this.lvPlayer2Ships.UseCompatibleStateImageBehavior = false;
            this.lvPlayer2Ships.DragEnter += new System.Windows.Forms.DragEventHandler(this.playerDragEnter);
            this.lvPlayer2Ships.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvPlayer2Ships_DragDrop);
            this.lvPlayer2Ships.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvPlayer2Ships_ItemSelectionChanged);
            this.lvPlayer2Ships.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lvPlayer2Ships_KeyPress);
            this.lvPlayer2Ships.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvPlayer1Ships_ItemDrag);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 12);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(714, 394);
            this.splitContainer2.SplitterDistance = 501;
            this.splitContainer2.TabIndex = 5;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lvGeneralShipLoaders);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.rtbDescription);
            this.splitContainer3.Panel2.Controls.Add(this.lDescription);
            this.splitContainer3.Panel2.Controls.Add(this.tbPointValue);
            this.splitContainer3.Panel2.Controls.Add(this.LPointValue);
            this.splitContainer3.Panel2.Controls.Add(this.tbFullName);
            this.splitContainer3.Panel2.Controls.Add(this.lFullName);
            this.splitContainer3.Size = new System.Drawing.Size(205, 388);
            this.splitContainer3.SplitterDistance = 157;
            this.splitContainer3.TabIndex = 4;
            // 
            // lvGeneralShipLoaders
            // 
            this.lvGeneralShipLoaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvGeneralShipLoaders.Location = new System.Drawing.Point(3, 3);
            this.lvGeneralShipLoaders.Name = "lvGeneralShipLoaders";
            this.lvGeneralShipLoaders.Size = new System.Drawing.Size(199, 151);
            this.lvGeneralShipLoaders.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvGeneralShipLoaders.TabIndex = 3;
            this.lvGeneralShipLoaders.UseCompatibleStateImageBehavior = false;
            this.lvGeneralShipLoaders.SelectedIndexChanged += new System.EventHandler(this.lvGeneralShipLoaders_SelectedIndexChanged);
            this.lvGeneralShipLoaders.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvGeneralShipLoaders_ItemDrag);
            // 
            // rtbDescription
            // 
            this.rtbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbDescription.Location = new System.Drawing.Point(3, 59);
            this.rtbDescription.Name = "rtbDescription";
            this.rtbDescription.ReadOnly = true;
            this.rtbDescription.Size = new System.Drawing.Size(199, 165);
            this.rtbDescription.TabIndex = 5;
            this.rtbDescription.Text = "";
            // 
            // lDescription
            // 
            this.lDescription.AutoSize = true;
            this.lDescription.Location = new System.Drawing.Point(3, 41);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(60, 13);
            this.lDescription.TabIndex = 4;
            this.lDescription.Text = "Description";
            // 
            // tbPointValue
            // 
            this.tbPointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPointValue.Location = new System.Drawing.Point(147, 18);
            this.tbPointValue.Name = "tbPointValue";
            this.tbPointValue.ReadOnly = true;
            this.tbPointValue.Size = new System.Drawing.Size(55, 20);
            this.tbPointValue.TabIndex = 3;
            // 
            // LPointValue
            // 
            this.LPointValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LPointValue.AutoSize = true;
            this.LPointValue.Location = new System.Drawing.Point(145, 0);
            this.LPointValue.Name = "LPointValue";
            this.LPointValue.Size = new System.Drawing.Size(58, 13);
            this.LPointValue.TabIndex = 2;
            this.LPointValue.Text = "PointValue";
            // 
            // tbFullName
            // 
            this.tbFullName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFullName.Location = new System.Drawing.Point(3, 18);
            this.tbFullName.Name = "tbFullName";
            this.tbFullName.ReadOnly = true;
            this.tbFullName.Size = new System.Drawing.Size(139, 20);
            this.tbFullName.TabIndex = 1;
            // 
            // lFullName
            // 
            this.lFullName.AutoSize = true;
            this.lFullName.Location = new System.Drawing.Point(3, 0);
            this.lFullName.Name = "lFullName";
            this.lFullName.Size = new System.Drawing.Size(54, 13);
            this.lFullName.TabIndex = 0;
            this.lFullName.Text = "Full Name";
            // 
            // bStart
            // 
            this.bStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStart.Location = new System.Drawing.Point(651, 412);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(75, 23);
            this.bStart.TabIndex = 6;
            this.bStart.Text = "Start!";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "XML";
            this.saveFileDialog1.FileName = "New Fleet.XML";
            this.saveFileDialog1.Filter = "XML Files |*.xml";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "XML";
            this.openFileDialog1.FileName = "New Fleet.XML";
            this.openFileDialog1.Filter = "XML Files |*.xml";
            // 
            // BattleSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 447);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.splitContainer2);
            this.MinimumSize = new System.Drawing.Size(687, 315);
            this.Name = "BattleSetup";
            this.Text = "BattleSetup";
            this.Activated += new System.EventHandler(this.BattleSetup_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BattleSetup_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gbPlayer1.ResumeLayout(false);
            this.gbPlayer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayer1AIWingman)).EndInit();
            this.cmsPlayers.ResumeLayout(false);
            this.gbPlayer2.ResumeLayout(false);
            this.gbPlayer2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayer2AIWingman)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ShipLoaderListView lvPlayer1Ships;
        private ShipLoaderListView lvGeneralShipLoaders;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ShipLoaderListView lvPlayer2Ships;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.ContextMenuStrip cmsPlayers;
        private System.Windows.Forms.ToolStripMenuItem multiplyToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label LPointValue;
        private System.Windows.Forms.Label lFullName;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.RichTextBox rtbDescription;
        private System.Windows.Forms.TextBox tbPointValue;
        private System.Windows.Forms.TextBox tbFullName;
        private System.Windows.Forms.Label lPlayer2PointValue;
        private System.Windows.Forms.TextBox tbPlayer2PointValue;
        private System.Windows.Forms.Button bPlayer1Save;
        private System.Windows.Forms.Button bPlayer1Load1;
        private System.Windows.Forms.Label lPlayer1PointValue;
        private System.Windows.Forms.TextBox tbPlayer1PointValue;
        private System.Windows.Forms.Button bPlayer2Save;
        private System.Windows.Forms.Button bPlayer2Load;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lPlayer2Controler;
        private System.Windows.Forms.ComboBox cbPlayer2Controler;
        private System.Windows.Forms.ComboBox cbPlayer1Controler;
        private System.Windows.Forms.Label lPlayer1Controler;
        private System.Windows.Forms.Label lPlayer1AIWingman;
        private System.Windows.Forms.NumericUpDown nudPlayer1AIWingman;
        private System.Windows.Forms.Label lPlayer1Fleet;
        private System.Windows.Forms.Label lPlayer2AIWingman;
        private System.Windows.Forms.NumericUpDown nudPlayer2AIWingman;
        private System.Windows.Forms.Label lPlayer2Fleet;
        private System.Windows.Forms.GroupBox gbPlayer1;
        private System.Windows.Forms.GroupBox gbPlayer2;

    }
}