using Microsoft.Web.WebView2.Core;

namespace Volt;

sealed partial class BrowserWindow
{
    readonly Panel whatsAppPanel=new(){Dock=DockStyle.None,Width=720,Visible=false,Tag="chrome"};
    readonly Panel whatsAppBody=new(){Dock=DockStyle.Fill,Tag="surface"};
    readonly Panel whatsAppSplitter=new(){Dock=DockStyle.None,Width=5,Visible=false,Cursor=Cursors.SizeWE,Tag="chrome"};
    readonly CheckBox whatsAppKeepActive=new(){Text="Manter ativo",Checked=true,Dock=DockStyle.Right,Width=125,AccessibleName="Manter WhatsApp ativo em segundo plano"};
    readonly Label whatsAppStatus=new(){Dock=DockStyle.Fill,Text="Abra um atalho da barra lateral.",TextAlign=ContentAlignment.MiddleCenter,Tag="surface"};
    readonly FlowLayoutPanel sidebarItemsFlow=new(){Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new(2,4,2,4),Tag="chrome"};
    readonly System.Windows.Forms.Timer sidebarAnimTimer=new(){Interval=15};
    int sidebarAnimTargetWidth;
    float sidebarAnimCurrentWidth;
    bool sidebarOpening;
    string? currentSidebarItemId;
    BrowserView? customSidebarView;
    Label? sidebarTitleLabel;
    Point? whatsAppDragStart;
    int whatsAppDragWidth;
    OutsidePanelClick? outsideWhatsApp;
    Panel? browserContent;
    BrowserView? whatsAppView;
    bool whatsAppBusy;
    int whatsAppDownloads;
    DateTime whatsAppHiddenSince;
    PictureBox? sidebarLogo;

