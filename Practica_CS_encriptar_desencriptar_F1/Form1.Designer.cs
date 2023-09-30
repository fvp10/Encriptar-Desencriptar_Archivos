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
            this.desencriptar_ = new System.Windows.Forms.Button();
            this.encriptar_ = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // desencriptar_
            // 
            this.desencriptar_.Location = new System.Drawing.Point(601, 110);
            this.desencriptar_.Name = "desencriptar_";
            this.desencriptar_.Size = new System.Drawing.Size(75, 23);
            this.desencriptar_.TabIndex = 0;
            this.desencriptar_.Text = "Desencriptar";
            this.desencriptar_.UseVisualStyleBackColor = true;
            this.desencriptar_.Click += new System.EventHandler(this.desencriptar__Click);
            // 
            // encriptar_
            // 
            this.encriptar_.Location = new System.Drawing.Point(497, 110);
            this.encriptar_.Name = "encriptar_";
            this.encriptar_.Size = new System.Drawing.Size(75, 23);
            this.encriptar_.TabIndex = 1;
            this.encriptar_.Text = "Encriptar";
            this.encriptar_.UseVisualStyleBackColor = true;
            this.encriptar_.Click += new System.EventHandler(this.encriptar__Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 524);
            this.Controls.Add(this.encriptar_);
            this.Controls.Add(this.desencriptar_);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button desencriptar_;
        private System.Windows.Forms.Button encriptar_;
    }
}

