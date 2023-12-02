namespace Practica_CS_encriptar_desencriptar_F1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.encriptar_ = new System.Windows.Forms.Button();
            this.txt1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_selc = new System.Windows.Forms.Button();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.panelDESENC = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.compartir_ = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // encriptar_
            // 
            this.encriptar_.Location = new System.Drawing.Point(1018, 180);
            this.encriptar_.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.encriptar_.Name = "encriptar_";
            this.encriptar_.Size = new System.Drawing.Size(112, 35);
            this.encriptar_.TabIndex = 1;
            this.encriptar_.Text = "Encriptar";
            this.encriptar_.UseVisualStyleBackColor = true;
            this.encriptar_.Click += new System.EventHandler(this.encriptar__Click);
            // 
            // txt1
            // 
            this.txt1.Enabled = false;
            this.txt1.Location = new System.Drawing.Point(636, 185);
            this.txt1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt1.Name = "txt1";
            this.txt1.Size = new System.Drawing.Size(337, 26);
            this.txt1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_selc
            // 
            this.btn_selc.Location = new System.Drawing.Point(477, 182);
            this.btn_selc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_selc.Name = "btn_selc";
            this.btn_selc.Size = new System.Drawing.Size(112, 35);
            this.btn_selc.TabIndex = 3;
            this.btn_selc.Text = "Seleccionar";
            this.btn_selc.UseVisualStyleBackColor = true;
            this.btn_selc.Click += new System.EventHandler(this.btn_selc_Click);
            // 
            // panelContenedor
            // 
            this.panelContenedor.Location = new System.Drawing.Point(90, 295);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(766, 474);
            this.panelContenedor.TabIndex = 5;
            this.panelContenedor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelContenedor_Paint);
            // 
            // panelDESENC
            // 
            this.panelDESENC.Location = new System.Drawing.Point(906, 295);
            this.panelDESENC.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelDESENC.Name = "panelDESENC";
            this.panelDESENC.Size = new System.Drawing.Size(656, 474);
            this.panelDESENC.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(369, 272);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Archivos encriptados";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1140, 272);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Archivos Desencriptados";
            // 
            // compartir_
            // 
            this.compartir_.Location = new System.Drawing.Point(1144, 180);
            this.compartir_.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.compartir_.Name = "compartir_";
            this.compartir_.Size = new System.Drawing.Size(112, 35);
            this.compartir_.TabIndex = 9;
            this.compartir_.Text = "Compartir";
            this.compartir_.UseVisualStyleBackColor = true;
            this.compartir_.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1648, 817);
            this.Controls.Add(this.compartir_);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelDESENC);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.btn_selc);
            this.Controls.Add(this.txt1);
            this.Controls.Add(this.encriptar_);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button encriptar_;
        private System.Windows.Forms.TextBox txt1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_selc;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.Panel panelDESENC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button compartir_;
    }
}