    void InitializeSidebar(Panel content)
    {
        browserContent=content;
        var sidebar=new Panel {Dock=DockStyle.Left,Width=48,Tag="chrome"};
        sidebarLogo=new PictureBox {Dock=DockStyle.Top,Height=42,SizeMode=PictureBoxSizeMode.Zoom,Image=Icon?.ToBitmap(),Margin=new(4),Tag="chrome",AccessibleName="VOLT"};
        sidebar.Controls.Add(sidebarItemsFlow);
        sidebar.Controls.Add(sidebarLogo);

        RefreshSidebarItems();

        outsideWhatsApp=new OutsidePanelClick(this,point=>
            whatsAppPanel.Visible && RectangleToScreen(ClientRectangle).Contains(point)
            && !whatsAppPanel.RectangleToScreen(whatsAppPanel.ClientRectangle).Contains(point)
            && !whatsAppSplitter.RectangleToScreen(whatsAppSplitter.ClientRectangle).Contains(point)
            && !sidebar.RectangleToScreen(sidebar.ClientRectangle).Contains(point),HideWhatsApp);

        whatsAppKeepActive.CheckedChanged+=(_,_)=>
        {
            try{if(whatsAppKeepActive.Checked && whatsAppView?.CoreWebView2 is {} core && core.IsSuspended)core.Resume();}
            catch(Exception ex) when(ex is InvalidOperationException or System.Runtime.InteropServices.COMException){}
        };

        var header=new Panel {Dock=DockStyle.Top,Height=40,Padding=new(8,2,2,2),Tag="chrome"};
        sidebarTitleLabel=new Label {Text="WhatsApp",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,Font=new("Segoe UI",9.5f,FontStyle.Bold),Tag="chrome"};
        var close=MakeButton("Fechar janela");close.Dock=DockStyle.Right;close.Width=34;close.AccessibleName="Ocultar painel da barra lateral";
        close.Click+=(_,_)=>HideWhatsApp();chromeTips.SetToolTip(close,close.AccessibleName);
        var reload=MakeButton("↻");reload.Dock=DockStyle.Right;reload.Width=34;reload.AccessibleName="Recarregar painel lateral";
        reload.Click+=async(_,_)=>
        {
            if(currentSidebarItemId=="whatsapp")
            {
                if(whatsAppView?.CoreWebView2 is {} core)core.Reload();else await EnsureWhatsApp();
            }
            else
            {
                if(customSidebarView?.CoreWebView2 is {} core2)core2.Reload();
                else if(browsing.SidebarItems.FirstOrDefault(x=>x.Id==currentSidebarItemId) is {} item)
                    await EnsureCustomSidebarWeb(item.Url);
            }
        };
        chromeTips.SetToolTip(reload,"Recarregar");
        header.Controls.Add(sidebarTitleLabel);header.Controls.Add(whatsAppKeepActive);header.Controls.Add(reload);header.Controls.Add(close);
        whatsAppBody.Controls.Add(whatsAppStatus);whatsAppPanel.Controls.Add(whatsAppBody);whatsAppPanel.Controls.Add(header);
        content.Controls.Add(whatsAppSplitter);content.Controls.Add(whatsAppPanel);Controls.Add(sidebar);

        sidebarAnimTimer.Tick+=(_,_) =>
        {
            if(browserContent is null || closing) { sidebarAnimTimer.Stop(); return; }
            if(sidebarOpening)
            {
                float diff=sidebarAnimTargetWidth-sidebarAnimCurrentWidth;
                if(diff<=3f)
                {
                    sidebarAnimCurrentWidth=sidebarAnimTargetWidth;
                    sidebarAnimTimer.Stop();
                }
                else
                {
                    sidebarAnimCurrentWidth+=Math.Max(14f,diff*0.35f);
                    if(sidebarAnimCurrentWidth>sidebarAnimTargetWidth)sidebarAnimCurrentWidth=sidebarAnimTargetWidth;
                }
                whatsAppPanel.Width=(int)sidebarAnimCurrentWidth;
                LayoutWhatsAppOverlay();
            }
            else
            {
                if(sidebarAnimCurrentWidth<=4f)
                {
                    sidebarAnimCurrentWidth=0;
                    whatsAppPanel.Width=0;
                    whatsAppPanel.Visible=false;
                    whatsAppSplitter.Visible=false;
                    sidebarAnimTimer.Stop();
                    RefreshBrowserContent();
                }
                else
                {
                    sidebarAnimCurrentWidth-=Math.Max(16f,sidebarAnimCurrentWidth*0.35f);
                    if(sidebarAnimCurrentWidth<0)sidebarAnimCurrentWidth=0;
                    whatsAppPanel.Width=(int)sidebarAnimCurrentWidth;
                    LayoutWhatsAppOverlay();
                }
            }
        };

        whatsAppSplitter.MouseDown+=(_,e)=>
        {
            if(e.Button!=MouseButtons.Left)return;
            whatsAppDragStart=Cursor.Position;whatsAppDragWidth=whatsAppPanel.Width;whatsAppSplitter.Capture=true;
        };
        whatsAppSplitter.MouseMove+=(_,_)=>
        {
            if(whatsAppDragStart is not {} start)return;
            whatsAppPanel.Width=SidebarLayout.Width(content.ClientSize.Width,whatsAppDragWidth+Cursor.Position.X-start.X);
            sidebarAnimCurrentWidth=whatsAppPanel.Width;
            LayoutWhatsAppOverlay();
        };
        whatsAppSplitter.MouseUp+=(_,_)=>{whatsAppDragStart=null;whatsAppSplitter.Capture=false;};
        whatsAppSplitter.MouseCaptureChanged+=(_,_)=>{if(!whatsAppSplitter.Capture)whatsAppDragStart=null;};
        content.Resize+=(_,_)=>{if(whatsAppPanel.Visible)LayoutWhatsAppOverlay();};
    }

