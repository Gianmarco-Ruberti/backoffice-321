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
            lblDisplayTime = new Label();
            grpBusiness = new GroupBox();
            lblId = new Label();
            lblName = new Label();
            lblLogo = new Label();
            grpSpecial = new GroupBox();
            lblSpId = new Label();
            lblSpName = new Label();
            lblSpPic = new Label();
            lblSpPrice = new Label();
            grpTime = new GroupBox();
            grpProducts = new GroupBox();
            grpSchedule = new GroupBox();
            lblOpenStatus = new Label();
            lblOpenDetails = new Label();
            ((System.ComponentModel.ISupportInitialize)numSpecialPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSpecialPicture).BeginInit();
            grpBusiness.SuspendLayout();
            grpSpecial.SuspendLayout();
            grpTime.SuspendLayout();
            grpProducts.SuspendLayout();
            SuspendLayout();
            // 
            // txtBpId
            // 
            txtBpId.            Location = new Point(150, 29);
            txtBpId.Name = "txtBpId";
            txtBpId.            Size = new Size(260, 23);
            txtBpId.TabIndex = 3;
            // 
            // txtBpName
            // 
            txtBpName.            Location = new Point(150, 64);
            txtBpName.Name = "txtBpName";
            txtBpName.            Size = new Size(260, 23);
            txtBpName.TabIndex = 4;
            // 
            // txtBpLogo
            // 
            txtBpLogo.            Location = new Point(150, 99);
            txtBpLogo.Name = "txtBpLogo";
            txtBpLogo.            Size = new Size(260, 23);
            txtBpLogo.TabIndex = 5;
            // 
            // txtSpecialId
            // 
            txtSpecialId.            Location = new Point(150, 29);
            txtSpecialId.Name = "txtSpecialId";
            txtSpecialId.            Size = new Size(260, 23);
            txtSpecialId.TabIndex = 4;
            // 
            // txtSpecialName
            // 
            txtSpecialName.            Location = new Point(150, 64);
            txtSpecialName.Name = "txtSpecialName";
            txtSpecialName.            Size = new Size(260, 23);
            txtSpecialName.TabIndex = 5;
            // 
            // txtSpecialPicture
            // 
            txtSpecialPicture.            Location = new Point(150, 99);
            txtSpecialPicture.Name = "txtSpecialPicture";
            txtSpecialPicture.            Size = new Size(260, 23);
            txtSpecialPicture.TabIndex = 6;
            // 
            // numSpecialPrice
            // 
            numSpecialPrice.DecimalPlaces = 2;
            numSpecialPrice.Location = new Point(150, 134);
            numSpecialPrice.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numSpecialPrice.Name = "numSpecialPrice";
            numSpecialPrice.Size = new Size(260, 23);
            numSpecialPrice.TabIndex = 7;
            // 
            // lstProducts
            // 
            lstProducts.FormattingEnabled = true;
            lstProducts.IntegralHeight = false;
            lstProducts.ItemHeight = 17;
            lstProducts.Location = new Point(16, 30);
            lstProducts.Name = "lstProducts";
            lstProducts.Size = new Size(268, 380);
            lstProducts.TabIndex = 0;
            lstProducts.SelectedIndexChanged += lstProducts_SelectedIndexChanged;
            // 
            // btnExportJson
            // 
            btnExportJson.Location = new Point(660, 462);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new Size(145, 38);
            btnExportJson.TabIndex = 4;
            btnExportJson.Text = "Exporter Profil JSON";
            btnExportJson.UseVisualStyleBackColor = true;
            btnExportJson.Click += btnExportJson_Click;
            // 
            // btnImportJson
            // 
            btnImportJson.Location = new Point(815, 462);
            btnImportJson.Name = "btnImportJson";
            btnImportJson.Size = new Size(145, 38);
            btnImportJson.TabIndex = 5;
            btnImportJson.Text = "Importer Profil JSON";
            btnImportJson.UseVisualStyleBackColor = true;
            btnImportJson.Click += btnImportJson_Click;
            // 
            // picLogo
            // 
            picLogo.BorderStyle = BorderStyle.FixedSingle;
            picLogo.Location = new Point(450, 25);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(150, 125);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 6;
            picLogo.TabStop = false;
            // 
            // picSpecialPicture
            // 
            picSpecialPicture.BorderStyle = BorderStyle.FixedSingle;
            picSpecialPicture.Location = new Point(450, 25);
            picSpecialPicture.Name = "picSpecialPicture";
            picSpecialPicture.Size = new Size(150, 165);
            picSpecialPicture.SizeMode = PictureBoxSizeMode.Zoom;
            picSpecialPicture.TabIndex = 8;
            picSpecialPicture.TabStop = false;
            // 
            // lblDisplayTime
            // 
            lblDisplayTime.AutoSize = true;
            lblDisplayTime.Location = new Point(20, 40);
            lblDisplayTime.Name = "lblDisplayTime";
            lblDisplayTime.Size = new Size(97, 15);
            lblDisplayTime.TabIndex = 2;
            lblDisplayTime.Text = "Heure : (Aucune)";
            // 
            // grpBusiness
            // 
            grpBusiness.Controls.Add(lblId);
            grpBusiness.Controls.Add(lblName);
            grpBusiness.Controls.Add(lblLogo);
            grpBusiness.Controls.Add(txtBpId);
            grpBusiness.Controls.Add(txtBpName);
            grpBusiness.Controls.Add(txtBpLogo);
            grpBusiness.Controls.Add(picLogo);
            grpBusiness.Location = new Point(20, 20);
            grpBusiness.Name = "grpBusiness";
            grpBusiness.Size = new Size(620, 175);
            grpBusiness.TabIndex = 0;
            grpBusiness.TabStop = false;
            grpBusiness.Text = "Informations du Commerce";
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(20, 32);
            lblId.Name = "lblId";
            lblId.Size = new Size(86, 15);
            lblId.TabIndex = 0;
            lblId.Text = "ID Commerce :";
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(20, 67);
            lblName.Name = "lblName";
            lblName.Size = new Size(40, 15);
            lblName.TabIndex = 1;
            lblName.Text = "Nom :";
            // 
            // lblLogo
            // 
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(20, 102);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(87, 15);
            lblLogo.TabIndex = 2;
            lblLogo.Text = "Logo (Base64) :";
            // 
            // grpSpecial
            // 
            grpSpecial.Controls.Add(lblSpId);
            grpSpecial.Controls.Add(lblSpName);
            grpSpecial.Controls.Add(lblSpPic);
            grpSpecial.Controls.Add(lblSpPrice);
            grpSpecial.Controls.Add(txtSpecialId);
            grpSpecial.Controls.Add(txtSpecialName);
            grpSpecial.Controls.Add(txtSpecialPicture);
            grpSpecial.Controls.Add(numSpecialPrice);
            grpSpecial.Controls.Add(picSpecialPicture);
            grpSpecial.Location = new Point(20, 210);
            grpSpecial.Name = "grpSpecial";
            grpSpecial.Size = new Size(620, 205);
            grpSpecial.TabIndex = 1;
            grpSpecial.TabStop = false;
            grpSpecial.Text = "Produit Spécial / Vedette";
            // 
            // lblSpId
            // 
            lblSpId.AutoSize = true;
            lblSpId.Location = new Point(20, 32);
            lblSpId.Name = "lblSpId";
            lblSpId.Size = new Size(66, 15);
            lblSpId.TabIndex = 0;
            lblSpId.Text = "ID Produit :";
            // 
            // lblSpName
            // 
            lblSpName.AutoSize = true;
            lblSpName.Location = new Point(20, 67);
            lblSpName.Name = "lblSpName";
            lblSpName.Size = new Size(40, 15);
            lblSpName.TabIndex = 1;
            lblSpName.Text = "Nom :";
            // 
            // lblSpPic
            // 
            lblSpPic.AutoSize = true;
            lblSpPic.Location = new Point(20, 102);
            lblSpPic.Name = "lblSpPic";
            lblSpPic.Size = new Size(93, 15);
            lblSpPic.TabIndex = 2;
            lblSpPic.Text = "Image (Base64) :";
            // 
            // lblSpPrice
            // 
            lblSpPrice.AutoSize = true;
            lblSpPrice.Location = new Point(20, 137);
            lblSpPrice.Name = "lblSpPrice";
            lblSpPrice.Size = new Size(49, 15);
            lblSpPrice.TabIndex = 3;
            lblSpPrice.Text = "Prix (€) :";
            // 
            // grpTime
            // 
            grpTime.Controls.Add(lblDisplayTime);
            grpTime.Location = new Point(20, 430);
            grpTime.Name = "grpTime";
            grpTime.Size = new Size(620, 95);
            grpTime.TabIndex = 2;
            grpTime.TabStop = false;
            grpTime.Text = "Synchronisation de l'Heure Unix";
            // 
            // grpProducts
            // 
            grpProducts.Controls.Add(lstProducts);
            grpProducts.Location = new Point(660, 20);
            grpProducts.Name = "grpProducts";
            grpProducts.Size = new Size(300, 425);
            grpProducts.TabIndex = 3;
            grpProducts.TabStop = false;
            grpProducts.Text = "Liste des Produits";
            // 
            // grpSchedule
            // 
            grpSchedule.Controls.Add(lblOpenStatus);
            grpSchedule.Controls.Add(lblOpenDetails);
            grpSchedule.Location = new Point(980, 20);
            grpSchedule.Name = "grpSchedule";
            grpSchedule.Size = new Size(300, 330);
            grpSchedule.TabIndex = 6;
            grpSchedule.TabStop = false;
            grpSchedule.Text = "Horaires d'ouverture";
            // 
            // lblOpenStatus
            // 
            lblOpenStatus.BorderStyle = BorderStyle.FixedSingle;
            lblOpenStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOpenStatus.Location = new Point(15, 22);
            lblOpenStatus.Name = "lblOpenStatus";
            lblOpenStatus.Size = new Size(270, 40);
            lblOpenStatus.TabIndex = 0;
            lblOpenStatus.Text = "—";
            lblOpenStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblOpenDetails
            // 
            lblOpenDetails.ForeColor = SystemColors.GrayText;
            lblOpenDetails.Location = new Point(15, 66);
            lblOpenDetails.Name = "lblOpenDetails";
            lblOpenDetails.Size = new Size(270, 20);
            lblOpenDetails.TabIndex = 1;
            lblOpenDetails.Text = "";
            lblOpenDetails.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1300, 560);
            Controls.Add(grpBusiness);
            Controls.Add(grpSpecial);
            Controls.Add(grpTime);
            Controls.Add(grpProducts);
            Controls.Add(grpSchedule);
            Controls.Add(btnExportJson);
            Controls.Add(btnImportJson);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainPage";
            Text = "BackOffice - Business Profile";
            ((System.ComponentModel.ISupportInitialize)numSpecialPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSpecialPicture).EndInit();
            grpBusiness.ResumeLayout(false);
            grpBusiness.PerformLayout();
            grpSpecial.ResumeLayout(false);
            grpSpecial.PerformLayout();
            grpTime.ResumeLayout(false);
            grpTime.PerformLayout();
            grpProducts.ResumeLayout(false);
            ResumeLayout(false);
        }
        private GroupBox grpBusiness;
        private Label lblId;
        private Label lblName;
        private Label lblLogo;
        private GroupBox grpSpecial;
        private Label lblSpId;
        private Label lblSpName;
        private Label lblSpPic;
        private Label lblSpPrice;
        private GroupBox grpTime;
        private GroupBox grpProducts;
        private GroupBox grpSchedule;
        private Label lblOpenStatus;
        private Label lblOpenDetails;
    }
}

#endregion