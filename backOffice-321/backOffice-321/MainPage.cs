using backOffice_321.data;
using backOffice_321.ui;
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
        // DéCLARATION DES CONTRÔLES (CHAMPS DE LA CLASSE)
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

        private readonly WeeklySchedule weeklySchedule = WeeklySchedule.CreateDefault();
        private readonly Dictionary<DayOfWeek, (CheckBox IsOpen, DateTimePicker Open, DateTimePicker Close)> scheduleRows
            = new Dictionary<DayOfWeek, (CheckBox IsOpen, DateTimePicker Open, DateTimePicker Close)>();
        private System.Windows.Forms.Timer scheduleRefreshTimer;

        public MainPage()
        {
            InitializeComponent();
            InitializeDefaultData();
            InitializeScheduleRows();

            // Thème sombre appliqué après la création de tous les contrôles (y compris les lignes d'horaires)
            Theme.Apply(this);
            lblOpenDetails.ForeColor = Theme.Muted;
            lblDisplayTime.ForeColor = Theme.Accent;
            btnImportJson.BackColor = Theme.Accent;
            btnImportJson.ForeColor = Theme.Background;
            btnImportJson.FlatAppearance.BorderColor = Theme.Accent;

            UpdateUI();
            RefreshOpenStatus();

            // Le statut ouvert/fermé se met à jour tout seul quand l'heure change
            scheduleRefreshTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            scheduleRefreshTimer.Tick += (_, _) => RefreshOpenStatus();
            scheduleRefreshTimer.Start();

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
                    .WithTcpServer("mqtt.blue.section-inf.ch", 1883)
                    .WithCredentials("ict","321")
                    .Build();

                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    string topic = e.ApplicationMessage.Topic;

                    if (topic == "umddcmid3b/time")
                    {
                        string payload = e.ApplicationMessage.ConvertPayloadToString();

                        if (TryGetUnixTime(payload, out long unixTime))
                        {
                            ShowTime(unixTime);
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

        // Construit une ligne par jour de la semaine : jour ouvert + heure d'ouverture + heure de fermeture
        private void InitializeScheduleRows()
        {
            DayOfWeek[] days =
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
            };

            int y = 95;

            foreach (DayOfWeek day in days)
            {
                DaySchedule schedule = weeklySchedule.GetDay(day) ?? new DaySchedule { Day = day };

                var lblDay = new Label
                {
                    Location = new System.Drawing.Point(15, y + 3),
                    Size = new System.Drawing.Size(45, 20),
                    Text = WeeklySchedule.GetShortDayName(day)
                };

                var chkIsOpen = new CheckBox
                {
                    Location = new System.Drawing.Point(62, y),
                    Size = new System.Drawing.Size(56, 23),
                    Text = "Ouvert",
                    Checked = schedule.IsOpen
                };

                var dtOpen = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Location = new System.Drawing.Point(121, y),
                    Size = new System.Drawing.Size(68, 23),
                    Value = DateTime.Today.Add(schedule.OpenTime)
                };

                var dtClose = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Location = new System.Drawing.Point(192, y),
                    Size = new System.Drawing.Size(68, 23),
                    Value = DateTime.Today.Add(schedule.CloseTime)
                };

                chkIsOpen.CheckedChanged += (_, _) =>
                {
                    schedule.IsOpen = chkIsOpen.Checked;
                    RefreshOpenStatus();
                };

                dtOpen.ValueChanged += (_, _) =>
                {
                    schedule.OpenTime = dtOpen.Value.TimeOfDay;
                    RefreshOpenStatus();
                };

                dtClose.ValueChanged += (_, _) =>
                {
                    schedule.CloseTime = dtClose.Value.TimeOfDay;
                    RefreshOpenStatus();
                };

                grpSchedule.Controls.Add(lblDay);
                grpSchedule.Controls.Add(chkIsOpen);
                grpSchedule.Controls.Add(dtOpen);
                grpSchedule.Controls.Add(dtClose);

                scheduleRows[day] = (chkIsOpen, dtOpen, dtClose);
                y += 29;
            }
        }

        private void RefreshOpenStatus()
        {
            bool isOpen = weeklySchedule.IsOpenAt(DateTime.Now, out string stateText, out string detailsText);

            lblOpenStatus.Text = stateText;
            lblOpenStatus.BackColor = Theme.Input;
            lblOpenStatus.ForeColor = isOpen ? Theme.Open : Theme.Closed;
            lblOpenDetails.Text = detailsText;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            scheduleRefreshTimer?.Stop();
            scheduleRefreshTimer?.Dispose();
            base.OnFormClosed(e);
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

        private void EnsureSpecialInProducts()
        {
            if (currentBusiness.Special != null && !currentBusiness.Products.Contains(currentBusiness.Special))
            {
                currentBusiness.Products.Add(currentBusiness.Special);
            }
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

            EnsureSpecialInProducts();
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

        private void lstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProducts.SelectedIndex < 0 || lstProducts.SelectedIndex >= currentBusiness.Products.Count)
                return;

            DisplayProduct(currentBusiness.Products[lstProducts.SelectedIndex]);
        }

        private void DisplayProduct(Product product)
        {
            if (product == null) return;

            txtSpecialId.Text = product.Id;
            txtSpecialName.Text = product.Name;
            txtSpecialPicture.Text = product.Picture;

            numSpecialPrice.Value = Math.Min(Math.Max(product.Price, numSpecialPrice.Minimum), numSpecialPrice.Maximum);

            picSpecialPicture.Image = string.IsNullOrEmpty(product.Picture) ? null : Base64ToImage(product.Picture);
        }

        // Extrait l'heure Unix (en secondes) d'un message de temps.
        // Seul le contrat doc/contracts est accepté : enveloppe { v, id, from, to, ts, type, payload }
        // dont le payload time vaut { "timestamp": 1794218169 }.
        // L'ancien modèle du README { "UnixTimeStamp": ... }, diffusé par d'autres nœuds sur le
        // même topic, n'a pas d'enveloppe : il est ignoré.
        private static bool TryGetUnixTime(string json, out long unixTime)
        {
            unixTime = 0;

            try
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement;
                    if (root.ValueKind != JsonValueKind.Object)
                        return false;

                    // Message sans enveloppe (ancien format) : on ne l'affiche pas
                    if (!TryGetProperty(root, "payload", out JsonElement payloadElement) ||
                        payloadElement.ValueKind != JsonValueKind.Object)
                    {
                        System.Diagnostics.Debug.WriteLine($"Message de temps ignoré (sans enveloppe) : {json}");
                        return false;
                    }

                    return TryGetEnvelopeUnixTime(json, root, payloadElement, out unixTime);
                }
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur MQTT JSON : {ex.Message}");
                return false;
            }
        }

        // Valide l'enveloppe et son payload contre les schémas de doc/contracts
        // (envelope.schema.json et time.schema.json).
        private static bool TryGetEnvelopeUnixTime(string json, JsonElement root, JsonElement payloadElement, out long unixTime)
        {
            unixTime = 0;

            if (!EnvelopeValidator.ValidateEnvelope(json, out string envelopeError))
            {
                System.Diagnostics.Debug.WriteLine($"Enveloppe MQTT invalide : {envelopeError}");
                return false;
            }

            string? type = TryGetProperty(root, "type", out JsonElement typeElement) ? typeElement.GetString() : null;
            if (!string.Equals(type, "time", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!TimeEnvelopeValidator.ValidateTimeEnvelope(payloadElement.GetRawText(), out string payloadError))
            {
                System.Diagnostics.Debug.WriteLine($"Payload time invalide : {payloadError}");
                return false;
            }

            return TryGetProperty(payloadElement, "timestamp", out JsonElement timestampElement) &&
                   TryReadUnixSeconds(timestampElement, out unixTime);
        }

        private static bool TryReadUnixSeconds(JsonElement element, out long unixTime)
        {
            unixTime = 0;

            if (element.ValueKind == JsonValueKind.Number)
            {
                if (!element.TryGetInt64(out unixTime))
                    return false;
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                if (!long.TryParse(element.GetString(), out unixTime))
                    return false;
            }
            else
            {
                return false;
            }

            // Bornes supportées par DateTimeOffset (jusqu'au 31.12.9999)
            return unixTime >= 0 && unixTime <= 253402300799;
        }

        private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private void ShowTime(long unixTime)
        {
            DateTime dateTime;
            try
            {
                dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Heure Unix invalide ({unixTime}) : {ex.Message}");
                return;
            }

            void UpdateLabel()
            {
                lblDisplayTime.Text = $"Heure : {dateTime:G}";
                lblDisplayTime.Refresh();
            }

            if (IsDisposed)
                return;

            // Les messages MQTT arrivent sur un thread de fond : on repasse sur le thread de l'UI
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)UpdateLabel);
            else
                UpdateLabel();
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            // Logique d'export du commerce existante
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
                    // --- IMPORTATION DU COMMERCE ---
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
    }
}