    void RefreshSidebarItems()
    {
        sidebarItemsFlow.SuspendLayout();
        try
        {
            foreach(Control c in sidebarItemsFlow.Controls.Cast<Control>().ToArray()) c.Dispose();
            sidebarItemsFlow.Controls.Clear();

            // WhatsApp button
            var whatsapp=new ChromeButton
            {
                Kind="whatsapp",Width=40,Height=40,Margin=new(0,2,0,4),
                BackColor=Color.FromArgb(239,244,248),ForeColor=Color.White,Tag="chrome",AccessibleName="Abrir ou ocultar WhatsApp"
            };
            whatsapp.Click+=async(_,_)=>await ToggleSidebarItem("whatsapp","WhatsApp","https://web.whatsapp.com/");
            chromeTips.SetToolTip(whatsapp,"WhatsApp");
            sidebarItemsFlow.Controls.Add(whatsapp);

            // Custom items
            foreach(var item in browsing.SidebarItems)
            {
                var targetItem=item;
                string label=targetItem.Title.Length>2?targetItem.Title[..2].ToUpperInvariant():targetItem.Title.ToUpperInvariant();
                var btn=new ChromeButton
                {
                    Text=label,Width=40,Height=40,Margin=new(0,2,0,4),
                    BackColor=Color.FromArgb(239,244,248),ForeColor=darkMode?BrowserTheme.Text:Color.FromArgb(37,51,67),
                    Font=new("Segoe UI",9f,FontStyle.Bold),Tag="chrome",AccessibleName=targetItem.Title
                };
                btn.Click+=async(_,_)=>await ToggleSidebarItem(targetItem.Id,targetItem.Title,targetItem.Url);
                chromeTips.SetToolTip(btn,$"{targetItem.Title}\n{targetItem.Url}");

                var context=new ContextMenuStrip();
                context.Items.Add("Abrir",null,async(_,_)=>await ToggleSidebarItem(targetItem.Id,targetItem.Title,targetItem.Url));
                context.Items.Add("Remover da barra lateral",null,(_,_)=>
                {
                    browsing.SidebarItems.Remove(targetItem);
                    SaveBrowsingPreferences();
                    RefreshSidebarItems();
                    status.Text=$"Atalho '{targetItem.Title}' removido da barra lateral.";
                });
                btn.ContextMenuStrip=context;
                btn.Disposed+=(_,_)=>context.Dispose();
                sidebarItemsFlow.Controls.Add(btn);
            }

            // Add (+) button
            var addBtn=new ChromeButton
            {
                Text="+",Width=40,Height=40,Margin=new(0,4,0,4),
                BackColor=Color.FromArgb(239,244,248),ForeColor=darkMode?BrowserTheme.Text:Color.FromArgb(57,72,89),
                Font=new("Segoe UI",12f,FontStyle.Bold),Tag="chrome",AccessibleName="Adicionar atalho à barra lateral"
            };
            addBtn.Click+=(_,_)=>ShowAddSidebarItemDialog();
            chromeTips.SetToolTip(addBtn,"Adicionar atalho à barra lateral");
            sidebarItemsFlow.Controls.Add(addBtn);
        }
        finally
        {
            sidebarItemsFlow.ResumeLayout(true);
            BrowserTheme.Apply(sidebarItemsFlow,darkMode);
        }
    }

