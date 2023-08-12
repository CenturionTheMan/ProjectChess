namespace ChessGui_WF
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.boardUserControl = new ChessGui_WF.BoardUserControl();
            this.initPosTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.unmakeLastMove = new System.Windows.Forms.Button();
            this.allPositionsButton = new System.Windows.Forms.Button();
            this.currentSideLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(518, 467);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(355, 45);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "START";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // boardUserControl
            // 
            this.boardUserControl.BackColor = System.Drawing.Color.SaddleBrown;
            this.boardUserControl.Location = new System.Drawing.Point(12, 12);
            this.boardUserControl.Name = "boardUserControl";
            this.boardUserControl.Size = new System.Drawing.Size(500, 500);
            this.boardUserControl.TabIndex = 1;
            this.boardUserControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.boardUserControl_MouseDown);
            // 
            // initPosTextBox
            // 
            this.initPosTextBox.Location = new System.Drawing.Point(109, 0);
            this.initPosTextBox.Multiline = true;
            this.initPosTextBox.Name = "initPosTextBox";
            this.initPosTextBox.Size = new System.Drawing.Size(246, 43);
            this.initPosTextBox.TabIndex = 2;
            this.initPosTextBox.Text = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "INITIAL POSITION:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.initPosTextBox);
            this.panel1.Location = new System.Drawing.Point(518, 418);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(355, 43);
            this.panel1.TabIndex = 4;
            // 
            // unmakeLastMove
            // 
            this.unmakeLastMove.Location = new System.Drawing.Point(521, 158);
            this.unmakeLastMove.Name = "unmakeLastMove";
            this.unmakeLastMove.Size = new System.Drawing.Size(191, 52);
            this.unmakeLastMove.TabIndex = 5;
            this.unmakeLastMove.Text = "UNMAKE LAST MOVE";
            this.unmakeLastMove.UseVisualStyleBackColor = true;
            this.unmakeLastMove.Click += new System.EventHandler(this.unmakeLastMove_Click);
            // 
            // allPositionsButton
            // 
            this.allPositionsButton.Location = new System.Drawing.Point(521, 216);
            this.allPositionsButton.Name = "allPositionsButton";
            this.allPositionsButton.Size = new System.Drawing.Size(191, 43);
            this.allPositionsButton.TabIndex = 6;
            this.allPositionsButton.Text = "ShowAllPositions";
            this.allPositionsButton.UseVisualStyleBackColor = true;
            this.allPositionsButton.Click += new System.EventHandler(this.allPositionsButton_Click);
            // 
            // currentSideLabel
            // 
            this.currentSideLabel.AutoSize = true;
            this.currentSideLabel.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.currentSideLabel.Location = new System.Drawing.Point(518, 12);
            this.currentSideLabel.Name = "currentSideLabel";
            this.currentSideLabel.Size = new System.Drawing.Size(70, 24);
            this.currentSideLabel.TabIndex = 7;
            this.currentSideLabel.Text = "TEMP";
            this.currentSideLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Linen;
            this.ClientSize = new System.Drawing.Size(881, 521);
            this.Controls.Add(this.currentSideLabel);
            this.Controls.Add(this.allPositionsButton);
            this.Controls.Add(this.unmakeLastMove);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.boardUserControl);
            this.Controls.Add(this.startButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BoardUserControl boardUserControl1;
        private Button startButton;
        private BoardUserControl boardUserControl;
        private TextBox initPosTextBox;
        private Label label1;
        private Panel panel1;
        private Button unmakeLastMove;
        private Button allPositionsButton;
        private Label currentSideLabel;
    }
}