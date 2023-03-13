namespace SharpGrid
{
    partial class SharpGrid
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
            this.SuspendLayout();
            // 
            // SharpGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SharpGrid";
            this.Size = new System.Drawing.Size(160, 160);
            this.Load += new System.EventHandler(this.SharpGrid_Load);
            this.BackColorChanged += new System.EventHandler(this.SharpGrid_BackColorChanged);
            this.ForeColorChanged += new System.EventHandler(this.SharpGrid_ForeColorChanged);
            this.SizeChanged += new System.EventHandler(this.SharpGrid_SizeChanged);
            this.Click += new System.EventHandler(this.SharpGrid_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SharpGrid_Paint);
            this.DoubleClick += new System.EventHandler(this.SharpGrid_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SharpGrid_MouseDown);
            this.MouseLeave += new System.EventHandler(this.SharpGrid_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SharpGrid_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SharpGrid_MouseUp);
            this.Resize += new System.EventHandler(this.SharpGrid_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