    void ShowAddSidebarItemDialog()
    {
        using var dialog=new Form
        {
            Text="Adicionar à Barra Lateral",
            ClientSize=new(460,280),
            StartPosition=FormStartPosition.CenterParent,
            FormBorderStyle=FormBorderStyle.FixedDialog,
            MaximizeBox=false,MinimizeBox=false,
            BackColor=darkMode?BrowserTheme.Surface:SystemColors.Control,
            ForeColor=darkMode?BrowserTheme.Text:SystemColors.ControlText
        };

        var nameLabel=new Label {Text="Nome do atalho:",Bounds=new(20,16,420,20),ForeColor=dialog.ForeColor};
        var nameInput=new TextBox {Bounds=new(20,38,420,28),BackColor=darkMode?BrowserTheme.Background:Color.White,ForeColor=dialog.ForeColor};

        var urlLabel=new Label {Text="Endereço (URL):",Bounds=new(20,74,420,20),ForeColor=dialog.ForeColor};
        var urlInput=new TextBox {Bounds=new(20,96,420,28),BackColor=darkMode?BrowserTheme.Background:Color.White,ForeColor=dialog.ForeColor};

        var presetsLabel=new Label {Text="Atalhos rápidos:",Bounds=new(20,132,420,20),ForeColor=dialog.ForeColor};
        var presetsFlow=new FlowLayoutPanel {Bounds=new(20,154,420,40),WrapContents=true};

        foreach(var (pName,pUrl) in new[]
        {
            ("ChatGPT","https://chatgpt.com"),
            ("Telegram","https://web.telegram.org"),
            ("Tradutor","https://translate.google.com"),
            ("Notion","https://www.notion.so"),
            ("X","https://x.com")
        })
        {
            var pBtn=new Button
            {
                Text=pName,AutoSize=true,Height=28,Margin=new(0,0,6,0),
                BackColor=darkMode?Color.FromArgb(49,61,77):Color.FromArgb(230,238,246),
                ForeColor=dialog.ForeColor,FlatStyle=FlatStyle.Flat
            };
            pBtn.FlatAppearance.BorderSize=0;
            string n=pName, u=pUrl;
            pBtn.Click+=(_,_)=>{nameInput.Text=n;urlInput.Text=u;};
            presetsFlow.Controls.Add(pBtn);
        }

        var btnCancel=new Button
        {
            Text="Cancelar",DialogResult=DialogResult.Cancel,Bounds=new(230,225,100,32),
            BackColor=darkMode?Color.FromArgb(49,61,77):SystemColors.ButtonFace,ForeColor=dialog.ForeColor
        };
        var btnAdd=new Button
        {
            Text="Adicionar",Bounds=new(340,225,100,32),
            BackColor=Color.FromArgb(32,120,220),ForeColor=Color.White,FlatStyle=FlatStyle.Flat
        };
        btnAdd.FlatAppearance.BorderSize=0;

        btnAdd.Click+=(_,_)=>
        {
            string title=nameInput.Text.Trim();
            string url=urlInput.Text.Trim();
            if(string.IsNullOrWhiteSpace(title)) { MessageBox.Show(dialog,"Digite um nome para o atalho.","VOLT"); return; }
            if(!ResourcePolicy.IsHttp(url)) url="https://"+url;
            if(!ResourcePolicy.IsHttp(url)) { MessageBox.Show(dialog,"Digite uma URL válida (ex: https://site.com).","VOLT"); return; }

            browsing.SidebarItems.Add(new SidebarItemModel {Title=title,Url=url});
            SaveBrowsingPreferences();
            RefreshSidebarItems();
            status.Text=$"Atalho '{title}' adicionado à barra lateral.";
            dialog.DialogResult=DialogResult.OK;
            dialog.Close();
        };

        dialog.Controls.AddRange(new Control[]{nameLabel,nameInput,urlLabel,urlInput,presetsLabel,presetsFlow,btnCancel,btnAdd});
        dialog.AcceptButton=btnAdd;dialog.CancelButton=btnCancel;
        dialog.ShowDialog(this);
    }

    async Task ToggleSidebarItem(string id, string title, string url)
    {
        if(whatsAppPanel.Visible && currentSidebarItemId==id)
        {
            HideWhatsApp();
            return;
        }

        currentSidebarItemId=id;
        if(sidebarTitleLabel!=null) sidebarTitleLabel.Text=title;

        if(id=="whatsapp")
        {
            whatsAppKeepActive.Visible=true;
            if(customSidebarView!=null) customSidebarView.Visible=false;
            if(whatsAppView?.CoreWebView2 is {} core)
            {
                core.Resume();
                whatsAppView.Visible=true;
                whatsAppView.SetPageVisible(true,true);
            }
            else
            {
                await EnsureWhatsApp();
            }
        }
        else
        {
            whatsAppKeepActive.Visible=false;
            if(whatsAppView!=null) whatsAppView.Visible=false;
            await EnsureCustomSidebarWeb(url);
        }

        AnimateOpenSidebar();
    }

