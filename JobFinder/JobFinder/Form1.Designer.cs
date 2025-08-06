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
            lb_Search_Explanation = new Label();
            txtb_Search = new TextBox();
            lb_Search_City = new Label();
            textBox1 = new TextBox();
            btn_Teste = new Button();
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
            // lb_Search_Explanation
            // 
            lb_Search_Explanation.AllowDrop = true;
            lb_Search_Explanation.AutoSize = true;
            lb_Search_Explanation.Location = new Point(12, 38);
            lb_Search_Explanation.Name = "lb_Search_Explanation";
            lb_Search_Explanation.Size = new Size(474, 15);
            lb_Search_Explanation.TabIndex = 2;
            lb_Search_Explanation.Text = "Por favor meta todos os termos que quer que sejam procurados, separados por um enter";
            // 
            // txtb_Search
            // 
            txtb_Search.Location = new Point(12, 56);
            txtb_Search.Multiline = true;
            txtb_Search.Name = "txtb_Search";
            txtb_Search.PlaceholderText = "Examples:\\n.Net Junior Dev\\C# Dev\\nC# software developer";
            txtb_Search.Size = new Size(255, 164);
            txtb_Search.TabIndex = 3;
            // 
            // lb_Search_City
            // 
            lb_Search_City.AutoSize = true;
            lb_Search_City.Location = new Point(12, 223);
            lb_Search_City.Name = "lb_Search_City";
            lb_Search_City.Size = new Size(231, 15);
            lb_Search_City.TabIndex = 4;
            lb_Search_City.Text = "Cidade, se mais que uma , pf seprar por \";\"";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 241);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Exemplo: Porto;Lisboa";
            textBox1.Size = new Size(255, 23);
            textBox1.TabIndex = 5;
            // 
            // btn_Teste
            // 
            btn_Teste.AccessibleRole = AccessibleRole.MenuBar;
            btn_Teste.Location = new Point(533, 8);
            btn_Teste.Name = "btn_Teste";
            btn_Teste.Size = new Size(118, 27);
            btn_Teste.TabIndex = 6;
            btn_Teste.Text = "teste";
            btn_Teste.UseVisualStyleBackColor = true;
            btn_Teste.Click += btn_Teste_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btn_Teste);
            Controls.Add(textBox1);
            Controls.Add(lb_Search_City);
            Controls.Add(txtb_Search);
            Controls.Add(lb_Search_Explanation);
            Controls.Add(btn_Begin);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_Begin;
        private Label lb_Search_Explanation;
        private TextBox txtb_Search;
        private Label lb_Search_City;
        private TextBox textBox1;
        private Button btn_Teste;
    }
}
