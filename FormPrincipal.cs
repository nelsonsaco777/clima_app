using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppMeteo
{
    public class FormPrincipal : Form
    {
        private Panel painelTopo;
        private Panel painelCorpo;
        private Panel painelPesquisa;
        private Panel painelAtual;
        private Panel painelDetalhes;
        private Panel painelRodape;

        private TextBox txtCidade;
        private Button btnPesquisar;
        private Label lblStatus;

        private Label lblIconeAtual;
        private Label lblTemperatura;
        private Label lblDescricao;
        private Label lblCidade;
        private Label lblDataHora;
        private Label lblMaxMin;

        private Label lblHumidadeValor;
        private Label lblVentoValor;

        private FlowLayoutPanel flpPrevisao;
        private System.Windows.Forms.Timer timerHora;

        private readonly ServicoMeteo _servico;

        private readonly Color corFundo    = Color.FromArgb(15,  23,  42);
        private readonly Color corPainel   = Color.FromArgb(30,  41,  59);
        private readonly Color corCard     = Color.FromArgb(51,  65,  85);
        private readonly Color corAcento   = Color.FromArgb(56, 189, 248);
        private readonly Color corTexto    = Color.FromArgb(226, 232, 240);
        private readonly Color corTextoSub = Color.FromArgb(148, 163, 184);
        private readonly Color corBorda    = Color.FromArgb(71,  85, 105);

        public FormPrincipal()
        {
            _servico = new ServicoMeteo();
            InicializarComponentes();
            ConfigurarTimer();
        }

        private void InicializarComponentes()
        {
            this.Text = "App Meteorologia";
            this.Size = new Size(820, 680);
            this.MinimumSize = new Size(750, 600);
            this.BackColor = corFundo;
            this.ForeColor = corTexto;
            this.Font = new Font("Segoe UI", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // TOPO
            painelTopo = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = corPainel };

            var lblTitulo = new Label
            {
                Text = "METEOROLOGIA",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = corAcento,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            lblDataHora = new Label
            {
                Font = new Font("Segoe UI", 9f),
                ForeColor = corTextoSub,
                AutoSize = true,
                Location = new Point(20, 45)
            };
            AtualizarHora();

            painelTopo.Controls.Add(lblTitulo);
            painelTopo.Controls.Add(lblDataHora);

            // PESQUISA
            painelPesquisa = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = corFundo };

            var painelInput = new Panel
            {
                Location = new Point(20, 13),
                Size = new Size(586, 36),
                BackColor = corCard
            };

            txtCidade = new TextBox
            {
                PlaceholderText = "Digite o nome da cidade (ex: Luanda, Lisboa, Porto)...",
                Font = new Font("Segoe UI", 10f),
                BackColor = corCard,
                ForeColor = corTexto,
                BorderStyle = BorderStyle.None,
                Location = new Point(10, 7),
                Width = 566
            };
            txtCidade.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; _ = PesquisarCidade(); }
            };
            painelInput.Controls.Add(txtCidade);

            btnPesquisar = new Button
            {
                Text = "Pesquisar",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = corAcento,
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(616, 13),
                Size = new Size(140, 36),
                Cursor = Cursors.Hand
            };
            btnPesquisar.FlatAppearance.BorderSize = 0;
            btnPesquisar.Click += async (s, e) => await PesquisarCidade();

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(248, 113, 113),
                AutoSize = true,
                Location = new Point(20, 52)
            };

            painelPesquisa.Controls.Add(painelInput);
            painelPesquisa.Controls.Add(btnPesquisar);
            painelPesquisa.Controls.Add(lblStatus);

            // CORPO
            painelCorpo = new Panel { Dock = DockStyle.Fill, BackColor = corFundo };

            // Card temperatura atual
            painelAtual = new Panel
            {
                Location = new Point(20, 10),
                Size = new Size(360, 255),
                BackColor = corPainel
            };

            lblCidade = new Label
            {
                Text = "---",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = corTexto,
                Location = new Point(20, 18),
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };

            lblIconeAtual = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 44f),
                Location = new Point(20, 55),
                AutoSize = true
            };

            lblTemperatura = new Label
            {
                Text = "--C",
                Font = new Font("Segoe UI", 40f, FontStyle.Bold),
                ForeColor = corAcento,
                Location = new Point(130, 60),
                AutoSize = true
            };

            lblDescricao = new Label
            {
                Text = "Pesquise uma cidade para comecar",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = corTextoSub,
                Location = new Point(20, 170),
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };

            lblMaxMin = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = corTextoSub,
                Location = new Point(20, 200),
                AutoSize = true
            };

            painelAtual.Controls.Add(lblCidade);
            painelAtual.Controls.Add(lblIconeAtual);
            painelAtual.Controls.Add(lblTemperatura);
            painelAtual.Controls.Add(lblDescricao);
            painelAtual.Controls.Add(lblMaxMin);

            // Card detalhes
            painelDetalhes = new Panel
            {
                Location = new Point(395, 10),
                Size = new Size(370, 255),
                BackColor = corPainel
            };

            var lblTituloDetalhes = new Label
            {
                Text = "DETALHES",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = corTextoSub,
                Location = new Point(20, 18),
                AutoSize = true
            };

            var lblHumidadeTit = new Label
            {
                Text = "Humidade",
                Font = new Font("Segoe UI", 9f),
                ForeColor = corTextoSub,
                Location = new Point(20, 55),
                AutoSize = true
            };

            lblHumidadeValor = new Label
            {
                Text = "--",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = corTexto,
                Location = new Point(20, 75),
                AutoSize = true
            };

            var lblVentoTit = new Label
            {
                Text = "Vento",
                Font = new Font("Segoe UI", 9f),
                ForeColor = corTextoSub,
                Location = new Point(20, 145),
                AutoSize = true
            };

            lblVentoValor = new Label
            {
                Text = "--",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = corTexto,
                Location = new Point(20, 165),
                AutoSize = true
            };

            painelDetalhes.Controls.Add(lblTituloDetalhes);
            painelDetalhes.Controls.Add(lblHumidadeTit);
            painelDetalhes.Controls.Add(lblHumidadeValor);
            painelDetalhes.Controls.Add(lblVentoTit);
            painelDetalhes.Controls.Add(lblVentoValor);

            // Titulo previsao
            var lblTituloPrevisao = new Label
            {
                Text = "PREVISAO PARA 7 DIAS",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = corTextoSub,
                Location = new Point(20, 280),
                AutoSize = true
            };

            flpPrevisao = new FlowLayoutPanel
            {
                Location = new Point(20, 300),
                Size = new Size(745, 145),
                BackColor = corFundo,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };

            painelCorpo.Controls.Add(painelAtual);
            painelCorpo.Controls.Add(painelDetalhes);
            painelCorpo.Controls.Add(lblTituloPrevisao);
            painelCorpo.Controls.Add(flpPrevisao);

            // RODAPE
            painelRodape = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = corPainel };
            var lblRodape = new Label
            {
                Text = "Dados: Open-Meteo API (gratuito, sem chave) | App Meteorologia C# WinForms",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = corTextoSub,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            painelRodape.Controls.Add(lblRodape);

            this.Controls.Add(painelCorpo);
            this.Controls.Add(painelPesquisa);
            this.Controls.Add(painelTopo);
            this.Controls.Add(painelRodape);
        }

        private void ConfigurarTimer()
        {
            timerHora = new System.Windows.Forms.Timer { Interval = 1000 };
            timerHora.Tick += (s, e) => AtualizarHora();
            timerHora.Start();
        }

        private void AtualizarHora()
        {
            if (lblDataHora != null)
                lblDataHora.Text = DateTime.Now.ToString(
                    "dddd, dd 'de' MMMM 'de' yyyy  -  HH:mm:ss",
                    new System.Globalization.CultureInfo("pt-PT"));
        }

        private async Task PesquisarCidade()
        {
            string cidade = txtCidade.Text.Trim();
            if (string.IsNullOrEmpty(cidade)) return;

            lblStatus.ForeColor = corTextoSub;
            lblStatus.Text = "A carregar dados...";
            btnPesquisar.Enabled = false;

            try
            {
                var dados = await _servico.ObterMeteo(cidade);
                AtualizarUI(dados);
                lblStatus.Text = "Atualizado as " + DateTime.Now.ToString("HH:mm:ss");
                lblStatus.ForeColor = Color.FromArgb(74, 222, 128);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(248, 113, 113);
            }
            finally
            {
                btnPesquisar.Enabled = true;
            }
        }

        private void AtualizarUI(DadosMeteo dados)
        {
            lblCidade.Text = dados.Cidade;
            lblTemperatura.Text = dados.Temperatura.ToString("0") + "C";
            lblIconeAtual.Text = dados.IconeTempo;
            lblDescricao.Text = dados.DescricaoTempo;
            lblMaxMin.Text = "Max: " + dados.TemperaturaMax.ToString("0") + "C  Min: " + dados.TemperaturaMin.ToString("0") + "C";

            lblHumidadeValor.Text = dados.Humidade.ToString() + "%";
            lblVentoValor.Text = dados.VelocidadeVento.ToString("0") + " km/h " + dados.DirecaoVento;

            flpPrevisao.Controls.Clear();
            foreach (var dia in dados.Previsao)
                flpPrevisao.Controls.Add(CriarCardPrevisao(dia));
        }

        private Panel CriarCardPrevisao(PrevisaoDia dia)
        {
            var card = new Panel
            {
                Size = new Size(98, 135),
                BackColor = corCard,
                Margin = new Padding(0, 0, 8, 0)
            };

            card.Controls.Add(new Label { Text = dia.DiaSemana, Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = corTextoSub, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 8), Width = 98 });
            card.Controls.Add(new Label { Text = dia.Data, Font = new Font("Segoe UI", 7.5f), ForeColor = corTextoSub, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 24), Width = 98 });
            card.Controls.Add(new Label { Text = dia.IconeTempo, Font = new Font("Segoe UI", 20f), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 42), Width = 98, Height = 38 });
            card.Controls.Add(new Label { Text = "Max: " + dia.TempMax.ToString("0"), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = corTexto, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 84), Width = 98 });
            card.Controls.Add(new Label { Text = "Min: " + dia.TempMin.ToString("0"), Font = new Font("Segoe UI", 8f), ForeColor = corTextoSub, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 104), Width = 98 });

            card.MouseEnter += (s, e) => card.BackColor = corBorda;
            card.MouseLeave += (s, e) => card.BackColor = corCard;

            return card;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerHora?.Stop();
            base.OnFormClosed(e);
        }
    }
}
