namespace backOffice_321
{
    partial class MainPage
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
            this.grpCommerce = new System.Windows.Forms.GroupBox();
            this.btnChooseLogo = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.txtComName = new System.Windows.Forms.TextBox();
            this.lblComName = new System.Windows.Forms.Label();
            this.txtComId = new System.Windows.Forms.TextBox();
            this.lblComId = new System.Windows.Forms.Label();

            this.grpProduit = new System.Windows.Forms.GroupBox();
            this.numPrice = new System.Windows.Forms.NumericUpDown();
            this.lblPrice = new System.Windows.Forms.Label();
            this.btnChooseProdPic = new System.Windows.Forms.Button();
            this.picProdPicture = new System.Windows.Forms.PictureBox();
            this.txtProdName = new System.Windows.Forms.TextBox();
            this.lblProdName = new System.Windows.Forms.Label();
            this.txtProdId = new System.Windows.Forms.TextBox();
            this.lblProdId = new System.Windows.Forms.Label();

            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();

            this.grpCommerce.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.grpProduit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProdPicture)).BeginInit();
            this.SuspendLayout();

            // 
            // grpCommerce
            // 
            this.grpCommerce.Controls.Add(this.btnChooseLogo);
            this.grpCommerce.Controls.Add(this.picLogo);
            this.grpCommerce.Controls.Add(this.txtComName);
            this.grpCommerce.Controls.Add(this.lblComName);
            this.grpCommerce.Controls.Add(this.txtComId);
            this.grpCommerce.Controls.Add(this.lblComId);
            this.grpCommerce.Location = new System.Drawing.Point(12, 12);
            this.grpCommerce.Name = "grpCommerce";
            this.grpCommerce.Size = new System.Drawing.Size(400, 200);
            this.grpCommerce.TabIndex = 0;
            this.grpCommerce.TabStop = false;
            this.grpCommerce.Text = "Informations de la boulangerie";

            // btnChooseLogo
            this.btnChooseLogo.Location = new System.Drawing.Point(180, 100);
            this.btnChooseLogo.Name = "btnChooseLogo";
            this.btnChooseLogo.Size = new System.Drawing.Size(110, 30);
            this.btnChooseLogo.TabIndex = 5;
            this.btnChooseLogo.Text = "Choisir logo";
            this.btnChooseLogo.UseVisualStyleBackColor = true;
            this.btnChooseLogo.Click += new System.EventHandler(this.btnChooseLogo_Click);

            // picLogo
            this.picLogo.Location = new System.Drawing.Point(20, 100);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(140, 80);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 4;
            this.picLogo.TabStop = false;

            // txtComName
            this.txtComName.Location = new System.Drawing.Point(120, 60);
            this.txtComName.Name = "txtComName";
            this.txtComName.Size = new System.Drawing.Size(200, 23);
            this.txtComName.TabIndex = 3;

            // lblComName
            this.lblComName.AutoSize = true;
            this.lblComName.Location = new System.Drawing.Point(20, 63);
            this.lblComName.Name = "lblComName";
            this.lblComName.Size = new System.Drawing.Size(34, 15);
            this.lblComName.TabIndex = 2;
            this.lblComName.Text = "Nom:";

            // txtComId
            this.txtComId.Location = new System.Drawing.Point(120, 25);
            this.txtComId.Name = "txtComId";
            this.txtComId.Size = new System.Drawing.Size(200, 23);
            this.txtComId.TabIndex = 1;
            this.txtComId.Text = "COM-1234";

            // lblComId
            this.lblComId.AutoSize = true;
            this.lblComId.Location = new System.Drawing.Point(20, 28);
            this.lblComId.Name = "lblComId";
            this.lblComId.Size = new System.Drawing.Size(76, 15);
            this.lblComId.TabIndex = 0;
            this.lblComId.Text = "ID :";

            // 
            // grpProduit
            // 
            this.grpProduit.Controls.Add(this.numPrice);
            this.grpProduit.Controls.Add(this.lblPrice);
            this.grpProduit.Controls.Add(this.btnChooseProdPic);
            this.grpProduit.Controls.Add(this.picProdPicture);
            this.grpProduit.Controls.Add(this.txtProdName);
            this.grpProduit.Controls.Add(this.lblProdName);
            this.grpProduit.Controls.Add(this.txtProdId);
            this.grpProduit.Controls.Add(this.lblProdId);
            this.grpProduit.Location = new System.Drawing.Point(12, 230);
            this.grpProduit.Name = "grpProduit";
            this.grpProduit.Size = new System.Drawing.Size(400, 230);
            this.grpProduit.TabIndex = 1;
            this.grpProduit.TabStop = false;
            this.grpProduit.Text = "Produit Phare";

            // numPrice
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Location = new System.Drawing.Point(120, 95);
            this.numPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numPrice.Name = "numPrice";
            this.numPrice.Size = new System.Drawing.Size(100, 23);
            this.numPrice.TabIndex = 7;
            this.numPrice.Value = new decimal(new int[] { 250, 0, 0, 131072 });

            // lblPrice
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(20, 97);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(30, 15);
            this.lblPrice.TabIndex = 6;
            this.lblPrice.Text = "Prix:";

            // btnChooseProdPic
            this.btnChooseProdPic.Location = new System.Drawing.Point(140, 130);
            this.btnChooseProdPic.Name = "btnChooseProdPic";
            this.btnChooseProdPic.Size = new System.Drawing.Size(110, 30);
            this.btnChooseProdPic.TabIndex = 5;
            this.btnChooseProdPic.Text = "Choisir photo";
            this.btnChooseProdPic.UseVisualStyleBackColor = true;
            this.btnChooseProdPic.Click += new System.EventHandler(this.btnChooseProdPic_Click);

            // picProdPicture
            this.picProdPicture.Location = new System.Drawing.Point(20, 130);
            this.picProdPicture.Name = "picProdPicture";
            this.picProdPicture.Size = new System.Drawing.Size(90, 90);
            this.picProdPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picProdPicture.TabIndex = 4;
            this.picProdPicture.TabStop = false;

            // txtProdName
            this.txtProdName.Location = new System.Drawing.Point(120, 60);
            this.txtProdName.Name = "txtProdName";
            this.txtProdName.Size = new System.Drawing.Size(200, 23);
            this.txtProdName.TabIndex = 3;

            // lblProdName
            this.lblProdName.AutoSize = true;
            this.lblProdName.Location = new System.Drawing.Point(20, 63);
            this.lblProdName.Name = "lblProdName";
            this.lblProdName.Size = new System.Drawing.Size(34, 15);
            this.lblProdName.TabIndex = 2;
            this.lblProdName.Text = "Nom:";

            // txtProdId
            this.txtProdId.Location = new System.Drawing.Point(120, 25);
            this.txtProdId.Name = "txtProdId";
            this.txtProdId.Size = new System.Drawing.Size(200, 23);
            this.txtProdId.TabIndex = 1;
            this.txtProdId.Text = "PROD-0001";

            // lblProdId
            this.lblProdId.AutoSize = true;
            this.lblProdId.Location = new System.Drawing.Point(20, 28);
            this.lblProdId.Name = "lblProdId";
            this.lblProdId.Size = new System.Drawing.Size(84, 15);
            this.lblProdId.TabIndex = 0;
            this.lblProdId.Text = "ID:";

            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 475);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Sauvegarder (JSON)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(222, 475);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(190, 35);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Charger (JSON)";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 525);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpProduit);
            this.Controls.Add(this.grpCommerce);
            this.Name = "MainPage";
            this.Text = "Backoffice - Module 321";
            this.grpCommerce.ResumeLayout(false);
            this.grpCommerce.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.grpProduit.ResumeLayout(false);
            this.grpProduit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picProdPicture)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpCommerce;
        private System.Windows.Forms.TextBox txtComId;
        private System.Windows.Forms.Label lblComId;
        private System.Windows.Forms.TextBox txtComName;
        private System.Windows.Forms.Label lblComName;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnChooseLogo;

        private System.Windows.Forms.GroupBox grpProduit;
        private System.Windows.Forms.TextBox txtProdId;
        private System.Windows.Forms.Label lblProdId;
        private System.Windows.Forms.TextBox txtProdName;
        private System.Windows.Forms.Label lblProdName;
        private System.Windows.Forms.NumericUpDown numPrice;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.PictureBox picProdPicture;
        private System.Windows.Forms.Button btnChooseProdPic;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
    }
}