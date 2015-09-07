using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DBAbstractDataLayer.DataAccessRules;
using GISA.Controls;
using GISA.Controls.Nivel;
using GISA.Controls.Localizacao;
using GISA.Fedora.FedoraHandler;
using GISA.EADGen;
using GISA.GUIHelper;
using GISA.Import;
using GISA.Model;
using GISA.SharedResources;


namespace GISA
{
	public class MasterPanelSeries : GISA.MasterPanelNiveis, INivelNavigatorProvider
	{

	#region  Windows Form Designer generated code 

		public MasterPanelSeries() : base()
		{
			//This call is required by the Windows Form Designer.
			InitializeComponent();

            MenuItemPrintAutoEliminacao.Click += MenuItemPrint_Click;
            MenuItemPrintAutoEliminacaoPortaria.Click += MenuItemPrint_Click;
            MenuItemPrintInventarioResumido.Click += MenuItemPrint_Click;
            MenuItemPrintInventarioDetalhado.Click += MenuItemPrint_Click;
            MenuItemPrintCatalogoResumido.Click += MenuItemPrint_Click;
            MenuItemPrintCatalogoDetalhado.Click += MenuItemPrint_Click;
            rhTable.RelacaoHierarquicaRowChanged += rhTable_RelacaoHierarquicaRowChangingRelacaoHierarquicaRowDeleting;
            rhTable.RelacaoHierarquicaRowDeleting += rhTable_RelacaoHierarquicaRowChangingRelacaoHierarquicaRowDeleting;
            this.nivelNavigator1.KeyUpDeleteEvent += new NivelNavigator.KeyUpDeleteEventHandler(nivelNavigator1_KeyUpDeleteEvent);
            //ToolBarButtonEAD.
            base.StackChanged += MasterPanelSeries_StackChanged;

			ShowToolBarButtons();

			//Add any initialization after the InitializeComponent() call
			GetExtraResources();
			// A REPOR: ser� necess�rio corrigir e repor quando necessitarmos de cronologia
			//CollectDateTimeFromRelacaoHierarquica()

            this.nivelNavigator1.MultiSelect = true;
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
			//RemoveHandlers()
			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components = null;

		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.pnlToolbarPadding.SuspendLayout();
			this.SuspendLayout();
			//
			//lblFuncao
			//
			this.lblFuncao.Location = new System.Drawing.Point(0, 0);
			this.lblFuncao.Name = "lblFuncao";
			this.lblFuncao.TabIndex = 0;
            this.lblFuncao.Text = "Estrutura org�nica";
			//
			//pnlToolbarPadding
			//
			this.pnlToolbarPadding.DockPadding.Left = 5;
			this.pnlToolbarPadding.DockPadding.Right = 5;
			this.pnlToolbarPadding.Location = new System.Drawing.Point(0, 24);
			this.pnlToolbarPadding.Name = "pnlToolbarPadding";
			this.pnlToolbarPadding.Size = new System.Drawing.Size(600, 24);
			//
			//MasterPanelSeries
			//
			this.Name = "MasterPanelSeries";
			this.Controls.SetChildIndex(this.lblFuncao, 0);
			this.Controls.SetChildIndex(this.pnlToolbarPadding, 0);
			this.Controls.SetChildIndex(this.nivelNavigator1, 0);
			this.pnlToolbarPadding.ResumeLayout(false);
           // this.VisibleChanged += new EventHandler(MasterPanelSeries_VisibleChanged);
			this.ResumeLayout(false);

			//INSTANT C# NOTE: Converted event handlers:
			

		}


	#endregion

		private GISADataset.NivelRow pseudoContextNivel;
        private bool isSelectedNivelRemovable = false;

		private void ShowToolBarButtons()
		{
			foreach (ToolBarButton button in base.ToolBar.Buttons)
				button.Visible = true;
		}

		protected override void beforeNewSelection_Action(ControloNivelListEstrutural.BeforeNewSelectionEventArgs e)
		{
			if (e.node == null)
			{
				GISADataset.NivelRow nRow = null;
				UpdateContext(nRow);
			}
			else
				UpdateContext(e.node.NivelRow);

			if (e.selectionChange && ((frmMain)TopLevelControl).MasterPanel is MasterPanelSeries)
				updateContextStatusBar(e.node);
		}

