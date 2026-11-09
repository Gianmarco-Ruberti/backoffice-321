using backOffice_321.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace backOffice_321
{
    public partial class MainPage : Form
    {
        private BusinessProfile currentBusiness = new BusinessProfile();

        public MainPage()
        {
            InitializeComponent();
            InitializeDefaultData();
            UpdateUI();
        }

        private void InitializeDefaultData()
        {
            currentBusiness = new BusinessProfile
            {
                ID = "COM-0277",
                Name = "L'Antre du Chocomancien",
                Logo = "data:image/webp;base64,UklGRvQzAQBXRUJQVlA4WAoAAAAQAAAAgwMAwQEAQUxQSC0AAAABDzD/ERHCTNs2EMqfbItg5+SI/k9Ad23Gf/zHf/zHf/zHf/zHf…ogFK2YKXLi8ImXaH+PTVNM3sML9gAO/DwBhU3yS12+6669YgAJpfV7c0iudUBipgsDpJDhEG25TD6mOd8y5VKE6zpHHu024zPpIOrbH0YVlzWjpB3XwAEFzIAA==",
                Special = new Product
                {
                    Id = "PROD-3530",
                    Name = "gâteau fondant au chocolat",
                    Picture = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/4R5uRXhpZgAASUkqAAgAAAAAAA4AAAAJAP4ABAABAAAAAQAAAAABBAABAAAAAAEAAAEBBAABA…kGG0nh1xU61h9gAVZHIUAxEbxZ2VdGcDf1FiZWeAv0FmM4fYY8yWuUVXzKtukczdzpqYXM4vY+vFJSu4PgrjeHgIY05CQxMrK26MzQeVEdBXKpXWesGqvkPP/9k=",
                    Price = 2.50m
                },
                Products = new List<Product>
                {
                }
            };
        }

        private void ReadUnixTimeFromJson(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var timeData = JsonSerializer.Deserialize<TimeData>(jsonString);

                if (timeData != null)
                {
                    // Conversion du timestamp Unix en DateTime (heure locale)
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeData.UnixTimeStamp).LocalDateTime;

                    // Affichage de l'heure
                    MessageBox.Show($"Heure convertie : {dateTime}", "Lecture Unix Time", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUI()
        {
            txtBpId.Text = currentBusiness.ID;
            txtBpName.Text = currentBusiness.Name;
            txtBpLogo.Text = currentBusiness.Logo;

            // Affichage du Logo dans le PictureBox
            if (!string.IsNullOrEmpty(currentBusiness.Logo))
            {
                picLogo.Image = Base64ToImage(currentBusiness.Logo);
                picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                picLogo.Image = null;
            }

            if (currentBusiness.Special != null)
            {
                txtSpecialId.Text = currentBusiness.Special.Id;
                txtSpecialName.Text = currentBusiness.Special.Name;
                txtSpecialPicture.Text = currentBusiness.Special.Picture;
                numSpecialPrice.Value = currentBusiness.Special.Price;

                // Affichage de l'image du produit spécial dans le PictureBox
                if (!string.IsNullOrEmpty(currentBusiness.Special.Picture))
                {
                    picSpecialPicture.Image = Base64ToImage(currentBusiness.Special.Picture);
                    picSpecialPicture.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    picSpecialPicture.Image = null;
                }
            }

            lstProducts.Items.Clear();
            foreach (var prod in currentBusiness.Products)
            {
                lstProducts.Items.Add($"{prod.Id} - {prod.Name} ({prod.Price:C2})");
            }
        }

        private System.Drawing.Image Base64ToImage(string base64String)
        {
            try
            {
                // Si c'est un lien URL ou un placeholder textuel, on ne tente pas de décoder en Base64
                if (string.IsNullOrEmpty(base64String) || base64String.StartsWith("http") || !base64String.Contains("</strong>") && base64String.Length < 50)
                {
                    return null; // Ou gérez l'affichage d'une image par défaut
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return System.Drawing.Image.FromStream(ms);
                }
            }
            catch
            {
                // En cas d'erreur de décodage (chaîne corrompue ou placeholder texte)
                return null;
            }
        }

        private bool ValidateInputs()
        {
            if (!Regex.IsMatch(txtBpId.Text, @"^COM-[0-9]{4}$"))
            {
                MessageBox.Show("L'ID du commerce doit respecter le format COM-XXXX (ex: COM-1234)", "Erreur de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtSpecialId.Text, @"^PROD-[0-9]{4}$"))
            {
                MessageBox.Show("L'ID du produit doit respecter le format PROD-XXXX (ex: PROD-1234)", "Erreur de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveFromUI()
        {
            currentBusiness.ID = txtBpId.Text;
            currentBusiness.Name = txtBpName.Text;
            currentBusiness.Logo = txtBpLogo.Text;

            currentBusiness.Special = new Product
            {
                Id = txtSpecialId.Text,
                Name = txtSpecialName.Text,
                Picture = txtSpecialPicture.Text,
                Price = numSpecialPrice.Value
            };
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            SaveFromUI();

            var exportData = new
            {
                ID = currentBusiness.ID,
                Name = currentBusiness.Name,
                Logo = currentBusiness.Logo,
                Special = currentBusiness.Special
            };

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Fichiers JSON (*.json)|*.json",
                FileName = $"{currentBusiness.ID}.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(exportData, options);
                File.WriteAllText(sfd.FileName, jsonString);
                MessageBox.Show("Business Profile exporté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ExportCurrentUnixTime(string filePath)
        {
            // Récupérer le timestamp Unix actuel (en secondes)
            long currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var timeData = new TimeData
            {
                UnixTimeStamp = currentUnixTimestamp
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(timeData, options);

            File.WriteAllText(filePath, jsonString);
            MessageBox.Show("Timestamp Unix exporté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnImportJson_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Fichiers JSON (*.json)|*.json"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string jsonString = File.ReadAllText(ofd.FileName);
                    var loadedBusiness = JsonSerializer.Deserialize<BusinessProfile>(jsonString);

                    if (loadedBusiness != null)
                    {
                        // On met à jour les informations principales
                        currentBusiness.ID = loadedBusiness.ID;
                        currentBusiness.Name = loadedBusiness.Name;
                        currentBusiness.Logo = loadedBusiness.Logo;

                        // Si le fichier contient un produit spécial
                        if (loadedBusiness.Special != null && !string.IsNullOrEmpty(loadedBusiness.Special.Id))
                        {
                            currentBusiness.Special = loadedBusiness.Special;

                            // ASTUCE : Si vous voulez que le produit spécial apparaisse aussi dans la liste sur le côté
                            if (!currentBusiness.Products.Exists(existing => existing.Id == loadedBusiness.Special.Id))
                            {
                                currentBusiness.Products.Add(loadedBusiness.Special);
                            }
                        }

                        // Si le fichier contient d'autres produits, on les fusionne
                        if (loadedBusiness.Products != null && loadedBusiness.Products.Count > 0)
                        {
                            foreach (var p in loadedBusiness.Products)
                            {
                                if (!currentBusiness.Products.Exists(existing => existing.Id == p.Id))
                                {
                                    currentBusiness.Products.Add(p);
                                }
                            }
                        }

                        UpdateUI();
                        MessageBox.Show("Business Profile chargé avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la lecture du fichier : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}