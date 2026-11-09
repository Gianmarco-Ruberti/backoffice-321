namespace backOffice_321
{
    partial class MainPage
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        ///<param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            txtBpId = new TextBox();
            txtBpName = new TextBox();
            txtBpLogo = new TextBox();
            txtSpecialId = new TextBox();
            txtSpecialName = new TextBox();
            txtSpecialPicture = new TextBox();
            numSpecialPrice = new NumericUpDown();
            lstProducts = new ListBox();
            btnExportJson = new Button();
            btnImportJson = new Button();
            picLogo = new PictureBox();
            picSpecialPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)numSpecialPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSpecialPicture).BeginInit();
            SuspendLayout();
            // 
            // txtBpId
            // 
            txtBpId.Location = new Point(35, 35);
            txtBpId.Margin = new Padding(4, 3, 4, 3);
            txtBpId.Name = "txtBpId";
            txtBpId.Size = new Size(233, 23);
            txtBpId.TabIndex = 0;
            // 
            // txtBpName
            // 
            txtBpName.Location = new Point(35, 81);
            txtBpName.Margin = new Padding(4, 3, 4, 3);
            txtBpName.Name = "txtBpName";
            txtBpName.Size = new Size(233, 23);
            txtBpName.TabIndex = 1;
            // 
            // txtBpLogo
            // 
            txtBpLogo.Location = new Point(35, 127);
            txtBpLogo.Margin = new Padding(4, 3, 4, 3);
            txtBpLogo.MaxLength = 0;
            txtBpLogo.Name = "txtBpLogo";
            txtBpLogo.Size = new Size(233, 23);
            txtBpLogo.TabIndex = 2;
            // 
            // txtSpecialId
            // 
            txtSpecialId.Location = new Point(35, 288);
            txtSpecialId.Margin = new Padding(4, 3, 4, 3);
            txtSpecialId.Name = "txtSpecialId";
            txtSpecialId.Size = new Size(233, 23);
            txtSpecialId.TabIndex = 3;
            // 
            // txtSpecialName
            // 
            txtSpecialName.Location = new Point(35, 335);
            txtSpecialName.Margin = new Padding(4, 3, 4, 3);
            txtSpecialName.Name = "txtSpecialName";
            txtSpecialName.Size = new Size(233, 23);
            txtSpecialName.TabIndex = 4;
            // 
            // txtSpecialPicture
            // 
            txtSpecialPicture.Location = new Point(35, 381);
            txtSpecialPicture.Margin = new Padding(4, 3, 4, 3);
            txtSpecialPicture.MaxLength = 0;
            txtSpecialPicture.Name = "txtSpecialPicture";
            txtSpecialPicture.Size = new Size(233, 23);
            txtSpecialPicture.TabIndex = 5;
            // 
            // numSpecialPrice
            // 
            numSpecialPrice.DecimalPlaces = 2;
            numSpecialPrice.Location = new Point(35, 427);
            numSpecialPrice.Margin = new Padding(4, 3, 4, 3);
            numSpecialPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numSpecialPrice.Name = "numSpecialPrice";
            numSpecialPrice.Size = new Size(233, 23);
            numSpecialPrice.TabIndex = 6;
            // 
            // lstProducts
            // 
            lstProducts.FormattingEnabled = true;
            lstProducts.ItemHeight = 15;
            lstProducts.Location = new Point(607, 35);
            lstProducts.Margin = new Padding(4, 3, 4, 3);
            lstProducts.Name = "lstProducts";
            lstProducts.Size = new Size(291, 439);
            lstProducts.TabIndex = 7;
            // 
            // btnExportJson
            // 
            btnExportJson.Location = new Point(35, 485);
            btnExportJson.Margin = new Padding(4, 3, 4, 3);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new Size(117, 35);
            btnExportJson.TabIndex = 8;
            btnExportJson.Text = "Exporter JSON";
            btnExportJson.UseVisualStyleBackColor = true;
            btnExportJson.Click += btnExportJson_Click;
            // 
            // btnImportJson
            // 
            btnImportJson.Location = new Point(163, 485);
            btnImportJson.Margin = new Padding(4, 3, 4, 3);
            btnImportJson.Name = "btnImportJson";
            btnImportJson.Size = new Size(117, 35);
            btnImportJson.TabIndex = 9;
            btnImportJson.Text = "Importer JSON";
            btnImportJson.UseVisualStyleBackColor = true;
            btnImportJson.Click += btnImportJson_Click;
            // 
            // picLogo
            // 
            picLogo.BorderStyle = BorderStyle.FixedSingle;
            picLogo.Location = new Point(292, 35);
            picLogo.Margin = new Padding(4, 3, 4, 3);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(256, 115);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 10;
            picLogo.TabStop = false;
            // 
            // picSpecialPicture
            // 
            picSpecialPicture.BorderStyle = BorderStyle.FixedSingle;
            picSpecialPicture.Location = new Point(292, 288);
            picSpecialPicture.Margin = new Padding(4, 3, 4, 3);
            picSpecialPicture.Name = "picSpecialPicture";
            picSpecialPicture.Size = new Size(163, 161);
            picSpecialPicture.SizeMode = PictureBoxSizeMode.Zoom;
            picSpecialPicture.TabIndex = 11;
            picSpecialPicture.TabStop = false;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 554);
            Controls.Add(picSpecialPicture);
            Controls.Add(picLogo);
            Controls.Add(btnImportJson);
            Controls.Add(btnExportJson);
            Controls.Add(lstProducts);
            Controls.Add(numSpecialPrice);
            Controls.Add(txtSpecialPicture);
            Controls.Add(txtSpecialName);
            Controls.Add(txtSpecialId);
            Controls.Add(txtBpLogo);
            Controls.Add(txtBpName);
            Controls.Add(txtBpId);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainPage";
            Text = "BackOffice - Business Profile";
            ((System.ComponentModel.ISupportInitialize)numSpecialPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSpecialPicture).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBpId;
        private System.Windows.Forms.TextBox txtBpName;
        private System.Windows.Forms.TextBox txtBpLogo;
        private System.Windows.Forms.TextBox txtSpecialId;
        private System.Windows.Forms.TextBox txtSpecialName;
        private System.Windows.Forms.TextBox txtSpecialPicture;
        private System.Windows.Forms.NumericUpDown numSpecialPrice;
        private System.Windows.Forms.ListBox lstProducts;
        private System.Windows.Forms.Button btnExportJson;
        private System.Windows.Forms.Button btnImportJson;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.PictureBox picSpecialPicture;
    }
}