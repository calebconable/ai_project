namespace VS_Project.Forms
{
    partial class KMeansForm
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
            this.scottPlot = new ScottPlot.FormsPlot();
            this.SuspendLayout();
            // 
            // scottPlot
            // 
            this.scottPlot.Location = new System.Drawing.Point(36, 26);
            this.scottPlot.Name = "scottPlot";
            this.scottPlot.Size = new System.Drawing.Size(400, 300);
            this.scottPlot.TabIndex = 0;
            // 
            // KMeansForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.scottPlot);
            this.Name = "KMeansForm";
            this.Text = "KMeansForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ScottPlot.FormsPlot scottPlot;
    }
}