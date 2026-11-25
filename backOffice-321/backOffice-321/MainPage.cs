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

        // Heure reçue par MQTT : sert de référence pour le statut d'ouverture, comme pour l'affichage
        private DateTime? mqttTime;

        // Dossier des profils JSON : à côté du dossier du projet (C:\...\GitHub\backoffice-321\Json)
        private const string JsonFolderPath = @"C:\Users\py13koy\Documents\GitHub\backoffice-321\Json";

        // Les fichiers JSON utilisent des noms en camelCase : la lecture doit ignorer la casse
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        // Les heures réglées dans l'application sont enregistrées dans le profil utilisateur
        // et rechargées au démarrage suivant.
        private static readonly string ScheduleFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "backOffice-321",
            "opening-hours.json");

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
        private System.Windows.Forms.Timer scheduleRefreshTimer;

        // Nœud auquel cette application s'identifie (topics umddcmid3b/...)
        private const string NodeName = "umddcmid3b";
        private const string TimeTopic = NodeName + "/time";
        //private const string TimeTopic = NodeName + "/timebackup";
        private const string LogTopic = NodeName + "/log";

        // Dernier état connu : sert à ne publier un log que lorsque l'état change
        private bool? lastOpenState;
       
        public MainPage()
        {
            InitializeComponent();
            LoadBusinessProfilesFromFolder();
            LoadSchedule();
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

                    if (topic == TimeTopic)
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
                    .WithTopicFilter(TimeTopic)
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

                var dtOpen = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Location = new System.Drawing.Point(64, y),
                    Size = new System.Drawing.Size(80, 23),
                    Value = DateTime.Today.Add(schedule.OpenTime)
                };

                var dtClose = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Location = new System.Drawing.Point(152, y),
                    Size = new System.Drawing.Size(80, 23),
                    Value = DateTime.Today.Add(schedule.CloseTime)
                };

                dtOpen.ValueChanged += (_, _) =>
                {
                    schedule.OpenTime = dtOpen.Value.TimeOfDay;
                    SaveSchedule();
                    RefreshOpenStatus();
                };

                dtClose.ValueChanged += (_, _) =>
                {
                    schedule.CloseTime = dtClose.Value.TimeOfDay;
                    SaveSchedule();
                    RefreshOpenStatus();
                };

                grpSchedule.Controls.Add(lblDay);
                grpSchedule.Controls.Add(dtOpen);
                grpSchedule.Controls.Add(dtClose);
                y += 29;
            }
        }

        // Relit les horaires enregistrés lors d'une session précédente
        private void LoadSchedule()
        {
            if (!File.Exists(ScheduleFilePath))
                return;

            try
            {
                WeeklySchedule? saved = JsonSerializer.Deserialize<WeeklySchedule>(File.ReadAllText(ScheduleFilePath), JsonOptions);

                if (saved?.Days == null)
                    return;

                foreach (DaySchedule savedDay in saved.Days)
                {
                    DaySchedule? target = weeklySchedule.GetDay(savedDay.Day);

                    if (target == null)
                        continue;

                    target.IsOpen = true;
                    target.OpenTime = savedDay.OpenTime;
                    target.CloseTime = savedDay.CloseTime;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Horaires enregistrés ignorés : {ex.Message}");
            }
        }

        // Enregistre les horaires dès qu'une heure est modifiée
        private void SaveSchedule()
        {
            try
            {
                string? folder = Path.GetDirectoryName(ScheduleFilePath);

                if (!string.IsNullOrEmpty(folder))
                    Directory.CreateDirectory(folder);

                File.WriteAllText(ScheduleFilePath, JsonSerializer.Serialize(weeklySchedule, JsonOptions));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Horaires non enregistrés : {ex.Message}");
            }
        }

        private void RefreshOpenStatus()
        {
            // On se cale sur l'heure reçue par MQTT ; tant qu'aucune heure n'est arrivée,
            // on utilise l'heure locale de la machine.
            DateTime moment = mqttTime ?? DateTime.Now;

            bool isOpen = weeklySchedule.IsOpenAt(moment, out string stateText, out string detailsText);

            lblOpenStatus.Text = stateText;
            lblOpenStatus.BackColor = Theme.Input;
            lblOpenStatus.ForeColor = isOpen ? Theme.Open : Theme.Closed;
            lblOpenDetails.Text = detailsText;

            // Un log n'est envoyé que si l'état était déjà connu et qu'il vient de changer
            bool stateChanged = lastOpenState.HasValue && lastOpenState.Value != isOpen;
            lastOpenState = isOpen;

            if (stateChanged)
                _ = PublishOpenStateLogAsync(isOpen, moment);
        }

        // Publie le changement d'état ouvert/fermé sur umddcmid3b/log, au nom du commerce
        private async Task PublishOpenStateLogAsync(bool isOpen, DateTime moment)
        {
            if (mqttClient == null || !mqttClient.IsConnected)
                return;

            try
            {
                // Émetteur = ID du commerce (COM-XXXX), comme les autres nœuds du réseau
                string from = string.IsNullOrWhiteSpace(currentBusiness.ID) ? NodeName : currentBusiness.ID.Trim();
                string shopName = string.IsNullOrWhiteSpace(currentBusiness.Name) ? from : currentBusiness.Name.Trim();
                string text = $"{shopName} est maintenant {(isOpen ? "ouvert" : "fermer")} ({moment:dd.MM.yyyy HH:mm})";
                string payload = LogEnvelope.Create(from, "*", "info", text);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(LogTopic)
                    .WithPayload(payload)
                    .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Log d'état non publié : {ex.Message}");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            scheduleRefreshTimer?.Stop();
            scheduleRefreshTimer?.Dispose();
            base.OnFormClosed(e);
        }

        // Au démarrage, le commerce et ses produits viennent des fichiers JSON du dossier Json
        private void LoadBusinessProfilesFromFolder()
        {
            currentBusiness = new BusinessProfile();

            string folder = FindJsonFolder();

            if (!Directory.Exists(folder))
            {
                System.Diagnostics.Debug.WriteLine($"Dossier JSON introuvable : {folder}");
                return;
            }

            string[] files = Directory.GetFiles(folder, "*.json");
            Array.Sort(files);

            // Commerce affiché par défaut à l'ouverture, quel que soit l'ordre alphabétique
            const string defaultProfileId = "COM-0277";
            files = files
                .OrderByDescending(file => string.Equals(Path.GetFileNameWithoutExtension(file), defaultProfileId, StringComparison.OrdinalIgnoreCase))
                .ThenBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (string file in files)
            {
                try
                {
                    BusinessProfile? profile = JsonSerializer.Deserialize<BusinessProfile>(File.ReadAllText(file), JsonOptions);

                    if (profile != null)
                        MergeBusinessProfile(profile);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Profil JSON ignoré ({Path.GetFileName(file)}) : {ex.Message}");
                }
            }
        }

        // Cherche le dossier Json en remontant depuis le dossier de l'exécutable (bin\Debug\net8.0-windows\...)
        private static string FindJsonFolder()
        {
            DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

            for (int i = 0; i < 6 && directory != null; i++, directory = directory.Parent)
            {
                string candidate = Path.Combine(directory.FullName, "Json");

                if (Directory.Exists(candidate))
                    return candidate;
            }

            return JsonFolderPath;
        }

        // Le premier profil rencontré fournit l'identité du commerce ; les produits de tous les fichiers sont fusionnés
        private void MergeBusinessProfile(BusinessProfile profile)
        {
            if (string.IsNullOrWhiteSpace(currentBusiness.ID))
            {
                currentBusiness.ID = profile.ID ?? string.Empty;
                currentBusiness.Name = profile.Name ?? string.Empty;
                currentBusiness.Logo = profile.Logo ?? string.Empty;
            }

            if (profile.Special != null && !string.IsNullOrWhiteSpace(profile.Special.Id))
            {
                if (string.IsNullOrWhiteSpace(currentBusiness.Special?.Id))
                    currentBusiness.Special = profile.Special;

                AddProductIfMissing(profile.Special);
            }

            if (profile.Products != null)
            {
                foreach (Product product in profile.Products)
                {
                    if (product != null)
                        AddProductIfMissing(product);
                }
            }
        }

        // Indique si un produit ayant cet ID figure déjà dans la liste (sans tenir compte de la casse)
        private bool ProductExists(string id)
        {
            if (currentBusiness.Products == null || string.IsNullOrWhiteSpace(id))
                return false;

            string wanted = id.Trim();

            return currentBusiness.Products.Exists(existing =>
                string.Equals((existing?.Id ?? string.Empty).Trim(), wanted, StringComparison.OrdinalIgnoreCase));
        }

        private void AddProductIfMissing(Product product)
        {
            if (currentBusiness.Products == null)
                currentBusiness.Products = new List<Product>();

            if (product != null && !ProductExists(product.Id))
                currentBusiness.Products.Add(product);
        }

        private void EnsureSpecialInProducts()
        {
            if (currentBusiness.Special != null && !ProductExists(currentBusiness.Special.Id))
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
                // Les JSON peuvent contenir des placeholders non base64 : on n'affiche pas d'image
                System.Diagnostics.Debug.WriteLine($"Image ignorée : {ex.Message}");
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
                mqttTime = dateTime;
                lblDisplayTime.Text = $"Heure : {dateTime:G}";
                lblDisplayTime.Refresh();
                RefreshOpenStatus();
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
                    var loadedBusiness = JsonSerializer.Deserialize<BusinessProfile>(jsonString, JsonOptions);

                    if (loadedBusiness != null)
                    {
                        currentBusiness.ID = loadedBusiness.ID;
                        currentBusiness.Name = loadedBusiness.Name;
                        currentBusiness.Logo = loadedBusiness.Logo;

                        if (loadedBusiness.Special != null && !string.IsNullOrEmpty(loadedBusiness.Special.Id))
                        {
                            currentBusiness.Special = loadedBusiness.Special;

                            // Si l'ID est déjà dans la liste, le produit importé est ignoré
                            if (!ProductExists(loadedBusiness.Special.Id))
                            {
                                currentBusiness.Products.Add(loadedBusiness.Special);
                            }
                        }

                        if (loadedBusiness.Products != null && loadedBusiness.Products.Count > 0)
                        {
                            foreach (var p in loadedBusiness.Products)
                            {
                                if (p == null || ProductExists(p.Id))
                                    continue;

                                currentBusiness.Products.Add(p);
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