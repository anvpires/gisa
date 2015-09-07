using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using GISA.Model;
using DBAbstractDataLayer.DataAccessRules;
using GISA.SharedResources;

namespace GISA
{
	public class PanelAvaliacaoSeleccaoEliminacao : GISA.GISAPanel
	{

	#region  Windows Form Designer generated code 

		public PanelAvaliacaoSeleccaoEliminacao() : base()
		{

			//This call is required by the Windows Form Designer.
			InitializeComponent();

			//Add any initialization after the InitializeComponent() call
            cbInforAnaliseTipo.SelectedIndexChanged += cbInforAnaliseTipo_SelectedIndexChanged;
            cbNivel.SelectedIndexChanged += cbNivel_SelectedIndexChanged;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnRemove.Click += btnRemove_Click;
            btnAddDiploma.Click += btnAddDiploma_Click;
            btnRemoveDiploma.Click += btnRemoveDiploma_Click;
            lstVwInforRelacionada.SelectedIndexChanged += LstVwInforRelacionada_SelectedIndexChanged;

			GetExtraResources();
		}

		//Form overrides dispose to clean up the component list.
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components = null;

		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		//    Friend WithEvents clnReferencia As System.Windows.Forms.ColumnHeader
        internal System.Windows.Forms.GroupBox GroupBox2;
        private GroupBox grpEnquadramentoLegal;
        private GroupBox grpRef;
        internal GISA.Controls.PxPageIntegerBox txtRef;
        private GroupBox grpDiploma;
        private TextBox txtDiploma;
        internal Button btnAddDiploma;
        internal Button btnRemoveDiploma;
        internal GroupBox grpObservacoes;
        internal TextBox txtObservacoes;
        internal ControlAutoEliminacao ControlAutoEliminacao1;
        internal GroupBox grpPrazoConservacao;
        internal NumericUpDown nudPrazoConservacao;
        internal Label lblAnos;
        internal GroupBox grpPublicacao;
        internal CheckBox chkPublicar;
        internal GroupBox grpDestinoFinal;
        internal ComboBox cbDestinoFinal;
        internal Panel Panel1;
        internal GroupBox grpTipoAvaliacao;
        internal CheckBox chkModeloAvaliacao;
        internal Panel pnlAvaliacaoTabela;
        internal GroupBox grpAvaliacaoTabela;
        internal GroupBox grpDensidade;
        internal GroupBox grpInforRelacionada;
        internal Button btnRemove;
        internal Button btnEdit;
        internal Button btnAdd;
        internal ListView lstVwInforRelacionada;
        internal ColumnHeader clnTitulo;
        internal ColumnHeader clnDensidade;
        internal ColumnHeader clnSubDensidade;
        internal ColumnHeader clnPonderacao;
        internal GroupBox grpInformacaoAnalise;
        internal Label lblInforAnaliseSubTipo;
        internal Label lblInforAnaliseTipo;
        internal ComboBox cbInforAnaliseSubTipo;
        internal ComboBox cbInforAnaliseTipo;
        internal GroupBox grpPertinencia;
        internal GroupBox grpNivel;
        internal ComboBox cbNivel;
        internal GroupBox grpPonderacao;
        internal TextBox txtPonderacao;
        internal GroupBox grpFrequenciaUso;
        internal ComboBox cbFrequenciaUso;
        internal ComboBox cbModeloAvaliacao;
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.grpDensidade = new System.Windows.Forms.GroupBox();
            this.grpInforRelacionada = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstVwInforRelacionada = new System.Windows.Forms.ListView();
            this.clnTitulo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnDensidade = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnSubDensidade = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clnPonderacao = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpInformacaoAnalise = new System.Windows.Forms.GroupBox();
            this.lblInforAnaliseSubTipo = new System.Windows.Forms.Label();
            this.lblInforAnaliseTipo = new System.Windows.Forms.Label();
            this.cbInforAnaliseSubTipo = new System.Windows.Forms.ComboBox();
            this.cbInforAnaliseTipo = new System.Windows.Forms.ComboBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.grpTipoAvaliacao = new System.Windows.Forms.GroupBox();
            this.chkModeloAvaliacao = new System.Windows.Forms.CheckBox();
            this.pnlAvaliacaoTabela = new System.Windows.Forms.Panel();
            this.grpAvaliacaoTabela = new System.Windows.Forms.GroupBox();
            this.cbModeloAvaliacao = new System.Windows.Forms.ComboBox();
            this.grpPertinencia = new System.Windows.Forms.GroupBox();
            this.grpNivel = new System.Windows.Forms.GroupBox();
            this.cbNivel = new System.Windows.Forms.ComboBox();
            this.grpPonderacao = new System.Windows.Forms.GroupBox();
            this.txtPonderacao = new System.Windows.Forms.TextBox();
            this.grpFrequenciaUso = new System.Windows.Forms.GroupBox();
            this.cbFrequenciaUso = new System.Windows.Forms.ComboBox();
            this.grpEnquadramentoLegal = new System.Windows.Forms.GroupBox();
            this.grpRef = new System.Windows.Forms.GroupBox();
            this.txtRef = new GISA.Controls.PxPageIntegerBox();
            this.grpDiploma = new System.Windows.Forms.GroupBox();
            this.txtDiploma = new System.Windows.Forms.TextBox();
            this.btnAddDiploma = new System.Windows.Forms.Button();
            this.btnRemoveDiploma = new System.Windows.Forms.Button();
            this.grpObservacoes = new System.Windows.Forms.GroupBox();
            this.txtObservacoes = new System.Windows.Forms.TextBox();
            this.ControlAutoEliminacao1 = new GISA.ControlAutoEliminacao();
            this.grpPrazoConservacao = new System.Windows.Forms.GroupBox();
            this.nudPrazoConservacao = new System.Windows.Forms.NumericUpDown();
            this.lblAnos = new System.Windows.Forms.Label();
            this.grpPublicacao = new System.Windows.Forms.GroupBox();
            this.chkPublicar = new System.Windows.Forms.CheckBox();
            this.grpDestinoFinal = new System.Windows.Forms.GroupBox();
            this.cbDestinoFinal = new System.Windows.Forms.ComboBox();
            this.GroupBox2.SuspendLayout();
            this.grpDensidade.SuspendLayout();
            this.grpInforRelacionada.SuspendLayout();
            this.grpInformacaoAnalise.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.grpTipoAvaliacao.SuspendLayout();
            this.pnlAvaliacaoTabela.SuspendLayout();
            this.grpAvaliacaoTabela.SuspendLayout();
            this.grpPertinencia.SuspendLayout();
            this.grpNivel.SuspendLayout();
            this.grpPonderacao.SuspendLayout();
            this.grpFrequenciaUso.SuspendLayout();
            this.grpEnquadramentoLegal.SuspendLayout();
            this.grpRef.SuspendLayout();
            this.grpDiploma.SuspendLayout();
            this.grpObservacoes.SuspendLayout();
            this.grpPrazoConservacao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrazoConservacao)).BeginInit();
            this.grpPublicacao.SuspendLayout();
            this.grpDestinoFinal.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox2.Controls.Add(this.grpDensidade);
            this.GroupBox2.Controls.Add(this.Panel1);
            this.GroupBox2.Controls.Add(this.grpPertinencia);
            this.GroupBox2.Controls.Add(this.grpFrequenciaUso);
            this.GroupBox2.Controls.Add(this.grpEnquadramentoLegal);
            this.GroupBox2.Controls.Add(this.grpObservacoes);
            this.GroupBox2.Controls.Add(this.ControlAutoEliminacao1);
            this.GroupBox2.Controls.Add(this.grpPrazoConservacao);
            this.GroupBox2.Controls.Add(this.grpPublicacao);
            this.GroupBox2.Controls.Add(this.grpDestinoFinal);
            this.GroupBox2.Location = new System.Drawing.Point(3, 3);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(794, 594);
            this.GroupBox2.TabIndex = 0;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "3.2. Avalia��o, sele��o e elimina��o";
            // 
            // grpDensidade
            // 
            this.grpDensidade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDensidade.Controls.Add(this.grpInforRelacionada);
            this.grpDensidade.Controls.Add(this.grpInformacaoAnalise);
            this.grpDensidade.Location = new System.Drawing.Point(6, 95);
            this.grpDensidade.Name = "grpDensidade";
            this.grpDensidade.Size = new System.Drawing.Size(782, 216);
            this.grpDensidade.TabIndex = 0;
            this.grpDensidade.TabStop = false;
            this.grpDensidade.Text = "Densidade";
            // 
            // grpInforRelacionada
            // 
            this.grpInforRelacionada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInforRelacionada.Controls.Add(this.btnRemove);
            this.grpInforRelacionada.Controls.Add(this.btnEdit);
            this.grpInforRelacionada.Controls.Add(this.btnAdd);
            this.grpInforRelacionada.Controls.Add(this.lstVwInforRelacionada);
            this.grpInforRelacionada.Location = new System.Drawing.Point(6, 85);
            this.grpInforRelacionada.Name = "grpInforRelacionada";
            this.grpInforRelacionada.Size = new System.Drawing.Size(770, 121);
            this.grpInforRelacionada.TabIndex = 0;
            this.grpInforRelacionada.TabStop = false;
            this.grpInforRelacionada.Text = "Informa��o relacionada";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemove.Location = new System.Drawing.Point(739, 90);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(24, 24);
            this.btnRemove.TabIndex = 9;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEdit.Location = new System.Drawing.Point(739, 61);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(24, 24);
            this.btnEdit.TabIndex = 8;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAdd.Location = new System.Drawing.Point(739, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(24, 24);
            this.btnAdd.TabIndex = 7;
            // 
            // lstVwInforRelacionada
            // 
            this.lstVwInforRelacionada.AllowDrop = true;
            this.lstVwInforRelacionada.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVwInforRelacionada.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clnTitulo,
            this.clnDensidade,
            this.clnSubDensidade,
            this.clnPonderacao});
            this.lstVwInforRelacionada.FullRowSelect = true;
            this.lstVwInforRelacionada.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstVwInforRelacionada.HideSelection = false;
            this.lstVwInforRelacionada.LabelWrap = false;
            this.lstVwInforRelacionada.Location = new System.Drawing.Point(8, 16);
            this.lstVwInforRelacionada.Name = "lstVwInforRelacionada";
            this.lstVwInforRelacionada.Size = new System.Drawing.Size(726, 97);
            this.lstVwInforRelacionada.TabIndex = 6;
            this.lstVwInforRelacionada.UseCompatibleStateImageBehavior = false;
            this.lstVwInforRelacionada.View = System.Windows.Forms.View.Details;
            // 
            // clnTitulo
            // 
            this.clnTitulo.Text = "T�tulo";
            this.clnTitulo.Width = 323;
            // 
            // clnDensidade
            // 
            this.clnDensidade.Text = "Tipo de produ��o";
            this.clnDensidade.Width = 108;
            // 
            // clnSubDensidade
            // 
            this.clnSubDensidade.Text = "Grau de densidade";
            this.clnSubDensidade.Width = 108;
            // 
            // clnPonderacao
            // 
            this.clnPonderacao.Text = "Pondera��o";
            this.clnPonderacao.Width = 70;
            // 
            // grpInformacaoAnalise
            // 
            this.grpInformacaoAnalise.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInformacaoAnalise.Controls.Add(this.lblInforAnaliseSubTipo);
            this.grpInformacaoAnalise.Controls.Add(this.lblInforAnaliseTipo);
            this.grpInformacaoAnalise.Controls.Add(this.cbInforAnaliseSubTipo);
            this.grpInformacaoAnalise.Controls.Add(this.cbInforAnaliseTipo);
            this.grpInformacaoAnalise.Location = new System.Drawing.Point(6, 19);
            this.grpInformacaoAnalise.Name = "grpInformacaoAnalise";
            this.grpInformacaoAnalise.Size = new System.Drawing.Size(770, 60);
            this.grpInformacaoAnalise.TabIndex = 0;
            this.grpInformacaoAnalise.TabStop = false;
            this.grpInformacaoAnalise.Text = "Informa��o em an�lise";
            // 
            // lblInforAnaliseSubTipo
            // 
            this.lblInforAnaliseSubTipo.Location = new System.Drawing.Point(344, 16);
            this.lblInforAnaliseSubTipo.Name = "lblInforAnaliseSubTipo";
            this.lblInforAnaliseSubTipo.Size = new System.Drawing.Size(104, 16);
            this.lblInforAnaliseSubTipo.TabIndex = 4;
            this.lblInforAnaliseSubTipo.Text = "Grau de densidade";
            // 
            // lblInforAnaliseTipo
            // 
            this.lblInforAnaliseTipo.Location = new System.Drawing.Point(8, 16);
            this.lblInforAnaliseTipo.Name = "lblInforAnaliseTipo";
            this.lblInforAnaliseTipo.Size = new System.Drawing.Size(96, 16);
            this.lblInforAnaliseTipo.TabIndex = 3;
            this.lblInforAnaliseTipo.Text = "Tipo de produ��o";
            // 
            // cbInforAnaliseSubTipo
            // 
            this.cbInforAnaliseSubTipo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbInforAnaliseSubTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInforAnaliseSubTipo.Location = new System.Drawing.Point(344, 32);
            this.cbInforAnaliseSubTipo.Name = "cbInforAnaliseSubTipo";
            this.cbInforAnaliseSubTipo.Size = new System.Drawing.Size(418, 21);
            this.cbInforAnaliseSubTipo.TabIndex = 5;
            // 
            // cbInforAnaliseTipo
            // 
            this.cbInforAnaliseTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInforAnaliseTipo.Location = new System.Drawing.Point(8, 32);
            this.cbInforAnaliseTipo.Name = "cbInforAnaliseTipo";
            this.cbInforAnaliseTipo.Size = new System.Drawing.Size(328, 21);
            this.cbInforAnaliseTipo.TabIndex = 4;
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.Panel1.Controls.Add(this.grpTipoAvaliacao);
            this.Panel1.Controls.Add(this.pnlAvaliacaoTabela);
            this.Panel1.Location = new System.Drawing.Point(14, 115);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(633, 56);
            this.Panel1.TabIndex = 17;
            this.Panel1.Visible = false;
            // 
            // grpTipoAvaliacao
            // 
            this.grpTipoAvaliacao.Controls.Add(this.chkModeloAvaliacao);
            this.grpTipoAvaliacao.Location = new System.Drawing.Point(7, 0);
            this.grpTipoAvaliacao.Name = "grpTipoAvaliacao";
            this.grpTipoAvaliacao.Size = new System.Drawing.Size(192, 52);
            this.grpTipoAvaliacao.TabIndex = 16;
            this.grpTipoAvaliacao.TabStop = false;
            this.grpTipoAvaliacao.Text = "Tipo de Avalia��o";
            // 
            // chkModeloAvaliacao
            // 
            this.chkModeloAvaliacao.Location = new System.Drawing.Point(36, 18);
            this.chkModeloAvaliacao.Name = "chkModeloAvaliacao";
            this.chkModeloAvaliacao.Size = new System.Drawing.Size(124, 24);
            this.chkModeloAvaliacao.TabIndex = 10;
            this.chkModeloAvaliacao.Text = "Avaliar por Tabela";
            // 
            // pnlAvaliacaoTabela
            // 
            this.pnlAvaliacaoTabela.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAvaliacaoTabela.Controls.Add(this.grpAvaliacaoTabela);
            this.pnlAvaliacaoTabela.Location = new System.Drawing.Point(206, 0);
            this.pnlAvaliacaoTabela.Name = "pnlAvaliacaoTabela";
            this.pnlAvaliacaoTabela.Size = new System.Drawing.Size(421, 52);
            this.pnlAvaliacaoTabela.TabIndex = 15;
            // 
            // grpAvaliacaoTabela
            // 
            this.grpAvaliacaoTabela.Controls.Add(this.cbModeloAvaliacao);
            this.grpAvaliacaoTabela.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAvaliacaoTabela.Location = new System.Drawing.Point(0, 0);
            this.grpAvaliacaoTabela.Name = "grpAvaliacaoTabela";
            this.grpAvaliacaoTabela.Size = new System.Drawing.Size(421, 52);
            this.grpAvaliacaoTabela.TabIndex = 13;
            this.grpAvaliacaoTabela.TabStop = false;
            this.grpAvaliacaoTabela.Text = "Avalia��o por Tabela";
            // 
            // cbModeloAvaliacao
            // 
            this.cbModeloAvaliacao.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbModeloAvaliacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModeloAvaliacao.Enabled = false;
            this.cbModeloAvaliacao.Location = new System.Drawing.Point(8, 20);
            this.cbModeloAvaliacao.Name = "cbModeloAvaliacao";
            this.cbModeloAvaliacao.Size = new System.Drawing.Size(407, 21);
            this.cbModeloAvaliacao.TabIndex = 11;
            // 
            // grpPertinencia
            // 
            this.grpPertinencia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPertinencia.Controls.Add(this.grpNivel);
            this.grpPertinencia.Controls.Add(this.grpPonderacao);
            this.grpPertinencia.Location = new System.Drawing.Point(6, 19);
            this.grpPertinencia.Name = "grpPertinencia";
            this.grpPertinencia.Size = new System.Drawing.Size(609, 70);
            this.grpPertinencia.TabIndex = 0;
            this.grpPertinencia.TabStop = false;
            this.grpPertinencia.Text = "Pertin�ncia";
            // 
            // grpNivel
            // 
            this.grpNivel.Controls.Add(this.cbNivel);
            this.grpNivel.Location = new System.Drawing.Point(8, 16);
            this.grpNivel.Name = "grpNivel";
            this.grpNivel.Size = new System.Drawing.Size(260, 47);
            this.grpNivel.TabIndex = 0;
            this.grpNivel.TabStop = false;
            this.grpNivel.Text = "N�vel";
            // 
            // cbNivel
            // 
            this.cbNivel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNivel.Location = new System.Drawing.Point(8, 16);
            this.cbNivel.Name = "cbNivel";
            this.cbNivel.Size = new System.Drawing.Size(244, 21);
            this.cbNivel.TabIndex = 1;
            // 
            // grpPonderacao
            // 
            this.grpPonderacao.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPonderacao.Controls.Add(this.txtPonderacao);
            this.grpPonderacao.Location = new System.Drawing.Point(272, 16);
            this.grpPonderacao.Name = "grpPonderacao";
            this.grpPonderacao.Size = new System.Drawing.Size(330, 47);
            this.grpPonderacao.TabIndex = 0;
            this.grpPonderacao.TabStop = false;
            this.grpPonderacao.Text = "Pondera��o";
            // 
            // txtPonderacao
            // 
            this.txtPonderacao.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPonderacao.Location = new System.Drawing.Point(8, 16);
            this.txtPonderacao.Name = "txtPonderacao";
            this.txtPonderacao.Size = new System.Drawing.Size(316, 20);
            this.txtPonderacao.TabIndex = 2;
            // 
            // grpFrequenciaUso
            // 
            this.grpFrequenciaUso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFrequenciaUso.Controls.Add(this.cbFrequenciaUso);
            this.grpFrequenciaUso.Location = new System.Drawing.Point(621, 19);
            this.grpFrequenciaUso.Name = "grpFrequenciaUso";
            this.grpFrequenciaUso.Size = new System.Drawing.Size(167, 70);
            this.grpFrequenciaUso.TabIndex = 0;
            this.grpFrequenciaUso.TabStop = false;
            this.grpFrequenciaUso.Text = "Frequ�ncia de uso";
            // 
            // cbFrequenciaUso
            // 
            this.cbFrequenciaUso.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFrequenciaUso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFrequenciaUso.Location = new System.Drawing.Point(8, 28);
            this.cbFrequenciaUso.Name = "cbFrequenciaUso";
            this.cbFrequenciaUso.Size = new System.Drawing.Size(151, 21);
            this.cbFrequenciaUso.TabIndex = 3;
            // 
            // grpEnquadramentoLegal
            // 
            this.grpEnquadramentoLegal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEnquadramentoLegal.Controls.Add(this.grpRef);
            this.grpEnquadramentoLegal.Controls.Add(this.grpDiploma);
            this.grpEnquadramentoLegal.Location = new System.Drawing.Point(6, 317);
            this.grpEnquadramentoLegal.Name = "grpEnquadramentoLegal";
            this.grpEnquadramentoLegal.Size = new System.Drawing.Size(782, 77);
            this.grpEnquadramentoLegal.TabIndex = 0;
            this.grpEnquadramentoLegal.TabStop = false;
            this.grpEnquadramentoLegal.Text = "Enquadramento legal";
            // 
            // grpRef
            // 
            this.grpRef.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRef.Controls.Add(this.txtRef);
            this.grpRef.Location = new System.Drawing.Point(579, 19);
            this.grpRef.Name = "grpRef";
            this.grpRef.Size = new System.Drawing.Size(197, 51);
            this.grpRef.TabIndex = 0;
            this.grpRef.TabStop = false;
            this.grpRef.Text = "Refer�ncia na tabela de sele��o";
            // 
            // txtRef
            // 
            this.txtRef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRef.Location = new System.Drawing.Point(6, 20);
            this.txtRef.Name = "txtRef";
            this.txtRef.Size = new System.Drawing.Size(185, 20);
            this.txtRef.TabIndex = 13;
            this.txtRef.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpDiploma
            // 
            this.grpDiploma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDiploma.Controls.Add(this.txtDiploma);
            this.grpDiploma.Controls.Add(this.btnAddDiploma);
            this.grpDiploma.Controls.Add(this.btnRemoveDiploma);
            this.grpDiploma.Location = new System.Drawing.Point(6, 19);
            this.grpDiploma.Name = "grpDiploma";
            this.grpDiploma.Size = new System.Drawing.Size(567, 51);
            this.grpDiploma.TabIndex = 0;
            this.grpDiploma.TabStop = false;
            this.grpDiploma.Text = "Diploma";
            // 
            // txtDiploma
            // 
            this.txtDiploma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiploma.Location = new System.Drawing.Point(6, 20);
            this.txtDiploma.Name = "txtDiploma";
            this.txtDiploma.ReadOnly = true;
            this.txtDiploma.Size = new System.Drawing.Size(494, 20);
            this.txtDiploma.TabIndex = 10;
            this.txtDiploma.TextChanged += new System.EventHandler(this.txtDiploma_TextChanged);
            // 
            // btnAddDiploma
            // 
            this.btnAddDiploma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDiploma.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddDiploma.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddDiploma.Location = new System.Drawing.Point(507, 16);
            this.btnAddDiploma.Name = "btnAddDiploma";
            this.btnAddDiploma.Size = new System.Drawing.Size(24, 24);
            this.btnAddDiploma.TabIndex = 11;
            // 
            // btnRemoveDiploma
            // 
            this.btnRemoveDiploma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveDiploma.Enabled = false;
            this.btnRemoveDiploma.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRemoveDiploma.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRemoveDiploma.Location = new System.Drawing.Point(537, 16);
            this.btnRemoveDiploma.Name = "btnRemoveDiploma";
            this.btnRemoveDiploma.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveDiploma.TabIndex = 12;
            // 
            // grpObservacoes
            // 
            this.grpObservacoes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpObservacoes.Controls.Add(this.txtObservacoes);
            this.grpObservacoes.Location = new System.Drawing.Point(3, 454);
            this.grpObservacoes.Name = "grpObservacoes";
            this.grpObservacoes.Size = new System.Drawing.Size(788, 134);
            this.grpObservacoes.TabIndex = 0;
            this.grpObservacoes.TabStop = false;
            this.grpObservacoes.Text = "Observa��es";
            // 
            // txtObservacoes
            // 
            this.txtObservacoes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservacoes.Location = new System.Drawing.Point(8, 16);
            this.txtObservacoes.MaxLength = 2147483646;
            this.txtObservacoes.Multiline = true;
            this.txtObservacoes.Name = "txtObservacoes";
            this.txtObservacoes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtObservacoes.Size = new System.Drawing.Size(772, 110);
            this.txtObservacoes.TabIndex = 18;
            // 
            // ControlAutoEliminacao1
            // 
            this.ControlAutoEliminacao1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlAutoEliminacao1.ContentsEnabled = true;
            this.ControlAutoEliminacao1.Location = new System.Drawing.Point(402, 400);
            this.ControlAutoEliminacao1.Name = "ControlAutoEliminacao1";
            this.ControlAutoEliminacao1.Size = new System.Drawing.Size(281, 48);
            this.ControlAutoEliminacao1.TabIndex = 16;
            // 
            // grpPrazoConservacao
            // 
            this.grpPrazoConservacao.Controls.Add(this.nudPrazoConservacao);
            this.grpPrazoConservacao.Controls.Add(this.lblAnos);
            this.grpPrazoConservacao.Location = new System.Drawing.Point(236, 400);
            this.grpPrazoConservacao.Name = "grpPrazoConservacao";
            this.grpPrazoConservacao.Size = new System.Drawing.Size(160, 48);
            this.grpPrazoConservacao.TabIndex = 0;
            this.grpPrazoConservacao.TabStop = false;
            this.grpPrazoConservacao.Text = "Prazo conserva��o";
            // 
            // nudPrazoConservacao
            // 
            this.nudPrazoConservacao.Location = new System.Drawing.Point(8, 18);
            this.nudPrazoConservacao.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudPrazoConservacao.Name = "nudPrazoConservacao";
            this.nudPrazoConservacao.Size = new System.Drawing.Size(103, 20);
            this.nudPrazoConservacao.TabIndex = 15;
            // 
            // lblAnos
            // 
            this.lblAnos.Location = new System.Drawing.Point(117, 20);
            this.lblAnos.Name = "lblAnos";
            this.lblAnos.Size = new System.Drawing.Size(30, 17);
            this.lblAnos.TabIndex = 0;
            this.lblAnos.Text = "Anos";
            // 
            // grpPublicacao
            // 
            this.grpPublicacao.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPublicacao.Controls.Add(this.chkPublicar);
            this.grpPublicacao.Location = new System.Drawing.Point(689, 400);
            this.grpPublicacao.Name = "grpPublicacao";
            this.grpPublicacao.Size = new System.Drawing.Size(99, 48);
            this.grpPublicacao.TabIndex = 0;
            this.grpPublicacao.TabStop = false;
            this.grpPublicacao.Text = "Publica��o";
            // 
            // chkPublicar
            // 
            this.chkPublicar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPublicar.Location = new System.Drawing.Point(6, 16);
            this.chkPublicar.Name = "chkPublicar";
            this.chkPublicar.Size = new System.Drawing.Size(90, 24);
            this.chkPublicar.TabIndex = 17;
            this.chkPublicar.Text = "Publicar";
            // 
            // grpDestinoFinal
            // 
            this.grpDestinoFinal.Controls.Add(this.cbDestinoFinal);
            this.grpDestinoFinal.Location = new System.Drawing.Point(6, 400);
            this.grpDestinoFinal.Name = "grpDestinoFinal";
            this.grpDestinoFinal.Size = new System.Drawing.Size(224, 48);
            this.grpDestinoFinal.TabIndex = 0;
            this.grpDestinoFinal.TabStop = false;
            this.grpDestinoFinal.Text = "Destino final";
            // 
            // cbDestinoFinal
            // 
            this.cbDestinoFinal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDestinoFinal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDestinoFinal.DropDownWidth = 260;
            this.cbDestinoFinal.ItemHeight = 13;
            this.cbDestinoFinal.Location = new System.Drawing.Point(8, 18);
            this.cbDestinoFinal.Name = "cbDestinoFinal";
            this.cbDestinoFinal.Size = new System.Drawing.Size(208, 21);
            this.cbDestinoFinal.TabIndex = 14;
            // 
            // PanelAvaliacaoSeleccaoEliminacao
            // 
            this.Controls.Add(this.GroupBox2);
            this.Name = "PanelAvaliacaoSeleccaoEliminacao";
            this.GroupBox2.ResumeLayout(false);
            this.grpDensidade.ResumeLayout(false);
            this.grpInforRelacionada.ResumeLayout(false);
            this.grpInformacaoAnalise.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.grpTipoAvaliacao.ResumeLayout(false);
            this.pnlAvaliacaoTabela.ResumeLayout(false);
            this.grpAvaliacaoTabela.ResumeLayout(false);
            this.grpPertinencia.ResumeLayout(false);
            this.grpNivel.ResumeLayout(false);
            this.grpPonderacao.ResumeLayout(false);
            this.grpPonderacao.PerformLayout();
            this.grpFrequenciaUso.ResumeLayout(false);
            this.grpEnquadramentoLegal.ResumeLayout(false);
            this.grpRef.ResumeLayout(false);
            this.grpDiploma.ResumeLayout(false);
            this.grpDiploma.PerformLayout();
            this.grpObservacoes.ResumeLayout(false);
            this.grpObservacoes.PerformLayout();
            this.grpPrazoConservacao.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudPrazoConservacao)).EndInit();
            this.grpPublicacao.ResumeLayout(false);
            this.grpDestinoFinal.ResumeLayout(false);
            this.ResumeLayout(false);

		}

	#endregion

		private void GetExtraResources()
		{
			btnAdd.Image = SharedResourcesOld.CurrentSharedResources.Adicionar;
			btnEdit.Image = SharedResourcesOld.CurrentSharedResources.Editar;
			btnRemove.Image = SharedResourcesOld.CurrentSharedResources.Apagar;
            btnAddDiploma.Image = SharedResourcesOld.CurrentSharedResources.Adicionar;
            btnRemoveDiploma.Image = SharedResourcesOld.CurrentSharedResources.Apagar;

			base.ParentChanged += PanelAvaliacaoSeleccaoEliminacao_ParentChanged;
		}

		// runs only once. sets tooltip as soon as it's parent appears
		private void PanelAvaliacaoSeleccaoEliminacao_ParentChanged(object sender, System.EventArgs e)
		{
			MultiPanel.CurrentToolTip.SetToolTip(btnAdd, SharedResourcesOld.CurrentSharedResources.AdicionarString);
			MultiPanel.CurrentToolTip.SetToolTip(btnEdit, SharedResourcesOld.CurrentSharedResources.EditarString);
			MultiPanel.CurrentToolTip.SetToolTip(btnRemove, SharedResourcesOld.CurrentSharedResources.ApagarString);
            MultiPanel.CurrentToolTip.SetToolTip(btnAddDiploma, SharedResourcesOld.CurrentSharedResources.DMCriarString);
            MultiPanel.CurrentToolTip.SetToolTip(btnRemoveDiploma, SharedResourcesOld.CurrentSharedResources.DMEliminarString);
			base.ParentChanged -= PanelAvaliacaoSeleccaoEliminacao_ParentChanged;
		}

		private GISADataset.FRDBaseRow CurrentFRDBase;
		private GISADataset.SFRDAvaliacaoRow CurrentSFRDAvaliacao;
		private string QueryFilter;

		private DataTable mPonderacoesTable;
		private DataTable PonderacoesTable
		{
			get
			{
				if (mPonderacoesTable == null)
				{
					mPonderacoesTable = new DataTable();
					mPonderacoesTable.Columns.Add("ID", typeof(decimal));
					mPonderacoesTable.Columns.Add("Designacao", typeof(string));

					mPonderacoesTable.Rows.Add(new object[] {-1, ""});
					mPonderacoesTable.Rows.Add(new object[] {0, "0 (Zero)"});
					mPonderacoesTable.Rows.Add(new object[] {1, "1 (Um)"});
				}
				return mPonderacoesTable;
			}
		}

		private bool isInactiveEstruturalPanel = false;

		public override void LoadData(DataRow CurrentDataRow, IDbConnection conn)
		{
			IsLoaded = false; //TODO: FIXME: IsLoaded e isInactivePanel devem estar a servir o mesmo prop�sito, verificar.
			isInactiveEstruturalPanel = false;
			CurrentFRDBase = (GISADataset.FRDBaseRow)CurrentDataRow;

			GISADataset.RelacaoHierarquicaRow rhRow = null;
			rhRow = TipoNivelRelacionado.GetPrimeiraRelacaoEncontrada(CurrentFRDBase.NivelRow);

			// Garantir que este painel s� est� activo quando tal fizer sentido (s�rie, sub-s�rie e documentos soltos)
			if ( !(rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR) && ! (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR) && ! (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL))
			{
				grpNivel.Enabled = false;
				grpPonderacao.Enabled = false;
				grpInformacaoAnalise.Enabled = false;
				grpInforRelacionada.Enabled = false;
				grpFrequenciaUso.Enabled = false;
                grpEnquadramentoLegal.Enabled = false;
                grpObservacoes.Enabled = false;

				// se se tratar de um documento que nao constitui s�rie o painel ficar� inactivo
				if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D)
				{
					chkModeloAvaliacao.Enabled = true;
					grpDestinoFinal.Enabled = true;
					grpPrazoConservacao.Enabled = true;
					grpPublicacao.Enabled = true;
                    grpObservacoes.Enabled = true;
				}
				else
				{
					isInactiveEstruturalPanel = true;
					chkModeloAvaliacao.Enabled = false;
					grpDestinoFinal.Enabled = false;
					grpPrazoConservacao.Enabled = false;
					
                    if (CurrentFRDBase.NivelRow.TipoNivelRow.ID == TipoNivel.ESTRUTURAL || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
						grpPublicacao.Enabled = true;
					else
						grpPublicacao.Enabled = false;
				}
			}
			else
			{
				chkModeloAvaliacao.Enabled = true;
				grpNivel.Enabled = true;
				grpPonderacao.Enabled = true;
				grpInformacaoAnalise.Enabled = true;
				grpInforRelacionada.Enabled = true;
				grpFrequenciaUso.Enabled = true;
                grpEnquadramentoLegal.Enabled = true;
				
                if (GisaDataSetHelper.UsingGestaoIntegrada())
					grpDestinoFinal.Enabled = true;
				else
					grpDestinoFinal.Enabled = false;

				grpPrazoConservacao.Enabled = true;
				grpPublicacao.Enabled = true;
				grpObservacoes.Enabled = true;
				ControlAutoEliminacao1.ContentsEnabled = false;
			}

            FRDRule.Current.LoadPanelAvaliacaoSeleccaoEliminacaoData(GisaDataSetHelper.GetInstance(), CurrentFRDBase.ID, CurrentFRDBase.IDNivel, PermissoesHelper.GrpAcessoPublicados.ID, conn);

            if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.DOCUMENTAL)
            {
                var frdRow = rhRow.NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0];
                FRDRule.Current.LoadPanelAvaliacaoSeleccaoEliminacaoData(GisaDataSetHelper.GetInstance(), frdRow.ID, frdRow.IDNivel, PermissoesHelper.GrpAcessoPublicados.ID, conn);
            }

			IsLoaded = true;
		}

		private long originalModeloAvaliacao = -1;
		public override void ModelToView()
		{
			IsPopulated = false;
			GISADataset.SFRDAvaliacaoRow[] avaliacaoRows = null;
			avaliacaoRows = CurrentFRDBase.GetSFRDAvaliacaoRows();
			if (avaliacaoRows.Length != 0)
			{
				CurrentSFRDAvaliacao = CurrentFRDBase.GetSFRDAvaliacaoRows()[0];
			}
			else
			{
				// Criar um SFRDAvalia��o caso n�o exista ainda
				CurrentSFRDAvaliacao = GisaDataSetHelper.GetInstance().SFRDAvaliacao. NewSFRDAvaliacaoRow();
				CurrentSFRDAvaliacao.FRDBaseRow = CurrentFRDBase;
				CurrentSFRDAvaliacao.IDPertinencia = 1;
				CurrentSFRDAvaliacao.IDDensidade = 1;
				CurrentSFRDAvaliacao.IDSubdensidade = 1;
				CurrentSFRDAvaliacao.Publicar = false;
                CurrentSFRDAvaliacao.AvaliacaoTabela = false;
				GisaDataSetHelper.GetInstance().SFRDAvaliacao.AddSFRDAvaliacaoRow(CurrentSFRDAvaliacao);
			}

			cbNivel.DataSource = GisaDataSetHelper.GetInstance().TipoPertinencia;
			cbNivel.DisplayMember = "Designacao";
			try
			{
				cbNivel.ValueMember = "ID";
			}
			catch (Exception Ex)
			{
				MessageBox.Show(Ex.Message);
			}

			cbInforAnaliseTipo.DataSource = GisaDataSetHelper.GetInstance().TipoDensidade;
			cbInforAnaliseTipo.DisplayMember = "Designacao";
			cbInforAnaliseTipo.ValueMember = "ID";

            cbInforAnaliseSubTipo.DataSource = GisaDataSetHelper.GetInstance().TipoSubDensidade;
			cbInforAnaliseSubTipo.DisplayMember = "Designacao";
			cbInforAnaliseSubTipo.ValueMember = "ID";

			cbFrequenciaUso.DataSource = PonderacoesTable;
			cbFrequenciaUso.DisplayMember = "Designacao";
			cbFrequenciaUso.ValueMember = "ID";

			DataTable dt2 = new DataTable();
			dt2.Columns.Add("ID", typeof(int));
			dt2.Columns.Add("Designacao", typeof(string));
			dt2.Rows.Add(new object[] {-1, ""});
			dt2.Rows.Add(new object[] {1, "Conserva��o"});
			dt2.Rows.Add(new object[] {0, "Elimina��o"});
			cbDestinoFinal.DisplayMember = "Designacao";
			cbDestinoFinal.ValueMember = "ID";
			cbDestinoFinal.DataSource = dt2;

            PopulateDiploma();
            if (CurrentSFRDAvaliacao.IsRefTabelaAvaliacaoNull())
                txtRef.Text = string.Empty;
            else
                txtRef.Text = CurrentSFRDAvaliacao.RefTabelaAvaliacao.ToString();

			GISADataset.RelacaoHierarquicaRow rhRow = null;
			rhRow = TipoNivelRelacionado.GetPrimeiraRelacaoEncontrada(CurrentFRDBase.NivelRow);

			// definir estados dos controlos mediante o tipo de avalia��o selecionado
			if (! CurrentSFRDAvaliacao.AvaliacaoTabela)
			{
				cbModeloAvaliacao.SelectedValue = -1;
				chkModeloAvaliacao.Checked = false;
				EnableChangeAvaliacaoControls(true);

                //// se o contexto selecionado for um documento que constitua uma s�rie, s� � permitido avali�-lo segundo
                //// um modelo de avalia��o caso a sua s�rie (ou sub-s�rie) esteja igualmente avaliada dessa forma
                //if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.DOCUMENTAL)
                //{
                //    GISADataset.SFRDAvaliacaoRow[] parentSFRDAvaliacaoRow = CurrentFRDBase.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0].GetSFRDAvaliacaoRows();

                //    if (parentSFRDAvaliacaoRow.Length > 0 && parentSFRDAvaliacaoRow[0].AvaliacaoTabela)
                //        chkModeloAvaliacao.Enabled = true;
                //    else
                //    {
                //        // se o caso descrito a cima n�o se verificar impede-se que o documento possa ser avaliado segundo um 
                //        // modelo de avalia��o
                //        chkModeloAvaliacao.Enabled = false;
                //    }
                //}
			}
			else
			{
				chkModeloAvaliacao.Checked = true;
				EnableChangeAvaliacaoControls(false);
			}

			// popular modelos de avalia��o
			ArrayList modelosAvaliacaoData = new ArrayList();
			if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.DOCUMENTAL)
			{
                //// o contexto selecionado � um documento que constitui uma s�rie

                //GISADataset.SFRDAvaliacaoRow[] parentSFRDAvaliacaoRow = CurrentFRDBase.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0].GetSFRDAvaliacaoRows();

                //if (parentSFRDAvaliacaoRow.Length == 0 || (! CurrentSFRDAvaliacao.AvaliacaoTabela && (! (parentSFRDAvaliacaoRow[0].AvaliacaoTabela) || (parentSFRDAvaliacaoRow[0].AvaliacaoTabela && parentSFRDAvaliacaoRow[0].IsIDModeloAvaliacaoNull()))))
                //{
                //    // nem o documento nem a sua s�rie (ou sub-s�rie) est�o avaliados por tabelas de avalia��o (ou a s�rie ou sub-s�rie 
                //    // est�o por avaliar mas no modo de avalia��o por tabelas); neste caso n�o � necess�rio adicionar 
                //    // qualquer elemento � cbModeloAvaliacao
                //}
                //else if (! CurrentSFRDAvaliacao.AvaliacaoTabela && parentSFRDAvaliacaoRow[0].AvaliacaoTabela && ! (parentSFRDAvaliacaoRow[0].IsIDModeloAvaliacaoNull()))
                //{
                //    // o documento n�o est� avaliado segundo uma tabela de avalia��o mas a sua s�rie (ou sub-s�rie) est�

                //    modelosAvaliacaoData.Add(parentSFRDAvaliacaoRow[0].ModelosAvaliacaoRow);
                //    cbModeloAvaliacao.DataSource = modelosAvaliacaoData;
                //    cbModeloAvaliacao.DisplayMember = "Designacao";
                //    cbModeloAvaliacao.ValueMember = "ID";
                //}
                //else if (CurrentSFRDAvaliacao.AvaliacaoTabela && (! (parentSFRDAvaliacaoRow[0].AvaliacaoTabela) || (parentSFRDAvaliacaoRow[0].AvaliacaoTabela && parentSFRDAvaliacaoRow[0].IsIDModeloAvaliacaoNull())))
                //{
                //    if (CurrentSFRDAvaliacao.ModelosAvaliacaoRow == null)
                //    {
                //        var ma = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.NewRow());
                //        ma.ID = -1;
                //        ma.Designacao = "";
                //        modelosAvaliacaoData.Add(ma);
                //        CurrentSFRDAvaliacao.ModelosAvaliacaoRow = ma;
                //    }
                //    else
                //        modelosAvaliacaoData.Add(CurrentSFRDAvaliacao.ModelosAvaliacaoRow);

                //    originalModeloAvaliacao = CurrentSFRDAvaliacao.ModelosAvaliacaoRow.ID;
                //    cbModeloAvaliacao.SelectedValue = originalModeloAvaliacao;
                //    cbModeloAvaliacao.DataSource = modelosAvaliacaoData;
                //    cbModeloAvaliacao.DisplayMember = "Designacao";
                //    cbModeloAvaliacao.ValueMember = "ID";
                //}
                //else if (CurrentSFRDAvaliacao.AvaliacaoTabela && parentSFRDAvaliacaoRow[0].AvaliacaoTabela && ! (parentSFRDAvaliacaoRow[0].IsIDModeloAvaliacaoNull()))
                //{
                //    // documento avaliado segundo uma tabela de avalia��o mais antiga que aquela utilizada na sua s�rie (ou
                //    // sub-s�rie)
                //    if (CurrentSFRDAvaliacao.ModelosAvaliacaoRow == null)
                //    {
                //        var ma = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.NewRow());
                //        ma.ID = -1;
                //        ma.Designacao = "";
                //        modelosAvaliacaoData.Add(ma);
                //        CurrentSFRDAvaliacao.ModelosAvaliacaoRow = ma;
                //    }
                //    else
                //        modelosAvaliacaoData.Add(CurrentSFRDAvaliacao.ModelosAvaliacaoRow);

                //    if ((CurrentSFRDAvaliacao.ModelosAvaliacaoRow == null) || (CurrentSFRDAvaliacao.ModelosAvaliacaoRow.ID != parentSFRDAvaliacaoRow[0].ModelosAvaliacaoRow.ID))
                //    {
                //        modelosAvaliacaoData.Add(parentSFRDAvaliacaoRow[0].ModelosAvaliacaoRow);
                //        originalModeloAvaliacao = parentSFRDAvaliacaoRow[0].ModelosAvaliacaoRow.ID;
                //    }
                //    cbModeloAvaliacao.Enabled = true;
                //    cbDestinoFinal.Enabled = false;
                //    cbModeloAvaliacao.SelectedValue = originalModeloAvaliacao;
                //    cbModeloAvaliacao.DataSource = modelosAvaliacaoData;
                //    cbModeloAvaliacao.DisplayMember = "Designacao";
                //    cbModeloAvaliacao.ValueMember = "ID";
                //}
                //else
                //    Debug.Assert(false, "Situa��o n�o prevista");
			}
			else if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
			{
				// n�o � necess�rio adicionar qualquer elemento � combobox devido ao facto de os sub-documentos n�o 
				// serem alvo de avalia��o individual
			}
			else
			{
				// nos casos restantes (s�ries, sub-s�ries e documentos soltos) � necess�rio adicionar todos os elementos
				// dispon�veis
				GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.NewRow());
				modAvRow.ID = -1;
				modAvRow.Designacao = "";
				modelosAvaliacaoData.Add(modAvRow);

				long lstModelosID = GetMostRecentList();
				if (lstModelosID > 0)
				{
					//'Limpar a id -1
					//modelosAvaliacaoData.Clear()
					modelosAvaliacaoData.AddRange(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("IDListaModelosAvaliacao=" + lstModelosID.ToString()));
				}
				cbModeloAvaliacao.DataSource = modelosAvaliacaoData;
				cbModeloAvaliacao.DisplayMember = "Designacao";
				cbModeloAvaliacao.ValueMember = "ID";

				if (CurrentSFRDAvaliacao.IsIDModeloAvaliacaoNull())
					originalModeloAvaliacao = -1;
				else
					originalModeloAvaliacao = CurrentSFRDAvaliacao.IDModeloAvaliacao;

				cbModeloAvaliacao.SelectedValue = originalModeloAvaliacao;
			}

			chkPublicar.Checked = CurrentSFRDAvaliacao.Publicar;

			if (! CurrentSFRDAvaliacao.IsObservacoesNull())
				txtObservacoes.Text = CurrentSFRDAvaliacao.Observacoes;
			else
				txtObservacoes.Text = string.Empty;

			UpdateLstVwInforRelacionadaButtonsState();
			UpdatePanel();

			AddHandlersAvaliacao();

			IsPopulated = true;
		}

		public override void ViewToModel()
		{
			if (CurrentFRDBase == null || CurrentFRDBase.RowState == DataRowState.Detached || ! IsLoaded)
				return;

			if (! chkModeloAvaliacao.Checked)
			{
                CurrentSFRDAvaliacao.IDPertinencia = (long)cbNivel.SelectedValue;

				int densidadeValue = System.Convert.ToInt32(cbInforAnaliseTipo.SelectedValue);
				if (! CurrentSFRDAvaliacao.IsIDDensidadeNull() )
				{
					if (cbInforAnaliseTipo.SelectedValue != null)
					{
						if (CurrentSFRDAvaliacao.IDDensidade != densidadeValue)
							CurrentSFRDAvaliacao.IDDensidade = densidadeValue;
						else
						{
							// valor nao modificado. nao intervir
						}
					}
					else
						CurrentSFRDAvaliacao["IDDensidade"] = DBNull.Value;
				}
				else
				{
					if (cbInforAnaliseTipo.SelectedValue != null)
						CurrentSFRDAvaliacao.IDDensidade = densidadeValue;
					else
					{
						// valor nao modificado. nao intervir
					}
				}

				int subDensidadeValue = System.Convert.ToInt32(cbInforAnaliseSubTipo.SelectedValue);
				if (! CurrentSFRDAvaliacao.IsIDSubdensidadeNull())
				{
					if (cbInforAnaliseSubTipo.SelectedValue != null)
					{
						if (CurrentSFRDAvaliacao.IDSubdensidade != subDensidadeValue)
							CurrentSFRDAvaliacao.IDSubdensidade = subDensidadeValue;
						else
						{
							// valor nao modificado. nao intervir
						}
					}
					else
						CurrentSFRDAvaliacao["IDSubdensidade"] = DBNull.Value;
				}
				else
				{
					if (cbInforAnaliseSubTipo.SelectedValue != null)
						CurrentSFRDAvaliacao.IDSubdensidade = subDensidadeValue;
					else
					{
						// valor nao modificado. nao intervir
					}
				}

				int freqValue = System.Convert.ToInt32(cbFrequenciaUso.SelectedValue);
				if (! CurrentSFRDAvaliacao.IsFrequenciaNull())
				{
					if (freqValue != -1)
					{
						if (CurrentSFRDAvaliacao.Frequencia != freqValue)
							CurrentSFRDAvaliacao.Frequencia = freqValue;
						else
						{
							// valor nao modificado. nao intervir
						}
					}
					else
						CurrentSFRDAvaliacao["Frequencia"] = DBNull.Value;
				}
				else
				{
					if (freqValue != -1)
						CurrentSFRDAvaliacao.Frequencia = freqValue;
					else
					{
						// valor nao modificado. nao intervir
					}
				}
			}
			else
			{
				CurrentSFRDAvaliacao["IDPertinencia"] = DBNull.Value;
				CurrentSFRDAvaliacao["IDDensidade"] = DBNull.Value;
				CurrentSFRDAvaliacao["IDSubdensidade"] = DBNull.Value;
				CurrentSFRDAvaliacao["Frequencia"] = DBNull.Value;

				//apagar avalia��es relaccionadas caso o m�todo de avalia��o seja por tabela
				foreach (ListViewItem item in lstVwInforRelacionada.Items)
				{
					((DataRow)item.Tag).Delete();
					item.Remove();
				}

			}

			int destinoNewValue = 0;
			destinoNewValue = System.Convert.ToInt32(cbDestinoFinal.SelectedValue);
			if (destinoNewValue == -1)
			{
				if (! CurrentSFRDAvaliacao.IsPreservarNull())
					CurrentSFRDAvaliacao["Preservar"] = DBNull.Value;
			}
			else
			{
				if (CurrentSFRDAvaliacao.IsPreservarNull() || (System.Convert.ToBoolean(destinoNewValue) ^ CurrentSFRDAvaliacao.Preservar))
					CurrentSFRDAvaliacao.Preservar = System.Convert.ToBoolean(destinoNewValue);
			}

			short prazoConservacao = 0;
			try
			{
				prazoConservacao = CurrentSFRDAvaliacao.PrazoConservacao;
			}
			catch (StrongTypingException)
			{
				prazoConservacao = 0;
			}
			if (prazoConservacao != System.Convert.ToInt16(nudPrazoConservacao.Text))
				CurrentSFRDAvaliacao.PrazoConservacao = System.Convert.ToInt16(nudPrazoConservacao.Text);

			if (! (txtObservacoes.Text.Equals(GisaDataSetHelper.GetDBNullableText(CurrentSFRDAvaliacao, "Observacoes"))))
				CurrentSFRDAvaliacao.Observacoes = txtObservacoes.Text;

            if (chkPublicar.Checked ^ CurrentSFRDAvaliacao.Publicar)
            {
                CurrentSFRDAvaliacao.Publicar = chkPublicar.Checked;
                PermissoesHelper.ChangeDocPermissionPublicados(CurrentFRDBase.NivelRow.ID, chkPublicar.Checked);
            }

			long autoNewValue = long.MinValue;
			if (ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue != null)
				autoNewValue = (long)ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue;
			
            if (autoNewValue == long.MinValue)
			{
				if (! CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull() )
					CurrentSFRDAvaliacao["IDAutoEliminacao"] = DBNull.Value;
			}
			else
			{
				if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull() || (autoNewValue != CurrentSFRDAvaliacao.IDAutoEliminacao))
					CurrentSFRDAvaliacao.IDAutoEliminacao = autoNewValue;
			}

			CurrentSFRDAvaliacao.AvaliacaoTabela = chkModeloAvaliacao.Checked;

			long modeloNewValue = long.MinValue;
			if (cbModeloAvaliacao.SelectedValue != null)
				modeloNewValue = (long)cbModeloAvaliacao.SelectedValue;

			if (modeloNewValue == long.MinValue)
			{
				if (! (CurrentSFRDAvaliacao.IsIDModeloAvaliacaoNull()))
					CurrentSFRDAvaliacao["IDModeloAvaliacao"] = DBNull.Value;
			}
			else
			{
				if (CurrentSFRDAvaliacao.IsIDModeloAvaliacaoNull() || (modeloNewValue != CurrentSFRDAvaliacao.IDModeloAvaliacao))
				{
					if (modeloNewValue > 0)
						CurrentSFRDAvaliacao.IDModeloAvaliacao = modeloNewValue;
					else
						CurrentSFRDAvaliacao["IDModeloAvaliacao"] = DBNull.Value;
				}
			}

            if (txtRef.Text.Length == 0)
                CurrentSFRDAvaliacao["RefTabelaAvaliacao"] = DBNull.Value;
            else
                CurrentSFRDAvaliacao.RefTabelaAvaliacao = System.Convert.ToInt32(txtRef.Text);
		}

		public override void Deactivate()
		{
			RemoveHandlersAvaliacao();

			// if seguinte serve exclusivamente para debug
			if (CurrentFRDBase != null && CurrentFRDBase.RowState == DataRowState.Detached)
				Debug.WriteLine("OCORREU SITUA��O DE ERRO NO PAINEL AVALIACAO. EM PRINCIPIO NINGUEM DEU POR ELE.");

			if (CurrentFRDBase == null || CurrentFRDBase.RowState == DataRowState.Detached)
				return;

            GUIHelper.GUIHelper.clearField(cbNivel);
            GUIHelper.GUIHelper.clearField(cbInforAnaliseTipo);
            GUIHelper.GUIHelper.clearField(cbInforAnaliseSubTipo);
            GUIHelper.GUIHelper.clearField(lstVwInforRelacionada);
            GUIHelper.GUIHelper.clearField(chkPublicar);
            GUIHelper.GUIHelper.clearField(chkModeloAvaliacao);
			cbModeloAvaliacao.DataSource = null;
			cbModeloAvaliacao.Items.Clear();
            GUIHelper.GUIHelper.clearField(nudPrazoConservacao);
            GUIHelper.GUIHelper.clearField(txtDiploma);
            txtRef.Clear();
		}

		private void AddHandlersAvaliacao()
		{
			cbDestinoFinal.SelectedIndexChanged += cbDestinoFinal_SelectedIndexChanged;
			cbModeloAvaliacao.SelectedIndexChanged += cbModeloAvaliacao_SelectedIndexChanged;
			chkModeloAvaliacao.CheckStateChanged += chkModeloAvaliacao_CheckedChanged;
		}

		private void RemoveHandlersAvaliacao()
		{
			cbDestinoFinal.SelectedIndexChanged -= cbDestinoFinal_SelectedIndexChanged;
			cbModeloAvaliacao.SelectedIndexChanged -= cbModeloAvaliacao_SelectedIndexChanged;
			chkModeloAvaliacao.CheckStateChanged -= chkModeloAvaliacao_CheckedChanged;
		}

		private long GetMostRecentList()
		{
			GISADataset.ListaModelosAvaliacaoRow lstModAvRow = null;
			if (GisaDataSetHelper.GetInstance().ListaModelosAvaliacao.Count > 0)
			{
				lstModAvRow = (GISADataset.ListaModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ListaModelosAvaliacao.Select("", "DataInicio DESC")[0]);
				return lstModAvRow.ID;
			}
			return -1;
		}

		private void cbInforAnaliseTipo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				cbInforAnaliseSubTipo.DataSource = GisaDataSetHelper.GetInstance().TipoSubDensidade. Select("IDTipoDensidade=" + cbInforAnaliseTipo.SelectedValue.ToString());
			}
			catch (Exception)
			{
				cbInforAnaliseSubTipo.DataSource = GisaDataSetHelper.GetInstance().TipoSubDensidade. Select("IDTipoDensidade Is Null");
			}
		}

		private void cbNivel_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cbNivel.SelectedItem != null)
				txtPonderacao.Text = ((GISADataset.TipoPertinenciaRow)(((DataRowView)cbNivel.SelectedItem).Row)).Ponderacao;
		}

	    #region  Informa��o Relacionada 
		private void btnAdd_Click(object sender, EventArgs e)
		{
			AddInformacaoRelacionada();
		}

		private void AddInformacaoRelacionada()
		{
			FormComparacao f = new FormComparacao();
			f.Text = f.Text + " - Adi��o";
			f.LoadData(false);
            
			if (f.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				GISADataset.SFRDAvaliacaoRelRow newRow = null;
				newRow = GisaDataSetHelper.GetInstance().SFRDAvaliacaoRel.NewSFRDAvaliacaoRelRow();
				newRow.IDFRDBase = CurrentFRDBase.ID;

				try
				{
					newRow.IDNivel = f.RelacaoHierarquica.ID;
				}
				catch (Exception)
				{
					MessageBox.Show("N�vel n�o especificado (!)");
					Trace.WriteLine("N�vel n�o especificado (!)");
					newRow["IDNivel"] = DBNull.Value;
				}

				if (newRow.SFRDAvaliacaoRow.FRDBaseRow.IDNivel == newRow.IDNivel)
				{
					MessageBox.Show("N�o s�o permitidas as rela��es de um n�vel consigo pr�prio.", f.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				try
				{
					newRow.Densidade = f.Densidade.ID;
				}
				catch (Exception)
				{
					MessageBox.Show("Densidade n�o especificada (!)");
					Trace.WriteLine("Densidade n�o especificada (!)");
					newRow["Densidade"] = DBNull.Value;
				}
				try
				{
					newRow.SubDensidade = f.SubDensidade.ID;
				}
				catch (Exception)
				{
					MessageBox.Show("Subdensidade n�o especificada (!)");
					Trace.WriteLine("Subdensidade n�o especificada (!)");
				}
				newRow.Ponderacao = f.Ponderacao;
				try
				{
					GisaDataSetHelper.GetInstance().SFRDAvaliacaoRel.AddSFRDAvaliacaoRelRow(newRow);
				}
				catch (ConstraintException)
				{
					MessageBox.Show("N�o � permitida a exist�ncia de rela��es duplicadas", f.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				AddRowToLstVwInforRelacionada(newRow);
			}
		}

		private void AddRowToLstVwInforRelacionada(GISADataset.SFRDAvaliacaoRelRow row)
		{

			string titulo = Nivel.GetDesignacao(row.NivelRow);
			ListViewItem item = null;
			item = lstVwInforRelacionada.Items.Add(titulo);

			item.SubItems.Add(row.TipoDensidadeRow.Designacao);
			item.SubItems.Add(row.TipoSubDensidadeRow.Designacao);
			item.SubItems.Add(System.Convert.ToString(row.Ponderacao));
			item.Tag = row;
		}

		public void btnEdit_Click(object sender, System.EventArgs e)
		{

			GISADataset.SFRDAvaliacaoRelRow row = null;
			ListViewItem item = null;
			try
			{
				//Get the selected row
				item = lstVwInforRelacionada.SelectedItems[0];
				row = (GISADataset.SFRDAvaliacaoRelRow)item.Tag;
			}
			catch (ArgumentOutOfRangeException)
			{
				//If there is no item selected then no item is edited
				return;
			}
			FormComparacao f = new FormComparacao();
			f.LoadData(true);

			f.Text = f.Text + " - Edi��o";
			f.Densidade = row.TipoDensidadeRow;
			f.SubDensidade = row.TipoSubDensidadeRow;
			f.Ponderacao = row.Ponderacao;
			// Utilizar a primeira rela��o encontrada uma vez que na realidade ser�o adicionadas todas as rela��es deste n�vel.
			IDbConnection conn = GisaDataSetHelper.GetConnection();
			try
			{
				conn.Open();
				DBAbstractDataLayer.DataAccessRules.NivelRule.Current.LoadNivelParents(row.NivelRow.ID, GisaDataSetHelper.GetInstance(), conn);
			}
			finally
			{
				conn.Close();
			}

			f.RelacaoHierarquica = row.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0];
            f.cnList.trVwLocalizacao.ExpandAll();
			if (f.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				//Update information in the data row

				try
				{
					row.Densidade = f.Densidade.ID;
				}
				catch (Exception)
				{
					MessageBox.Show("FIXME: densidade n�o especificada (!)");
				}
				try
				{
					row.SubDensidade = f.SubDensidade.ID;
				}
				catch (Exception)
				{
					MessageBox.Show("FIXME: subdensidade n�o especificada (!)");
				}
				row.Ponderacao = f.Ponderacao;

				//Update information in the list
				string titulo = row.NivelRow.GetNivelDesignadoRows()[0].Designacao;
				item.SubItems[0].Text = titulo;
				item.SubItems[1].Text = row.TipoDensidadeRow.Designacao;
				item.SubItems[2].Text = row.TipoSubDensidadeRow.Designacao;
				item.SubItems[3].Text = System.Convert.ToString(row.Ponderacao);
			}
		}

		public void btnRemove_Click(object sender, System.EventArgs e)
		{
            GUIHelper.GUIHelper.deleteSelectedLstVwItems(lstVwInforRelacionada);
		}

		private void lstVwInforRelacionada_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyValue == Convert.ToInt32(Keys.Delete))
			{
                GUIHelper.GUIHelper.deleteSelectedLstVwItems((ListView)sender);
			}
		}

		private void LstVwInforRelacionada_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateLstVwInforRelacionadaButtonsState();
		}

		private void UpdateLstVwInforRelacionadaButtonsState()
		{
			switch (lstVwInforRelacionada.SelectedItems.Count)
			{
				case 0:
					btnRemove.Enabled = false;
					btnEdit.Enabled = false;
					break;
				case 1:
					btnRemove.Enabled = true;
					btnEdit.Enabled = true;
					break;
				default:
					btnRemove.Enabled = true;
					btnEdit.Enabled = false;
					break;
			}
		}
	#endregion

        #region Enquadramento legal
        private void btnAddDiploma_Click(object sender, System.EventArgs e)
        {
            FormPickDiplomaModelo f = new FormPickDiplomaModelo();
            f.caList.AllowedNoticiaAut(TipoNoticiaAut.Diploma); //, TipoNoticiaAut.Modelo
            f.Text = "Diplomas";
            f.LoadData();

            if (f.caList.Items.Count > 0)
                f.caList.SelectItem(f.caList.Items[0]);

            if (f.ShowDialog(this) == DialogResult.OK)
                CreateNewRelation(((GISADataset.ControloAutDicionarioRow)(f.caList.SelectedItems[0].Tag)).ControloAutRow);

            PopulateDiploma();
        }

        private void PopulateDiploma()
        {
            foreach (GISADataset.IndexFRDCARow indexRow in (GISADataset.IndexFRDCARow[])GisaDataSetHelper.GetInstance().IndexFRDCA.Select(string.Format("IDFRDBase={0} AND Selector={1}", CurrentFRDBase.ID, 1)))
            {
                if (indexRow.ControloAutRow.IDTipoNoticiaAut == (long)TipoNoticiaAut.Diploma)
                {
                    txtDiploma.Text = ((GISADataset.ControloAutDicionarioRow)(GisaDataSetHelper.GetInstance().ControloAutDicionario.Select(string.Format("IDControloAut={0} AND IDTipoControloAutForma={1}", indexRow.IDControloAut, 1))[0])).DicionarioRow.Termo;
                    txtDiploma.Tag = indexRow;
                    return; //s� existe um diploma associado
                }
            }
            txtDiploma.Text = string.Empty;
        }

        private void CreateNewRelation(GISADataset.ControloAutRow row)
        {
            if (row == null || PresentInIndex(row))
                return;

            var IndexFRDCARow = GisaDataSetHelper.GetInstance().IndexFRDCA.NewIndexFRDCARow();
            IndexFRDCARow.FRDBaseRow = CurrentFRDBase;
            IndexFRDCARow.ControloAutRow = row;
            IndexFRDCARow.Selector = 1;
            GisaDataSetHelper.GetInstance().IndexFRDCA.AddIndexFRDCARow(IndexFRDCARow);
        }

        private void RemoveDiploma()
        {
            if (txtDiploma.Tag != null)
                ((DataRow)txtDiploma.Tag).Delete();
            txtDiploma.Tag = null;
        }

        private bool PresentInIndex(GISADataset.ControloAutRow ControloAutRow)
        {
            var row = GisaDataSetHelper.GetInstance().IndexFRDCA.Cast<GISADataset.IndexFRDCARow>()
                .SingleOrDefault(r => r.RowState != DataRowState.Deleted && r.IDFRDBase == CurrentFRDBase.ID && r.IDControloAut == ControloAutRow.ID);
            if (row != null)
            {
                if (row["Selector"] == DBNull.Value)
                    MessageBox.Show("N�o � possivel adicionar o diploma porque j� est� associado a esta unidade informacional via 3.1. �mbito e Conte�do", "Adicionar diploma legal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            row = GisaDataSetHelper.GetInstance().IndexFRDCA.Cast<GISADataset.IndexFRDCARow>()
                .SingleOrDefault(r => r.RowState == DataRowState.Deleted && (long)r["IDFRDBase", DataRowVersion.Original] == CurrentFRDBase.ID && (long)r["IDControloAut", DataRowVersion.Original] == ControloAutRow.ID);
            if (row != null) { RemoveDiploma();  row.RejectChanges(); txtDiploma.Tag = row; return true; }

            RemoveDiploma();

            return false;
        }

        public void btnRemoveDiploma_Click(object sender, EventArgs e)
        {
            RemoveDiploma();
            PopulateDiploma();
        }

        private void txtDiploma_TextChanged(object sender, EventArgs e)
        {
            btnRemoveDiploma.Enabled = txtDiploma.Text.Length > 0;
        }
        #endregion

        private void SelectFirstComboItem(ComboBox combobox)
		{
			if (combobox.Items.Count > 0)
				combobox.SelectedIndex = 0;
		}

		private void cbDestinoFinal_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			GISADataset.RelacaoHierarquicaRow rhRow = null;
			rhRow = TipoNivelRelacionado.GetPrimeiraRelacaoEncontrada(CurrentFRDBase.NivelRow);

			if ((chkModeloAvaliacao.Checked) && (System.Convert.ToInt32(cbModeloAvaliacao.SelectedValue) != -1))
			{
				if (cbDestinoFinal.SelectedIndex == 2)
				{
					if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR || (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL))
					{
						GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("ID=" + cbModeloAvaliacao.SelectedValue.ToString())[0]);

						nudPrazoConservacao.Enabled = true;
						if (! (CurrentSFRDAvaliacao.IsPrazoConservacaoNull()) && CurrentSFRDAvaliacao.PrazoConservacao > modAvRow.PrazoConservacao)
						{
							nudPrazoConservacao.Minimum = modAvRow.PrazoConservacao;
							nudPrazoConservacao.Value = CurrentSFRDAvaliacao.PrazoConservacao;
						}
						else
						{
							// os valores podem ser iguais ou n�o h� nenhum prazo de conserva��o no CurrentSFRDAvaliacao
							nudPrazoConservacao.Minimum = modAvRow.PrazoConservacao;
							nudPrazoConservacao.Value = modAvRow.PrazoConservacao;
						}

						if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D)
						{
							ControlAutoEliminacao1.ContentsEnabled = true;
							if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
								ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
							else
								ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
						}
						else
						{
							ControlAutoEliminacao1.ContentsEnabled = false;
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
						}
					}
					else
					{
						// valor deve ser o m�ximo 
                        GISADataset.SFRDAvaliacaoRow parentSFRDAvaliacaoRow = rhRow.NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0].GetSFRDAvaliacaoRows()[0];
						GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("ID=" + cbModeloAvaliacao.SelectedValue.ToString())[0]);

						nudPrazoConservacao.Enabled = false;
						if (parentSFRDAvaliacaoRow.IsPrazoConservacaoNull())
							nudPrazoConservacao.Value = modAvRow.PrazoConservacao;
						else if (parentSFRDAvaliacaoRow.PrazoConservacao > modAvRow.PrazoConservacao)
							nudPrazoConservacao.Value = parentSFRDAvaliacaoRow.PrazoConservacao;
						else
							nudPrazoConservacao.Value = modAvRow.PrazoConservacao;
						ControlAutoEliminacao1.ContentsEnabled = true;
						if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
						else
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
					}
				}
				else
				{
					nudPrazoConservacao.Enabled = false;
					nudPrazoConservacao.Minimum = 0;
					nudPrazoConservacao.Value = 0;
					ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
					ControlAutoEliminacao1.ContentsEnabled = false;
				}
			}
			else
			{
				if (cbDestinoFinal.SelectedIndex == 2)
				{
					if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR || (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL))
					{
						nudPrazoConservacao.Enabled = true;
						if (CurrentSFRDAvaliacao.IsPrazoConservacaoNull())
						{
							nudPrazoConservacao.Minimum = 0;
							nudPrazoConservacao.Value = 0;
						}
						else
						{
							nudPrazoConservacao.Minimum = 0;
							nudPrazoConservacao.Value = CurrentSFRDAvaliacao.PrazoConservacao;
						}

						if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D)
						{
							ControlAutoEliminacao1.ContentsEnabled = true;
							if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
								ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
							else
								ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
						}
						else
						{
							ControlAutoEliminacao1.ContentsEnabled = false;
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
						}
					}
					else
					{
						nudPrazoConservacao.Enabled = false;
						ControlAutoEliminacao1.ContentsEnabled = true;
						if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
						else
							ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
					}
				}
				else
				{
					nudPrazoConservacao.Enabled = false;
					nudPrazoConservacao.Minimum = 0;
					nudPrazoConservacao.Value = 0;
					ControlAutoEliminacao1.cbAutoEliminacao.SelectedIndex = 0;
					ControlAutoEliminacao1.ContentsEnabled = false;
				}

                if (cbDestinoFinal.SelectedIndex == 0)
                    CurrentSFRDAvaliacao["Preservar"] = DBNull.Value;
                else if(cbDestinoFinal.SelectedIndex == 1)
                    CurrentSFRDAvaliacao.Preservar = true;
                else
                    CurrentSFRDAvaliacao.Preservar = false;

                this.TheGenericDelegate.Invoke();
			}
		}

		private int lastDestinoFinal = -1;
		private void cacheDestinoFinal()
		{
			if (CurrentSFRDAvaliacao.IsPreservarNull())
				lastDestinoFinal = -1;
			else if (CurrentSFRDAvaliacao.Preservar)
				lastDestinoFinal = 1;
			else
				lastDestinoFinal = 0;
		}

		public override void OnShowPanel()
		{
			object oldValue = ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue;
			// assegurar que a combo dos autos de elimina��o tem os dados 
			// mais recentes existentes em mem�ria
			ControlAutoEliminacao1.rebindToData();
			ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = oldValue;
		}

		private void chkModeloAvaliacao_CheckedChanged(object sender, System.EventArgs e)
		{
			RemoveHandlersAvaliacao();

			nudPrazoConservacao.Minimum = 0;
			nudPrazoConservacao.Value = 0;
			cbDestinoFinal.Enabled = true;
			cbDestinoFinal.SelectedIndex = -1;
			nudPrazoConservacao.Enabled = false;
			ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = 0;

			UpdatePanel();
			EnableChangeAvaliacaoControls(! chkModeloAvaliacao.Checked);
			AddHandlersAvaliacao();
			
            //UpdatePanel2Height();
		}

		private void EnableChangeAvaliacaoControls(bool enabled)
		{
			if (enabled)
			{
				//mostra controlos referentes a avalia��o sist�mica
				pnlAvaliacaoTabela.Visible = false;
				//pnlAvaliacaoSistemica.Visible = true;
			}
			else
			{
				//mostra controlos referentes a avalia��o por tabela
				pnlAvaliacaoTabela.Visible = true;
				//pnlAvaliacaoSistemica.Visible = false;

				// s� � poss�vel mudar a tabela de avalia��o se o n�vel de descri��o se tratar de uma s�rie ou sub-s�rie ou documento solto
				if ((CurrentSFRDAvaliacao.FRDBaseRow.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].IDTipoNivelRelacionado == TipoNivelRelacionado.D && ! (CurrentSFRDAvaliacao.FRDBaseRow.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL)) || CurrentSFRDAvaliacao.FRDBaseRow.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
					cbModeloAvaliacao.Enabled = false;
				else
					cbModeloAvaliacao.Enabled = true;

				//Panel2.BringToFront();
				Panel1.SendToBack();
			}
		}

		private void cbModeloAvaliacao_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			GISADataset.RelacaoHierarquicaRow rhRow = null;
			rhRow = TipoNivelRelacionado.GetPrimeiraRelacaoEncontrada(CurrentFRDBase.NivelRow);

			if (cbModeloAvaliacao.SelectedValue != null && ((GISADataset.ModelosAvaliacaoRow)cbModeloAvaliacao.SelectedValue).ID > 0)
			{
				GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("ID=" + cbModeloAvaliacao.SelectedValue.ToString())[0]);

				if (modAvRow.IsPreservarNull())
					cbDestinoFinal.SelectedValue = -1;
				else
					cbDestinoFinal.SelectedValue = modAvRow.Preservar;

				if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.DOCUMENTAL)
				{

					if (cbModeloAvaliacao.Items.Count == 1)
					{
						cbDestinoFinal.Enabled = true;
						cbModeloAvaliacao.Enabled = false;
					}
					else if (cbModeloAvaliacao.Items.Count == 2)
					{
						// um documento, no caso de estar avaliado segundo uma tabela de avalia��o diferente da sua s�rie (ou 
						// sub-s�rie), s� � permitido editar o seu destino final caso se seleccione a nova tabela. Caso contr�rio,
						// o controlo estar� desactivado.

						if (CurrentSFRDAvaliacao.IsIDModeloAvaliacaoNull() || CurrentSFRDAvaliacao.IDModeloAvaliacao != (long)cbModeloAvaliacao.SelectedValue)
							cbDestinoFinal.Enabled = true;
						else
							cbDestinoFinal.Enabled = false;
					}
					else
						cbDestinoFinal.Enabled = false;
				}
				else if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL)
					cbDestinoFinal.Enabled = true;
				else
					cbDestinoFinal.Enabled = false;
			}
			else
			{
				cbDestinoFinal.Enabled = false;
				cbDestinoFinal.SelectedValue = -1;
				nudPrazoConservacao.Minimum = 0;
				nudPrazoConservacao.Value = 0;
			}			
		}

		private void UpdatePanel()
		{
			GISADataset.RelacaoHierarquicaRow rhRow = null;
			rhRow = TipoNivelRelacionado.GetPrimeiraRelacaoEncontrada(CurrentFRDBase.NivelRow);

            //if (chkModeloAvaliacao.Checked)
            //{
            //    if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.DOCUMENTAL)
            //    {
            //        if (cbModeloAvaliacao.Items.Count == 1)
            //        {
            //            cbDestinoFinal.Enabled = true;
            //            cbModeloAvaliacao.Enabled = false;
            //        }
            //        else if (cbModeloAvaliacao.Items.Count == 2)
            //        {
            //            // um documento, no caso de estar avaliado segundo uma tabela de avalia��o diferente da sua s�rie (ou 
            //            // sub-s�rie), s� � permitido editar o seu destino final caso se seleccione a nova tabela. Caso contr�rio,
            //            // o controlo estar� desactivado.

            //            if (CurrentSFRDAvaliacao.IsIDModeloAvaliacaoNull() || CurrentSFRDAvaliacao.IDModeloAvaliacao != (long)cbModeloAvaliacao.SelectedValue)
            //                cbDestinoFinal.Enabled = true;
            //            else
            //                cbDestinoFinal.Enabled = false;
            //        }
            //        else
            //            cbDestinoFinal.Enabled = false;

            //        if (cbModeloAvaliacao.SelectedValue != null && ((long)cbModeloAvaliacao.SelectedValue) > 0)
            //        {
            //            GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("ID=" + cbModeloAvaliacao.SelectedValue.ToString())[0]);

            //            if (CurrentSFRDAvaliacao.IsPreservarNull())
            //            {
            //                cbDestinoFinal.SelectedValue = -1;
            //                nudPrazoConservacao.Enabled = false;
            //                nudPrazoConservacao.Minimum = 0;
            //                nudPrazoConservacao.Value = 0;
            //                ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
            //                ControlAutoEliminacao1.ContentsEnabled = false;
            //            }
            //            else
            //            {
            //                cbDestinoFinal.SelectedValue = CurrentSFRDAvaliacao.Preservar;
            //                nudPrazoConservacao.Enabled = false;
            //                if (CurrentSFRDAvaliacao.Preservar)
            //                {
            //                    nudPrazoConservacao.Minimum = 0;
            //                    nudPrazoConservacao.Value = 0;
            //                    ControlAutoEliminacao1.ContentsEnabled = false;
            //                }
            //                else
            //                {
            //                    GISADataset.SFRDAvaliacaoRow parentSFRDAvaliacaoRow = rhRow.NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0].GetSFRDAvaliacaoRows()[0];

            //                    if (! (parentSFRDAvaliacaoRow.IsPrazoConservacaoNull()) && (parentSFRDAvaliacaoRow.PrazoConservacao > modAvRow.PrazoConservacao))
            //                        nudPrazoConservacao.Value = parentSFRDAvaliacaoRow.PrazoConservacao;
            //                    else
            //                        nudPrazoConservacao.Value = modAvRow.PrazoConservacao;

            //                    ControlAutoEliminacao1.ContentsEnabled = true;
            //                }

            //                if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
            //                    ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
            //                else
            //                    ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
            //            }
            //        }
            //        else
            //        {
            //            // no caso de o documento nunca ter sido avaliado com um modelo de avalia��o, selecciona-se o modelo
            //            // usado no n�vel pai caso exista
            //            GISADataset.SFRDAvaliacaoRow parentSFRDAvaliacaoRow = CurrentFRDBase.NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].NivelRowByNivelRelacaoHierarquicaUpper.GetFRDBaseRows()[0].GetSFRDAvaliacaoRows()[0];

            //            if (parentSFRDAvaliacaoRow.AvaliacaoTabela)
            //            {
            //                if (parentSFRDAvaliacaoRow.IsIDModeloAvaliacaoNull())
            //                {
            //                    // o nivel pai est� marcado como avaliado por um modelo de avali��o mas n�o tem nenhum modelo
            //                    // associado
            //                }
            //                else
            //                {
            //                    GISADataset.ModelosAvaliacaoRow modAvRow = parentSFRDAvaliacaoRow.ModelosAvaliacaoRow;
            //                    cbModeloAvaliacao.SelectedValue = modAvRow.ID;

            //                    if (modAvRow.IsPreservarNull())
            //                        cbDestinoFinal.SelectedValue = -1;
            //                    else
            //                    {
            //                        cbDestinoFinal.SelectedValue = modAvRow.Preservar;
            //                        nudPrazoConservacao.Enabled = false;
            //                        if (modAvRow.Preservar)
            //                            nudPrazoConservacao.Value = 0;
            //                        else
            //                        {
            //                            //TODO: FIXME: deve aparecer o valor m�nimo entre o valor atribuido na s�rie e o valor difinido no
            //                            //modelo de avalia��o
            //                            nudPrazoConservacao.Value = modAvRow.PrazoConservacao;
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                cbDestinoFinal.Enabled = false;
            //                cbDestinoFinal.SelectedValue = -1;
            //                nudPrazoConservacao.Minimum = 0;
            //                nudPrazoConservacao.Value = 0;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // s�ries, sub-s�ries e documentos soltos

            //        cbModeloAvaliacao.Enabled = true;
            //        if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D)
            //            cbDestinoFinal.Enabled = true;
            //        else
            //            cbDestinoFinal.Enabled = false;
					
            //        if (cbModeloAvaliacao.SelectedValue != null && ((long)cbModeloAvaliacao.SelectedValue) > 0)
            //        {
            //            GISADataset.ModelosAvaliacaoRow modAvRow = (GISADataset.ModelosAvaliacaoRow)(GisaDataSetHelper.GetInstance().ModelosAvaliacao.Select("ID=" + cbModeloAvaliacao.SelectedValue.ToString())[0]);

            //            // popular destino final
            //            int preservarVal = 0;
            //            if (modAvRow.IsPreservarNull())
            //                preservarVal = -1;
            //            else if (modAvRow.Preservar)
            //                preservarVal = 1;
            //            else
            //                preservarVal = 0;

            //            cbDestinoFinal.SelectedValue = preservarVal;

            //            //popular prazo conserva��o
            //            if (preservarVal == 0)
            //            {
            //                nudPrazoConservacao.Enabled = true;

            //                if (! (CurrentSFRDAvaliacao.IsPrazoConservacaoNull()) && CurrentSFRDAvaliacao.PrazoConservacao > modAvRow.PrazoConservacao)
            //                {
            //                    nudPrazoConservacao.Minimum = modAvRow.PrazoConservacao;
            //                    nudPrazoConservacao.Value = CurrentSFRDAvaliacao.PrazoConservacao;
            //                }
            //                else
            //                {
            //                    // os valores podem ser iguais ou n�o h� nenhum prazo de conserva��o no CurrentSFRDAvaliacao
            //                    nudPrazoConservacao.Minimum = modAvRow.PrazoConservacao;
            //                    nudPrazoConservacao.Value = modAvRow.PrazoConservacao;
            //                }
            //            }
            //            else
            //            {
            //                nudPrazoConservacao.Enabled = false;
            //                nudPrazoConservacao.Minimum = 0;
            //                nudPrazoConservacao.Value = 0;
            //            }

            //            //popular auto elimina��o
            //            if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D)
            //            {
            //                ControlAutoEliminacao1.ContentsEnabled = true;
            //                if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
            //                    ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
            //                else
            //                    ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
            //            }
            //            else
            //            {
            //                ControlAutoEliminacao1.ContentsEnabled = false;
            //                ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
            //            }
            //        }
            //        else
            //        {
            //            cbDestinoFinal.SelectedValue = -1;
            //            nudPrazoConservacao.Minimum = 0;
            //            nudPrazoConservacao.Value = 0;
            //            ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
            //            nudPrazoConservacao.Enabled = false;
            //            ControlAutoEliminacao1.ContentsEnabled = false;
            //        }
            //    }
            //}
            //else
            //{
				//Nivel
				if (CurrentSFRDAvaliacao.IsIDPertinenciaNull())
					CurrentSFRDAvaliacao.IDPertinencia = 1;

				cbNivel.SelectedValue = CurrentSFRDAvaliacao.IDPertinencia;

				//Densidade
				if (CurrentSFRDAvaliacao.IsIDDensidadeNull())
					cbInforAnaliseTipo.SelectedValue = 1;
				else
					cbInforAnaliseTipo.SelectedValue = CurrentSFRDAvaliacao.IDDensidade;

				//Sub-densidade
				if (CurrentSFRDAvaliacao.IsIDSubdensidadeNull())
					cbInforAnaliseSubTipo.SelectedValue = 8;
				else
					cbInforAnaliseSubTipo.SelectedValue = CurrentSFRDAvaliacao.IDSubdensidade;

				//Frequ�ncia de uso
				try
				{
					cbFrequenciaUso.SelectedValue = CurrentSFRDAvaliacao.Frequencia;
				}
				catch (StrongTypingException)
				{
					cbFrequenciaUso.SelectedValue = -1;
				}

				if (GisaDataSetHelper.UsingGestaoIntegrada())
					cbDestinoFinal.Enabled = true;
				else
					cbDestinoFinal.Enabled = false;

				int originalDestinoFinal = -1;
				if (GisaDataSetHelper.UsingGestaoIntegrada())
				{
					try
					{
						originalDestinoFinal = (int)(((System.Convert.ToBoolean(CurrentSFRDAvaliacao.Preservar)) ? 1 : 0));
					}
					catch (StrongTypingException)
					{
						// Em modo de gestao n�o integrada for�ar a "Conserva��o" como valor por omiss�o
						if (GisaDataSetHelper.UsingGestaoIntegrada())
							originalDestinoFinal = -1;
						else
							originalDestinoFinal = 1;
					}
				}
				else
					originalDestinoFinal = 1;

				cbDestinoFinal.SelectedValue = originalDestinoFinal;
				lastDestinoFinal = originalDestinoFinal;

				ControlAutoEliminacao1.rebindToData();
				if (! CurrentSFRDAvaliacao.AvaliacaoTabela)
				{
					if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
						ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue; // selecionar o item vazio (sem texto)
					else
						ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
				}
				else
					ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;

				lstVwInforRelacionada.Items.Clear();
				QueryFilter = string.Format("IDFRDBase = {0}", CurrentFRDBase.ID);
				foreach (GISADataset.SFRDAvaliacaoRelRow arRow in GisaDataSetHelper.GetInstance().SFRDAvaliacaoRel.Select(QueryFilter))
					AddRowToLstVwInforRelacionada(arRow);

				cbDestinoFinal.Enabled = true;
				if (cbDestinoFinal.SelectedIndex == 2)
				{
					if (! isInactiveEstruturalPanel)
					{
						// Prazo de conserva��o
						if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR || (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.D && rhRow.NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL))
						{
							nudPrazoConservacao.Enabled = true;
							if (! CurrentSFRDAvaliacao.IsPrazoConservacaoNull())
							{
								nudPrazoConservacao.Minimum = 0;
								nudPrazoConservacao.Value = CurrentSFRDAvaliacao.PrazoConservacao;
							}
							else
							{
								nudPrazoConservacao.Minimum = 0;
								nudPrazoConservacao.Value = 0;
							}
						}
						else
						{
							// O prazo de conserva��o s� pode estar dispon�vel nas s�ries, subs�ries e documentos soltos
							nudPrazoConservacao.Enabled = false;
							nudPrazoConservacao.Minimum = 0;
							nudPrazoConservacao.Value = 0;
						}

						// Auto de elimina��o
						if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR)
							ControlAutoEliminacao1.ContentsEnabled = false;
						else
						{
							ControlAutoEliminacao1.ContentsEnabled = true;

							ControlAutoEliminacao1.rebindToData();
							if (! CurrentSFRDAvaliacao.AvaliacaoTabela)
							{
								if (CurrentSFRDAvaliacao.IsIDAutoEliminacaoNull())
									ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue; // selecionar o item vazio (sem texto)
								else
									ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = CurrentSFRDAvaliacao.IDAutoEliminacao;
							}
							else
								ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
						}
					}
				}
				else
				{
					//Conserva��o ou por avaliar

					//Prazo de conserva��o
					nudPrazoConservacao.Enabled = false;
					nudPrazoConservacao.Minimum = 0;
					nudPrazoConservacao.Value = 0;

					//Auto de elimina��o
					ControlAutoEliminacao1.ContentsEnabled = false;
					if (((long)ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue) != long.MinValue) // long.MinValue � o valor do item vazio
						ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue = long.MinValue;
				}
			//}
		}

		private void cacheValues()
		{
			long nivel = (long)cbNivel.SelectedValue;
			string ponderacao = txtPonderacao.Text;
			long frequencia = (long)cbFrequenciaUso.SelectedValue;
			long tipoProducao = (long)cbInforAnaliseTipo.SelectedValue;
			long grauDensidade = (long)cbInforAnaliseSubTipo.SelectedValue;
			long destinoFinalS = (long)cbDestinoFinal.SelectedValue;
			decimal prazoConservacaoS = nudPrazoConservacao.Value;
			long autoEliminacaoS = (long)ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue;

			long destinoFinalT = (long)cbDestinoFinal.SelectedValue;
			decimal prazoConservacaoT = nudPrazoConservacao.Value;
			long autoEliminacaoT = (long)ControlAutoEliminacao1.cbAutoEliminacao.SelectedValue;

		}
	}

} //end of root namespace