    async Task EnsureCustomSidebarWeb(string url)
    {
        if(closing)return;
        whatsAppStatus.Text="Carregando...";
        whatsAppStatus.Visible=false;

        if(customSidebarView is null)
        {
            var view=new BrowserView
            {
                Dock=DockStyle.Fill,Visible=true,
                BackColor=darkMode?BrowserTheme.Background:Color.White,
                DefaultBackgroundColor=darkMode?BrowserTheme.Background:Color.White
            };
            customSidebarView=view;
            whatsAppBody.Controls.Add(view);
            view.BringToFront();

            string profile=AppPaths.ProfileDirectory;
            environmentTask??=CoreWebView2Environment.CreateAsync(null,profile);
            var env=await environmentTask;
            if(closing || customSidebarView!=view)return;
            environment=env;
            await view.EnsureCoreWebView2Async(env);
            if(closing || customSidebarView!=view)return;
            SetWebTheme(view);
            var core=view.CoreWebView2;
            core.Settings.AreDevToolsEnabled=false;
            core.NewWindowRequested+=(_,e)=>{e.Handled=true;if(e.IsUserInitiated && ResourcePolicy.IsHttp(e.Uri))_=AddTab(e.Uri);};
        }

        customSidebarView.Visible=true;
        customSidebarView.BringToFront();
        customSidebarView.SetPageVisible(true,true);
        if(customSidebarView.CoreWebView2 is {} core2)
        {
            core2.Navigate(url);
        }
    }

    void AnimateOpenSidebar()
    {
        if(browserContent is null || closing)return;
        int preferred=whatsAppView is null?(int)(760*DeviceDpi/96f):whatsAppPanel.Width;
        int target=SidebarLayout.Width(browserContent.ClientSize.Width,preferred<=0?(int)(760*DeviceDpi/96f):preferred);

        sidebarOpening=true;
        sidebarAnimTargetWidth=target;
        sidebarAnimCurrentWidth=whatsAppPanel.Visible?whatsAppPanel.Width:0;
        whatsAppPanel.Width=(int)sidebarAnimCurrentWidth;
        whatsAppPanel.Visible=true;
        whatsAppSplitter.Visible=true;
        whatsAppPanel.BringToFront();
        whatsAppSplitter.BringToFront();
        LayoutWhatsAppOverlay();
        sidebarAnimTimer.Start();
        outsideWhatsApp?.Start();
    }

    void HideWhatsApp()
    {
        if(!whatsAppPanel.Visible && !sidebarOpening) return;
        sidebarOpening=false;
        sidebarAnimCurrentWidth=whatsAppPanel.Width;
        sidebarAnimTimer.Start();
        outsideWhatsApp?.Stop();
        whatsAppHiddenSince=DateTime.UtcNow;
    }

    void LayoutWhatsAppOverlay()
    {
        if(browserContent is null)return;
        int width=whatsAppPanel.Width;
        whatsAppPanel.SetBounds(0,0,width,browserContent.ClientSize.Height);
        whatsAppSplitter.SetBounds(width,0,5,browserContent.ClientSize.Height);
    }

    void RefreshBrowserContent()
    {
        if(browserContent is null || closing)return;
        browserContent.PerformLayout();
        tabs.PerformLayout();
        if(Current is {} tab)
        {
            tab.Page.PerformLayout();
            foreach(Control child in tab.Page.Controls)child.PerformLayout();
        }
        browserContent.Invalidate(true);
        browserContent.Update();
    }

