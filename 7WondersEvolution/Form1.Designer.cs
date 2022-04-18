namespace _7WondersEvolution
{
    partial class Form1
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
            this.lblGeneration = new System.Windows.Forms.Label();
            this.txtGeneration = new System.Windows.Forms.TextBox();
            this.dgvPlayers = new System.Windows.Forms.DataGridView();
            this.nudGamesPlayed = new System.Windows.Forms.NumericUpDown();
            this.lblGamesPlayed = new System.Windows.Forms.Label();
            this.btnNextGeneration = new System.Windows.Forms.Button();
            this.nudPlayers = new System.Windows.Forms.NumericUpDown();
            this.lblPlayers = new System.Windows.Forms.Label();
            this.lblGenerations = new System.Windows.Forms.Label();
            this.nudGenerations = new System.Windows.Forms.NumericUpDown();
            this.lblAverageVPs = new System.Windows.Forms.Label();
            this.txtAverageVPs = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.colGeneration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDna = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVPs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGamesPlayed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerations)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGeneration
            // 
            this.lblGeneration.AutoSize = true;
            this.lblGeneration.Location = new System.Drawing.Point(28, 66);
            this.lblGeneration.Name = "lblGeneration";
            this.lblGeneration.Size = new System.Drawing.Size(62, 13);
            this.lblGeneration.TabIndex = 0;
            this.lblGeneration.Text = "Generation:";
            // 
            // txtGeneration
            // 
            this.txtGeneration.Location = new System.Drawing.Point(96, 63);
            this.txtGeneration.Name = "txtGeneration";
            this.txtGeneration.ReadOnly = true;
            this.txtGeneration.Size = new System.Drawing.Size(48, 20);
            this.txtGeneration.TabIndex = 1;
            this.txtGeneration.Text = "0";
            // 
            // dgvPlayers
            // 
            this.dgvPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlayers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGeneration,
            this.City,
            this.ColDna,
            this.colGames,
            this.colPosition,
            this.colVPs});
            this.dgvPlayers.Location = new System.Drawing.Point(28, 89);
            this.dgvPlayers.Name = "dgvPlayers";
            this.dgvPlayers.Size = new System.Drawing.Size(768, 786);
            this.dgvPlayers.TabIndex = 3;
            // 
            // nudGamesPlayed
            // 
            this.nudGamesPlayed.Location = new System.Drawing.Point(384, 18);
            this.nudGamesPlayed.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudGamesPlayed.Name = "nudGamesPlayed";
            this.nudGamesPlayed.Size = new System.Drawing.Size(44, 20);
            this.nudGamesPlayed.TabIndex = 4;
            this.nudGamesPlayed.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lblGamesPlayed
            // 
            this.lblGamesPlayed.AutoSize = true;
            this.lblGamesPlayed.Location = new System.Drawing.Point(301, 20);
            this.lblGamesPlayed.Name = "lblGamesPlayed";
            this.lblGamesPlayed.Size = new System.Drawing.Size(77, 13);
            this.lblGamesPlayed.TabIndex = 5;
            this.lblGamesPlayed.Text = "Games to play:";
            // 
            // btnNextGeneration
            // 
            this.btnNextGeneration.Location = new System.Drawing.Point(663, 15);
            this.btnNextGeneration.Name = "btnNextGeneration";
            this.btnNextGeneration.Size = new System.Drawing.Size(133, 23);
            this.btnNextGeneration.TabIndex = 7;
            this.btnNextGeneration.Text = "Advance Generations";
            this.btnNextGeneration.UseVisualStyleBackColor = true;
            this.btnNextGeneration.Click += new System.EventHandler(this.btnNextGeneration_Click);
            // 
            // nudPlayers
            // 
            this.nudPlayers.Location = new System.Drawing.Point(156, 18);
            this.nudPlayers.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudPlayers.Name = "nudPlayers";
            this.nudPlayers.Size = new System.Drawing.Size(48, 20);
            this.nudPlayers.TabIndex = 8;
            this.nudPlayers.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // lblPlayers
            // 
            this.lblPlayers.AutoSize = true;
            this.lblPlayers.Location = new System.Drawing.Point(31, 20);
            this.lblPlayers.Name = "lblPlayers";
            this.lblPlayers.Size = new System.Drawing.Size(120, 13);
            this.lblPlayers.TabIndex = 9;
            this.lblPlayers.Text = "Players/city/generation:";
            // 
            // lblGenerations
            // 
            this.lblGenerations.AutoSize = true;
            this.lblGenerations.Location = new System.Drawing.Point(537, 18);
            this.lblGenerations.Name = "lblGenerations";
            this.lblGenerations.Size = new System.Drawing.Size(67, 13);
            this.lblGenerations.TabIndex = 10;
            this.lblGenerations.Text = "Generations:";
            // 
            // nudGenerations
            // 
            this.nudGenerations.Location = new System.Drawing.Point(610, 15);
            this.nudGenerations.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudGenerations.Name = "nudGenerations";
            this.nudGenerations.Size = new System.Drawing.Size(47, 20);
            this.nudGenerations.TabIndex = 11;
            this.nudGenerations.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblAverageVPs
            // 
            this.lblAverageVPs.AutoSize = true;
            this.lblAverageVPs.Location = new System.Drawing.Point(187, 65);
            this.lblAverageVPs.Name = "lblAverageVPs";
            this.lblAverageVPs.Size = new System.Drawing.Size(72, 13);
            this.lblAverageVPs.TabIndex = 12;
            this.lblAverageVPs.Text = "Average VPs:";
            // 
            // txtAverageVPs
            // 
            this.txtAverageVPs.Location = new System.Drawing.Point(266, 63);
            this.txtAverageVPs.Name = "txtAverageVPs";
            this.txtAverageVPs.ReadOnly = true;
            this.txtAverageVPs.Size = new System.Drawing.Size(68, 20);
            this.txtAverageVPs.TabIndex = 13;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // colGeneration
            // 
            this.colGeneration.HeaderText = "Generation";
            this.colGeneration.Name = "colGeneration";
            // 
            // City
            // 
            this.City.HeaderText = "City";
            this.City.Name = "City";
            // 
            // ColDna
            // 
            this.ColDna.HeaderText = "DNA";
            this.ColDna.Name = "ColDna";
            this.ColDna.ReadOnly = true;
            // 
            // colGames
            // 
            this.colGames.HeaderText = "Games";
            this.colGames.Name = "colGames";
            this.colGames.ReadOnly = true;
            // 
            // colPosition
            // 
            this.colPosition.HeaderText = "Avg Position";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            // 
            // colVPs
            // 
            this.colVPs.HeaderText = "Avg VPs";
            this.colVPs.Name = "colVPs";
            this.colVPs.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 887);
            this.Controls.Add(this.txtAverageVPs);
            this.Controls.Add(this.lblAverageVPs);
            this.Controls.Add(this.nudGenerations);
            this.Controls.Add(this.lblGenerations);
            this.Controls.Add(this.lblPlayers);
            this.Controls.Add(this.nudPlayers);
            this.Controls.Add(this.btnNextGeneration);
            this.Controls.Add(this.lblGamesPlayed);
            this.Controls.Add(this.nudGamesPlayed);
            this.Controls.Add(this.dgvPlayers);
            this.Controls.Add(this.txtGeneration);
            this.Controls.Add(this.lblGeneration);
            this.Name = "Form1";
            this.Text = "7 Wonders - Evolution";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGamesPlayed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGeneration;
        private System.Windows.Forms.TextBox txtGeneration;
        private System.Windows.Forms.DataGridView dgvPlayers;
        private System.Windows.Forms.NumericUpDown nudGamesPlayed;
        private System.Windows.Forms.Label lblGamesPlayed;
        private System.Windows.Forms.Button btnNextGeneration;
        private System.Windows.Forms.NumericUpDown nudPlayers;
        private System.Windows.Forms.Label lblPlayers;
        private System.Windows.Forms.Label lblGenerations;
        private System.Windows.Forms.NumericUpDown nudGenerations;
        private System.Windows.Forms.Label lblAverageVPs;
        private System.Windows.Forms.TextBox txtAverageVPs;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGeneration;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDna;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGames;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVPs;
    }
}

