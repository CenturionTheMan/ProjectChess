namespace ChessGui_WF
{
    partial class PieceUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PieceUserControl));
            this.SuspendLayout();
            // 
            // PieceUserControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "PieceUserControl";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PieceUserControl_MouseDown);
            this.MouseLeave += new System.EventHandler(this.PieceUserControl_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PieceUserControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PieceUserControl_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