        protected override void NivelDocumentalListNavigator1_BeforeNewListSelection(object sender, BeforeNewSelectionEventArgs e)
        {
            var topLevelControl = (frmMain)TopLevelControl;
            topLevelControl.EnterWaitMode();

            if (e.ItemToBeSelected != null)
            {
                var nRow = e.ItemToBeSelected.Tag as GISADataset.NivelRow;

                if (this.nivelNavigator1.EPFilterMode && this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
                    PermissoesHelper.UpdateNivelPermissions(nRow, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
                else
                {
                    var nUpperRow = GisaDataSetHelper.GetInstance().Nivel.Cast<GISADataset.NivelRow>().FirstOrDefault(r => r.RowState != DataRowState.Deleted && r.ID == this.nivelNavigator1.ContextBreadCrumbsPathID);
                    if (nUpperRow != null)
                        PermissoesHelper.UpdateNivelPermissions(nRow, nUpperRow, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
                }
            }

            if (topLevelControl != null && topLevelControl.MasterPanelCount == 1)
            {
                try
                {
                    Debug.WriteLine("NivelDocumentalListNavigator1_BeforeNewListSelection");
                    ListViewItem lvItem = null;
                    if (this.nivelNavigator1.SelectedItems.Count <= 1)
                        lvItem = e.ItemToBeSelected;

                    e.SelectionChange = UpdateContext(lvItem);
                    if (e.SelectionChange)
                    {
                        UpdateToolBarButtons(e.ItemToBeSelected);
                        updateContextStatusBar(e.ItemToBeSelected);

                        if (this.nivelNavigator1.SelectedItems.Count > 1)
                            // verificar se existe permiss�o de edi��o no conjunto de niveis selecionados
                            ToolBarButtonCut.Enabled = CheckEditPermission(this.nivelNavigator1.SelectedItems.Select(item => item.Tag as GISADataset.NivelRow).ToList());
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    throw;
                }
            }

            ((frmMain)TopLevelControl).LeaveWaitMode();
        }

        private bool CheckEditPermission(List<GISADataset.NivelRow> nivelrows)
        {
            var permissoes = new Dictionary<long, Dictionary<string, byte>>();
            var nivelIds = nivelrows.Select(r => r.ID).ToList();
            var ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetTempConnection());
            try
            {
                permissoes = DBAbstractDataLayer.DataAccessRules.PermissoesRule.Current.CalculateEffectivePermissions(nivelIds, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID, ho.Connection);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                throw ex;
            }
            finally
            {
                ho.Dispose();
            }

            return permissoes.Values.Select(d => d["Escrever"]).Min() == 1;
        }

		private void GetExtraResources()
		{
			ToolBar.ImageList = SharedResourcesOld.CurrentSharedResources.NVLManipulacaoImageList;
			ToolBarButtonCreateED.ImageIndex = 0;
			ToolBarButtonCreateAny.ImageIndex = 1;
			ToolBarButtonEdit.ImageIndex = 2;
			ToolBarButtonRemove.ImageIndex = 3;
			ToolBarButtonCut.ImageIndex = 4;
			ToolBarButtonPaste.ImageIndex = 5;
			ToolBarButtonToggleEstruturaSeries.ImageIndex = 7; // 6/7
			ToolBarButtonFiltro.ImageIndex = 8;
			ToolBarButtonPrint.ImageIndex = 9;

			string[] strs = SharedResourcesOld.CurrentSharedResources.NVLManipulacaoStrings;
			ToolBarButtonCreateED.ToolTipText = strs[0];
			ToolBarButtonCreateAny.ToolTipText = strs[1];
			ToolBarButtonEdit.ToolTipText = strs[2];
			ToolBarButtonRemove.ToolTipText = strs[3];
			ToolBarButtonCut.ToolTipText = strs[4];
			ToolBarButtonPaste.ToolTipText = strs[5];
			ToolBarButtonToggleEstruturaSeries.ToolTipText = strs[7]; // 6/7
			ToolBarButtonFiltro.ToolTipText = strs[8];
			ToolBarButtonPrint.ToolTipText = strs[9];
		}

		private void EditNivel(FormAddNivel frm, GISADataset.NivelRow NivelRow)
		{
            GISADataset.TipoNivelRelacionadoRow tnrRow;
            if (NivelRow.IDTipoNivel == TipoNivel.LOGICO && GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select("ID="+NivelRow.ID.ToString()).Length == 0)
                tnrRow = GisaDataSetHelper.GetInstance().TipoNivelRelacionado.Cast<GISADataset.TipoNivelRelacionadoRow>().Single(r => r.ID == TipoNivelRelacionado.ED);
            else
                tnrRow = GisaDataSetHelper.GetInstance().RelacaoHierarquica.Cast<GISADataset.RelacaoHierarquicaRow>().First(r => r.ID == NivelRow.ID && r.RowState != DataRowState.Deleted).TipoNivelRelacionadoRow;

			string WindowTitle = string.Format("Editar {0}", tnrRow.Designacao);
			// Don't allow to edit a Nivel without a Controlo 
			// Autoridade using a form with Controlo Autoridade.
			// frm will only be of type FormNivelEstrutural if the
			// related administration option is set to "demand a ControloAutoridade"
			if (frm is FormNivelEstrutural && ! (NivelRow.CatCode.Trim().Equals("CA")))
			{
				MessageBox.Show("O n�vel selecionado n�o foi definido com " + "base numa entidade produtora." + System.Environment.NewLine + "Assim, para que este n�vel seja edit�vel � necess�rio " + "que a aplica��o esteja " + System.Environment.NewLine + "configurada para lidar com n�veis n�o controlados.", WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			else if (! (frm is FormNivelEstrutural) && ! (NivelRow.CatCode.Trim().Equals("NVL")))
			{
				MessageBox.Show("O n�vel selecionado foi definido com " + "base numa entidade produtora." + System.Environment.NewLine + "Assim, para que este n�vel seja edit�vel � necess�rio " + "que a aplica��o esteja " + System.Environment.NewLine + "configurada para lidar com n�veis controlados.", WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			frm.Text = WindowTitle;
			frm.txtCodigo.Text = NivelRow.Codigo;
			frm.txtDesignacao.Text = Nivel.GetDesignacao(NivelRow);
			// se se tratar de uma s�rie ou subs�rie o c�digo ser� sempre autom�tico
			if (tnrRow.ID == TipoNivelRelacionado.SR || tnrRow.ID == TipoNivelRelacionado.SSR)
				frm.txtCodigo.Enabled = false;

			frm.LoadData();

			if (NivelRow.TipoNivelRow.ID != TipoNivel.LOGICO)
			{
				// populate controls with data so that it can be edited

				if (frm is FormNivelEstrutural)
				{
                    FormNivelEstrutural tempWith1 = (FormNivelEstrutural)frm;
					if (NivelRow.CatCode.Trim().Equals("CA"))
					{
						tempWith1.caList.txtFiltroDesignacao.Text = tempWith1.txtDesignacao.Text;
                        tempWith1.caList.ReloadList();
						tempWith1.chkControloAut = true;
					}
					else if (NivelRow.CatCode.Trim().Equals("NVL"))
						tempWith1.chkControloAut = false;
				}
			}

			// show form and receive user feedback
			switch (frm.ShowDialog())
			{
				case DialogResult.OK:
				Trace.WriteLine("A editar n�vel...");
					GISADataset.NivelDesignadoRow ndRow = null;
					
					// Um Nivel documental deve ter obrigatoriamente um NivelDesignado.
                    Debug.Assert(NivelRow.GetNivelDesignadoRows().Length > 0);
					ndRow = NivelRow.GetNivelDesignadoRows()[0];
                    
                    NivelRow.Codigo = frm.txtCodigo.Text;
                    ndRow.Designacao = frm.txtDesignacao.Text;

                    // registar a edi��o do item selecionado
                    if (NivelRow.IDTipoNivel != TipoNivel.LOGICO)
                        CurrentContext.RaiseRegisterModificationEvent(NivelRow.GetFRDBaseRows()[0]);

					PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments pcArgs = new PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments();
					pcArgs.nRowID = NivelRow.ID;
					pcArgs.ndRowID = ndRow.ID;
					// Se se tratar de uma entidade detentora n�o passar os Ids de uma rela��o
					// hier�rquica para um n�vel superior pois n�o existe nenhum.
					if (NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().Length > 0)
					{
						pcArgs.rhRowID = NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].ID;
						pcArgs.rhRowIDUpper = NivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].IDUpper;
					}
					pcArgs.testOnlyWithinNivel = true;

                    PersistencyHelper.SaveResult successfulSave = PersistencyHelper.save(DelegatesHelper.ensureUniqueCodigo, pcArgs);
					if (! pcArgs.successful)
						MessageBox.Show(pcArgs.message, "Cria��o de unidade de descri��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else if (successfulSave == PersistencyHelper.SaveResult.successful)
                    {
                        GisaDataSetHelper.HoldOpen ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
                        try
                        {
                            List<string> IDNiveis = new List<string>();
                            IDNiveis.Add(NivelRow.ID.ToString());
                            GISA.Search.Updater.updateNivelDocumental(IDNiveis);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                            throw;
                        }
                        finally
                        {
                            ho.Dispose();
                        }
                    }

                    PersistencyHelper.cleanDeletedData(PersistencyHelper.determinaNuvem("RelacaoHierarquica"));

					// Actualizar a interface com os novos valores. Se editarmos a 
					// raiz (estrutural) da vista documental � necess�rio actualizar 
					// automaticamente tamb�m a vista estrutural.

					if (! (NivelRow.RowState == DataRowState.Detached))
					{
                        if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
							this.nivelNavigator1.UpdateSelectedNodeName(Nivel.GetDesignacao(NivelRow));
						else
                            this.nivelNavigator1.UpdateSelectedListItemName(Nivel.GetDesignacao(NivelRow));
					}

					// For�ar a grava��o do documento
					CurrentContext.SetNivelEstrututalDocumental(null);
					CurrentContext.SetNivelEstrututalDocumental(NivelRow);
					break;
			}
		}
        #region ToolBar Buttons Action
        protected override void ToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e) //Handles ToolBar.ButtonClick
        {
            if (e.Button == ToolBarButtonEdit)
                EditNivelAction();
            else if (e.Button == ToolBarButtonToggleEstruturaSeries)
                ToggleTreeViews(this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural);
            else if (e.Button == ToolBarButtonCreateED)
                CreateED();
            else if (e.Button == ToolBarButtonRemove)
                RemoveNivel();
            else if (e.Button == ToolBarButtonCut)
                CutNivel();
            else if (e.Button == ToolBarButtonPaste)
                PasteNivel();
            else if (e.Button == ToolBarButtonPrint || e.Button == ToolBarButtonCreateAny)
            {
                if (e.Button.DropDownMenu != null && e.Button.DropDownMenu is ContextMenu)
                    ((ContextMenu)e.Button.DropDownMenu).Show(ToolBar, new System.Drawing.Point(e.Button.Rectangle.X, e.Button.Rectangle.Y + e.Button.Rectangle.Height));
            }
            else if (e.Button == ToolBarButtonFiltro)
            {
                base.ToolBar_ButtonClick(sender, e);
            }
            else if (e.Button == ToolBarButtonEAD)
                init_geracao_EAD();

            else if (e.Button == ToolBarButtonImportExcel)
                init_importacao();
        }        

        private void EditNivelAction()
        {
            GISADataset.NivelRow nRow = null;
            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
            {
                if (this.nivelNavigator1.EPFilterMode)
                    nRow = this.nivelNavigator1.SelectedNivel;
                else
                    nRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).NivelRow;
            }
            else
                nRow = this.nivelNavigator1.SelectedNivel;

            if (nRow.TipoNivelRow.ID == TipoNivel.ESTRUTURAL || nRow.TipoNivelRow.ID == TipoNivel.OUTRO)
            {
                var trace_msg = string.Format("Trying to edit UI of type {0}...",  nRow.TipoNivelRow.ID.ToString());
                Trace.WriteLine(trace_msg);
                Trace.WriteLine("The nivel row ID is: " + nRow.ID.ToString());
                Debug.Assert(false, trace_msg);
            }

            // Criar o tipo de form correcto conforme o tipo de n�vel em edi��o
            FormAddNivel frm = null;
            if (nRow.TipoNivelRow.ID == TipoNivel.LOGICO)
            {
                // N�vel � uma Entidade detentora ou um grupo de arquivos
                frm = new FormAddNivel();
                frm.IDTipoNivelRelacionado = nRow.TipoNivelRow.ID;
            }
            else if (nRow.TipoNivelRow.ID == TipoNivel.ESTRUTURAL)
            {
                //// N�vel estrutural
                //if (GisaDataSetHelper.UsingNiveisOrganicos())
                //    frm = new FormNivelEstrutural();
                //else
                //    frm = new FormAddNivel();

                //frm.IDTipoNivelRelacionado = nRow.TipoNivelRow.ID;
            }
            else if (nRow.TipoNivelRow.ID == TipoNivel.DOCUMENTAL)
            {
                // N�vel documental
                frm = new FormAddNivel();
                frm.IDTipoNivelRelacionado = nRow.TipoNivelRow.ID;
            }
            else if (nRow.TipoNivelRow.ID == TipoNivel.OUTRO)
            {
                // N�vel � uma unidade f�sica
            }

            EditNivel(frm, nRow);
        }

        private void CreateED()
        {
            // as ED usam o mesmo form que os n�veis documentais
            FormAddNivel frm = new FormAddNivel();
            GISADataset.TipoNivelRelacionadoRow tnrRow = null;
            tnrRow = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(null);
            frm.IDTipoNivelRelacionado = tnrRow.ID; // necess�rio para valida��o do c�digo parcial
            handleNewNivel(frm, null, tnrRow);
            UpdateToolBarButtons();
        }

        private void RemoveNivel()
        {
            GISATreeNode node = null;
            GISATreeNode parentNode = null;
            GISADataset.NivelRow nUpperRow = null;
            GISADataset.RelacaoHierarquicaRow rhRow = null;
            GISADataset.NivelRow nRow = null;
            var objDigital = default(ObjDigital);
            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
            {
                if (this.nivelNavigator1.SelectedNode != null)
                {
                    nRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).NivelRow;
                    rhRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).RelacaoHierarquicaRow;
                }
                node = (GISATreeNode)this.nivelNavigator1.SelectedNode;
                parentNode = (GISATreeNode)node.Parent;
                nUpperRow = (GISADataset.NivelRow)node.NivelUpperRow;
            }
            else
            {
                nRow = this.nivelNavigator1.SelectedNivel;
                // Verificar se a relac��o hier�rquica ainda � a mesma apresentada na interface (se o
                // utilizador estiver a ver a lista que contem o n�vel a apagar e entretanto outro utilizador
                // o ter colocado noutro ponto da �rvore, a rela��o hier�rquica presente em mem�ria deixa
                // de corresponder com aquela que � apresentada na interface quando esse n�vel � selecionado;
                // quando o n�vel � selecionado a informa��o no DataSet de trabalho � actualizado mas n�o
                // actualiza a interface)
                if (GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRow.ID, this.nivelNavigator1.ContextBreadCrumbsPathID)).Length > 0)
                    rhRow = (GISADataset.RelacaoHierarquicaRow)(GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRow.ID, this.nivelNavigator1.ContextBreadCrumbsPathID))[0]);
                else
                {
                    MessageBox.Show("Esta opera��o n�o pode ser conclu�da pelo facto de a localiza��o na estrutura " + System.Environment.NewLine + "do n�vel selecionado ter sido alterada por outro utilizador.", "Eliminar N�vel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                nUpperRow = (GISADataset.NivelRow)(GisaDataSetHelper.GetInstance().Nivel.Select(string.Format("ID={0}", this.nivelNavigator1.ContextBreadCrumbsPathID))[0]);

                if (!FedoraHelper.CanDeleteODsAssociated2UI(nRow, out objDigital))
                    return;
            }

            // N�o permitir a elimina��o de rela��es entre EPs
            if (TipoNivel.isNivelOrganico(nRow) && TipoNivel.isNivelOrganico(nUpperRow))
            {
                MessageBox.Show("A altera��o de rela��es entre entidades produtoras dever�o ser efectuadas atrav�s do controlo de autoridade.", "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var assocODs = FedoraHelper.GetAssociatedODsDetailsMsg(nRow.ID);
            if (assocODs.Length > 0)
            {
                FormDeletionReport form = new FormDeletionReport();
                form.Text = "Elimina��o de unidade de informa��o";
                form.Interrogacao = "A unidade de informa��o selecionada tem objeto(s) digital(ais) associado(s). " + System.Environment.NewLine +
                        "Se eliminar esta unidade de informa��o, os objeto(s) digital(ais) " + System.Environment.NewLine + " tamb�m ser�o eliminado(s)." + System.Environment.NewLine +
                        "Pretende continuar?";
                form.Detalhes = assocODs;

                if (form.ShowDialog() == DialogResult.Cancel) return;
            }
            else if (MessageBox.Show("Tem a certeza que deseja eliminar o n�vel selecionado?", "Elimina��o de rela��o", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;

            Trace.WriteLine("A apagar n�vel...");

            // actualizar objecto digital caso exista
            var preTransactionAction = new PreTransactionAction();
            var fedArgs = new PersistencyHelper.FedoraIngestPreTransactionArguments();
            preTransactionAction.args = fedArgs;

            preTransactionAction.preTransactionDelegate = delegate(PersistencyHelper.PreTransactionArguments preTransactionArgs)
            {
                bool ingestSuccess = true;
                string msg = null;

                var odsToIngest = FedoraHelper.DeleteObjDigital(nRow);
                odsToIngest.ForEach(od => ingestSuccess &= SessionHelper.AppConfiguration.GetCurrentAppconfiguration().FedoraHelperSingleton.Ingest(od, out msg));

                preTransactionArgs.cancelAction = !ingestSuccess;
                preTransactionArgs.message = msg;

            };

            // Se se tratar da rela��o entre um n�vel org�nico e uma ED ou GA 
            // ou se se tratar da rela��o entre um n�vel documental e um n�vel 
            // org�nio � necess�rio proceder de outras formas
            if (TipoNivel.isNivelOrganico(nRow) && TipoNivel.isNivelLogico(nUpperRow))
            {
                if (MessageBox.Show(
                    "Por favor tenha em aten��o que a entidade produtora propriamente " + System.Environment.NewLine +
                    "dita n�o ser� eliminada, apenas a rela��o ao n�vel superior o ser�.", "Elimina��o de rela��o", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;

                ((frmMain)TopLevelControl).EnterWaitMode();

                string key = ControloNivelList.getKey(node);
                PersistencyHelper.canDeleteRHRowPreConcArguments args = new PersistencyHelper.canDeleteRHRowPreConcArguments();
                args.nRowID = node.NivelRow.ID;
                args.nUpperRowID = node.NivelUpperRow.ID;
                args.rhRowID = node.RelacaoHierarquicaRow.ID;
                args.rhRowIDUpper = node.RelacaoHierarquicaRow.IDUpper;
                CurrentContext.RaiseRegisterModificationEvent(nRow.GetFRDBaseRows()[0]);
                var successfulSave = PersistencyHelper.save(DelegatesHelper.verifyIfCanDeleteRH, args);
                //tds.Add(GisaDataSetHelper.GetInstance().Tables["RelacaoHierarquica"]);
                PersistencyHelper.cleanDeletedData(PersistencyHelper.determinaNuvem("RelacaoHierarquica"));
                if (args.deleteSuccessful)
                {
                    GISA.Search.Updater.updateProdutor(node.NivelRow.GetNivelControloAutRows()[0].IDControloAut);

                    //Prevenir o beforeNewSelection que o Remove ia desencadear.
                    this.nivelNavigator1.AddHandlers();
                    this.nivelNavigator1.RemoveFromTreeview(node, key);
                    this.nivelNavigator1.RemoveHandlers();
                }
                else
                    MessageBox.Show(args.message, "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ((frmMain)TopLevelControl).LeaveWaitMode();
            }
            else if (TipoNivel.isNivelDocumental(nRow) && TipoNivel.isNivelOrganico(nUpperRow))
            {
                // Verificar que existem v�rias rela��es hier�rquicas deste 
                // n�vel documental a entidades produtoras superiores. Nesse 
                // caso dever� ser removida a rela��o, caso contr�rio, se n�o 
                // existirem subn�veis documentais, ser� eliminado o pr�prio 
                // n�vel(documental)
                int numRHs = nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().Length;
                if (numRHs > 1)
                {
                    if (MessageBox.Show(
                        "Por favor tenha em aten��o que s�o v�rios os produtores deste " + System.Environment.NewLine +
                        "n�vel documental. O n�vel documental propriamente dito n�o " + System.Environment.NewLine +
                        "ser� eliminado, apenas a sua rela��o ao n�vel org�nico " + System.Environment.NewLine +
                        "superior o ser�.", "Elimina��o de rela��o", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        return;

                    ((frmMain)TopLevelControl).EnterWaitMode();

                    CurrentContext.RaiseRegisterModificationEvent(nRow.GetFRDBaseRows()[0]);

                    PersistencyHelper.canDeleteRHRowPreConcArguments args = new PersistencyHelper.canDeleteRHRowPreConcArguments();
                    args.nRowID = nRow.ID;
                    args.nUpperRowID = nUpperRow.ID;
                    args.rhRowID = rhRow.ID;
                    args.rhRowIDUpper = rhRow.IDUpper;
                    PersistencyHelper.SaveResult successfulSave = PersistencyHelper.save(DelegatesHelper.verifyIfCanDeleteRH, args);
                    PersistencyHelper.cleanDeletedData(PersistencyHelper.determinaNuvem("RelacaoHierarquica"));
                    if (args.deleteSuccessful)
                    {
                        if (successfulSave == PersistencyHelper.SaveResult.successful)
                        {
                            List<string> IDNiveis = new List<string>();
                            IDNiveis.Add(args.nRowID.ToString());
                            GISA.Search.Updater.updateNivelDocumental(IDNiveis);
                            GISA.Search.Updater.updateNivelDocumentalComProdutores(nRow.ID);

                            this.nivelNavigator1.RemoveSelectedLVItem();
                        }
                    }
                    else
                        MessageBox.Show(args.message, "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ((frmMain)TopLevelControl).LeaveWaitMode();
                }
                else if (numRHs == 1)
                {
                    // Verificar que n�o existem subn�veis documentais
                    int numSubRHs = GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("IDUpper={0}", nRow.ID)).Length;
                    if (numSubRHs > 0)
                    {
                        MessageBox.Show("S� � poss�vel eliminar os n�veis que n�o tenham outros directamente associados", "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show(
                            "Por favor tenha em aten��o que este n�vel documental � produzido" + System.Environment.NewLine +
                            "por apenas uma entidade. Ao remover esta rela��o ser� perdida " + System.Environment.NewLine +
                            "n�o s� a rela��o como o n�vel documental propriamente dito.", "Elimina��o de rela��o", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                            return;

                        ((frmMain)TopLevelControl).EnterWaitMode();

                        CurrentContext.RaiseRegisterModificationEvent(nRow.GetFRDBaseRows()[0]);

                        PersistencyHelper.canDeleteRHRowPreConcArguments argsPca = new PersistencyHelper.canDeleteRHRowPreConcArguments();
                        argsPca.nRowID = nRow.ID;
                        argsPca.nUpperRowID = nUpperRow.ID;
                        argsPca.rhRowID = 0;
                        argsPca.rhRowIDUpper = 0;
                        PersistencyHelper.DeleteIDXPreSaveArguments argsPsa = new PersistencyHelper.DeleteIDXPreSaveArguments();
                        argsPsa.ID = nRow.ID;
                        PersistencyHelper.SaveResult successfulSave = PersistencyHelper.save(DelegatesHelper.verifyIfCanDeleteRH, argsPca, Nivel.DeleteNivelXInDataBase, argsPsa, preTransactionAction);
                        if (argsPca.deleteSuccessful)
                        {
                            if (successfulSave == PersistencyHelper.SaveResult.successful)
                            {
                                List<string> IDNiveis = new List<string>();
                                IDNiveis.Add(nRow.ID.ToString());
                                GISA.Search.Updater.updateNivelDocumental(IDNiveis);
                                GISA.Search.Updater.updateNivelDocumentalComProdutores(nRow.ID);

                                this.nivelNavigator1.RemoveSelectedLVItem();
                            }
                        }
                        else
                        {
                            // se o n�vel a eliminar se tratar de uma s�rie ou documento solto mas que 
                            // por motivos de conflito de concorr�ncia n�o foi poss�vel executar, 
                            // o refrescamento dos bot�es � feito tendo como o contexto o pr�prio
                            // n�vel que se pretendeu eliminar para desta forma o estado dos mesmos
                            // estar correcta (caso contr�rio o estado dos bot�es referir-se-ia a 
                            // n�o haver qualquer item selecionado
                            MessageBox.Show(argsPca.message, "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            UpdateToolBarButtons(this.nivelNavigator1.SelectedItems[0]);
                        }
                        PersistencyHelper.cleanDeletedData(new List<TableDepthOrdered.TableCloudType>(new TableDepthOrdered.TableCloudType[] { PersistencyHelper.determinaNuvem("RelacaoHierarquica"), PersistencyHelper.determinaNuvem("FRDBase") }));

                        ((frmMain)TopLevelControl).LeaveWaitMode();
                    }
                }
                else
                    Debug.Assert(false, "Should never happen. There must be a relation with an upper Nivel.");
            }
            else
            {
                // Entre todos os outros tipos de n�vel proceder normalmente
                if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
                {
                    ((frmMain)TopLevelControl).EnterWaitMode();

                    string key = ControloNivelList.getKey(node);
                    PersistencyHelper.canDeleteRHRowPreConcArguments argsPca = new PersistencyHelper.canDeleteRHRowPreConcArguments();
                    argsPca.nRowID = node.NivelRow.ID;
                    //se n�o se tratar de uma entidade detentora nem de um grupo de arquivos n�o � necess�rio criar uma entrada na tabela de controlo de descri��o
                    if (node.NivelUpperRow != null && node.NivelRow.IDTipoNivel != TipoNivel.LOGICO)
                    {
                        argsPca.nUpperRowID = node.NivelUpperRow.ID;
                        CurrentContext.RaiseRegisterModificationEvent(nRow.GetFRDBaseRows()[0]);
                    }
                    else
                        argsPca.nUpperRowID = 0;

                    argsPca.rhRowID = 0;
                    argsPca.rhRowIDUpper = 0;
                    PersistencyHelper.DeleteIDXPreSaveArguments argsPsa = new PersistencyHelper.DeleteIDXPreSaveArguments();
                    argsPsa.ID = nRow.ID;
                    PersistencyHelper.save(DelegatesHelper.verifyIfCanDeleteRH, argsPca, Nivel.DeleteNivelXInDataBase, argsPsa);
                    PersistencyHelper.cleanDeletedData(new List<TableDepthOrdered.TableCloudType>(new TableDepthOrdered.TableCloudType[] { PersistencyHelper.determinaNuvem("RelacaoHierarquica"), PersistencyHelper.determinaNuvem("FRDBase") }));
                    if (argsPca.deleteSuccessful)
                        this.nivelNavigator1.RemoveFromTreeview(node, key);
                    else
                        MessageBox.Show(argsPca.message, "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ((frmMain)TopLevelControl).LeaveWaitMode();
                }
                else
                {
                    if ((nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].IDTipoNivelRelacionado == TipoNivelRelacionado.D ||
                        nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].IDTipoNivelRelacionado == TipoNivelRelacionado.SD) &&
                        NiveisHelper.NivelFoiMovimentado(nRow.ID))
                    {
                        if (MessageBox.Show(
                                "Por favor tenha em aten��o que este n�vel documental j� foi " + System.Environment.NewLine +
                                "requisitado/devolvido. Ao remover n�vel documental ser�o perdidos " + System.Environment.NewLine +
                                "todos os seus registos referentes a requisi��es e devolu��es.", "Elimina��o de n�vel documental", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                            return;
                    }

                    ((frmMain)TopLevelControl).EnterWaitMode();

                    CurrentContext.RaiseRegisterModificationEvent(nRow.GetFRDBaseRows()[0]);

                    PersistencyHelper.canDeleteRHRowPreConcArguments argsPca = new PersistencyHelper.canDeleteRHRowPreConcArguments();
                    argsPca.nRowID = nRow.ID;
                    argsPca.nUpperRowID = nUpperRow.ID;
                    argsPca.rhRowID = 0;
                    argsPca.rhRowIDUpper = 0;
                    PersistencyHelper.DeleteIDXPreSaveArguments argsPsa = new PersistencyHelper.DeleteIDXPreSaveArguments();
                    argsPsa.ID = nRow.ID;
                    PersistencyHelper.SaveResult successfulSave = PersistencyHelper.save(DelegatesHelper.verifyIfCanDeleteRH, argsPca, Nivel.DeleteNivelXInDataBase, argsPsa, preTransactionAction);
                    PersistencyHelper.cleanDeletedData(new List<TableDepthOrdered.TableCloudType>(new TableDepthOrdered.TableCloudType[] { PersistencyHelper.determinaNuvem("RelacaoHierarquica"), PersistencyHelper.determinaNuvem("FRDBase"), PersistencyHelper.determinaNuvem("ObjetoDigital") }));
                    if (argsPca.deleteSuccessful)
                    {
                        if (successfulSave == PersistencyHelper.SaveResult.successful)
                        {
                            List<string> IDNiveis = new List<string>();
                            IDNiveis.Add(argsPsa.ID.ToString());
                            GISA.Search.Updater.updateNivelDocumental(IDNiveis);
                            if (nRow.RowState == DataRowState.Detached)
                                GISA.Search.Updater.updateNivelDocumentalComProdutores(argsPsa.ID);
                            else
                                GISA.Search.Updater.updateNivelDocumentalComProdutores(nRow.ID);

                            this.nivelNavigator1.RemoveSelectedLVItem();
                        }
                    }
                    else
                        MessageBox.Show(argsPca.message, "Elimina��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ((frmMain)TopLevelControl).LeaveWaitMode();
                }
            }

            RevalidateClipboardNivelItem();

            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && parentNode != null)
            {
                //remover o n� do controlo embora sem refrescar a interface
                this.nivelNavigator1.SelectParentNode(parentNode);
                UpdateContext();
            }
            else
            {
                //A selec��o foi limpa aquando da elimina��o do item
                UpdateToolBarButtons();
            }
        }

        private void CutNivel()
        {
            ToolBarButtonPaste.Enabled = false;

            if (this.nivelNavigator1.PanelToggleState != NivelNavigator.ToggleState.Documental) return;

            var nRows = this.nivelNavigator1.SelectedNiveis;
            var rhRows = nRows.SelectMany(r => r.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()).Where(rh => rh.IDUpper == this.nivelNavigator1.ContextBreadCrumbsPathID).ToList();

            if (nRows.Count != rhRows.Count)
                MessageBox.Show("Esta opera��o n�o pode ser conclu�da pelo facto de a localiza��o na estrutura " + System.Environment.NewLine +
                    "do n�vel selecionado ter sido alterada por outro utilizador.", "Recortar N�vel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                mClipboardNivelItems = rhRows;
        }

        private void PasteNivel()
        {
            var sources = mClipboardNivelItems;
            var sourceRows = sources.Select(r => r.NivelRowByNivelRelacaoHierarquica).ToList();
            var sourceIDs = sourceRows.Select(r => r.ID).ToList();
            var targetRow =
                this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural ?
                ((GISATreeNode)this.nivelNavigator1.SelectedNode).NivelRow :
                GisaDataSetHelper.GetInstance().Nivel.Cast<GISADataset.NivelRow>().Single(r => r.ID == this.nivelNavigator1.ContextBreadCrumbsPathID);

            var ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
            try
            {
                // carregar todas as rela��es hier�rquicas dos niveis cortados
                DBAbstractDataLayer.DataAccessRules.NivelRule.Current.LoadNivelRelacoesHierarquicas(GisaDataSetHelper.GetInstance(), sourceIDs, ho.Connection);
                // carregar toda a informa��o sobre permiss�es dos niveis cortados e do seu parent
                DBAbstractDataLayer.DataAccessRules.PermissoesRule.Current.LoadDataCIPermissoes(GisaDataSetHelper.GetInstance(), sourceIDs, ho.Connection);
                DBAbstractDataLayer.DataAccessRules.PermissoesRule.Current.LoadDataCIPermissoes(GisaDataSetHelper.GetInstance(), targetRow.ID, ho.Connection);
                // carregar toda a informa��o referente aos objetos digitais dos niveis cortados, do seu upper e do n�vel de destino
                if (SessionHelper.AppConfiguration.GetCurrentAppconfiguration().IsFedoraEnable())
                {
                    var idTipoNivelRelacionado = sources.First().IDTipoNivelRelacionado;
                    if (idTipoNivelRelacionado == TipoNivelRelacionado.SD) // se os documentos cortados forem subdocumentos, ent�o basta passar o nivel do pai que os subdocumentos s�o tb carregados
                        DBAbstractDataLayer.DataAccessRules.FedoraRule.Current.LoadObjDigitalData(GisaDataSetHelper.GetInstance(), sources.First().IDUpper, TipoNivelRelacionado.D, ho.Connection);
                    else
                        sourceIDs.ForEach(ID => DBAbstractDataLayer.DataAccessRules.FedoraRule.Current.LoadObjDigitalData(GisaDataSetHelper.GetInstance(), ID, idTipoNivelRelacionado, ho.Connection));

                    DBAbstractDataLayer.DataAccessRules.FedoraRule.Current.LoadObjDigitalData(GisaDataSetHelper.GetInstance(), targetRow.ID, targetRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().First().IDTipoNivelRelacionado, ho.Connection);
                }
            }
            catch (Exception ex) { Trace.WriteLine(ex.ToString()); throw ex; }
            finally
            {
                ho.Dispose();
            }

            // verificar se o nivel cortado for um documento solto com v�rios produtores e se vai ser colado debaixo de uma s�rie ou subs�rie
            var docsSoltos = new StringBuilder();
            sourceRows.ForEach(sourceRow =>
            {
                var rhRows = sourceRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica();
                var isDocumentoSolto = sourceRow.IDTipoNivel == TipoNivel.DOCUMENTAL && rhRows[0].NivelRowByNivelRelacaoHierarquicaUpper.IDTipoNivel == TipoNivel.ESTRUTURAL;

                if (isDocumentoSolto && rhRows.Count() > 1)
                    docsSoltos.Append(sourceRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine);
            }
            );

            if (docsSoltos.Length > 0)
            {
                var formReport = new FormDeletionReport();
                formReport.Text = "Mover n�vel";
                formReport.Interrogacao = "As seguintes unidades informacionais recortadas t�m mais do que um produtor." + System.Environment.NewLine +
                                "Ao completar esta opera��o ir� perder todas as rela��es destes n�veis com os respetivos produtores." + System.Environment.NewLine +
                                "Pretende continuar?";
                formReport.Detalhes = docsSoltos.ToString();
                var formResult = formReport.ShowDialog();

                if (formResult == DialogResult.Cancel) return;
            }

            try
            {
                Trace.WriteLine("A colar n�veis...");

                var argsPc = new PersistencyHelper.LstPasteRhXPreConcArguments();
                argsPc.pasteRhXPreConcArguments = new List<PersistencyHelper.PasteRhXPreConcArguments>();
                argsPc.IDTipoNivelRelacionado = sources.First().IDTipoNivelRelacionado;
                //var argsPs = new PersistencyHelper.LstPasteRhXPreSaveArguments();
                //argsPs.lstPasteRhXPreSaveArguments = new List<PersistencyHelper.PasteRhXPreSaveArguments>();
                //argsPs.IDTipoNivelRelacionado = sources.First().IDTipoNivelRelacionado;

                // Garantir que n�o exista ainda a rela��o que se pretende criar.
                // Essa situa��o pode existir por exemplo no caso de uma s�rie 
                // cont�nua em que se tente mover uma das rela��es para os mesmos 
                // dois n�s j� afectados pela outra rela��o.
                sourceRows.Select(r => r.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0])
                    .Where(r => r.IDUpper != targetRow.ID)
                    .Select(r => r.NivelRowByNivelRelacaoHierarquica).ToList()
                    .ForEach(sourceRow =>
                    {
                        // Actualizar localiza��o do n�vel colado
                        var rhRow = sourceRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0];
                        var newrhRow = GisaDataSetHelper.GetInstance().RelacaoHierarquica.NewRelacaoHierarquicaRow();
                        newrhRow.ID = rhRow.ID;
                        newrhRow.IDUpper = targetRow.ID;
                        newrhRow.IDTipoNivelRelacionado = rhRow.IDTipoNivelRelacionado;

                        var oldParent = rhRow.NivelRowByNivelRelacaoHierarquicaUpper;
                        var newParent = newrhRow.NivelRowByNivelRelacaoHierarquicaUpper;
                        if (oldParent.IDTipoNivel == TipoNivel.DOCUMENTAL && newParent.IDTipoNivel == TipoNivel.ESTRUTURAL)
                            PermissoesHelper.AddNewNivelGrantPermissions(sourceRow);
                        else if (oldParent.IDTipoNivel == TipoNivel.ESTRUTURAL && newParent.IDTipoNivel == TipoNivel.DOCUMENTAL)
                            PermissoesHelper.UndoAddNivelGrantPermissions(sourceRow, PermissoesHelper.GrpAcessoCompleto);

                        var aPc = new PersistencyHelper.PasteRhXPreConcArguments();
                        aPc.rhRowOldID = rhRow.ID;
                        aPc.rhRowOldIDUpper = rhRow.IDUpper;
                        aPc.rhRowNew = newrhRow;
                        aPc.nivel = targetRow;
                        aPc.ensureUniqueCodigoArgs = new PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments();
                        aPc.ensureUniqueCodigoArgs.nRowID = newrhRow.NivelRowByNivelRelacaoHierarquica.ID;
                        aPc.ensureUniqueCodigoArgs.ndRowID = newrhRow.NivelRowByNivelRelacaoHierarquica.GetNivelDesignadoRows()[0].ID;
                        aPc.ensureUniqueCodigoArgs.rhRowID = newrhRow.ID;
                        aPc.ensureUniqueCodigoArgs.rhRowIDUpper = newrhRow.IDUpper;
                        aPc.ensureUniqueCodigoArgs.testOnlyWithinNivel = true;

                        argsPc.pasteRhXPreConcArguments.Add(aPc);

                        var manageDocsPermissionsPreConcArguments = new PersistencyHelper.ManageDocsPermissionsPreConcArguments();
                        manageDocsPermissionsPreConcArguments.nRow = sourceRow;
                        manageDocsPermissionsPreConcArguments.oldParentRow = rhRow.NivelRowByNivelRelacaoHierarquicaUpper;
                        manageDocsPermissionsPreConcArguments.newParentRow = targetRow;


                        //var pasteRhXPreSaveArguments = new PersistencyHelper.PasteRhXPreSaveArguments();
                        aPc.manageDocsPermissionsArgs = manageDocsPermissionsPreConcArguments;

                        if (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
                        {
                            var setNivelOrderPreConcArguments = new PersistencyHelper.SetNivelOrderPreConcArguments();
                            setNivelOrderPreConcArguments.nRowID = sourceRow.ID;
                            setNivelOrderPreConcArguments.nRowIDUpper = targetRow.ID;
                            aPc.setNivelOrderPreConcArguments = setNivelOrderPreConcArguments;
                        }

                        //aPc.pasteRhXPreSaveArguments = pasteRhXPreSaveArguments;
                        //argsPs.lstPasteRhXPreSaveArguments.Add(pasteRhXPreSaveArguments);

                        PersistencyHelper.UpdatePermissionsPostSaveArguments argsPostSave = new PersistencyHelper.UpdatePermissionsPostSaveArguments();
                        aPc.updatePermissionsPostSaveArgs = argsPostSave;
                    }
                );

                PostSaveAction postSaveAction = new PostSaveAction();
                PersistencyHelper.UpdatePermissionsPostSaveArguments argPostSave = new PersistencyHelper.UpdatePermissionsPostSaveArguments();
                postSaveAction.args = argPostSave;
                postSaveAction.postSaveDelegate = delegate(PersistencyHelper.PostSaveArguments postSaveArgs)
                {
                    // registar a edi��o dos n�veis cuja opera��o foi bem sucedida
                    var noErrorArgs = argsPc.pasteRhXPreConcArguments.Where(a => a.PasteError == PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NoError).ToList();
                    noErrorArgs.ForEach(ID => CurrentContext.RaiseRegisterModificationEvent(ID.nivel.GetFRDBaseRows()[0]));

                    PersistencyHelperRule.Current.saveRows(GisaDataSetHelper.GetInstance().FRDBaseDataDeDescricao,
                        GisaDataSetHelper.GetInstance().FRDBaseDataDeDescricao.Cast<GISADataset.FRDBaseDataDeDescricaoRow>().Where(frd => frd.RowState == DataRowState.Added).ToArray(), postSaveArgs.tran);
                };

                PersistencyHelper.SaveResult successfulSave = PersistencyHelper.save(verifyIfCanPaste, argsPc, postSaveAction, true);
                PersistencyHelper.cleanDeletedData(PersistencyHelper.determinaNuvem("Nivel"));

                mClipboardNivelItems.Clear();
                ToolBarButtonPaste.Enabled = false;

                var nDeletedArg = argsPc.pasteRhXPreConcArguments.FirstOrDefault(a => a.PasteError == PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NDeleted);
                if (nDeletedArg != null)
                {
                    MessageBox.Show(nDeletedArg.message, "Elimina��o de n�vel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var errorAvaliacao = string.Empty;
                var errorRHDeleted = string.Empty;
                var errorNotUniqueCodigo = string.Empty;
                var errorNivelDeleted = string.Empty;
                var errorObjDigital = string.Empty;
                foreach (var arg in argsPc.pasteRhXPreConcArguments)
                {
                    switch (arg.PasteError)
                    {
                        case PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.Avaliacao:
                            if (errorAvaliacao.Length == 0)
                                errorAvaliacao += arg.message + System.Environment.NewLine;
                            errorAvaliacao += arg.manageDocsPermissionsArgs.nRow.Codigo + " - " + arg.manageDocsPermissionsArgs.nRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine;
                            break;
                        case PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.RHDeleted:
                            if (errorRHDeleted.Length == 0)
                                errorRHDeleted += arg.message + System.Environment.NewLine;
                            errorRHDeleted += arg.manageDocsPermissionsArgs.nRow.Codigo + " - " + arg.manageDocsPermissionsArgs.nRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine;
                            break;
                        case PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NotUniqueCodigo:
                            if (errorNotUniqueCodigo.Length == 0)
                                errorNotUniqueCodigo += arg.message + System.Environment.NewLine;
                            errorNotUniqueCodigo += arg.manageDocsPermissionsArgs.nRow.Codigo + " - " + arg.manageDocsPermissionsArgs.nRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine;
                            break;
                        case PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.ObjDigital:
                            if (errorObjDigital.Length == 0)
                                errorObjDigital += arg.message + System.Environment.NewLine;
                            errorObjDigital += arg.manageDocsPermissionsArgs.nRow.Codigo + " - " + arg.manageDocsPermissionsArgs.nRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine;
                            break;
                        default:
                            if (arg.nivel.RowState == DataRowState.Detached)
                            {
                                if (errorNivelDeleted.Length == 0)
                                    errorNivelDeleted += arg.message + System.Environment.NewLine;
                                errorNivelDeleted += arg.manageDocsPermissionsArgs.nRow.Codigo + " - " + arg.manageDocsPermissionsArgs.nRow.GetNivelDesignadoRows()[0].Designacao + System.Environment.NewLine;
                            }
                            break;
                    }
                }

                if (errorAvaliacao.Length > 0 || errorRHDeleted.Length > 0 || errorNotUniqueCodigo.Length > 0 || errorNivelDeleted.Length > 0)
                {
                    var message = "Para os n�veis seguintes n�o foi poss�vel completar a opera��o.";

                    FormDeletionReport form = new FormDeletionReport();
                    form.Text = "Mover n�veis";
                    form.Interrogacao = message;
                    form.Detalhes = errorAvaliacao + errorRHDeleted + errorNotUniqueCodigo + errorNivelDeleted;
                    form.SetBtnOKVisible(false);
                    form.ShowDialog();
                }

                // Cut/Paste de niveis documentais bem sucedidos
                if (successfulSave == PersistencyHelper.SaveResult.successful)
                {
                    var IDs = argsPc.pasteRhXPreConcArguments.Where(a => a.PasteError == PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NoError).ToList();

                    // recarregar a lista de niveis
                    if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental)
                        this.nivelNavigator1.ReloadList();

                    List<string> IDNiveis = new List<string>();
                    IDNiveis.AddRange(IDs.Select(r => r.nivel.ID.ToString()));
                    GISA.Search.Updater.updateNivelDocumental(IDNiveis);
                    GISA.Search.Updater.updateNivelDocumentalComProdutores(IDNiveis);

                    // TODO: arranjar um s�tio melhor para fazer a ingest�o
                    string msg = null;

                    bool ingestSuccess = true;
                    if (argsPc.ODsToIngest != null)
                        argsPc.ODsToIngest.ForEach(od => ingestSuccess &= SessionHelper.AppConfiguration.GetCurrentAppconfiguration().FedoraHelperSingleton.Ingest(od, out msg));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                MessageBox.Show("N�o foi poss�vel realizar esta opera��o.");
            }
        }

        private void init_geracao_EAD()
        {
            GISADataset.RelacaoHierarquicaRow rhRow = null;
            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && this.nivelNavigator1.SelectedNode != null)
                rhRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).RelacaoHierarquicaRow;
            else if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental)
            {
                if (this.nivelNavigator1.SelectedNivel != null)
                    rhRow = ((GISADataset.NivelRow)(this.nivelNavigator1.SelectedNivel)).GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0];
            }

            if (rhRow != null)
            {
                GisaDataSetHelper.HoldOpen ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
                try
                {
                    EADGenerator_GUI eadGen = new EADGenerator_GUI();
                    eadGen.launch_EAD_generator(ho.Connection, rhRow.IDUpper, rhRow.ID, getFilename("EAD_ID_" + rhRow.ID) + ".xml", TopLevelControl);
                }
                finally { ho.Dispose(); }
            }
        }

        private void init_importacao()
        {
            if (PersistencyHelper.hasCurrentDatasetChanges())
            {
                var result = MessageBox.Show("O contexto atual foi editado e precisa ser gravado para se proceder � importa��o." + System.Environment.NewLine + "Pretende continuar?", "Importa��o", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel) return;


                GISADataset.RelacaoHierarquicaRow rhRow = null;
                if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && this.nivelNavigator1.SelectedNode != null)
                    rhRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).RelacaoHierarquicaRow;
                else if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental)
                    rhRow = ((GISADataset.NivelRow)(this.nivelNavigator1.SelectedNivel)).GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0];

                CurrentContext.RaiseRegisterModificationEvent(rhRow.NivelRowByNivelRelacaoHierarquica.GetFRDBaseRows().First());

                ((frmMain)TopLevelControl).EnterWaitMode();
                PersistencyHelper.save();
                PersistencyHelper.cleanDeletedData();
                ((frmMain)TopLevelControl).LeaveWaitMode();
            }

            string fileLocation = ImportFromExcelHelper.GetFileToImport();

            if (fileLocation.Length == 0) return;

            ((frmMain)TopLevelControl).EnterWaitMode();
            ImportFromExcelHelper.ImportFromExcel(fileLocation);
            ((frmMain)TopLevelControl).LeaveWaitMode();
        }
        #endregion
        

        void nivelNavigator1_KeyUpDeleteEvent(EventArgs e)
        {
            if (isSelectedNivelRemovable)
                RemoveNivel();
        }

        

        private void ToolBarButtonCreateMenuItemClick(object sender, EventArgs e)
		{
            ((frmMain)TopLevelControl).EnterWaitMode();

			GISADataset.TipoNivelRelacionadoRow tnrRow = ((TipoNivelMenuItem)sender).Row;

			FormAddNivel frm = null;
			GISADataset.NivelRow nivelRow = null;
			if (tnrRow.TipoNivelRow.ID == TipoNivel.LOGICO)
			{
				frm = new FormAddNivel();
                nivelRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).NivelRow;
			}
			else if (tnrRow.TipoNivelRow.ID == TipoNivel.DOCUMENTAL)
			{
				// N�vel documental
				frm = new FormNivelDocumental();
                nivelRow = (GISADataset.NivelRow)(GisaDataSetHelper.GetInstance().Nivel.Select(string.Format("ID={0}", this.nivelNavigator1.ContextBreadCrumbsPathID.ToString()))[0]);
                ((FormNivelDocumental)frm).NivelRow = nivelRow;
			}
			else
			{
				// N�vel estrutural
				if (GisaDataSetHelper.UsingNiveisOrganicos())
				{
					frm = new FormNivelEstrutural();
					((FormNivelEstrutural)frm).caList.txtFiltroDesignacao.Clear();
					((FormNivelEstrutural)frm).caList.ReloadList();
				}
				else
					frm = new FormNivelDocumental();

                nivelRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).NivelRow;
			}
            long click = DateTime.Now.Ticks;
			frm.IDTipoNivelRelacionado = tnrRow.ID; // necess�rio para valida��o do c�digo parcial
			bool successfulSave = handleNewNivel(frm, nivelRow, tnrRow);
            Debug.WriteLine("<<NewNivel>> " + new TimeSpan(DateTime.Now.Ticks - click).ToString());

            ((frmMain)TopLevelControl).LeaveWaitMode();

			// O UpdateToolBarButtons n�o � executado quando � adicionado um novo n�vel documental pois 
			// esta opera��o j� � feita durante a inser��o do n� (caso essa inser��o n�o aconte�a por 
			// motivos de conflito de concorr�ncia o m�todo pode ser executado de forma a impedir o 
			// acesso de op��es ao utilizador que poderiam levar a um crash da aplica��o). Executar o 
			// UpdateToolBarButtons neste ponto para a situa��o acima mencionada vai alterar 
			// erradamente o estado dos bot�es
            if ((this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && successfulSave) || (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && !successfulSave))
			{
                if (this.nivelNavigator1.SelectedItems.Count > 0)
                    UpdateToolBarButtons(this.nivelNavigator1.SelectedItems[0]);
				else
					UpdateToolBarButtons();
			}
		}

		// Trata a cria��o de novos n�veis e respectivas rela��es. Caso se trate 
		// de um n�vel org�nico (estrutural e que esteja associado a uma EP) o 
		// n�vel correspondente dever� j� existir e n�o ser� por isso criado, 
		// ser� criada apenas a rela��o.
		private bool handleNewNivel(Form frm, GISADataset.NivelRow parentNivelRow, GISADataset.TipoNivelRelacionadoRow tnrRow)
		{
			frm.Text = "Criar " + tnrRow.Designacao;

			// se se tratar de uma s�rie ou subs�rie
			if (tnrRow.ID == TipoNivelRelacionado.SR || tnrRow.ID == TipoNivelRelacionado.SSR)
			{
				FormNivelDocumental frmDoc = (FormNivelDocumental)frm;
				frmDoc.grpCodigo.Text += " previsto";
				frmDoc.txtCodigo.Enabled = false;

				GisaDataSetHelper.HoldOpen ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
				try
				{
					NivelRule.Current.FillTipoNivelRelacionadoCodigo(GisaDataSetHelper.GetInstance(), ho.Connection);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					throw;
				}
				finally
				{
					ho.Dispose();
				}
                frmDoc.txtCodigo.Text = NiveisHelper.getNextSeriesCodigo(false);
			}

			bool successfulSave = true;
			switch (frm.ShowDialog())
			{
				case DialogResult.OK:
				{
					Trace.WriteLine("A criar n�vel...");
                    long click = DateTime.Now.Ticks;
					GISADataset.NivelRow nRow = null;
					GISADataset.NivelDesignadoRow ndRow = null;
					GISADataset.NivelControloAutRow ncaRow = null;
					GISADataset.FRDBaseRow frdRow = null;

					string designacaoUFAssociada = string.Empty;
					bool addNewUF = false;

                    PostSaveAction postSaveAction = null;

					// Create a new Nivel with or without a Not�cia autoridade
					if (frm is FormNivelEstrutural && ((FormNivelEstrutural)frm).chkControloAut)
					{
						GISADataset.ControloAutRow caRow = null;
						caRow = ((GISADataset.ControloAutDicionarioRow)(((FormNivelEstrutural)frm). caList.SelectedItems[0].Tag)).ControloAutRow;

						GisaDataSetHelper.HoldOpen ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
						try
						{
							DBAbstractDataLayer.DataAccessRules.NivelRule.Current.LoadNivelByControloAut(caRow.ID, GisaDataSetHelper.GetInstance(), ho.Connection);
						}
						finally
						{
							ho.Dispose();
						}

						ncaRow = caRow.GetNivelControloAutRows()[0];
						nRow = ncaRow.NivelRow;

						// Impedir cria��o de rela��es repetidas entre niveis 
						// estruturais e n�veis "logicos"
						if (GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1} OR ID={1} AND IDUpper={0}", parentNivelRow.ID, nRow.ID)).Length > 0)
						{
							MessageBox.Show("A rela��o pretendida j� existe e n�o pode ser duplicada.", "Adi��o de rela��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return false;
						}
					}
					else
					{
						nRow = GisaDataSetHelper.GetInstance().Nivel. AddNivelRow(tnrRow.TipoNivelRow, ((FormAddNivel)frm).txtCodigo.Text.Trim(), "NVL", new byte[]{}, 0);
						ndRow = GisaDataSetHelper.GetInstance().NivelDesignado. AddNivelDesignadoRow(nRow, ((FormAddNivel)frm).txtDesignacao.Text.Trim(), new byte[]{}, 0);

                        if (nRow.IDTipoNivel != TipoNivel.LOGICO)
						    frdRow = GisaDataSetHelper.GetInstance().FRDBase.AddFRDBaseRow(nRow, (GISADataset.TipoFRDBaseRow)(GisaDataSetHelper.GetInstance().TipoFRDBase. Select("ID=" + DomainValuesHelper.stringifyEnumValue(TipoFRDBase.FRDOIRecolha))[0]), "", "", new byte[]{}, 0);

						if (nRow.IDTipoNivel == TipoNivel.DOCUMENTAL && ((FormNivelDocumental)frm).CreateUFAssociada)
						{
							designacaoUFAssociada = ((FormNivelDocumental)frm).DesignacaoUF;
							addNewUF = true;
						}
					}

                    // valores por omiss�o
                    var globalConfig = GisaDataSetHelper.GetInstance().GlobalConfig.Cast<GISADataset.GlobalConfigRow>().Single();
                    if (globalConfig.ApplyDefaultValues && nRow.IDTipoNivel == TipoNivel.DOCUMENTAL)
                    {
                        var sfrdcaRow = GisaDataSetHelper.GetInstance().SFRDCondicaoDeAcesso
                            .AddSFRDCondicaoDeAcessoRow(frdRow, "", globalConfig.IsCondicaoDeAcessoNull() ? "" : globalConfig.CondicaoDeAcesso,
                            globalConfig.IsCondicaoDeReproducaoNull() ? "" : globalConfig.CondicaoDeReproducao, "", new byte[] { }, 0);

                        foreach (GISADataset.ConfigLinguaRow r in globalConfig.GetConfigLinguaRows())
                            GisaDataSetHelper.GetInstance().SFRDLingua.AddSFRDLinguaRow(sfrdcaRow, r.Iso639Row, new byte[] { }, 0);

                        foreach (GISADataset.ConfigAlfabetoRow r in globalConfig.GetConfigAlfabetoRows())
                            GisaDataSetHelper.GetInstance().SFRDAlfabeto.AddSFRDAlfabetoRow(sfrdcaRow, r.Iso15924Row, new byte[] { }, 0);
                    }

					GISADataset.RelacaoHierarquicaRow rhRow = null;
					// garantir que os n�s raiz n�o s�o criados com pais
					if (tnrRow.ID != TipoNivelRelacionado.ED)
						rhRow = GisaDataSetHelper.GetInstance().RelacaoHierarquica. AddRelacaoHierarquicaRow(nRow, parentNivelRow, tnrRow, null, null, null, null, null, null, null, new byte[]{}, 0);

					// S� adicionar permiss�es ao grupo TODOS dos n�veis l�gicos e a n�veis documentais imediatamente
					// abaixo de n�veis org�nicos (Documentos soltos e s�ries); caso se se trate de um n�vel estrutural 
					// controlado, as permiss�es j� foram atribuidas aquando da cria��o do controlo de autoridade 
					if (nRow.IDTipoNivel == TipoNivel.LOGICO || (nRow.IDTipoNivel == TipoNivel.DOCUMENTAL && parentNivelRow.IDTipoNivel == TipoNivel.ESTRUTURAL))
					{
                        var nUpperRow = rhRow == null ? default(GISADataset.NivelRow) : rhRow.NivelRowByNivelRelacaoHierarquicaUpper;
                        PermissoesHelper.AddNewNivelGrantPermissions(nRow, nUpperRow);
					}

                    // actualizar permiss�es impl�citas
                    postSaveAction = new PostSaveAction();
                    PersistencyHelper.UpdatePermissionsPostSaveArguments args = new PersistencyHelper.UpdatePermissionsPostSaveArguments();
                    postSaveAction.args = args;

                    postSaveAction.postSaveDelegate = delegate(PersistencyHelper.PostSaveArguments postSaveArgs)
                    {
                        if (!postSaveArgs.cancelAction && nRow != null && nRow.RowState != DataRowState.Detached && nRow.RowState != DataRowState.Deleted)
                        {
                            if (addNewUF)
                            {
                                // registar a cria��o da unidade f�sica
                                GISADataset.FRDBaseRow frdUFRow =
                                    nRow.GetFRDBaseRows()[0].GetSFRDUnidadeFisicaRows()[0].NivelRow.GetFRDBaseRows()[0];
                                CurrentContext.RaiseRegisterModificationEvent(frdUFRow);
                            }

                            // registar a cria��o do nivel documental
                            GISADataset.FRDBaseRow frdDocRow = null;
                            GISADataset.FRDBaseRow[] frdDocRows = nRow.GetFRDBaseRows();
                            if (frdDocRows.Length > 0)
                                frdDocRow = frdDocRows[0];
                            CurrentContext.RaiseRegisterModificationEvent(frdDocRow);

                            PersistencyHelperRule.Current.saveRows(GisaDataSetHelper.GetInstance().FRDBaseDataDeDescricao,
                                GisaDataSetHelper.GetInstance().FRDBaseDataDeDescricao.Cast<GISADataset.FRDBaseDataDeDescricaoRow>().Where(frd => frd.RowState == DataRowState.Added).ToArray(), postSaveArgs.tran);
                        }
                    };

					// se se tratar de uma s�rie ou subs�rie
					if (tnrRow.ID == TipoNivelRelacionado.SR || tnrRow.ID == TipoNivelRelacionado.SSR)
					{
						// � necess�rio garantir que o c�digo gerado ainda n�o est� 
						// em uso, por isso geramo-lo dentro da pr�pria transac��o                        
                        
						PersistencyHelper.ValidateNivelAddAndAssocNewUFPreConcArguments pcArgs = new PersistencyHelper.ValidateNivelAddAndAssocNewUFPreConcArguments();                                                
						PersistencyHelper.SetNewCodigosPreSaveArguments psArgs = new PersistencyHelper.SetNewCodigosPreSaveArguments();                        
						PersistencyHelper.VerifyIfRHNivelUpperExistsPreConcArguments pcArgsNivel = new PersistencyHelper.VerifyIfRHNivelUpperExistsPreConcArguments();
						PersistencyHelper.FetchLastCodigoSeriePreSaveArguments psArgsNivel = new PersistencyHelper.FetchLastCodigoSeriePreSaveArguments();
						PersistencyHelper.AddEditUFPreConcArguments pcArgsUF = new PersistencyHelper.AddEditUFPreConcArguments();
						PersistencyHelper.IsCodigoUFBeingUsedPreSaveArguments psArgsUF = new PersistencyHelper.IsCodigoUFBeingUsedPreSaveArguments();
                        

						pcArgs.argsNivel = pcArgsNivel;
						pcArgs.argsUF = pcArgsUF;

						psArgs.argsNivel = psArgsNivel;
						psArgs.argsUF = psArgsUF;

						// dados que ser�o usados no delegate respons�vel pela cria��o do n�vel documental
						pcArgsNivel.nRowID = nRow.ID;
						pcArgsNivel.ndRowID = ndRow.ID;
						pcArgsNivel.rhRowID = rhRow.ID;
						pcArgsNivel.rhRowIDUpper = rhRow.IDUpper;
						pcArgsNivel.frdBaseID = frdRow.ID;

						// dados para a atribui��o de um c�digo ao n�vel documental
						psArgsNivel.nRowID = nRow.ID;
						psArgsNivel.pcArgs = pcArgsNivel;

						if (addNewUF)
						{
							// dados que ser�o usados no delegate respons�vel pela cria��o da unidade f�sica
							pcArgsUF.Operation = PersistencyHelper.AddEditUFPreConcArguments.Operations.Create;
							pcArgsUF.psa = psArgsUF;

							// dados que ser�o usados no delegate que far� a associa��o entre o n�vel documental e unidade f�sica
							pcArgs.addNewUF = true;
							pcArgs.IDFRDBaseNivelDoc = frdRow.ID;
                            pcArgs.produtor = this.nivelNavigator1.SelectedNode;
							pcArgs.designacaoUFAssociada = designacaoUFAssociada;
						}

						// permitir ao delegate selecionar o delegate correspondente ao tipo de n�vel que se est� a criar
						pcArgs.IDTipoNivelRelacionado = tnrRow.ID;

						psArgs.createNewNivelCodigo = true;
						psArgs.createNewUFCodigo = addNewUF;

                        PersistencyHelper.save(DelegatesHelper.ValidateNivelAddAndAssocNewUF, pcArgs, DelegatesHelper.SetNewCodigos, psArgs, postSaveAction);
                        
						if (! pcArgsNivel.RHNivelUpperExists)
						{
							successfulSave = false;
							MessageBox.Show(pcArgsNivel.message, "Cria��o de unidade de descri��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
                        else if (addNewUF && pcArgsUF.OperationError == PersistencyHelper.AddEditUFPreConcArguments.OperationErrors.NewUF)
                            MessageBox.Show(pcArgsUF.message, "Criar unidade f�sica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else if (tnrRow.ID == TipoNivelRelacionado.D || tnrRow.ID == TipoNivelRelacionado.SD)
					{                     
						// se se tratar de um (sub)documento � necess�rio garantir que se trata de um c�digo 
						// �nico dentro da sua s�rie (se constituir s�rie) ou nivel estrutural superior
						PersistencyHelper.ValidateNivelAddAndAssocNewUFPreConcArguments pcArgs = new PersistencyHelper.ValidateNivelAddAndAssocNewUFPreConcArguments();                                               
						PersistencyHelper.SetNewCodigosPreSaveArguments psArgs = new PersistencyHelper.SetNewCodigosPreSaveArguments();
						PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments pcArgsNivel = new PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments();
						PersistencyHelper.AddEditUFPreConcArguments pcArgsUF = new PersistencyHelper.AddEditUFPreConcArguments();
						PersistencyHelper.IsCodigoUFBeingUsedPreSaveArguments psArgsUF = new PersistencyHelper.IsCodigoUFBeingUsedPreSaveArguments();

						pcArgs.argsNivel = pcArgsNivel;
						pcArgs.argsUF = pcArgsUF;

						// dados que ser�o usados no delegate respons�vel pela cria��o do n�vel documental
						pcArgsNivel.nRowID = nRow.ID;
						pcArgsNivel.ndRowID = ndRow.ID;
						pcArgsNivel.rhRowID = rhRow.ID;
						pcArgsNivel.rhRowIDUpper = rhRow.IDUpper;
						pcArgsNivel.frdBaseID = frdRow.ID;
						pcArgsNivel.testOnlyWithinNivel = true;

						if (addNewUF)
						{
							// dados que ser�o usados no delegate respons�vel pela cria��o da unidade f�sica
							pcArgsUF.Operation = PersistencyHelper.AddEditUFPreConcArguments.Operations.Create;
							pcArgsUF.psa = psArgsUF;

							// dados que ser�o usados no delegate que far� a associa��o entre o n�vel documental e unidade f�sica
							pcArgs.addNewUF = true;
							pcArgs.IDFRDBaseNivelDoc = frdRow.ID;
                            pcArgs.produtor = this.nivelNavigator1.SelectedNode;
							pcArgs.designacaoUFAssociada = designacaoUFAssociada;
						}

						// permitir ao delegate selecionar o delegate correspondente ao tipo de n�vel que se est� a criar
						pcArgs.IDTipoNivelRelacionado = tnrRow.ID;

						psArgs.createNewNivelCodigo = false;
						psArgs.createNewUFCodigo = addNewUF;
                        psArgs.setNewCodigo = rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SD;
                        psArgs.argsUF = psArgsUF;
                        psArgs.argsNivelDocSimples = NiveisHelper.AddNivelDocumentoSimplesWithDelegateArgs(nRow.GetNivelDesignadoRows().Single(), rhRow.IDUpper, rhRow.IDTipoNivelRelacionado);

                        PersistencyHelper.save(DelegatesHelper.ValidateNivelAddAndAssocNewUF, pcArgs, DelegatesHelper.SetNewCodigos, psArgs, postSaveAction);
						if (! pcArgsNivel.successful)
						{
							successfulSave = false;
							MessageBox.Show(pcArgsNivel.message, "Cria��o de unidade de descri��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
						else if (parentNivelRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].TipoNivelRelacionadoRow.ID == TipoNivelRelacionado.SR)
						{
							GisaDataSetHelper.HoldOpen ho = new GisaDataSetHelper.HoldOpen(GisaDataSetHelper.GetConnection());
							try
							{
								DBAbstractDataLayer.DataAccessRules.FRDRule.Current.LoadSFRDAvaliacaoData(GisaDataSetHelper.GetInstance(), parentNivelRow.ID, ho.Connection);
							}
							finally
							{
								ho.Dispose();
							}
						}
					}
					else if (nRow.IDTipoNivel == TipoNivel.ESTRUTURAL && ! (nRow.CatCode.Trim().Equals("CA")))
					{                     
						// se se tratar de um nivel estrutural tem�tico-funcional
						// � necess�rio garantir que se trata de um c�digo �nico no sistema
						PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments pcArgs = new PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments();
						pcArgs.nRowID = nRow.ID;
						pcArgs.ndRowID = ndRow.ID;
						pcArgs.rhRowID = rhRow.ID;
						pcArgs.rhRowIDUpper = rhRow.IDUpper;
                        PersistencyHelper.save(DelegatesHelper.ensureUniqueCodigo, pcArgs, postSaveAction);
						if (! pcArgs.successful)
						{
							successfulSave = false;
							MessageBox.Show(pcArgs.message, "Cria��o de unidade de descri��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
					else if (nRow.IDTipoNivel == TipoNivel.LOGICO && nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().Length == 0)
					{
                     	// se se tratar de uma entidade detentora
						// � necess�rio garantir que se trata de um c�digo �nico no sistema
						PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments pcArgs = new PersistencyHelper.EnsureUniqueCodigoNivelPreConcArguments();
						pcArgs.nRowID = nRow.ID;
						pcArgs.ndRowID = ndRow.ID;
						pcArgs.testOnlyWithinNivel = true;
                        PersistencyHelper.save(DelegatesHelper.ensureUniqueCodigo, pcArgs, postSaveAction);
                        if (!pcArgs.successful)
                        {
                            successfulSave = false;
                            MessageBox.Show(pcArgs.message, "Cria��o de unidade de descri��o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            // for�ar o refresh das entidades produtoras (qualquer outro que esteja expandido
                            // vai ser colapsado)
                            resetEstrutura();
                        }

					}
                    else if (nRow.IDTipoNivel == TipoNivel.ESTRUTURAL && nRow.CatCode.Trim().Equals("CA"))
                    {
                        GISADataset.TipoFRDBaseRow tipoFRD = 
                            (GISADataset.TipoFRDBaseRow)(GisaDataSetHelper.GetInstance().TipoFRDBase.Select("ID=" + DomainValuesHelper.stringifyEnumValue(TipoFRDBase.FRDOIRecolha))[0]);

                        if (GisaDataSetHelper.GetInstance().FRDBase.Select(string.Format("IDNivel={0} AND IDTipoFRDBase={1}", nRow.ID.ToString(), tipoFRD.ID.ToString())).Length == 0)
                        {
                            GISADataset.FRDBaseRow frdNivelDocRow = GisaDataSetHelper.GetInstance().FRDBase.AddFRDBaseRow(nRow, tipoFRD, "", "", new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDDatasProducao.AddSFRDDatasProducaoRow(frdNivelDocRow, "", "", "", "", false, "", "", "", "", false, new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDConteudoEEstrutura.AddSFRDConteudoEEstruturaRow(frdNivelDocRow, "", "", new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDContexto.AddSFRDContextoRow(frdNivelDocRow, "", "", "", false, new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDDocumentacaoAssociada.AddSFRDDocumentacaoAssociadaRow(frdNivelDocRow, "", "", "", "", new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDDimensaoSuporte.AddSFRDDimensaoSuporteRow(frdNivelDocRow, "", new byte[] { }, 0);
                            GisaDataSetHelper.GetInstance().SFRDNotaGeral.AddSFRDNotaGeralRow(frdNivelDocRow, "", new byte[] { }, 0);
                            var CurrentSFRDAvaliacao = GisaDataSetHelper.GetInstance().SFRDAvaliacao.NewSFRDAvaliacaoRow();
                            CurrentSFRDAvaliacao.FRDBaseRow = frdNivelDocRow;
                            CurrentSFRDAvaliacao.IDPertinencia = 1;
                            CurrentSFRDAvaliacao.IDDensidade = 1;
                            CurrentSFRDAvaliacao.IDSubdensidade = 1;
                            CurrentSFRDAvaliacao.Publicar = false;
                            CurrentSFRDAvaliacao.Observacoes = "";
                            CurrentSFRDAvaliacao.AvaliacaoTabela = false;
                            GisaDataSetHelper.GetInstance().SFRDAvaliacao.AddSFRDAvaliacaoRow(CurrentSFRDAvaliacao);
                            var sfrdcda = GisaDataSetHelper.GetInstance().SFRDCondicaoDeAcesso.AddSFRDCondicaoDeAcessoRow(frdNivelDocRow, "", "", "", "", new byte[] { }, 0);

                            var caRow = nRow.GetNivelControloAutRows().Single().ControloAutRow;
                            if (!caRow.IsIDIso639p2Null())
                                GisaDataSetHelper.GetInstance().SFRDLingua.AddSFRDLinguaRow(sfrdcda, caRow.Iso639Row, new byte[] { }, 0);
                            else if (globalConfig.ApplyDefaultValues)
                            {
                                foreach (GISADataset.ConfigLinguaRow r in globalConfig.GetConfigLinguaRows())
                                    GisaDataSetHelper.GetInstance().SFRDLingua.AddSFRDLinguaRow(sfrdcda, r.Iso639Row, new byte[] { }, 0);
                            }
                            
                            if (!caRow.IsIDIso15924Null())
                                GisaDataSetHelper.GetInstance().SFRDAlfabeto.AddSFRDAlfabetoRow(sfrdcda, caRow.Iso15924Row, new byte[] { }, 0);
                            else if (globalConfig.ApplyDefaultValues)
                            {
                                foreach (GISADataset.ConfigAlfabetoRow r in globalConfig.GetConfigAlfabetoRows())
                                    GisaDataSetHelper.GetInstance().SFRDAlfabeto.AddSFRDAlfabetoRow(sfrdcda, r.Iso15924Row, new byte[] { }, 0);
                            }
                        }

                        var sucessfulSave = PersistencyHelper.save(postSaveAction);
                        if(sucessfulSave == PersistencyHelper.SaveResult.successful)
                            GISA.Search.Updater.updateProdutor(nRow.GetNivelControloAutRows()[0].IDControloAut);

                    }
                    else
                        PersistencyHelper.save(postSaveAction);

                    PersistencyHelper.cleanDeletedData(new List<TableDepthOrdered.TableCloudType>(new TableDepthOrdered.TableCloudType[] { PersistencyHelper.determinaNuvem("RelacaoHierarquica"), PersistencyHelper.determinaNuvem("FRDBase") }));

					if (! successfulSave)
						return successfulSave;

					// Para as EDs
					if (rhRow == null)
					{
						//update data
						resetEstrutura();
					}
					else
					{
                        if (addNewUF)
                        {
                            // registar a cria��o da unidade f�sica
                            GISADataset.FRDBaseRow frdUFRow =
                                nRow.GetFRDBaseRows()[0].GetSFRDUnidadeFisicaRows()[0].NivelRow.GetFRDBaseRows()[0];
                            GISA.Search.Updater.updateUnidadeFisica(frdUFRow.IDNivel);
                        }

                        if (nRow.IDTipoNivel == TipoNivel.DOCUMENTAL)
                        {
                            GISA.Search.Updater.updateNivelDocumentalComProdutores(nRow.ID);
                            GISA.Search.Updater.updateNivelDocumental(nRow.ID);
                        }

                        if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
                            this.nivelNavigator1.RefreshTreeViewControlSelectedBranch();
						else
                            this.nivelNavigator1.AddNivel(nRow);
					}
                    Debug.WriteLine("<<A criar n�vel...>> " + new TimeSpan(DateTime.Now.Ticks - click).ToString());
					break;
				}
				case DialogResult.Cancel:
				{
					successfulSave = false;
					break;
				}
			}

			return successfulSave;
		}       

	#region  save delegates 
        
		// Utilizado no contexto dos pastes
		private void verifyIfCanPaste(PersistencyHelper.PreConcArguments args)
		{
            var lstRHPca = args as PersistencyHelper.LstPasteRhXPreConcArguments;

            var odCompSourceRow = default(GISADataset.ObjetoDigitalRow);
            var odCompTargetRow = default(GISADataset.ObjetoDigitalRow);
            var odCompSource = default(ObjDigComposto);
            var odCompTarget = default(ObjDigComposto);
            var odsToIngest = new List<ObjDigital>();
            var cutPasteODs = lstRHPca.IDTipoNivelRelacionado == TipoNivelRelacionado.SD && SessionHelper.AppConfiguration.GetCurrentAppconfiguration().IsFedoraEnable() && lstRHPca.pasteRhXPreConcArguments.Count > 0;
            var sourceUpperRow = default(GISADataset.NivelRow);
            var targetRow = default(GISADataset.NivelRow);

            if (lstRHPca.pasteRhXPreConcArguments.Count > 0)
            {
                sourceUpperRow = lstRHPca.pasteRhXPreConcArguments.First().manageDocsPermissionsArgs.oldParentRow;
                targetRow = lstRHPca.pasteRhXPreConcArguments.First().manageDocsPermissionsArgs.newParentRow;
            }

            if (cutPasteODs)
            {
                // obter ODs Compostos dos doc/procs de origem e destino (no caso de n�o haver as variaveis ficam a null
                odCompSource = FedoraHelper.GetAssociatedODComposto(sourceUpperRow, out odCompSourceRow);
                odCompTarget = FedoraHelper.GetAssociatedODComposto(targetRow, out odCompTargetRow);
            }

            foreach (var arg in lstRHPca.pasteRhXPreConcArguments)
            {
                arg.tran = lstRHPca.tran;
                arg.gisaBackup = lstRHPca.gisaBackup;
                arg.continueSave = lstRHPca.continueSave;
                var uniqueCodigoPca = arg.ensureUniqueCodigoArgs;
                var updatePermissionsPostSaveArgs = arg.updatePermissionsPostSaveArgs;
                var manageDocsPermissionsArgs = arg.manageDocsPermissionsArgs;
                var pcArgsNivelDocSimples = arg.setNivelOrderPreConcArguments;

                GISADataset.RelacaoHierarquicaRow rhRowOld = (GISADataset.RelacaoHierarquicaRow)(GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", arg.rhRowOldID, arg.rhRowOldIDUpper))[0]);
                bool existsRH = DBAbstractDataLayer.DataAccessRules.NivelRule.Current.existsRelacaoHierarquica(arg.rhRowOldIDUpper.ToString(), arg.rhRowOldID.ToString(), args.tran);
                bool existsN = DBAbstractDataLayer.DataAccessRules.NivelRule.Current.existsNivel(arg.nivel.ID.ToString(), args.tran);

                // se este delegate voltar a ser executado por conflito de concorrencia ou deadlock o estado da
                // rhPca.rhRowNew n�o ser� detached!! (em caso de conflito, o RowState da rhPca.rhRowNew dever� Added)
                if (!(arg.rhRowNew.RowState == DataRowState.Detached))
                {
                    Debug.Assert(!(arg.rhRowNew.RowState == DataRowState.Added));
                    GISADataset.RelacaoHierarquicaRow row = GisaDataSetHelper.GetInstance().RelacaoHierarquica.NewRelacaoHierarquicaRow();
                    foreach (DataColumn col in GisaDataSetHelper.GetInstance().RelacaoHierarquica.Columns)
                        row[col] = arg.rhRowNew[col];

                    GisaDataSetHelper.GetInstance().RelacaoHierarquica.Rows.Remove(arg.rhRowNew);
                    GisaDataSetHelper.GetInstance().RelacaoHierarquica.AddRelacaoHierarquicaRow(row);
                }

                // O "paste" s� pode ser executado caso a rela��o antiga e o n�vel sobre o qual a opera��o � executada existam
                if (existsRH && existsN)
                {
                    // Se passarmos um documento avaliado e que n�o constitua s�rie 
                    // para dentro de uma s�rie j� existente temos de garantir que 
                    // a s�rie alvo n�o est� tamb�m ainda avaliada
                    if (rhRowOld.NivelRowByNivelRelacaoHierarquicaUpper.TipoNivelRow.ID == TipoNivel.ESTRUTURAL && rhRowOld.NivelRowByNivelRelacaoHierarquica.TipoNivelRow.ID == TipoNivel.DOCUMENTAL && arg.rhRowNew.NivelRowByNivelRelacaoHierarquicaUpper.TipoNivelRow.ID == TipoNivel.DOCUMENTAL)
                    {

                        var sourceRow = rhRowOld.NivelRowByNivelRelacaoHierarquica;

                        GISADataset.FRDBaseRow targetFrdRow = null;
                        GISADataset.SFRDAvaliacaoRow targetAvaliacaoRow = null;
                        GISADataset.FRDBaseRow sourceFrdRow = null;
                        GISADataset.SFRDAvaliacaoRow[] sourceAvaliacaoRows = null;

                        // carregar rows da avalia��o do nivel sobre o qual vai ser executado o paste
                        DBAbstractDataLayer.DataAccessRules.FRDRule.Current.LoadNivelAvaliacaoData(GisaDataSetHelper.GetInstance(), arg.nivel.ID, args.tran);

                        targetFrdRow = (GISADataset.FRDBaseRow)(GisaDataSetHelper.GetInstance().FRDBase.Select(string.Format("IDNivel={0} AND IDTipoFRDBase={1:d}", targetRow.ID, TipoFRDBase.FRDOIRecolha))[0]);
                        if (targetFrdRow.GetSFRDAvaliacaoRows().Length > 0)
                            targetAvaliacaoRow = (GISADataset.SFRDAvaliacaoRow)(targetFrdRow.GetSFRDAvaliacaoRows()[0]);

                        sourceFrdRow = (GISADataset.FRDBaseRow)(GisaDataSetHelper.GetInstance().FRDBase.Select(string.Format("IDNivel={0} AND IDTipoFRDBase={1:d}", sourceRow.ID, TipoFRDBase.FRDOIRecolha))[0]);
                        sourceAvaliacaoRows = sourceFrdRow.GetSFRDAvaliacaoRows();

                        if (sourceAvaliacaoRows.Length > 0 && !(sourceAvaliacaoRows[0].IsPreservarNull()) && (targetAvaliacaoRow == null || targetAvaliacaoRow.IsPreservarNull()))
                        {
                            arg.message = "N�o � poss�vel a inclus�o de sub-n�veis documentais j� avaliados em " + System.Environment.NewLine + 
                                "n�veis documentais ainda n�o avaliados. Para efectuar esta opera��o " + System.Environment.NewLine + 
                                "avalie o n�vel documental de destino ou remova a avalia��o do n�vel " + System.Environment.NewLine + 
                                "documental de origem.";
                            ToolBarButtonPaste.Enabled = false;
                            arg.PasteError = PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.Avaliacao;
                            continue;
                        }
                    }

                    // Garantir que o c�digo � �nico no contexto do seu novo n� pai (nos documentos) 
                    // ou que � �nico na aplica��o (estruturais tem�tico-funcionais)
                    if (uniqueCodigoPca != null)
                    {
                        if ((arg.rhRowNew.NivelRowByNivelRelacaoHierarquica.IDTipoNivel == TipoNivel.DOCUMENTAL && (arg.rhRowNew.IDTipoNivelRelacionado == TipoNivelRelacionado.D || arg.rhRowNew.IDTipoNivelRelacionado == TipoNivelRelacionado.SD)) || (arg.rhRowNew.NivelRowByNivelRelacaoHierarquica.IDTipoNivel == TipoNivel.ESTRUTURAL && arg.nivel.CatCode.Trim().Equals("NVL")))
                        {
                            uniqueCodigoPca.tran = arg.tran;

                            // em caso de ser necess�rio voltar a correr a transac��o � preciso garantir que o valor 
                            // do RowState da rhPca.rhRowOld seja o original
                            CutPasteUnidadeInformacao(arg, rhRowOld);

                            DelegatesHelper.ensureUniqueCodigo(uniqueCodigoPca);
                            if (!uniqueCodigoPca.successful)
                            {
                                GisaDataSetHelper.GetInstance().TrusteeNivelPrivilege.Cast<GISADataset.TrusteeNivelPrivilegeRow>()
                                    .Where(r => r.RowState != DataRowState.Unchanged)
                                    .ToList().ForEach(r => r.RejectChanges());
                                arg.rhRowNew.RejectChanges();
                                rhRowOld.RejectChanges();
                                GISADataset.RelacaoHierarquicaRow rhRow = (GISADataset.RelacaoHierarquicaRow)(arg.gisaBackup.Tables["RelacaoHierarquica"].Select(string.Format("ID={0} AND IDUpper={1}", rhRowOld.ID, rhRowOld.IDUpper))[0]);

                                arg.gisaBackup.Tables["RelacaoHierarquica"].Rows.Remove(rhRow);
                                arg.message = uniqueCodigoPca.message;
                                arg.PasteError = PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NotUniqueCodigo;
                                continue;
                            }
                        }
                        else if ((rhRowOld.IDTipoNivelRelacionado == TipoNivelRelacionado.SR && arg.rhRowNew.IDTipoNivelRelacionado == TipoNivelRelacionado.SR) ||
                            (rhRowOld.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR && arg.rhRowNew.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR))
                            // em caso de ser necess�rio voltar a correr a transac��o � preciso garantir que o valor 
                            // do RowState da rhPca.rhRowOld seja o original
                            CutPasteUnidadeInformacao(arg, rhRowOld);
                    }
                    else
                        // em caso de ser necess�rio voltar a correr a transac��o � preciso garantir que o valor 
                        // do RowState da rhPca.rhRowOld seja o original
                        CutPasteUnidadeInformacao(arg, rhRowOld);
                }
                else if (!existsRH)
                {
                    arg.message = "O n�vel que anteriormente recortou foi colado por outro utilizador " + Environment.NewLine + "sobre outro n�vel. Se pretender ainda col�-lo noutro local precisar� " + Environment.NewLine + "de o recortar novamente.";
                    arg.PasteError = PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.RHDeleted;
                    continue;
                }
                else if (!existsN)
                {
                    arg.message = "O n�vel selecionado foi apagado por outro utilizador. Por esse " + Environment.NewLine + "motivo a execu��o desta opera��o foi cancelada.";
                    arg.PasteError = PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.NDeleted;
                    continue;
                }

                manageDocsPermissionsArgs.changePermissions = true;
                manageDocsPermissionsArgs.tran = lstRHPca.tran;

                if (cutPasteODs && lstRHPca.IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
                {
                    Debug.Assert(manageDocsPermissionsArgs != null);
                    Debug.Assert(manageDocsPermissionsArgs.nRow != null);

                    var odSimples = FedoraHelper.CutPasteODSimples(manageDocsPermissionsArgs.nRow, odCompSourceRow, odCompSource, odCompTargetRow, odCompTarget);
                    if (odSimples != null)
                        odsToIngest.Add(odSimples);
                    else
                    {
                        arg.message = "O subdocumento selecionado tem um objeto digital associado que " + Environment.NewLine + "n�o est� a permitir terminar esta opera��o. N�o est� a ser poss�vel " + Environment.NewLine + "comunicar com o reposit�rio.";
                        arg.PasteError = PersistencyHelper.PasteRhXPreConcArguments.PasteErrors.ObjDigital;
                        continue;
                    }
                }

                // roolback sobre a atribui��o das permiss�es
                if (manageDocsPermissionsArgs.changePermissions)
                    ManageDocsPermissions(manageDocsPermissionsArgs);

                // atribuir n�mero de ordem aos documentos simples cortados
                if (lstRHPca.IDTipoNivelRelacionado == TipoNivelRelacionado.SD && pcArgsNivelDocSimples != null)
                {
                    pcArgsNivelDocSimples.tran = lstRHPca.tran;

                    DelegatesHelper.SetOrdemDocSimples(pcArgsNivelDocSimples.nRowID, pcArgsNivelDocSimples.nRowIDUpper, pcArgsNivelDocSimples.tran);

                    // actualizar ordem do objeto digital caso exista
                    FedoraHelper.UpdateODRowGUIOrder(pcArgsNivelDocSimples.nRowID);
                }
            }

            if (cutPasteODs)
            {
                // eliminar o od composto caso n�o tenha simples ou s� tenha 1
                var odSimples = FedoraHelper.DeleteODCompostoIfNecessary(sourceUpperRow, odCompSourceRow, odCompSource, odCompTarget);
                if (odSimples != null) odsToIngest.Add(odSimples);

                if (odCompSource == null && odCompTarget != null)
                    odsToIngest = new List<ObjDigital>() { odCompTarget };
                else if (odCompSource != null && odCompTarget == null)
                    odsToIngest.Add(odCompSource);
                else if (odCompSource != null && odCompTarget != null)
                    odsToIngest = new List<ObjDigital>() { odCompSource, odCompTarget };

                lstRHPca.ODsToIngest = odsToIngest;
            }
		}

        private static void CutPasteUnidadeInformacao(PersistencyHelper.PasteRhXPreConcArguments rhPca, GISADataset.RelacaoHierarquicaRow rhRowOld)
        {
            var nRow = rhRowOld.NivelRowByNivelRelacaoHierarquica;
            var tempgisaBackup1 = rhPca.gisaBackup;

            nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().ToList().ForEach(rh =>
                {
                    // backup old rhRows
                    PersistencyHelper.BackupRow(ref tempgisaBackup1, rh);
                    // delete old rhRows
                    rh.Delete();
                });
            rhPca.gisaBackup = tempgisaBackup1;

            // add new rhRow
            GisaDataSetHelper.GetInstance().RelacaoHierarquica.AddRelacaoHierarquicaRow(rhPca.rhRowNew);
        }

        private void CutPasteObjetoDigital(PersistencyHelper.ManageDocsPermissionsPreConcArguments manageDocsPermissionsPreConcArguments,
            PersistencyHelper.SetNivelOrderPreConcArguments setNivelOrderPreConcArguments,
            long IDTipoNivelRelacionado, GISADataset.ObjetoDigitalRow odCompSourceRow, ObjDigComposto odCompSource, GISADataset.ObjetoDigitalRow odCompTargetRow, ObjDigComposto odCompTarget,
            ref List<ObjDigital> odsToIngest)
        {
            // cut/paste dos objetos digitais simples associados aos documentos simples cortados
            if (IDTipoNivelRelacionado == TipoNivelRelacionado.SD)
            {
                var odSimples = FedoraHelper.CutPasteODSimples(manageDocsPermissionsPreConcArguments.nRow, odCompSourceRow, odCompSource, odCompTargetRow, odCompTarget);
                if (odSimples != null) odsToIngest.Add(odSimples);
            }

            // roolback sobre a atribui��o das permiss�es
            if (manageDocsPermissionsPreConcArguments.changePermissions)
                ManageDocsPermissions(manageDocsPermissionsPreConcArguments);

            // atribuir n�mero de ordem aos documentos simples cortados
            if (IDTipoNivelRelacionado == TipoNivelRelacionado.SD && setNivelOrderPreConcArguments != null)
            {
                DelegatesHelper.SetOrdemDocSimples(setNivelOrderPreConcArguments.nRowID, setNivelOrderPreConcArguments.nRowIDUpper, setNivelOrderPreConcArguments.tran);

                // actualizar ordem do objeto digital caso exista
                FedoraHelper.UpdateODRowGUIOrder(setNivelOrderPreConcArguments.nRowID);
            }
        }

		private void ManageDocsPermissions(PersistencyHelper.PreConcArguments args)
		{
			PersistencyHelper.ManageDocsPermissionsPreConcArguments mdpPsa = null;
			mdpPsa = (PersistencyHelper.ManageDocsPermissionsPreConcArguments)args;
			GISADataset.NivelRow nRow = mdpPsa.nRow;
			GISADataset.NivelRow newParentRow = mdpPsa.newParentRow;
			GISADataset.NivelRow oldParentRow = mdpPsa.oldParentRow;

			if (! mdpPsa.changePermissions)
				return;

			if ((newParentRow.IDTipoNivel == TipoNivel.ESTRUTURAL) && (oldParentRow.IDTipoNivel == TipoNivel.DOCUMENTAL))
			{
				// o documento deixou de constituir s�rie
                PermissoesHelper.AddNivelGrantPermissions(nRow, newParentRow, mdpPsa.tran);
			}
			else if ((oldParentRow.IDTipoNivel == TipoNivel.ESTRUTURAL) && (newParentRow.IDTipoNivel == TipoNivel.DOCUMENTAL))
			{
				// o documento passou a constituir s�rie
                PermissoesHelper.UndoAddNivelGrantPermissions(nRow);
			}
		}

		
	#endregion

		// Actualiza o contexto de acordo com o n� actualmente selecionado
		public override bool UpdateContext()
		{
            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
            {
                if (this.nivelNavigator1.EPFilterMode)
                {
                    ListViewItem item = new ListViewItem();
                    if (this.nivelNavigator1.SelectedItems.Count > 0)
                        item = this.nivelNavigator1.SelectedItems[0];

                    return UpdateContext(item);
                }
                else
                {
                    GISATreeNode node = this.nivelNavigator1.SelectedNode;
                    return UpdateContext(node);
                }
            }
            else
            {
                ListViewItem item = new ListViewItem();
                if (this.nivelNavigator1.SelectedItems.Count > 0)
                    item = this.nivelNavigator1.SelectedItems[0];

                return UpdateContext(item);
            }
		}

        private bool UpdateContext(GISATreeNode node)
        {
            // foi selecionado um n�vel estrutural
            bool successfulSave = false;
            GISADataset.NivelRow nRow = null;

            if (this.nivelNavigator1.SelectedNode != null)
            {
                node = this.nivelNavigator1.SelectedNode;
                nRow = ((GISATreeNode)node).NivelRow;
                successfulSave = UpdateContext(nRow);
            }
            else
            {
                nRow = null;
                successfulSave = UpdateContext(nRow);
            }

            if (successfulSave)
                updateContextStatusBar(node);

            return successfulSave;
        }

        private bool UpdateContext(ListViewItem item)
		{
			GISADataset.NivelRow nRow = null;
			bool successfulSave = false;

			if (item != null && item.ListView != null)
			{
				// foi selecionado um n�vel documental
				nRow = (GISADataset.NivelRow)item.Tag;
				successfulSave = UpdateContext(nRow);
                DelayedRemoveDeletedItems(this.nivelNavigator1.Items);
			}
			else if (item != null && item.ListView == null)
			{
				// nenhum n�vel documental selecionado
				nRow = null;
				successfulSave = UpdateContext(nRow);
                DelayedRemoveDeletedItems(this.nivelNavigator1.Items);
			}
            else if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && this.nivelNavigator1.SelectedItems.Count > 0)
			{
				// tratar a situa��o que consiste em visualizar um determinado n�vel (estrutural ou documental),
				// aceder a outra �rea de trabalho (por exemplo, Unidades F�sicas) e voltar � Organiza��o de 
				// Informa��o. Nesta situa��o, � o frmMain que chama o UpdateContext que n�o tem conhecimento
				// se o contexto � um n�vel estrutural ou documental.
                item = this.nivelNavigator1.SelectedItems[0];
				nRow = (GISADataset.NivelRow)item.Tag;
				successfulSave = UpdateContext(nRow);
                DelayedRemoveDeletedItems(this.nivelNavigator1.Items);
			}

			if (successfulSave)
                updateContextStatusBar(item);

			return successfulSave;
		}

		// Actualiza o contexto de acordo com o n�vel especificado
        private bool UpdateContext(GISADataset.NivelRow row)
        {
            if (((frmMain)this.TopLevelControl).isSuportPanel)
            {
                // prever a situa��o onde este painel est� a ser usado como suporte na �rea 
                // de Unidades Fisicas e onde � permitida m�ltipla selec��o;
                // neste caso n�o � necess�rio que a informa��o referente aos items selecionados
                // seja carregada
                bool rez = false;
                rez = CurrentContext.SetNivelEstrututalDocumental(null);
                pseudoContextNivel = row;
                return rez;
            }
            else
                return CurrentContext.SetNivelEstrututalDocumental(row);
        }

		public void UpdateToolBarConfig()
		{
			this.ToolBarButtonEdit.Enabled = false;
			this.ToolBarButtonRemove.Enabled = false;
			this.ToolBarButtonCut.Enabled = false;
			this.ToolBarButtonPaste.Enabled = false;
			this.ToolBarButtonCreateED.Enabled = false;
			this.ToolBarButtonCreateAny.Enabled = false;
			this.ToolBarButtonPrint.Enabled = false;
		}

		// actualiza o estado dos bot�es da toolbar para o contexto selecionado

		public override void UpdateToolBarButtons()
		{
			UpdateToolBarButtons(null);
		}

		public override void UpdateToolBarButtons(ListViewItem item)
		{
			//Obter selec��o actual
			GISATreeNode selectedNode = null;
			GISADataset.NivelRow nRow = null;
			GISADataset.NivelRow nUpperRow = null;
			GISADataset.RelacaoHierarquicaRow rhRow = null;
			GISADataset.TipoNivelRelacionadoRow tnrRow = null;
			// Estas variaveis identificam o contexto definido pelo breadcrumbspath da vista documental
			// (s� s�o usadas quando a vista actual � a documental)
			GISADataset.NivelRow nRowBC = null;
			GISADataset.NivelRow nUpperRowBC = null;
			GISADataset.RelacaoHierarquicaRow rhRowBC = null;
			GISADataset.TipoNivelRelacionadoRow tnrRowBC = null;

            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
			{
                //vista estrutural
                if (this.nivelNavigator1.EPFilterMode) // modo filtro
                {
                    if (item != null && item.ListView != null && !(((GISADataset.NivelRow)item.Tag).RowState == DataRowState.Detached))
                        //contexto da listview
                        nRow = (GISADataset.NivelRow)item.Tag;
                }
                else //modo �rvore
                {
                    selectedNode = (GISATreeNode)this.nivelNavigator1.SelectedNode;
                    if (selectedNode != null && !(selectedNode.NivelRow.RowState == DataRowState.Detached))
                    {
                        nRow = selectedNode.NivelRow;
                        nUpperRow = selectedNode.NivelUpperRow;
                    }
                }
			}
			else
			{
				//vista documental
				if (item != null && item.ListView != null && ! (((GISADataset.NivelRow)item.Tag).RowState == DataRowState.Detached))
				{
					//contexto da listview
					nRow = (GISADataset.NivelRow)item.Tag;
					nUpperRow = nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0].NivelRowByNivelRelacaoHierarquicaUpper;
				}

				//contexto do breadcrumbspath
                nRowBC = GisaDataSetHelper.GetInstance().Nivel.Cast<GISADataset.NivelRow>().SingleOrDefault(r => r.RowState != DataRowState.Deleted && r.ID == this.nivelNavigator1.ContextBreadCrumbsPathID);
                nUpperRowBC = GisaDataSetHelper.GetInstance().Nivel.Cast<GISADataset.NivelRow>().SingleOrDefault(r => r.RowState != DataRowState.Deleted && r.ID == this.nivelNavigator1.ContextBreadCrumbsPathIDUpper);
			}

			if (nRow != null && ! (nRow.RowState == DataRowState.Detached) && nUpperRow == null)
				tnrRow = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(null);
			else if (nUpperRow != null && ! (nUpperRow.RowState == DataRowState.Detached) && nRow != null && ! (nRow.RowState == DataRowState.Detached))
			{
                if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
				{
					rhRow = selectedNode.RelacaoHierarquicaRow;
					// excluimos desta forma as relacoes hirarquicas entretanto eliminadas (as que seriam NULL mas cujo n� respectivo teria um NivelUpper)
					if (selectedNode.NivelUpperRow != null && rhRow != null)
						tnrRow = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(rhRow);
				}
				else
				{
					if (item != null && item.ListView != null)
					{
						DataRow[] rhRows = GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRow.ID.ToString(), nUpperRow.ID.ToString()));
						// A rela��o pode ter desaparecido por algum motivo (ie, concorrencia).
						if (rhRows.Length > 0)
						{
							rhRow = (GISADataset.RelacaoHierarquicaRow)(rhRows[0]);
							tnrRow = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(rhRow);
						}
					}
					if (nRowBC != null && nUpperRowBC != null)
					{
						DataRow[] rhRowBCs = GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRowBC.ID.ToString(), nUpperRowBC.ID.ToString()));
						// A rela��o pode ter desaparecido por algum motivo (ie, concorrencia).
						if (rhRowBCs.Length > 0)
						{
							rhRowBC = (GISADataset.RelacaoHierarquicaRow)(rhRowBCs[0]);
							tnrRowBC = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(rhRowBC);
						}
					}
				}
			}

			// Determinar qual � o estado da selec��o
			SelectionState currentSelection = 0;
            if (nRow == null && this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && nRowBC != null && nUpperRowBC != null)
				currentSelection = SelectionState.NoNivelDocSelection;
            else if (nRow == null)
				currentSelection = SelectionState.NoSelection;
            else if (nRow.RowState == DataRowState.Detached || (nUpperRow != null && nUpperRow.RowState == DataRowState.Detached) || (rhRow != null && rhRow.RowState == DataRowState.Detached) || (rhRow == null && tnrRow == null) || (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && rhRowBC == null && tnrRowBC == null))
				currentSelection = SelectionState.DeletedSelection;
            else if (tnrRow.ID == TipoNivelRelacionado.ED && this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && !this.nivelNavigator1.EPFilterMode)
				currentSelection = SelectionState.ED;
            else if (tnrRow.ID == TipoNivelRelacionado.GA && this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && !this.nivelNavigator1.EPFilterMode)
				currentSelection = SelectionState.GA;
			else if (TipoNivel.isNivelOrganico(nRow))
				currentSelection = SelectionState.EstruturalOrganico;
			else if (TipoNivel.isNivelTematicoFuncional(nRow))
				currentSelection = SelectionState.EstruturalTematicoFuncional;
			else if (TipoNivel.isNivelDocumental(nRow))
				currentSelection = SelectionState.Documental;

			switch (currentSelection)
			{
				case SelectionState.NoSelection:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && !this.nivelNavigator1.EPFilterMode && AllowCreate;
					ToolBarButtonCreateAny.Enabled = false;
					ToolBarButtonCreateAny.DropDownMenu = new ContextMenu();
					ToolBarButtonEdit.Enabled = false;
					ToolBarButtonRemove.Enabled = false;
					ToolBarButtonCut.Enabled = false;
					ToolBarButtonPaste.Enabled = false;
                    ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental;
                    ToolBarButtonEAD.Enabled = false;
                    SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
					break;
				case SelectionState.NoNivelDocSelection:
					ToolBarButtonCreateED.Enabled = false;
					GISADataset.RelacaoHierarquicaRow[] bcRHRow = (GISADataset.RelacaoHierarquicaRow[])(GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRowBC.ID.ToString(), nUpperRowBC.ID.ToString())));
					if (bcRHRow.Length > 0)
					{
						if (bcRHRow[0].IDTipoNivelRelacionado < 10)
							ToolBarButtonCreateAny.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
						else
							ToolBarButtonCreateAny.Enabled = false;
						rhRowBC = (GISADataset.RelacaoHierarquicaRow)(GisaDataSetHelper.GetInstance().RelacaoHierarquica.Select(string.Format("ID={0} AND IDUpper={1}", nRowBC.ID.ToString(), nUpperRowBC.ID.ToString()))[0]);
						tnrRowBC = TipoNivelRelacionado.GetTipoNivelRelacionadoFromRelacaoHierarquica(rhRowBC);
						ConfigureContextMenu(nRowBC, nUpperRowBC, tnrRowBC, tnrRowBC);
						ToolBarButtonEdit.Enabled = false;
						ToolBarButtonRemove.Enabled = false;
						ToolBarButtonCut.Enabled = false;
						ToolBarButtonPaste.Enabled = isPastable(nRowBC, nUpperRowBC, tnrRowBC) && AllowCreate && PermissoesHelper.AllowCreate;
                        SetButtonFiltroState();
						configureMenuItemsPrint(rhRow);
					}
					else
					{
						ToolBarButtonCreateAny.Enabled = false;
						ToolBarButtonPaste.Enabled = false;
					}
                    ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental;
                    ToolBarButtonEAD.Enabled = true;
                    break;
				case SelectionState.DeletedSelection:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && AllowCreate;
					ToolBarButtonCreateAny.Enabled = false;
					ToolBarButtonCreateAny.DropDownMenu = new ContextMenu();
					ToolBarButtonEdit.Enabled = false;
					ToolBarButtonRemove.Enabled = false;
					ToolBarButtonCut.Enabled = false;
					ToolBarButtonPaste.Enabled = false;
                    ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = false;
                    break;
				case SelectionState.ED:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && AllowCreate;
					ToolBarButtonCreateAny.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
					ConfigureContextMenu(nRow, nUpperRow, tnrRow);
					ToolBarButtonEdit.Enabled = AllowEdit && PermissoesHelper.AllowEdit;
					ToolBarButtonRemove.Enabled = NiveisHelper.isRemovable(nRow, nUpperRow, true) && AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonCut.Enabled = false;
					ToolBarButtonPaste.Enabled = false;
					ToolBarButtonToggleEstruturaSeries.Enabled = false;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = false;
                    break;
				case SelectionState.GA:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && AllowCreate;
					ToolBarButtonCreateAny.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
					ConfigureContextMenu(nRow, nUpperRow, tnrRow);
					ToolBarButtonEdit.Enabled = AllowEdit && PermissoesHelper.AllowEdit;
                    ToolBarButtonRemove.Enabled = NiveisHelper.isRemovable(nRow, nUpperRow, false) && AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonCut.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
					ToolBarButtonPaste.Enabled = isPastable(nRow, nUpperRow, tnrRow) && AllowCreate && PermissoesHelper.AllowCreate;
					ToolBarButtonToggleEstruturaSeries.Enabled = false;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = false;
                    break;
				case SelectionState.EstruturalOrganico:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && !this.nivelNavigator1.EPFilterMode && AllowCreate;
                    ToolBarButtonCreateAny.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && AllowCreate && PermissoesHelper.AllowCreate;
                    if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural)
					{
						ConfigureContextMenu(nRow, nUpperRow, tnrRow);
						ToolBarButtonPaste.Enabled = isPastable(nRow, nUpperRow, tnrRow) && AllowCreate && PermissoesHelper.AllowCreate;
					}
					else
					{
						ConfigureContextMenu(nRowBC, nUpperRowBC, tnrRow, tnrRowBC);
						ToolBarButtonPaste.Enabled = isPastable(nRowBC, nUpperRowBC, tnrRowBC) && AllowCreate && PermissoesHelper.AllowCreate;
					}
					ToolBarButtonEdit.Enabled = false;
                    ToolBarButtonRemove.Enabled = NiveisHelper.isRemovable(nRow, nUpperRow, false) && !this.nivelNavigator1.EPFilterMode && AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonCut.Enabled = false;
                    tnrRow = nRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica().First().TipoNivelRelacionadoRow;
					ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.isTogglable(nRow, tnrRow) && PermissoesHelper.AllowExpand;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = true;
                    break;
				case SelectionState.EstruturalTematicoFuncional:
                    ToolBarButtonCreateED.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && AllowCreate;
					ToolBarButtonCreateAny.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
					ConfigureContextMenu(nRow, nUpperRow, tnrRow);
					ToolBarButtonEdit.Enabled = AllowEdit && PermissoesHelper.AllowEdit;
                    ToolBarButtonRemove.Enabled = NiveisHelper.isRemovable(nRow, nUpperRow, false) && AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonCut.Enabled = AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonPaste.Enabled = isPastable(nRow, nUpperRow, tnrRow) && AllowCreate && PermissoesHelper.AllowCreate;
                    ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.isTogglable() && PermissoesHelper.AllowExpand;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = true;
					break;
				case SelectionState.Documental:
					ToolBarButtonCreateED.Enabled = false;
					ToolBarButtonCreateAny.Enabled = AllowCreate && PermissoesHelper.AllowCreate;
					ConfigureContextMenu(nRowBC, nUpperRowBC, tnrRow, tnrRowBC);
					ToolBarButtonEdit.Enabled = AllowEdit && PermissoesHelper.AllowEdit;
                    ToolBarButtonRemove.Enabled = NiveisHelper.isRemovable(nRow, nUpperRow, false) && AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonCut.Enabled = AllowDelete && PermissoesHelper.AllowDelete;
					ToolBarButtonPaste.Enabled = isPastable(nRowBC, nUpperRowBC, tnrRowBC) && AllowCreate && PermissoesHelper.AllowCreate;
                    ToolBarButtonToggleEstruturaSeries.Enabled = this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental;
					SetButtonFiltroState();
					configureMenuItemsPrint(rhRow);
                    ToolBarButtonEAD.Enabled = true;
                    break;
			}

            isSelectedNivelRemovable = ToolBarButtonRemove.Enabled;

			if (! (((frmMain)TopLevelControl) == null) && ((frmMain)TopLevelControl).MasterPanelCount > 1 && this == ((frmMain)TopLevelControl).MasterPanel)
				UpdateToolBarConfig();
			else
				this.ToolBarButtonPrint.Enabled = true;
		}

		private void configureMenuItemsPrint(GISADataset.RelacaoHierarquicaRow rhRow)
		{
            if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && rhRow != null && (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.A || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SA || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SC || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSC))
			{
				MenuItemPrintInventarioDetalhado.Enabled = true;
				MenuItemPrintInventarioResumido.Enabled = true;
			}
			else
			{
				MenuItemPrintInventarioDetalhado.Enabled = false;
				MenuItemPrintInventarioResumido.Enabled = false;
			}

			if (rhRow != null && (rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.A || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SA || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SC || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSC || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SR || rhRow.IDTipoNivelRelacionado == TipoNivelRelacionado.SSR))
			{
				MenuItemPrintCatalogoDetalhado.Enabled = true;
				MenuItemPrintCatalogoResumido.Enabled = true;
			}
			else
			{
				MenuItemPrintCatalogoDetalhado.Enabled = false;
				MenuItemPrintCatalogoResumido.Enabled = false;
			}
		}

		private bool isPastable(GISADataset.NivelRow NivelRow, GISADataset.NivelRow NivelUpperRow, GISADataset.TipoNivelRelacionadoRow tnrRow)
		{
			RevalidateClipboardNivelItem();
			if (tnrRow == null)
				return false;
			else
				return ClipboardHasNivelItem() && ClipboardNivelRowIsValidSubNivelOf(tnrRow) && ! (ClipboardNivelRowIsAncestorOf(NivelRow));
		}


		private void ConfigureContextMenu(GISADataset.NivelRow NivelRow, GISADataset.NivelRow NivelUpperRow, GISADataset.TipoNivelRelacionadoRow tnrRow)
		{
			ConfigureContextMenu(NivelRow, NivelUpperRow, tnrRow, null);
		}

		private void ConfigureContextMenu(GISADataset.NivelRow NivelRow, GISADataset.NivelRow NivelUpperRow, GISADataset.TipoNivelRelacionadoRow tnrRow, GISADataset.TipoNivelRelacionadoRow tnrRowBC)
		{
			ContextMenu CurrentMenu = new ContextMenu();
			ToolBarButtonCreateAny.DropDownMenu = CurrentMenu;

			// force an update to the button's icon
			int i = ToolBarButtonCreateAny.ImageIndex;
			ToolBarButtonCreateAny.ImageIndex = -1;
			ImageList toolbarImageList = ToolBar.ImageList;
			ImageList niveisImageList = TipoNivelRelacionado.GetImageList();
			toolbarImageList.Images[2] = niveisImageList.Images[SharedResourcesOld.CurrentSharedResources.NivelImageEditar(System.Convert.ToInt32(tnrRow.GUIOrder))];
			toolbarImageList.Images[3] = niveisImageList.Images[SharedResourcesOld.CurrentSharedResources.NivelImageEliminar(System.Convert.ToInt32(tnrRow.GUIOrder))];
			ToolBarButtonCreateAny.ImageIndex = i;
			if (tnrRowBC == null)
                TipoNivelRelacionado.ConfigureMenu(GisaDataSetHelper.GetInstance(), tnrRow, ref ToolBarButtonCreateAny, ToolBarButtonCreateMenuItemClick, this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental); // IsDocumentView)
			else
                TipoNivelRelacionado.ConfigureMenu(GisaDataSetHelper.GetInstance(), tnrRowBC, ref ToolBarButtonCreateAny, ToolBarButtonCreateMenuItemClick, this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental); // IsDocumentView)
		}

		private List<GISADataset.RelacaoHierarquicaRow> mClipboardNivelItems = new List<GISADataset.RelacaoHierarquicaRow>();
		private bool ClipboardHasNivelItem()
		{
            return mClipboardNivelItems.Count > 0;
		}

		private void RevalidateClipboardNivelItem()
		{
			// Verificar se um eventual n� recortado se mant�m v�lido
			if (mClipboardNivelItems.Count > 0)
			{
                bool nivelApagado = false;
                mClipboardNivelItems.ForEach(item =>
                    {
                        if (item.RowState == DataRowState.Detached)
                        {
                            nivelApagado = true;
                            mClipboardNivelItems.Remove(item);
                        }
                        else
                        {
                            var ClipboardNivelRow = item.NivelRowByNivelRelacaoHierarquica;
                            var ClipboardNivelUpperRow = item.NivelRowByNivelRelacaoHierarquicaUpper;
                            if (ClipboardNivelRow.RowState == DataRowState.Detached || ClipboardNivelUpperRow.RowState == DataRowState.Detached)
                            {
                                nivelApagado = true;
                                mClipboardNivelItems.Remove(item);
                            }
                        }
                    }
                );

                if (nivelApagado)
                    MessageBox.Show("N�veis recortados anteriormente foram eliminados, deixando " + Environment.NewLine +
                                "por isso de ser poss�vel col�-los.", "Elimina��o de n�vel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private bool ClipboardNivelRowIsValidSubNivelOf(GISADataset.TipoNivelRelacionadoRow ParentTipoNivelRelacionado)
		{
            // todos os n�veis cortados s�o do mesmo tipo
			GISADataset.RelacaoHierarquicaRow ClipboardRHRow = mClipboardNivelItems[0];
			// Se o n�vel de origem for do mesmo tipo que o n�vel de destino � necess�rio verificar se se trata de um tipo recursivo
			if (ParentTipoNivelRelacionado.ID == ClipboardRHRow.IDTipoNivelRelacionado)
				return GisaDataSetHelper.GetInstance().TipoNivelRelacionado.Select(string.Format("ID={0} AND (Recursivo=1 OR Recursivo IS NULL)", ParentTipoNivelRelacionado.ID)).Length > 0;
			else
				return GisaDataSetHelper.GetInstance().RelacaoTipoNivelRelacionado.Select(string.Format("IDUpper={0} AND ID={1}", ParentTipoNivelRelacionado.ID, ClipboardRHRow.IDTipoNivelRelacionado)).Length > 0;
		}

		private bool ClipboardNivelRowIsAncestorOf(GISADataset.NivelRow targetRow)
		{
			// subir na �rvore partindo do n� sobre o qual se pretende "colar" e 
			// � procura do n� "recortado". Se o encontrarmos significa que n�o 
			// podemos permitir um "colar". Ao permiti-lo faziamos com que um 
			// descendente fosse pai de um seu ascendente (!)
            foreach(var ClipboardNivelItem in mClipboardNivelItems)
            {
                ArrayList openTargetRows = new ArrayList();
                openTargetRows.Add(targetRow);
                while (openTargetRows.Count > 0)
                {
                    targetRow = (GISADataset.NivelRow)(openTargetRows[0]);
                    if (targetRow == ((GISADataset.RelacaoHierarquicaRow)ClipboardNivelItem).NivelRowByNivelRelacaoHierarquica)
                        return true;

                    foreach (GISADataset.RelacaoHierarquicaRow nhRow in targetRow.GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica())
                        openTargetRows.Add(nhRow.NivelRowByNivelRelacaoHierarquicaUpper);
                    openTargetRows.Remove(targetRow);
                }
            }
			return false;
		}

		private void MenuItemPrint_Click(object sender, System.EventArgs e)
		{
			try
			{
				// relatorios em que o contexto actual � importante
				if (sender == MenuItemPrintInventarioResumido || sender == MenuItemPrintInventarioDetalhado || sender == MenuItemPrintCatalogoResumido || sender == MenuItemPrintCatalogoDetalhado)
				{
					GISADataset.RelacaoHierarquicaRow rhRow = null;
                    if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Estrutural && this.nivelNavigator1.SelectedNode != null)
                        rhRow = ((GISATreeNode)this.nivelNavigator1.SelectedNode).RelacaoHierarquicaRow;
                    else if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental)
                        rhRow = ((GISADataset.NivelRow)(this.nivelNavigator1.SelectedNivel)).GetRelacaoHierarquicaRowsByNivelRelacaoHierarquica()[0];

					bool IsTopDown = true;

					if (rhRow != null)
					{
                        ArrayList parameters = new ArrayList();
                        parameters.Add(rhRow.ID);
                        parameters.Add(
                            ((GISADataset.TipoNivelRelacionadoRow)
                                GisaDataSetHelper.GetInstance().TipoNivelRelacionado.Select("ID="+rhRow.IDTipoNivelRelacionado.ToString())[0]).Designacao
                            );

						if (sender == MenuItemPrintInventarioResumido)
						{							
							Reports.InventarioResumido report = new Reports.InventarioResumido(getFilename("InventarioResumido"), parameters, IsTopDown, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
							object o = new Reports.BackgroundRunner(TopLevelControl, report, 1); // ToDo: obter estimativa de n�s a serem apresentados no relat�rio e substituir '1'
						}
						else if (sender == MenuItemPrintCatalogoResumido)
						{
							Reports.CatalogoResumido report = new Reports.CatalogoResumido(getFilename("CatalogoResumido"), parameters, IsTopDown, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
							object o = new Reports.BackgroundRunner(TopLevelControl, report, 1); // ToDo: obter estimativa de n�s a serem apresentados no relat�rio e substituir '1'
						}
						else
						{
							FormCustomizableReports frm = new FormCustomizableReports();
                            frm.AddParameters(DBAbstractDataLayer.DataAccessRules.RelatorioRule.Current.BuildParamListInventCat(SessionHelper.AppConfiguration.GetCurrentAppconfiguration().IsLicObrEnable()));
							switch (frm.ShowDialog())
							{
								case DialogResult.OK:
								{
									ArrayList fields = new ArrayList();
									if (sender == MenuItemPrintInventarioDetalhado)
									{
										Reports.InventarioDetalhado report = new Reports.InventarioDetalhado(getFilename("InventarioDetalhado"), parameters, frm.GetSelectedParameters(), IsTopDown, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
										object o = new Reports.BackgroundRunner(TopLevelControl, report, 1); // ToDo: obter estimativa de n�s a serem apresentados no relat�rio e substituir '1'
									}
									else if (sender == MenuItemPrintCatalogoDetalhado)
									{
										Reports.CatalogoDetalhado report = new Reports.CatalogoDetalhado(getFilename("CatalogoDetalhado"), parameters, frm.GetSelectedParameters(), IsTopDown, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID);
										object o = new Reports.BackgroundRunner(TopLevelControl, report, 1); // ToDo: obter estimativa de n�s a serem apresentados no relat�rio e substituir '1'
									}
									break;
								}
								case DialogResult.Cancel:
								    break;
							}
						}
					}
				}
				else // relatorios em que o contexto actual n�o � importante
				{
                    if (sender == MenuItemPrintAutoEliminacao || sender == MenuItemPrintAutoEliminacaoPortaria)
					{
						if (((frmMain)TopLevelControl).SlavePanel is MultiPanelControl)
							((frmMain)TopLevelControl).SlavePanel.Recontextualize();
						else
							Debug.Assert(false, "Wrong slavepanel found");

						FormAutoEliminacaoPicker form = new FormAutoEliminacaoPicker();
						GISADataset.AutoEliminacaoRow aeRow = null;
						form.LoadData(true);
						form.lvwAutosEliminacao.MultiSelect = false;
						if (form.ShowDialog(this) == DialogResult.OK)
						{
                            object o;
                            aeRow = form.SelectedAutoEliminacao;
                            if (sender == MenuItemPrintAutoEliminacao)
							    o = new Reports.BackgroundRunner(TopLevelControl, new Reports.AutoEliminacao(getFilename("AutoEliminacao"), aeRow, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID), 1);
                            else if (sender == MenuItemPrintAutoEliminacaoPortaria)
                                o = new Reports.BackgroundRunner(TopLevelControl, new Reports.AutoEliminacaoPortaria(getFilename("AutoEliminacaoPortaria"), aeRow, SessionHelper.GetGisaPrincipal().TrusteeUserOperator.ID), 1);
						}
					}
				}
			}
			catch (Reports.OperationAbortedException)
			{
				// User canceled
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				throw;
			}
		}

		private string getFilename(string name)
		{
			return string.Format("{0}_{1}", name, DateTime.Now.ToString("yyyyMMdd"));
		}

        private void MasterPanelSeries_StackChanged(frmMain.StackOperation stackOperation, bool isSupport)
        {
            switch (stackOperation)
            {
                case frmMain.StackOperation.Push:
                    this.nivelNavigator1.MultiSelect = true;

                    if (this.nivelNavigator1.PanelToggleState == NivelNavigator.ToggleState.Documental && this.nivelNavigator1.ContextBreadCrumbsPathID > 0)
                        this.nivelNavigator1.ReloadList();
                    break;
                case frmMain.StackOperation.Pop:
                    this.nivelNavigator1.MultiSelect = false;
                    break;
            }
        }

	#region  Adi��o e remo��o de n�s das treeviews 
		private GISADataset.RelacaoHierarquicaDataTable rhTable = GisaDataSetHelper.GetInstance().RelacaoHierarquica;
		private void rhTable_RelacaoHierarquicaRowChangingRelacaoHierarquicaRowDeleting(object sender, GISADataset.RelacaoHierarquicaRowChangeEvent e)
		{
            NavigatorHelper.ForceRefresh(e, this, (frmMain)TopLevelControl);
        }

        public NivelNavigator NivelNavigator
        {
            get { return this.nivelNavigator1; }
        }
		
	#endregion        
	
        
    }
}