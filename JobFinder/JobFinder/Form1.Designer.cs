using OpenQA.Selenium;

namespace JobFinder
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
            btn_Begin = new Button();
            SuspendLayout();
            // 
            // btn_Begin
            // 
            btn_Begin.Location = new Point(12, 12);
            btn_Begin.Name = "btn_Begin";
            btn_Begin.Size = new Size(75, 23);
            btn_Begin.TabIndex = 0;
            btn_Begin.Text = "Begin";
            btn_Begin.UseVisualStyleBackColor = true;
            btn_Begin.Click += btn_Begin_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btn_Begin);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_Begin;

    }
}
