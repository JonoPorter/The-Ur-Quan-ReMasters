namespace ReMasters
{
    partial class MainMenu
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
            this.bQuit = new System.Windows.Forms.Button();
            this.bNewGame = new System.Windows.Forms.Button();
            this.bLoadGame = new System.Windows.Forms.Button();
            this.bHotSeat = new System.Windows.Forms.Button();
            this.bNetwork = new System.Windows.Forms.Button();
            this.gbSinglePlayer = new System.Windows.Forms.GroupBox();
            this.gbMultiPlayer = new System.Windows.Forms.GroupBox();
            this.gbSinglePlayer.SuspendLayout();
            this.gbMultiPlayer.SuspendLayout();
            this.SuspendLayout();
            // 
            // bQuit
            // 
            this.bQuit.Location = new System.Drawing.Point(205, 134);
            this.bQuit.Name = "bQuit";
            this.bQuit.Size = new System.Drawing.Size(75, 23);
            this.bQuit.TabIndex = 1;
            this.bQuit.Text = "Quit";
            this.bQuit.UseVisualStyleBackColor = true;
            // 
            // bNewGame
            // 
            this.bNewGame.Enabled = false;
            this.bNewGame.Location = new System.Drawing.Point(6, 19);
            this.bNewGame.Name = "bNewGame";
            this.bNewGame.Size = new System.Drawing.Size(75, 23);
            this.bNewGame.TabIndex = 0;
            this.bNewGame.Text = "New Game";
            this.bNewGame.UseVisualStyleBackColor = true;
            // 
            // bLoadGame
            // 
            this.bLoadGame.Enabled = false;
            this.bLoadGame.Location = new System.Drawing.Point(87, 19);
            this.bLoadGame.Name = "bLoadGame";
            this.bLoadGame.Size = new System.Drawing.Size(75, 23);
            this.bLoadGame.TabIndex = 1;
            this.bLoadGame.Text = "Load Game";
            this.bLoadGame.UseVisualStyleBackColor = true;
            // 
            // bHotSeat
            // 
            this.bHotSeat.Location = new System.Drawing.Point(6, 19);
            this.bHotSeat.Name = "bHotSeat";
            this.bHotSeat.Size = new System.Drawing.Size(75, 23);
            this.bHotSeat.TabIndex = 0;
            this.bHotSeat.Text = "Hot Seat";
            this.bHotSeat.UseVisualStyleBackColor = true;
            // 
            // bNetwork
            // 
            this.bNetwork.Enabled = false;
            this.bNetwork.Location = new System.Drawing.Point(87, 19);
            this.bNetwork.Name = "bNetwork";
            this.bNetwork.Size = new System.Drawing.Size(75, 23);
            this.bNetwork.TabIndex = 1;
            this.bNetwork.Text = "Network";
            this.bNetwork.UseVisualStyleBackColor = true;
            // 
            // gbSinglePlayer
            // 
            this.gbSinglePlayer.Controls.Add(this.bNewGame);
            this.gbSinglePlayer.Controls.Add(this.bLoadGame);
            this.gbSinglePlayer.Location = new System.Drawing.Point(12, 12);
            this.gbSinglePlayer.Name = "gbSinglePlayer";
            this.gbSinglePlayer.Size = new System.Drawing.Size(268, 55);
            this.gbSinglePlayer.TabIndex = 2;
            this.gbSinglePlayer.TabStop = false;
            this.gbSinglePlayer.Text = "Single Player";
            // 
            // gbMultiPlayer
            // 
            this.gbMultiPlayer.Controls.Add(this.bHotSeat);
            this.gbMultiPlayer.Controls.Add(this.bNetwork);
            this.gbMultiPlayer.Location = new System.Drawing.Point(12, 73);
            this.gbMultiPlayer.Name = "gbMultiPlayer";
            this.gbMultiPlayer.Size = new System.Drawing.Size(268, 55);
            this.gbMultiPlayer.TabIndex = 3;
            this.gbMultiPlayer.TabStop = false;
            this.gbMultiPlayer.Text = "Multi Player";
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 166);
            this.Controls.Add(this.gbMultiPlayer);
            this.Controls.Add(this.gbSinglePlayer);
            this.Controls.Add(this.bQuit);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.gbSinglePlayer.ResumeLayout(false);
            this.gbMultiPlayer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bLoadGame;
        private System.Windows.Forms.Button bNewGame;
        private System.Windows.Forms.Button bQuit;
        private System.Windows.Forms.Button bHotSeat;
        private System.Windows.Forms.Button bNetwork;
        private System.Windows.Forms.GroupBox gbSinglePlayer;
        private System.Windows.Forms.GroupBox gbMultiPlayer;

    }
}