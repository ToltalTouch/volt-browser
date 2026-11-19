using System.Drawing.Drawing2D;

namespace Volt;

sealed partial class BrowserWindow
{
    static GraphicsPath CreateRoundedRectPath(RectangleF r, float radius)
    {
        var path = new GraphicsPath();
        float d = radius * 2;
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    sealed class ModernPillButton : Button
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsActive { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Dark { get; set; }

        public ModernPillButton(string text)
        {
            Text = text;
            AutoSize = true;
            Height = 28;
            Padding = new Padding(8, 2, 8, 2);
            Margin = new Padding(2, 3, 2, 3);
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            DoubleBuffered = true;
            UseMnemonic = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg = IsActive
                ? (Dark ? Color.FromArgb(14, 165, 233) : Color.FromArgb(2, 132, 199))
                : (Dark ? Color.FromArgb(45, 56, 73) : Color.FromArgb(235, 240, 246));

            Color textCol = IsActive
                ? Color.White
                : (Dark ? Color.FromArgb(215, 226, 238) : Color.FromArgb(51, 65, 85));

            using var path = CreateRoundedRectPath(new RectangleF(0, 0, Width - 1, Height - 1), 5);
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);

            if (!IsActive)
            {
                using var border = new Pen(Dark ? Color.FromArgb(60, 75, 96) : Color.FromArgb(210, 220, 232), 1);
                g.DrawPath(border, path);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    sealed class ModernActionBtn : Button
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Dark { get; set; }

        public ModernActionBtn(string text)
        {
            Text = text;
            Height = 32;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            DoubleBuffered = true;
            Margin = new Padding(0, 3, 0, 3);
            UseMnemonic = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg = Dark ? Color.FromArgb(45, 56, 73) : Color.FromArgb(235, 240, 246);
            Color textCol = Dark ? Color.FromArgb(220, 230, 242) : Color.FromArgb(40, 55, 75);

            using var path = CreateRoundedRectPath(new RectangleF(0, 0, Width - 1, Height - 1), 5);
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);

            using var border = new Pen(Dark ? Color.FromArgb(60, 75, 96) : Color.FromArgb(210, 220, 232), 1);
            g.DrawPath(border, path);

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    sealed class ModernMenuTabBtn : Button
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsSelected { get; set; }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Dark { get; set; }

        public ModernMenuTabBtn(string text)
        {
            Text = text;
            Height = 32;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 8.75f, FontStyle.Bold);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            DoubleBuffered = true;
            Margin = new Padding(2, 0, 2, 0);
            UseMnemonic = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg = IsSelected
                ? (Dark ? Color.FromArgb(14, 165, 233) : Color.FromArgb(2, 132, 199))
                : (Dark ? Color.FromArgb(34, 43, 56) : Color.FromArgb(241, 245, 249));

            Color textCol = IsSelected
                ? Color.White
                : (Dark ? Color.FromArgb(165, 180, 200) : Color.FromArgb(71, 85, 105));

            using var path = CreateRoundedRectPath(new RectangleF(0, 0, Width - 1, Height - 1), 6);
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);

            if (!IsSelected)
            {
                using var border = new Pen(Dark ? Color.FromArgb(50, 63, 82) : Color.FromArgb(220, 228, 238), 1);
                g.DrawPath(border, path);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    sealed class ModernCardPanel : FlowLayoutPanel
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Dark { get; set; }

        public ModernCardPanel()
        {
            DoubleBuffered = true;
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(12, 10, 12, 10);
            Margin = new Padding(0, 0, 0, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color bg = Dark ? Color.FromArgb(36, 45, 59) : Color.White;
            Color border = Dark ? Color.FromArgb(52, 65, 85) : Color.FromArgb(226, 232, 240);

            using var path = CreateRoundedRectPath(new RectangleF(0, 0, Width - 1, Height - 1), 7);
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);

            using var pen = new Pen(border, 1);
            g.DrawPath(pen, path);
        }
    }

    sealed class ModernToolsDropDown : ToolStripDropDown
    {
        public ModernToolsDropDown()
        {
            Padding = Padding.Empty;
            Margin = Padding.Empty;
            DropShadowEnabled = true;
            AutoClose = true;
            DoubleBuffered = true;
        }
    }

    sealed class ModernMenuScrollContainer : Panel
    {
        public ModernMenuScrollContainer()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }

    ModernToolsDropDown? modernToolsPopup;
    Panel? modernMenuPanel;
    readonly List<(ModernPillButton Btn, int Minutes)> modernTimePills = new();
    readonly List<(ModernPillButton Btn, int Count)> modernPriorityPills = new();
    ModernPillButton? modernCustomPriorityPill;

    CheckBox? modernEconomyCheck;
    CheckBox? modernPinCheck;
    CheckBox? modernMediaCheck;
    CheckBox? modernDeepCheck;
    CheckBox? modernAutoMemCheck;
    CheckBox? modernBlockersCheck;
    CheckBox? modernDarkCheck;
    CheckBox? modernBookmarksCheck;
    CheckBox? modernRestoreTabsCheck;

    readonly List<ModernMenuTabBtn> modernTabButtons = new();
    readonly List<FlowLayoutPanel> modernTabPages = new();

    void InitializeModernToolsMenu(Button menuButton)
    {
        modernToolsPopup = new ModernToolsDropDown();
        int panelWidth = (int)(425 * DeviceDpi / 96f);
        int panelHeight = (int)(580 * DeviceDpi / 96f);
        int cardWidth = panelWidth - (int)(38 * DeviceDpi / 96f);
        int contentWidth = cardWidth - (int)(24 * DeviceDpi / 96f);

        var rootContainer = new ModernMenuScrollContainer
        {
            Width = panelWidth,
            Height = panelHeight,
            Padding = new Padding(10, 10, 10, 10)
        };
        modernMenuPanel = rootContainer;

        // --- HEADER ---
        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            Margin = Padding.Empty
        };
        var titleLbl = new Label
        {
            Text = "VOLT • Configurações & Ferramentas",
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 22,
            UseMnemonic = false
        };
        var subTitleLbl = new Label
        {
            Text = "Controle de abas, desempenho, aparência e segurança",
            Font = new Font("Segoe UI", 8.25f),
            Dock = DockStyle.Top,
            Height = 18,
            UseMnemonic = false
        };
        headerPanel.Controls.Add(subTitleLbl);
        headerPanel.Controls.Add(titleLbl);

        // --- TABS BAR ---
        var tabsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 36,
            RowCount = 1,
            ColumnCount = 4,
            Margin = new Padding(0, 4, 0, 8),
            Padding = Padding.Empty
        };
        tabsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tabsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tabsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        tabsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        var tabEco = new ModernMenuTabBtn("⚡ Economia") { Dock = DockStyle.Fill, IsSelected = true };
        var tabVisual = new ModernMenuTabBtn("🎨 Visual") { Dock = DockStyle.Fill };
        var tabSec = new ModernMenuTabBtn("🔒 Segurança") { Dock = DockStyle.Fill };
        var tabSys = new ModernMenuTabBtn("ℹ️ Sistema") { Dock = DockStyle.Fill };

        modernTabButtons.Clear();
        modernTabButtons.Add(tabEco);
        modernTabButtons.Add(tabVisual);
        modernTabButtons.Add(tabSec);
        modernTabButtons.Add(tabSys);

        tabsPanel.Controls.Add(tabEco, 0, 0);
        tabsPanel.Controls.Add(tabVisual, 1, 0);
        tabsPanel.Controls.Add(tabSec, 2, 0);
        tabsPanel.Controls.Add(tabSys, 3, 0);

        // --- CONTENT CONTAINER ---
        var pagesContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        // ================= PAGE 1: ECONOMIA =================
        var pageEco = CreateTabPage();

        // Card 1: Suspensão de abas
        var ecoCard1 = CreateMenuCard("SUSPENSÃO AUTOMÁTICA DE ABAS", cardWidth, contentWidth);

        modernEconomyCheck = CreateModernCheckBox("Ativar modo de economia (suspender abas)", economy);
        modernEconomyCheck.CheckedChanged += (_, _) =>
        {
            economy = modernEconomyCheck.Checked;
            if (!economy)
            {
                foreach (var tab in items.Values)
                    if (tab.Ready) tab.View!.CoreWebView2.Resume();
            }
            UpdateChrome();
        };
        ecoCard1.Controls.Add(modernEconomyCheck);

        var timeLabel = CreateSubtitleLabel("Tempo de inatividade antes de suspender:");
        ecoCard1.Controls.Add(timeLabel);

        var timeFlow = new FlowLayoutPanel
        {
            Width = contentWidth,
            AutoSize = true,
            WrapContents = true,
            Margin = new Padding(0, 2, 0, 6)
        };
        modernTimePills.Clear();
        foreach (var (mins, label) in new[]
        {
            (1, "1 min"),
            (3, "3 min"),
            (5, "5 min (Padrão)"),
            (15, "15 min"),
            (30, "30 min"),
            (60, "1 hora"),
            (0, "Nunca")
        })
        {
            var pill = new ModernPillButton(label) { IsActive = mins == sleepMinutes };
            int m = mins;
            pill.Click += (_, _) =>
            {
                SetSleepMinutes(m);
                UpdateModernMenuValues();
            };
            timeFlow.Controls.Add(pill);
            modernTimePills.Add((pill, mins));
        }
        ecoCard1.Controls.Add(timeFlow);

        var priorityLabel = CreateSubtitleLabel("Prioridade de abas (manter sempre ativas):");
        ecoCard1.Controls.Add(priorityLabel);

        var priorityFlow = new FlowLayoutPanel
        {
            Width = contentWidth,
            AutoSize = true,
            WrapContents = true,
            Margin = new Padding(0, 2, 0, 6)
        };
        modernPriorityPills.Clear();
        foreach (var (count, label) in new[]
        {
            (0, "0 (Nenhuma)"),
            (2, "2 abas"),
            (3, "3 abas (Recomendado)"),
            (5, "5 abas"),
            (10, "10 abas")
        })
        {
            var pill = new ModernPillButton(label) { IsActive = count == browsing.PriorityTabs };
            int c = count;
            pill.Click += (_, _) =>
            {
                SetPriorityTabs(c);
                UpdateModernMenuValues();
            };
            priorityFlow.Controls.Add(pill);
            modernPriorityPills.Add((pill, count));
        }

        modernCustomPriorityPill = new ModernPillButton("Personalizar…");
        modernCustomPriorityPill.Click += (_, _) =>
        {
            modernToolsPopup?.Hide();
            using var dialog = new Form
            {
                Text = "Prioridade das abas ativas",
                ClientSize = new(440, 160),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = darkMode ? BrowserTheme.Surface : SystemColors.Control,
                ForeColor = darkMode ? BrowserTheme.Text : SystemColors.ControlText
            };
            var label = new Label
            {
                Text = "Manter as primeiras N abas ativas (da esquerda para a direita):\nElas nunca entrarão em modo de economia.",
                AutoSize = false,
                Bounds = new(16, 16, 408, 44),
                ForeColor = dialog.ForeColor,
                UseMnemonic = false
            };
            var countInput = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = browsing.PriorityTabs,
                Bounds = new(16, 68, 90, 30),
                BackColor = darkMode ? BrowserTheme.Background : Color.White,
                ForeColor = dialog.ForeColor
            };
            var ok = new Button
            {
                Text = "Salvar",
                DialogResult = DialogResult.OK,
                Bounds = new(320, 110, 100, 32),
                BackColor = darkMode ? Color.FromArgb(45, 58, 73) : SystemColors.ButtonFace,
                ForeColor = dialog.ForeColor,
                UseMnemonic = false
            };
            dialog.Controls.AddRange(new Control[] { label, countInput, ok });
            dialog.AcceptButton = ok;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SetPriorityTabs((int)countInput.Value);
            }
        };
        priorityFlow.Controls.Add(modernCustomPriorityPill);
        ecoCard1.Controls.Add(priorityFlow);

        modernPinCheck = CreateModernCheckBox("Manter esta aba atual ativa", Current?.KeepAwake == true);
        modernPinCheck.CheckedChanged += (_, _) =>
        {
            if (Current is { } tab) tab.KeepAwake = modernPinCheck.Checked;
        };
        ecoCard1.Controls.Add(modernPinCheck);

        // Card 2: Memória & Recursos
        var ecoCard2 = CreateMenuCard("MEMÓRIA & RECURSOS DO SISTEMA", cardWidth, contentWidth);

        modernMediaCheck = CreateModernCheckBox("Pausar mídia ao sair da aba", pauseBackgroundMedia);
        modernMediaCheck.CheckedChanged += async (_, _) =>
        {
            pauseBackgroundMedia = modernMediaCheck.Checked;
            foreach (var tab in items.Values.ToArray())
                if (tab.Ready && tab.MediaInstalled && tab.View is { } web) await InstallMediaGuard(tab, web);
        };
        ecoCard2.Controls.Add(modernMediaCheck);

        modernDeepCheck = CreateModernCheckBox("Economia forte (descartar páginas após prazo)", deepEconomy);
        modernDeepCheck.CheckedChanged += (_, _) =>
        {
            if (modernDeepCheck.Checked && !deepEconomy)
            {
                if (MessageBox.Show(this,
                    "Ativar economia forte? Abas elegíveis serão descartadas depois do prazo configurado, economizando ainda mais memória.",
                    "Economia forte", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                {
                    modernDeepCheck.Checked = false;
                    return;
                }
            }
            deepEconomy = modernDeepCheck.Checked;
            UpdateChrome();
        };
        ecoCard2.Controls.Add(modernDeepCheck);

        modernAutoMemCheck = CreateModernCheckBox("Liberar memória acima de 1,2 GB", automaticMemory);
        modernAutoMemCheck.CheckedChanged += (_, _) =>
        {
            automaticMemory = modernAutoMemCheck.Checked;
            memoryPressure = false;
        };
        ecoCard2.Controls.Add(modernAutoMemCheck);

        var actionsPanel = new TableLayoutPanel
        {
            Width = contentWidth,
            Height = 36,
            RowCount = 1,
            ColumnCount = 2,
            Margin = new Padding(0, 6, 0, 4)
        };
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

        var btnSuspend = new ModernActionBtn("💤 Suspender inativas") { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 3, 0) };
        btnSuspend.Click += async (_, _) =>
        {
            modernToolsPopup?.Hide();
            await SuspendTabs(true);
        };
        var btnUnload = new ModernActionBtn("🧹 Liberar 2º plano") { Dock = DockStyle.Fill, Margin = new Padding(3, 0, 0, 0) };
        btnUnload.Click += (_, _) =>
        {
            modernToolsPopup?.Hide();
            UnloadBackgroundTabs();
        };
        actionsPanel.Controls.Add(btnSuspend, 0, 0);
        actionsPanel.Controls.Add(btnUnload, 1, 0);
        ecoCard2.Controls.Add(actionsPanel);

        var btnDiag = new ModernActionBtn("📊 Diagnóstico de abas e consumo") { Width = contentWidth };
        btnDiag.Click += (_, _) =>
        {
            modernToolsPopup?.Hide();
            ShowTabDiagnostics();
        };
        ecoCard2.Controls.Add(btnDiag);

        pageEco.Controls.Add(ecoCard1);
        pageEco.Controls.Add(ecoCard2);

        // ================= PAGE 2: VISUAL =================
        var pageVisual = CreateTabPage();
        var visCard1 = CreateMenuCard("TEMA & INTERFACE", cardWidth, contentWidth);

        modernDarkCheck = CreateModernCheckBox("Modo escuro (on/off)", darkMode);
        modernDarkCheck.CheckedChanged += (_, _) =>
        {
            if (darkMode != modernDarkCheck.Checked)
            {
                darkMode = modernDarkCheck.Checked;
                using (var redraw = new ThemeRedraw(this)) BrowserTheme.Apply(this, darkMode);
                addressSuggestions?.ApplyTheme(darkMode);
                ApplyModernMenuTheme(darkMode);
                BeginInvoke(new Action(() =>
                {
                    if (closing) return;
                    var ready = items.Values.FirstOrDefault(t => t.Ready);
                    if (ready is not null) SetWebTheme(ready.View!);
                    else if (whatsAppView?.CoreWebView2 is not null) SetWebTheme(whatsAppView);
                }));
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(themeFile)!);
                    File.WriteAllText(themeFile, darkMode ? "dark" : "light");
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
            }
        };
        visCard1.Controls.Add(modernDarkCheck);

        modernBookmarksCheck = CreateModernCheckBox("Mostrar barra de favoritos (Ctrl+Shift+B)", browsing.ShowBookmarksBar);
        modernBookmarksCheck.CheckedChanged += (_, _) =>
        {
            if (bookmarkBar.Visible != modernBookmarksCheck.Checked)
                ToggleBookmarksBar(modernBookmarksCheck.Checked);
        };
        visCard1.Controls.Add(modernBookmarksCheck);

        var visCard2 = CreateMenuCard("INICIALIZAÇÃO & SESSÃO", cardWidth, contentWidth);

        modernRestoreTabsCheck = CreateModernCheckBox("Restaurar abas abertas ao iniciar", browsing.RestoreTabsOnStartup);
        modernRestoreTabsCheck.CheckedChanged += (_, _) =>
        {
            browsing.RestoreTabsOnStartup = modernRestoreTabsCheck.Checked;
            SaveBrowsingPreferences();
            status.Text = browsing.RestoreTabsOnStartup
                ? "As abas abertas serão salvas e restauradas ao reiniciar."
                : "O VOLT iniciará com uma nova guia.";
        };
        visCard2.Controls.Add(modernRestoreTabsCheck);

        var restoreTip = new Label
        {
            Text = "Salva as abas abertas ao fechar e as reabre automaticamente ao reabrir o VOLT.",
            Font = new Font("Segoe UI", 8.25f),
            ForeColor = Color.Gray,
            Width = contentWidth,
            Height = 32,
            UseMnemonic = false,
            Margin = new Padding(20, 0, 0, 4)
        };
        visCard2.Controls.Add(restoreTip);

        var visCard3 = CreateMenuCard("ATALHOS & PRODUTIVIDADE", cardWidth, contentWidth);
        var shortcutsLabel = new Label
        {
            Text = "• Ctrl+T: Nova aba\n• Ctrl+Shift+B: Alternar barra de favoritos\n• Alt+P: Ativar Picture-in-Picture no vídeo\n• Arrastar aba: Destacar em uma nova janela",
            Font = new Font("Segoe UI", 8.5f),
            Width = contentWidth,
            Height = 72,
            UseMnemonic = false,
            Margin = new Padding(4, 2, 0, 2)
        };
        visCard3.Controls.Add(shortcutsLabel);

        pageVisual.Controls.Add(visCard1);
        pageVisual.Controls.Add(visCard2);
        pageVisual.Controls.Add(visCard3);

        // ================= PAGE 3: SEGURANÇA =================
        var pageSec = CreateTabPage();
        var secCard1 = CreateMenuCard("PRIVACIDADE & RASTREAMENTO", cardWidth, contentWidth);

        modernBlockersCheck = CreateModernCheckBox("Bloquear rastreadores conhecidos", blockTrackers);
        modernBlockersCheck.CheckedChanged += (_, _) =>
        {
            blockTrackers = modernBlockersCheck.Checked;
            UpdateChrome();
        };
        secCard1.Controls.Add(modernBlockersCheck);

        var btnClearHist = new ModernActionBtn("🗑 Limpar histórico de endereços") { Width = contentWidth };
        btnClearHist.Click += (_, _) =>
        {
            browsing.History.Clear();
            SaveBrowsingPreferences();
            RefreshAddressSuggestions();
            status.Text = "Histórico de endereços limpo com sucesso.";
            modernToolsPopup?.Hide();
        };
        secCard1.Controls.Add(btnClearHist);

        var secCard2 = CreateMenuCard("CREDENCIAIS & SENHAS", cardWidth, contentWidth);
        var btnPass = new ModernActionBtn("🔑 Senhas — importar e gerenciar…") { Width = contentWidth };
        btnPass.Click += (_, _) =>
        {
            modernToolsPopup?.Hide();
            ShowPasswords();
        };
        secCard2.Controls.Add(btnPass);

        var secCard3 = CreateMenuCard("IMPORTAÇÃO DE DADOS", cardWidth, contentWidth);
        var btnImport = new ModernActionBtn("📥 Importar de outros navegadores…") { Width = contentWidth };
        btnImport.Click += (_, _) =>
        {
            modernToolsPopup?.Hide();
            ShowBrowserImportDialog();
        };
        secCard3.Controls.Add(btnImport);

        var importTip = new Label
        {
            Text = "Transfira favoritos do Chrome, Edge, Brave, Opera ou arquivo HTML.",
            Font = new Font("Segoe UI", 8.25f),
            ForeColor = Color.Gray,
            Width = contentWidth,
            Height = 20,
            UseMnemonic = false,
            Margin = new Padding(2, 0, 0, 2)
        };
        secCard3.Controls.Add(importTip);

        pageSec.Controls.Add(secCard1);
        pageSec.Controls.Add(secCard2);
        pageSec.Controls.Add(secCard3);

        // ================= PAGE 4: SISTEMA =================
        var pageSys = CreateTabPage();
        var sysCard1 = CreateMenuCard("ATUALIZAÇÕES DO VOLT", cardWidth, contentWidth);

        var updateStatusLbl = new Label
        {
            Text = $"Versão atual: v{UpdateManager.CurrentVersionString}",
            Font = new Font("Segoe UI", 8.5f),
            Width = contentWidth,
            Height = 22,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 4)
        };
        sysCard1.Controls.Add(updateStatusLbl);

        var btnUpdate = new ModernActionBtn("🔍 Verificar atualizações agora") { Width = contentWidth };
        UpdateInfo? pendingUpdate = null;

        btnUpdate.Click += async (_, _) =>
        {
            if (pendingUpdate != null && pendingUpdate.HasUpdate)
            {
                btnUpdate.Enabled = false;
                btnUpdate.Text = "Baixando atualização... 0%";
                updateStatusLbl.Text = $"Baixando v{pendingUpdate.LatestVersion}...";
                try
                {
                    var progress = new Progress<int>(percent =>
                    {
                        btnUpdate.Text = $"Baixando atualização... {percent}%";
                    });

                    string downloaded = await UpdateManager.DownloadUpdateAsync(pendingUpdate, progress);
                    btnUpdate.Text = "Atualização pronta!";
                    updateStatusLbl.Text = "Pronto para aplicar a nova versão.";

                    if (MessageBox.Show(this,
                        $"A atualização para a versão v{pendingUpdate.LatestVersion} foi baixada com sucesso!\n\nDeseja reiniciar o VOLT agora para aplicar a atualização?",
                        "Atualização do VOLT",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UpdateManager.ApplyUpdateAndRestart(downloaded);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Falha ao baixar atualização: " + ex.Message, "Erro de Atualização");
                    btnUpdate.Text = "Tentar novamente";
                    btnUpdate.Enabled = true;
                }
                return;
            }

            btnUpdate.Enabled = false;
            btnUpdate.Text = "Verificando releases no GitHub...";
            updateStatusLbl.Text = "Consultando repositório...";

            try
            {
                var info = await UpdateManager.CheckForUpdatesAsync();
                if (info.HasUpdate)
                {
                    pendingUpdate = info;
                    updateStatusLbl.Text = $"🎉 Nova versão disponível: v{info.LatestVersion}!";
                    btnUpdate.Text = $"⬇ Baixar e Atualizar para v{info.LatestVersion}";
                    btnUpdate.Enabled = true;
                }
                else
                {
                    pendingUpdate = null;
                    updateStatusLbl.Text = $"✓ O VOLT está atualizado (v{info.CurrentVersion}).";
                    btnUpdate.Text = "Verificar novamente";
                    btnUpdate.Enabled = true;
                }
            }
            catch
            {
                updateStatusLbl.Text = "Não foi possível checar atualizações.";
                btnUpdate.Text = "Tentar novamente";
                btnUpdate.Enabled = true;
            }
        };
        sysCard1.Controls.Add(btnUpdate);

        var sysCard2 = CreateMenuCard("SOBRE O NAVEGADOR & DESEMPENHO", cardWidth, contentWidth);
        var infoDesc = new Label
        {
            Text = "• Modo de economia inteligente: descarrega memória de abas ociosas sem perder o estado.\n• Zoom persistente: escala personalizada mantida por site.\n• PiP flutuante (Alt+P): assista vídeos enquanto navega em outras guias.\n• Proteção contra suspensão: abas com som ativo ou downloads não são congeladas.",
            Font = new Font("Segoe UI", 8.25f),
            Width = contentWidth,
            Height = 84,
            UseMnemonic = false,
            Margin = new Padding(2, 2, 0, 4)
        };
        sysCard2.Controls.Add(infoDesc);

        pageSys.Controls.Add(sysCard1);
        pageSys.Controls.Add(sysCard2);

        // Register Pages
        modernTabPages.Clear();
        modernTabPages.Add(pageEco);
        modernTabPages.Add(pageVisual);
        modernTabPages.Add(pageSec);
        modernTabPages.Add(pageSys);

        foreach (var page in modernTabPages)
        {
            pagesContainer.Controls.Add(page);
        }

        void SelectTab(int index)
        {
            for (int i = 0; i < modernTabButtons.Count; i++)
            {
                modernTabButtons[i].IsSelected = i == index;
                modernTabButtons[i].Invalidate();
            }
            for (int i = 0; i < modernTabPages.Count; i++)
            {
                modernTabPages[i].Visible = i == index;
            }
        }

        tabEco.Click += (_, _) => SelectTab(0);
        tabVisual.Click += (_, _) => SelectTab(1);
        tabSec.Click += (_, _) => SelectTab(2);
        tabSys.Click += (_, _) => SelectTab(3);

        SelectTab(0);

        // Put together root container: pagesContainer (fill), tabsPanel (top), headerPanel (top)
        rootContainer.Controls.Add(pagesContainer);
        rootContainer.Controls.Add(tabsPanel);
        rootContainer.Controls.Add(headerPanel);

        var host = new ToolStripControlHost(rootContainer)
        {
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoSize = false,
            Size = rootContainer.Size
        };
        modernToolsPopup.Items.Add(host);

        menuButton.Click += (_, _) => ShowModernToolsMenu(menuButton);
        ApplyModernMenuTheme(darkMode);
    }

    static FlowLayoutPanel CreateTabPage()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(2, 4, 2, 8),
            Margin = Padding.Empty
        };
    }

    static ModernCardPanel CreateMenuCard(string headerText, int cardWidth, int contentWidth)
    {
        var card = new ModernCardPanel
        {
            Width = cardWidth
        };

        var titleLbl = new Label
        {
            Text = headerText,
            Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
            Width = contentWidth,
            Height = 20,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 4)
        };

        card.Controls.Add(titleLbl);
        return card;
    }

    static Label CreateSubtitleLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 4, 0, 2)
        };
    }

    static CheckBox CreateModernCheckBox(string text, bool isChecked)
    {
        return new CheckBox
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            Height = 26,
            Font = new Font("Segoe UI", 9f),
            Cursor = Cursors.Hand,
            UseMnemonic = false,
            Margin = new Padding(0, 2, 0, 4)
        };
    }

    void UpdateModernMenuValues()
    {
        if (modernPinCheck != null) modernPinCheck.Checked = Current?.KeepAwake == true;
        if (modernEconomyCheck != null) modernEconomyCheck.Checked = economy;
        if (modernMediaCheck != null) modernMediaCheck.Checked = pauseBackgroundMedia;
        if (modernDeepCheck != null) modernDeepCheck.Checked = deepEconomy;
        if (modernAutoMemCheck != null) modernAutoMemCheck.Checked = automaticMemory;
        if (modernBlockersCheck != null) modernBlockersCheck.Checked = blockTrackers;
        if (modernDarkCheck != null) modernDarkCheck.Checked = darkMode;
        if (modernBookmarksCheck != null) modernBookmarksCheck.Checked = browsing.ShowBookmarksBar;
        if (modernRestoreTabsCheck != null) modernRestoreTabsCheck.Checked = browsing.RestoreTabsOnStartup;

        foreach (var (btn, mins) in modernTimePills)
        {
            btn.IsActive = browsing.SleepMinutes == mins;
            btn.Invalidate();
        }

        bool matchedPriority = false;
        foreach (var (btn, count) in modernPriorityPills)
        {
            btn.IsActive = browsing.PriorityTabs == count;
            if (btn.IsActive) matchedPriority = true;
            btn.Invalidate();
        }

        if (modernCustomPriorityPill != null)
        {
            modernCustomPriorityPill.IsActive = !matchedPriority && browsing.PriorityTabs > 0;
            modernCustomPriorityPill.Text = browsing.PriorityTabs > 0 && !matchedPriority
                ? $"Personalizar ({browsing.PriorityTabs})"
                : "Personalizar…";
            modernCustomPriorityPill.Invalidate();
        }
    }

    void ApplyModernMenuTheme(bool isDark)
    {
        if (modernMenuPanel == null) return;

        Color bg = isDark ? Color.FromArgb(24, 30, 40) : Color.FromArgb(246, 248, 251);
        Color textPrimary = isDark ? Color.FromArgb(240, 244, 250) : Color.FromArgb(30, 41, 59);
        Color headerColor = isDark ? Color.FromArgb(14, 165, 233) : Color.FromArgb(2, 132, 199);
        Color textSec = isDark ? Color.FromArgb(150, 165, 185) : Color.FromArgb(100, 116, 139);

        modernMenuPanel.BackColor = bg;
        modernMenuPanel.ForeColor = textPrimary;

        foreach (var tabBtn in modernTabButtons)
        {
            tabBtn.Dark = isDark;
            tabBtn.Invalidate();
        }

        void ApplyToHierarchy(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is ModernCardPanel card)
                {
                    card.Dark = isDark;
                    card.Invalidate();
                }
                else if (c is ModernPillButton pill)
                {
                    pill.Dark = isDark;
                    pill.Invalidate();
                }
                else if (c is ModernActionBtn act)
                {
                    act.Dark = isDark;
                    act.Invalidate();
                }
                else if (c is CheckBox cb)
                {
                    cb.ForeColor = textPrimary;
                }
                else if (c is Label lbl)
                {
                    if (lbl.Font.Bold)
                        lbl.ForeColor = headerColor;
                    else
                        lbl.ForeColor = textSec;
                }

                if (c.HasChildren)
                {
                    ApplyToHierarchy(c);
                }
            }
        }

        ApplyToHierarchy(modernMenuPanel);
        modernMenuPanel.Invalidate(true);
    }

    void ShowModernToolsMenu(Button menuButton)
    {
        if (modernToolsPopup == null || modernMenuPanel == null) return;

        UpdateModernMenuValues();
        ApplyModernMenuTheme(darkMode);

        var buttonScreen = menuButton.PointToScreen(Point.Empty);
        var windowRect = RectangleToScreen(ClientRectangle);
        var menuSize = modernMenuPanel.Size;

        // Position menu inside window: align right edge of menu to right edge of button
        int targetX = buttonScreen.X + menuButton.Width - menuSize.Width;
        int targetY = buttonScreen.Y + menuButton.Height + 2;

        // Strictly clamp entirely within window bounds so no part ever spills outside
        targetX = Math.Clamp(targetX, windowRect.Left + 6, Math.Max(windowRect.Left + 6, windowRect.Right - menuSize.Width - 6));
        if (targetY + menuSize.Height > windowRect.Bottom - 6)
        {
            targetY = Math.Max(windowRect.Top + 6, buttonScreen.Y - menuSize.Height - 2);
        }

        modernToolsPopup.Show(targetX, targetY);
    }
}
