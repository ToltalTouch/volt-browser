namespace Volt;

sealed partial class BrowserWindow
{
    void ShowBrowserImportDialog()
    {
        modernToolsPopup?.Hide();

        using var dialog = new Form
        {
            Text = "Importar de outros navegadores",
            ClientSize = new Size((int)(460 * DeviceDpi / 96f), (int)(320 * DeviceDpi / 96f)),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = darkMode ? BrowserTheme.Surface : Color.FromArgb(248, 250, 252),
            ForeColor = darkMode ? BrowserTheme.Text : Color.FromArgb(30, 41, 59),
            Font = new Font("Segoe UI", 9f)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20, 16, 20, 16)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var headerTitle = new Label
        {
            Text = "Importar favoritos e dados de navegação",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = darkMode ? Color.FromArgb(14, 165, 233) : Color.FromArgb(2, 132, 199),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 4)
        };
        var headerSubtitle = new Label
        {
            Text = "Selecione o navegador de origem para importar seus favoritos diretamente para o VOLT.",
            ForeColor = darkMode ? Color.FromArgb(160, 175, 195) : Color.FromArgb(100, 116, 139),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 12)
        };

        var comboLabel = new Label
        {
            Text = "Navegador de origem:",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(0, 0, 0, 4)
        };

        var browserCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Top,
            Height = 30,
            BackColor = darkMode ? BrowserTheme.Background : Color.White,
            ForeColor = dialog.ForeColor
        };

        var detected = BrowserImport.GetDetectedBrowsers();
        int defaultSelectIndex = 0;
        int idx = 0;

        foreach (var b in detected)
        {
            browserCombo.Items.Add(new BrowserComboItem(b.Name, b.BookmarkPath, b.IsAvailable, false, false));
            if (b.IsAvailable && defaultSelectIndex == 0) defaultSelectIndex = idx;
            idx++;
        }

        browserCombo.Items.Add(new BrowserComboItem("Arquivo HTML de Favoritos (Exportado de qualquer navegador)…", "", true, true, false));
        browserCombo.Items.Add(new BrowserComboItem("Arquivo CSV de Senhas…", "", true, false, true));

        browserCombo.SelectedIndex = defaultSelectIndex;

        var checkBookmarks = new CheckBox
        {
            Text = "Importar Favoritos / Marcadores para a barra de favoritos",
            Checked = true,
            AutoSize = true,
            Cursor = Cursors.Hand,
            UseMnemonic = false,
            Margin = new Padding(0, 10, 0, 2)
        };

        var infoLabel = new Label
        {
            Text = "Os favoritos importados serão integrados à sua barra de favoritos e autocompletar.",
            Font = new Font("Segoe UI", 8.25f),
            ForeColor = darkMode ? Color.FromArgb(140, 155, 175) : Color.FromArgb(100, 116, 139),
            AutoSize = true,
            UseMnemonic = false,
            Margin = new Padding(20, 0, 0, 8)
        };

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 36,
            Margin = Padding.Empty
        };

        var btnCancel = new Button
        {
            Text = "Cancelar",
            DialogResult = DialogResult.Cancel,
            Height = 32,
            Width = 90,
            Cursor = Cursors.Hand,
            FlatStyle = FlatStyle.Flat,
            BackColor = darkMode ? Color.FromArgb(45, 58, 73) : SystemColors.ButtonFace,
            ForeColor = dialog.ForeColor,
            Margin = new Padding(6, 0, 0, 0),
            UseMnemonic = false
        };

        var btnImport = new Button
        {
            Text = "Importar",
            Height = 32,
            Width = 100,
            Cursor = Cursors.Hand,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(14, 165, 233),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            UseMnemonic = false
        };

        btnImport.Click += (_, _) =>
        {
            if (browserCombo.SelectedItem is not BrowserComboItem item) return;

            if (item.IsPasswords)
            {
                dialog.Close();
                ShowPasswords();
                return;
            }

            List<BrowserImport.ImportedBookmark> toImport = new();
            string sourceName = item.Name;

            if (item.IsHtmlFile)
            {
                using var fileDlg = new OpenFileDialog
                {
                    Title = "Selecionar arquivo HTML de favoritos",
                    Filter = "Arquivos HTML de favoritos (*.html;*.htm)|*.html;*.htm|Todos os arquivos (*.*)|*.*",
                    CheckFileExists = true
                };
                if (fileDlg.ShowDialog(dialog) != DialogResult.OK) return;

                try
                {
                    string html = File.ReadAllText(fileDlg.FileName);
                    toImport = BrowserImport.ParseHtmlBookmarks(html);
                    sourceName = Path.GetFileName(fileDlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(dialog, "Erro ao ler o arquivo HTML: " + ex.Message, "Importação de Favoritos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (!File.Exists(item.Path))
                {
                    MessageBox.Show(dialog, $"O arquivo de favoritos do {item.Name} não foi encontrado no seu perfil de usuário.", "Navegador não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                toImport = BrowserImport.ReadChromiumBookmarks(item.Path);
            }

            if (toImport.Count == 0)
            {
                MessageBox.Show(dialog, $"Nenhum favorito encontrado para importar de {sourceName}.", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int addedCount = 0;
            var existingUrls = new HashSet<string>(bookmarks.Select(b => b.Url), StringComparer.OrdinalIgnoreCase);

            foreach (var b in toImport)
            {
                if (existingUrls.Add(b.Url))
                {
                    bookmarks.Add(new SavedLink(b.Title, b.Url));
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                SaveBookmarks();
                RenderBookmarks();
                RefreshAddressSuggestions();
                status.Text = $"{addedCount} favoritos importados com sucesso de {sourceName}.";
            }

            MessageBox.Show(dialog,
                addedCount > 0
                    ? $"Importação concluída com sucesso!\n\n• {addedCount} novos favoritos importados de {sourceName}.\n• Total de favoritos no VOLT: {bookmarks.Count}."
                    : $"Todos os {toImport.Count} favoritos encontrados em {sourceName} já estavam salvos no seu VOLT.",
                "Importação Concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            dialog.Close();
        };

        buttonsPanel.Controls.Add(btnCancel);
        buttonsPanel.Controls.Add(btnImport);

        var topBox = new Panel { Dock = DockStyle.Top, AutoSize = true, Margin = Padding.Empty };
        topBox.Controls.Add(headerSubtitle);
        topBox.Controls.Add(headerTitle);

        var midBox = new Panel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 8) };
        browserCombo.Dock = DockStyle.Top;
        comboLabel.Dock = DockStyle.Top;
        midBox.Controls.Add(browserCombo);
        midBox.Controls.Add(comboLabel);

        var optionsBox = new Panel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 8) };
        optionsBox.Controls.Add(infoLabel);
        optionsBox.Controls.Add(checkBookmarks);

        layout.Controls.Add(topBox, 0, 0);
        layout.Controls.Add(midBox, 0, 1);
        layout.Controls.Add(optionsBox, 0, 2);
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, 3);
        layout.Controls.Add(buttonsPanel, 0, 4);

        dialog.Controls.Add(layout);
        dialog.AcceptButton = btnImport;
        dialog.CancelButton = btnCancel;

        dialog.ShowDialog(this);
    }

    record BrowserComboItem(string Name, string Path, bool IsAvailable, bool IsHtmlFile, bool IsPasswords)
    {
        public override string ToString()
        {
            if (IsHtmlFile || IsPasswords) return Name;
            return IsAvailable ? $"✓ {Name} (Instalado)" : $"✗ {Name} (Não detectado)";
        }
    }
}