    async Task EnsureWhatsApp()
    {
        if(closing || whatsAppBusy || whatsAppView is not null)return;
        whatsAppBusy=true;whatsAppStatus.Text="Carregando WhatsApp…";whatsAppStatus.Visible=true;
        var view=new BrowserView {Dock=DockStyle.Fill,Visible=true,BackColor=darkMode?BrowserTheme.Background:Color.White,DefaultBackgroundColor=darkMode?BrowserTheme.Background:Color.White};
        whatsAppView=view;whatsAppBody.Controls.Add(view);view.BringToFront();
        try
        {
            string profile = AppPaths.ProfileDirectory;
            environmentTask??=CoreWebView2Environment.CreateAsync(null,profile);
            var env=await environmentTask;
            if(closing || whatsAppView!=view)return;
            environment=env;await view.EnsureCoreWebView2Async(env);
            if(closing || whatsAppView!=view)return;
            SetWebTheme(view);view.SetPageVisible(whatsAppPanel.Visible,true);
            var core=view.CoreWebView2;core.Settings.AreDevToolsEnabled=false;
            core.NewWindowRequested+=(_,e)=>{e.Handled=true;if(e.IsUserInitiated && ResourcePolicy.IsHttp(e.Uri))_ = AddTab(e.Uri);};
            core.NavigationStarting+=(_,e)=>
            {
                if(e.IsUserInitiated && Uri.TryCreate(e.Uri,UriKind.Absolute,out var uri) && ResourcePolicy.IsHttp(e.Uri) && uri.Host!="web.whatsapp.com")
                {e.Cancel=true;_ = AddTab(e.Uri);}
            };
            core.DownloadStarting+=(_,e)=>
            {
                whatsAppDownloads++;bool finished=false;
                e.DownloadOperation.StateChanged+=(_,_)=>
                {
                    if(finished || e.DownloadOperation.State==CoreWebView2DownloadState.InProgress)return;
                    finished=true;if(whatsAppView==view)whatsAppDownloads=Math.Max(0,whatsAppDownloads-1);
                };
            };
            core.Navigate("https://web.whatsapp.com/");whatsAppStatus.Visible=false;
        }
        catch(Exception ex) when(ex is InvalidOperationException or System.Runtime.InteropServices.COMException)
        {
            if(!closing && whatsAppView==view)
            {DisposeWhatsApp();whatsAppStatus.Text="Não foi possível abrir o WhatsApp. Use o botão de recarregar para tentar novamente.";}
        }
        finally{whatsAppBusy=false;}
    }

    bool CanReleaseWhatsApp()
    {
        if(whatsAppPanel.Visible || whatsAppKeepActive.Checked || whatsAppBusy || whatsAppDownloads>0 || whatsAppView?.CoreWebView2 is not {} core)return false;
        try{return !core.IsDocumentPlayingAudio;}catch(Exception ex) when(ex is InvalidOperationException or System.Runtime.InteropServices.COMException){return false;}
    }

    bool ReleaseHiddenWhatsApp()
    {
        if(!CanReleaseWhatsApp())return false;
        DisposeWhatsApp();return true;
    }

    async Task SweepWhatsApp()
    {
        if(closing || !economy || !ResourcePolicy.SleepDue(sleepMinutes,DateTime.UtcNow-whatsAppHiddenSince) || !CanReleaseWhatsApp())return;
        var view=whatsAppView!;
        whatsAppBusy=true;
        try
        {
            var core=view.CoreWebView2;if(core.IsSuspended)return;
            view.SetPageVisible(false,true);await core.TrySuspendAsync();
            if(whatsAppView==view && (whatsAppPanel.Visible || whatsAppKeepActive.Checked))core.Resume();
        }
        catch(Exception ex) when(ex is InvalidOperationException or System.Runtime.InteropServices.COMException){}
        finally{whatsAppBusy=false;}
    }

    void DisposeWhatsApp()
    {
        var view=whatsAppView;whatsAppView=null;whatsAppDownloads=0;
        if(view is not null){whatsAppBody.Controls.Remove(view);view.Dispose();}
        var cView=customSidebarView;customSidebarView=null;
        if(cView is not null){whatsAppBody.Controls.Remove(cView);cView.Dispose();}
        sidebarAnimTimer.Stop();sidebarAnimTimer.Dispose();
        whatsAppStatus.Visible=true;
    }
}
