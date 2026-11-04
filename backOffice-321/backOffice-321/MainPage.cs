using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace backOffice_321
{
    public partial class MainPage : Form
    {
        // Modèle minimal de Business Profile
        private Boulangerie currentBp = new Boulangerie();

        public MainPage()
        {
            InitializeComponent();

            // assigne des valeurs fixes
            currentBp = new Boulangerie()
            {
                Id = "COM-0277",
                Name = "L'Antre du Chocomancien",
                Special = new Product()
                {
                    Id = "PROD-3530",
                    Name = "gâteau fondant au chocolat",
                    Price = 5.00m
                }
            };

            // affiche dans l'interface
            txtComId.Text = currentBp.Id;
            txtComName.Text = currentBp.Name;

            txtProdId.Text = currentBp.Special.Id;
            txtProdName.Text = currentBp.Special.Name;
            numPrice.Value = currentBp.Special.Price;

            // bloque les champs pour qu'ils ne puissent pas être changés
            txtComId.ReadOnly = true;
            txtComName.ReadOnly = true;
            txtProdId.ReadOnly = true;
            txtProdName.ReadOnly = true;
        }

        // --- Gestion des images et boutons ---

        private void btnChooseLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentBp.Logo = ImageHelper.FileToBase64(ofd.FileName);
                }
            }
        }

        private void btnChooseProdPic_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (currentBp.Special == null) currentBp.Special = new Product();
                    currentBp.Special.Picture = ImageHelper.FileToBase64(ofd.FileName);
                }
            }
        }

        // --- Sauvegarde en JSON ---

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Récupération des valeurs des champs texte de l'UI
                currentBp.Id = txtComId.Text;
                currentBp.Name = txtComName.Text;

                if (currentBp.Special != null)
                {
                    currentBp.Special.Id = txtProdId.Text;
                    currentBp.Special.Name = txtProdName.Text;
                    // currentBp.Special.Price = numPrice.Value; // Décommente si tu utilises un NumericUpDown
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(currentBp, options);

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "JSON files (*.json)|*.json";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, jsonString);
                        MessageBox.Show("Business sauvegardé avec succès !");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la sauvegarde : " + ex.Message);
            }
        }

        // --- Chargement d'un JSON (ex: reçu d'un camarade) ---

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string jsonString = File.ReadAllText(ofd.FileName);
                    currentBp = JsonSerializer.Deserialize<Boulangerie>(jsonString);

                    // Remise à jour des champs visuels avec les données chargées
                    txtComId.Text = currentBp.Id;
                    txtComName.Text = currentBp.Name;

                    if (currentBp.Special != null)
                    {
                        txtProdId.Text = currentBp.Special.Id;
                        txtProdName.Text = currentBp.Special.Name;
                        // numPrice.Value = currentBp.Special.Price;
                    }

                    MessageBox.Show("Business chargé avec succès !");
                }
            }
        }
    }

    // ==========================================
    // CLASSES DE MODÈLES ET UTILITAIRE
    // ==========================================

    public class Product
    {
        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"^PROD-[0-9]{4}$"))
                    throw new ArgumentException("Format d'ID produit invalide (attendu: PROD-XXXX)");
                _id = value;
            }
        }
        public string Name { get; set; }
        public string Picture { get; set; } // Base64
        public decimal Price { get; set; }
    }

    public class Boulangerie
    {
        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"^COM-[0-9]{4}$"))
                    throw new ArgumentException("Format d'ID commerce invalide (attendu: COM-XXXX)");
                _id = value;
            }
        }
        public string Name { get; set; }
        public string Logo { get; set; } // Base64
        public Product Special { get; set; }
    }

    public static class ImageHelper
    {
        public static string FileToBase64(string filePath)
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(imageBytes);
        }

        public static System.Drawing.Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return new System.Drawing.Bitmap(System.Drawing.Image.FromStream(ms));
            }
        }
    }
}