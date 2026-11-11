using backOffice_321.data;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet;
using SharpImage = SixLabors.ImageSharp.Image;

namespace backOffice_321
{
    public partial class MainPage : Form
    {
        private BusinessProfile currentBusiness = new BusinessProfile();
        private IMqttClient mqttClient;

        // ==========================================
        // DÉCLARATION DES CONTRÔLES (CHAMPS DE LA CLASSE)
        // ==========================================
        private TextBox txtBpId;
        private TextBox txtBpName;
        private TextBox txtBpLogo;
        private TextBox txtSpecialId;
        private TextBox txtSpecialName;
        private TextBox txtSpecialPicture;
        private NumericUpDown numSpecialPrice;
        private ListBox lstProducts;
        private Button btnExportJson;
        private Button btnImportJson;
        private PictureBox picLogo;
        private PictureBox picSpecialPicture;
        private Label lblDisplayTime;

        public MainPage()
        {
            InitializeComponent();
            InitializeDefaultData();
            UpdateUI();

            // Initialisation et connexion au broker MQTT en arrière-plan
            _ = InitializeMqttAsync();
        }

        private async Task InitializeMqttAsync()
        {
            try
            {
                var mqttFactory = new MqttClientFactory();
                mqttClient = mqttFactory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("mqqt.blue.section-inf.ch", 1883)
                    .WithCredentials("ict","321")
                    .Build();

                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    string topic = e.ApplicationMessage.Topic;

                    if (topic == "umddcmid3b/time")
                    {
                        string payload = e.ApplicationMessage.ConvertPayloadToString();

                        try
                        {
                            // Ajout d'options pour sécuriser la lecture JSON
                            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var timeData = JsonSerializer.Deserialize<TimeData>(payload, jsonOptions);

                            if (timeData != null)
                            {
                                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeData.UnixTimeStamp).LocalDateTime;

                                if (this.IsHandleCreated)
                                {
                                    this.Invoke((MethodInvoker)delegate {
                                        lblDisplayTime.Text = $"Heure : {dateTime:G}";
                                        lblDisplayTime.Refresh();
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Si une erreur survient, elle s'affichera dans la console de debug de Visual Studio
                            System.Diagnostics.Debug.WriteLine($"Erreur MQTT JSON : {ex.Message}");
                        }
                    }

                    return Task.CompletedTask;
                };

                await mqttClient.ConnectAsync(options, CancellationToken.None);

                var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter("umddcmid3b/time")
                    .Build();

                await mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de connexion MQTT : {ex.Message}", "Erreur MQTT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeDefaultData()
        {
            currentBusiness = new BusinessProfile
            {
                ID = "COM-0277",
                Name = "L'Antre du Chocomancien",
                Logo = "",
                Special = new Product
                {
                    Id = "PROD-3530",
                    Name = "gâteau fondant au chocolat",
                    Picture = "",
                    Price = 2.50m
                },
                Products = new List<Product>()
            };
        }

        private void UpdateUI()
        {
            txtBpId.Text = currentBusiness.ID;
            txtBpName.Text = currentBusiness.Name;
            txtBpLogo.Text = currentBusiness.Logo;

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
                if (string.IsNullOrEmpty(base64String))
                    return null;

                if (base64String.Contains(","))
                {
                    var parts = base64String.Split(',');
                    if (parts.Length > 1)
                    {
                        base64String = parts[1];
                    }
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);

                using (var image = SixLabors.ImageSharp.Image.Load(imageBytes))
                {
                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        return new Bitmap(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'affichage de l'image : {ex.Message}",
                        "Erreur Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        currentBusiness.ID = loadedBusiness.ID;
                        currentBusiness.Name = loadedBusiness.Name;
                        currentBusiness.Logo = loadedBusiness.Logo;

                        if (loadedBusiness.Special != null && !string.IsNullOrEmpty(loadedBusiness.Special.Id))
                        {
                            currentBusiness.Special = loadedBusiness.Special;

                            if (!currentBusiness.Products.Exists(existing => existing.Id == loadedBusiness.Special.Id))
                            {
                                currentBusiness.Products.Add(loadedBusiness.Special);
                            }
                        }

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

        private void btnExportTime_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Fichiers JSON (*.json)|*.json",
                FileName = "timedata.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    long currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    var timeData = new TimeData
                    {
                        UnixTimeStamp = (int)currentUnixTimestamp
                    };

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(timeData, options);

                    File.WriteAllText(sfd.FileName, jsonString);
                    MessageBox.Show("Timestamp Unix exporté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'export : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImportTime_Click(object sender, EventArgs e)
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
                    var timeData = JsonSerializer.Deserialize<TimeData>(jsonString);

                    if (timeData != null)
                    {
                        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeData.UnixTimeStamp).LocalDateTime;
                        lblDisplayTime.Text = $"Heure : {dateTime:G}";
                        MessageBox.Show($"Heure convertie : {dateTime}", "Lecture Unix Time", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la lecture : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}