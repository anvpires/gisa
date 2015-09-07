using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using GISA.Model;
using DBAbstractDataLayer.DataAccessRules;

namespace GISA.Model
{
	public class Concorrencia
	{
		//arraylist que mantem uma lista com as tabelas que n�o contem nenhuma rela��o com outras do tipo 1 para 1
		private static ArrayList mUndeletableTables = null;
		private static ArrayList UndeletableTables
		{
			get
			{
				if (mUndeletableTables == null)
				{
					mUndeletableTables = new ArrayList();
					foreach (DataTable dt in GisaDataSetHelper.GetInstance().Tables)
					{
						if (! (has1To1Relations(dt)))
							mUndeletableTables.Add(dt);
					}
				}
				return mUndeletableTables;
			}
		}

		private static DataSet mOriginalRowsDB = null;
		public static DataSet OriginalRowsDB
		{
            get { return mOriginalRowsDB; }
            set { mOriginalRowsDB = value; }
		}

		private static System.Text.StringBuilder mStrConcorrenciaUser = new System.Text.StringBuilder();
		public static System.Text.StringBuilder StrConcorrenciaUser
		{
            get { return mStrConcorrenciaUser; }
            set { mStrConcorrenciaUser = value; }
		}

		private static System.Text.StringBuilder mStrConcorrenciaBD = new System.Text.StringBuilder();
		public static System.Text.StringBuilder StrConcorrenciaBD
		{
            get { return mStrConcorrenciaBD; }
            set { mStrConcorrenciaBD = value; }
		}

		private static System.Text.StringBuilder mStrConcorrenciaLinhasNaoGravadas = new System.Text.StringBuilder();
		public static System.Text.StringBuilder StrConcorrenciaLinhasNaoGravadas
		{
            get { return mStrConcorrenciaLinhasNaoGravadas; }
            set { mStrConcorrenciaLinhasNaoGravadas = value; }
		}

		public DataSet mGisaBackup = null;
		public DataSet gisabackup
		{
            get { return mGisaBackup; }
		}

		//estrutura que permite guardar as linhas alteradas de uma tabela
		//recorri a utiliza��o de 2 arrayslists de forma a poder ter uma separa��o entre as linhas marcadas como added e modified e as aquelas marcadas como deleted.
		//isto de forma a facilitar posteriormente aquando da grava��o j� que tamb�m � executada 2 duas fases

		//apesar de ser a solu��o mais desejada, o uso dos arrays causava v�rios problemas. � necess�rio na altura da declara��o da variavel 
		//indicar o seu tamanho m�ximo. isto levava a que todas as posi��es do array eram inicializadas. isto levantava um problema dado
		//que havia a possibilidade de essas posi��es n�o serem ocupadas com rows o que na altura do fill causava problemas
		public struct changedRows
		{
			public string tab;
			public ArrayList rowsAdd;
			public ArrayList rowsMod;
			public ArrayList rowsDel;
			public changedRows(string tab, ArrayList rowsAdd, ArrayList rowsMod, ArrayList rowsDel)
			{
				this.tab = tab;
				this.rowsAdd = rowsAdd;
				this.rowsMod = rowsMod;
				this.rowsDel = rowsDel;
			}
		}

		//varios selects com blocos de IDs
		public DataSet getOriginalRowsDB(ArrayList changedrows, IDbTransaction tran)
		{

			//Este metodo tem como objectivo obter as linhas da base de dados correspondentes aquelas
			//alteradas na interface (modified e deleted. essas linhas sao colocadas no dataset 
			//originalRowsDS.Estas() sao obtidas em blocos, isto �, com um select � obtido um conjunto
			//de linhas. Isto porque, com um �nico select ocorre um stack overflow qd o nro de linhas 
			//pretendido � mto elevado (30000). Linha a linha, o tempo de processamento � muito elevado, 
			//aproximadamente 27s.

			//bloco de 50 linhas: entre 6 e 7 seg
			//bloco de 500 linhas (sempre para a mesma tabela e mm nro de linhas): 
			//                   1� passagem - 30S
			//                   passagens seguintes - 1s        

			//nRows: variavel onde fica guardado o nro de linhas da tabela que esta a ser processada 
			//       proveniente do dataset originalRowsDS
			//contador: variavel de apoio para o processamento de blocos de linhas a serem obtidas da base de
			//   dados
			//j: variavel iteradora sobre as colunas que compoem a chave primaria de uma coluna
			//s: string que vai conter o filtro dos select
			//pk: string que vai conter o nome da(s) coluna(s) que compoe(m) a chave primaria das
			//       tabelas

			DataTable dt = null;
			//Dim pk As System.Text.StringBuilder
			ArrayList rows = new ArrayList(); //array que vai conter as linhas marcadas como added, modified e deleted
			ArrayList childRows = new ArrayList(); // array que ir� contar as linhas filho que tb devem ser carregadas (as filhas das deleted, uma vez que tamb�m ter�o de ser apagadas)

			DataSet origRowsDB = new DataSet();
            origRowsDB.EnforceConstraints = false;
			origRowsDB.CaseSensitive = true;

			bool haColunasConcorrentes = false;

			long startTicks = 0;
			startTicks = DateTime.Now.Ticks;

			//por cada tabela
			foreach (changedRows tab in changedrows)
			{
				rows.Clear();
				dt = GisaDataSetHelper.GetInstance().Tables[tab.tab];

				if (tab.rowsAdd.Count + tab.rowsMod.Count + tab.rowsDel.Count > 0)
				{
					rows.AddRange(tab.rowsMod);
					rows.AddRange(tab.rowsAdd);
					rows.AddRange(tab.rowsDel);
					ConcorrenciaRule.Current.fillRowsToOtherDataset(dt, rows, origRowsDB, tran); // preencher o dataset com as linhas da BD a partir das linhas adicionadas/alteradas/eliminadas em mem�ria
				}
			}

			Debug.WriteLine("Get changed rows from DB: " + new TimeSpan(DateTime.Now.Ticks - startTicks).ToString());

			mOriginalRowsDB = origRowsDB;
			startTicks = DateTime.Now.Ticks;
			haColunasConcorrentes = getLinhasConcorrentes(changedrows, tran);
			Debug.WriteLine("Get linhas concorrentes: " + new TimeSpan(DateTime.Now.Ticks - startTicks).ToString());

			//so no caso de existirem linhas concorrentes � que o dataset com as linhas provenientes da BD � retornado
			//estrategia necessaria para facilitar, na altura de grava��o, o tratamento dos conflitos 
			if (haColunasConcorrentes)
				return mOriginalRowsDB;
			else
				return null;
		}

		//funcao que retorna um arraylist com todas as linhas marcadas como modified, added e deleted provenientes do dataset principal
		public ArrayList getAlteracoes(DataSet ds, ArrayList sortedTables)
		{
			long start = 0;
			start = DateTime.Now.Ticks;

			DataTable dt = null;
			ArrayList changedRowsArrayList = new ArrayList();

			DataSet gisaBackup = new DataSet();
            gisaBackup.EnforceConstraints = false;
			gisaBackup.CaseSensitive = true;

			//processamento efectuado tabela a tabela (o array resultante desta opera��o j� vai ordenado para o save de linhas marcadas como added e deleted)
			foreach (object o in sortedTables)
			{
				dt = GisaDataSetHelper.GetInstance().Tables[((TableDepthOrdered.tableDepth)o).tab.TableName];
				//so prossegue a opera��o se existir alguma linha alterada
				if (dt.Select("", "", DataViewRowState.ModifiedOriginal | DataViewRowState.Added | DataViewRowState.Deleted).Length > 0)
				{
					//array de suporte onde vao ser guardadas as linhas marcadas como modified e added
					ArrayList modif = new ArrayList();

					if (! (gisaBackup.Tables.Contains(dt.TableName)))
						gisaBackup.Tables.Add(dt.Clone());

					//por cada linha modificada verifica se foi realmente alterada; se sim, adiciona-se ao array
					DataRow[] rows = dt.Select("", "", DataViewRowState.ModifiedOriginal);
					foreach (DataRow dr in rows)
					{
						if (! (isModifiedRow(dr)))
							dr.AcceptChanges();
						else
						{
							modif.Add(dr);
							gisaBackup.Tables[dt.TableName].ImportRow(dr);
						}
					}

					ArrayList add = new ArrayList();
					//adicionar as linhas marcadas como added ao array
					foreach (DataRow dr in dt.Select("", "", DataViewRowState.Added))
					{
						add.Add(dr);
						gisaBackup.Tables[dt.TableName].ImportRow(dr);
					}

					//adicionar as linhas marcadas como deleted ao array
					ArrayList del = new ArrayList();
					foreach (DataRow dr in dt.Select("", "", DataViewRowState.Deleted))
					{
						del.Add(dr);
						gisaBackup.Tables[dt.TableName].ImportRow(dr);
					}

					//adiciona uma tabela com as respectivas linhas alteradas ao arraylist que mantem a lista de todas as linhas alteradas
					add.TrimToSize();
					modif.TrimToSize();
					del.TrimToSize();

					if (add.Count > 0 || modif.Count > 0 || del.Count > 0)
					{
						changedRowsArrayList.Add(new changedRows(dt.TableName, add, modif, del));
						mGisaBackup = gisaBackup;
					}
				}
			}
			Debug.WriteLine("<<getAlteracoes>>: " + new TimeSpan(DateTime.Now.Ticks - start).ToString());
			return changedRowsArrayList;
		}

        // ToDo: colocar m�todo numa classe respons�vel por registar altera��es feitas sobre CAs, UFs e Niveis
        public static bool WasRecordModified(DataRow record)
        {
            DataRow row;
            Queue<DataRow> rows = new Queue<DataRow>();
            rows.Enqueue(record);

            while (rows.Count > 0)
            {
                row = rows.Dequeue();

                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Deleted ||
                    (row.RowState == DataRowState.Modified && Concorrencia.isModifiedRow(row)))

                    return true;

                foreach (DataRelation drel in row.Table.ChildRelations)
                {
                    foreach (DataRow drow in row.GetChildRows(drel, DataRowVersion.Current))
                        rows.Enqueue(drow);

                    foreach (DataRow drow in row.GetChildRows(drel, DataRowVersion.Original))
                        rows.Enqueue(drow);
                }
            }

            return false;
        }

		//funcao que verifica se uma determinada row foi realmente alterada; se as versoes original e current sao diferentes
		//a versao retorna true
		public static bool isModifiedRow(DataRow row)
		{
			if (row.RowState != DataRowState.Modified && row.RowState != DataRowState.Unchanged)
				return true;

			foreach (DataColumn column in row.Table.Columns){

                if (column.DataType == typeof(byte[]))
                {
                    var orig = row[column, DataRowVersion.Original] as byte[];
                    var curr = row[column, DataRowVersion.Current] as byte[];

                    if (orig != null && !orig.SequenceEqual(curr))
                        return true;
                }
				else if (!(row[column, DataRowVersion.Original].Equals(row[column, DataRowVersion.Current])))
                {
					if (!((object.ReferenceEquals(row[column, DataRowVersion.Original], DBNull.Value) && row[column, DataRowVersion.Current] is string && row[column, DataRowVersion.Current].Equals("")) || (object.ReferenceEquals(row[column, DataRowVersion.Current], DBNull.Value) && row[column, DataRowVersion.Original] is string && row[column, DataRowVersion.Original].Equals(""))))
						return true;
				}
				else if (RowsChangedToModified != null && RowsChangedToModified.Contains(row))
					return true;
			}			
			return false;
		}

		public void ClearRowsChangedToModified()
		{
			if (RowsChangedToModified != null)
				RowsChangedToModified.Clear();
		}

		// Vari�vel que vai manter todas as rows cujo rowstate, inicialmente added, passou a ser modified. Num conflito
		// de concorr�ncia sobre estas linhas added, caso o utilizador optar pela op��o cancelar, estas linhas n�o eram
		// consideradas como alteradas no save seguinte uma vez que os valores das suas colunas actual e original eram 
		// iguais (resultado resultado da mudan�a do valor do rowstate)
        private static Hashtable RowsChangedToModified = new Hashtable();
		public bool getLinhasConcorrentes(ArrayList changedRows, IDbTransaction tran)
		{			
			ArrayList rows = new ArrayList();
			DataTable dt = null;
			string s = null;
			DataRow row = null;
			DataRow row2 = null;
			//Dim resurrectedRows As New ArrayList ' cont�m rows do dataset originalRowsDB que ser�o ressuscitadas
			Hashtable resurrectedRows = new Hashtable();

			System.Text.StringBuilder msgConcorrencia = new System.Text.StringBuilder();
			System.Text.StringBuilder str = new System.Text.StringBuilder();
			int c = 0;

			//mensagem que indica quais os campos que n�o puderam ser gravados em memoria
			System.Text.StringBuilder infoNaoGravada = new System.Text.StringBuilder();

			//iteracao sobre as tabelas linhas alteradas
			foreach (changedRows tab in changedRows)
			{
				rows.Clear();
				rows.AddRange(tab.rowsAdd);
				//dt usado para obter o nome das colunas da tabela actual
				dt = GisaDataSetHelper.GetInstance().Tables[tab.tab];

				//itera��o sobre as linhas com estado "added"
				foreach (DataRow rowWithinLoop in rows)
				{
                    //row = rowWithinLoop;

                    //s = ConcorrenciaRule.Current.buildFilter(dt, rowWithinLoop);

                    row = rowWithinLoop;
                    s = ConcorrenciaRule.Current.buildFilter(dt, rowWithinLoop);
                    DataRow originalRow = GetOriginalRow(dt, rowWithinLoop);

					//se existe uma linha na BD com a mesma PK ou unique
					//if (OriginalRowsDB.Tables[tab.tab].Select(s.ToString()).Length > 0)
                    if (originalRow != null)
					{
						row2 = originalRow;
						if ((byte)row2["isDeleted"] == 0)
						{
							//a linha existe e n�o est� marcada como deleted -> a linha a inserir � tratada como se j� tivesse side criada anteriormente e o utilizador quisesse edit�-la

							changeRowStateToModified(rowWithinLoop, tab, dt);
							//actualiza a chave prim�ria negativa (se for esse o caso) por aquela que j� existe na BD
							updateRow(rowWithinLoop, row2);

							//actualizar todas as childrows: 
							// - como a linha actual a ser adicionada j� tem uma equivalente na BD (caso daquelas que s�o t�pulos de tabelas com colunas unique) � necessario alterar o seu estado para modified
							// - as suas childrows tamb�m vem o seu estado mudado para modified caso tamb�m tenham uma linha correspondente na BD (como o ID das childrows inicialmente era negativo n�o foi possivel verificar se existiam linhas correspondentes na BD, pelo que � necess�rio voltar a obt�-las utilizando os IDs positivos)
							// - � necess�rio mudar igualmente o estado das childrows para "undeleted" (Exemplo: utilizador muda a forma autorizada para um termo novo e posteriormente volta a mudar para o termo antigo -> as linhas marcadas como deleted podem ainda estar na BD e portanto deve-se impedir que sejam adicionadas outras cmo o mesmo ID de forma a evitar excep��es nas chaves)
							foreach (DataRelation relation in dt.ChildRelations)
							{
								// obter todas as childrows da row em causa
								DataRow[] childRows = null;
								childRows = rowWithinLoop.GetChildRows(relation.RelationName);

								// gerar um filtro que selecciona apenas as childrows "added"
								string childFilter = string.Empty;
								childFilter = ConcorrenciaRule.Current.getQueryForRows(childRows, DataRowState.Added);

								if (childRows.Length > 0)
								{
									// preencher o dataset originalRowsDB com as childrows                                
									ConcorrenciaRule.Current.FillTableInGetLinhasConcorrentes(OriginalRowsDB, GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName], string.Format("WHERE {0} ", childFilter), DBAbstractDataLayer.DataAccessRules.SqlClient.SqlSyntax.DataDeletionStatus.All, tran);

									// gerar filtro que seleccionar� as childrows "added" no dataset 
									// de trabalho que estejam em conflito com rows na BD
									string conflictingRows = null;
									conflictingRows = ConcorrenciaRule.Current.getQueryForRows(GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName].Select(childFilter), DataRowState.Unchanged);

									if (conflictingRows.Length > 0)
									{
										// sao consideradas apenas as rows como added apesar das linhas 
										// obtidas pelo filtro s�o j� necess�riamente apenas as novas
										foreach (DataRow drow in GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName].Select(conflictingRows, "", DataViewRowState.Added))
										{
											// alterar o estado de added para modified
											changedRows r = getChangedRowsElement(changedRows, relation.ChildTable.TableName);
											changeRowStateToModified(drow, r, relation.ChildTable);
											drow["isDeleted"] = 0;
										}
									}
								}
							}
						}
						else if ((byte)row2["isDeleted"] == 1 && (! (UndeletableTables.Contains(rowWithinLoop.Table)) || (UndeletableTables.Contains(rowWithinLoop.Table) && ! (parentsExists(rowWithinLoop)))))
						{
							//a linha existe, est� marcada como deleted e n�o � poss�vel ressuscit�-la porque ou a sua tabela tem pelo menos um rela��o de 1 para 1 com outra tabela, ou n�o tem nenhuma rela��o desse tipo mas a parent row n�o existe
							//o tratamento desta situa��o � id�ntico �s linhas marcadas como modified mas n�o existem as correspondentes na BD

							//MessageBox.Show(String.Format("A informa��o relativa a {0} n�o pode ser guardada devido ao facto de o contexto ter sido apagado por outro utilizador.", MetaModelHelper.getFriendlyName(row.Table.TableName)))

							if (infoNaoGravada.Length > 0)
								infoNaoGravada.Append("; ");

							infoNaoGravada.Append(MetaModelHelper.getFriendlyName(rowWithinLoop.Table.TableName));
							
							removeChildRows(rowWithinLoop, changedRows);

							tab.rowsAdd.Remove(rowWithinLoop);
							rowWithinLoop.Table.Rows.Remove(rowWithinLoop);


						}
						else if ((byte)row2["isDeleted"] == 1 && UndeletableTables.Contains(rowWithinLoop.Table) && parentsExists(rowWithinLoop))
						{
							//a linha existe, est� marcada como deleted e � poss�vel ressuscit�-la porque porque a sua tabela n�o tem nenhuma rela��o do tipo 1 para 1 com outro e a sua parent row existe
							//nesta situa��o a linha � ressuscitada

							// no caso de a linha existir na BD e estar marcada como deleted � necessario voltar a mudar o seu estado para undeleted (isDeleted=false)
							rowWithinLoop["isDeleted"] = 0; //ToDo: verificar a utilidade desta instru��o

							changeRowStateToModified(rowWithinLoop, tab, dt);
							//actualiza a chave prim�ria negativa (se for esse o caso) por aquela que j� existe na BD
							updateRow(rowWithinLoop, row2);

							resurrectedRows.Add(row2, row2);

							//actualizar todas as childrows: 
							// - como a linha actual a ser adicionada j� tem uma equivalente na BD (caso daquelas que s�o t�pulos de tabelas com colunas unique) � necessario alterar o seu estado para modified
							// - as suas childrows tamb�m vem o seu estado mudado para modified caso tamb�m tenham uma linha correspondente na BD (como o ID das childrows inicialmente era negativo n�o foi possivel verificar se existiam linhas correspondentes na BD, pelo que � necess�rio voltar a obt�-las utilizando os IDs positivos)
							// - � necess�rio mudar igualmente o estado das childrows para "undeleted" (Exemplo: utilizador muda a forma autorizada para um termo novo e posteriormente volta a mudar para o termo antigo -> as linhas marcadas como deleted podem ainda estar na BD e portanto deve-se impedir que sejam adicionadas outras cmo o mesmo ID de forma a evitar excep��es nas chaves)
							foreach (DataRelation relation in dt.ChildRelations)
							{
								// obter todas as childrows da row em causa
								DataRow[] childRows = null;
								childRows = rowWithinLoop.GetChildRows(relation.RelationName);

								// gerar um filtro que selecciona apenas as childrows "added"
								string childFilter = null;
								childFilter = ConcorrenciaRule.Current.getQueryForRows(childRows, DataRowState.Added);

								if (childRows.Length > 0)
								{
									// preencher o dataset originalRowsDB com as childrows
									ConcorrenciaRule.Current.FillTableInGetLinhasConcorrentes(OriginalRowsDB, GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName], string.Format("where {0}", childFilter), DBAbstractDataLayer.DataAccessRules.SqlClient.SqlSyntax.DataDeletionStatus.All, tran);

									// gerar filtro que seleccionar� as childrows "added" no dataset 
									// de trabalho que estejam em conflito com rows na BD
									string conflictingRows = null;
									conflictingRows = ConcorrenciaRule.Current.getQueryForRows(GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName].Select(childFilter), DataRowState.Unchanged);

									if (conflictingRows.Length > 0)
									{
										// sao consideradas apenas as rows como added apesar das linhas 
										// obtidas pelo filtro s�o j� necess�riamente apenas as novas
										foreach (DataRow drow in GisaDataSetHelper.GetInstance().Tables[relation.ChildTable.TableName].Select(conflictingRows, "", DataViewRowState.Added))
										{
											// alterar o estado de added para modified
											changedRows r = getChangedRowsElement(changedRows, relation.ChildTable.TableName);

											DataRow origRow = OriginalRowsDB.Tables[relation.ChildTable.TableName].Select(ConcorrenciaRule.Current.buildFilter(relation.ChildTable, drow).ToString())[0];
											changeRowStateToModified(drow, r, relation.ChildTable);
											if ((byte)origRow["isDeleted"] == 1)
											{
												drow["isDeleted"] = 0;
												resurrectedRows.Add(origRow, origRow);
											}
										}
									}
								}
							}
						}
					}
					else
					{
						//a linha a ser adicionada n�o tem nenhuma correspondente na BD mas no entanto � necessario verificar se as linhas pai correspondentes existem (se for o caso)

						// No caso de n�o existirem linhas pai (mas existirem rela��es pai) as linhas a serem adicionadas e todas as que de si dependem (filhas, netas, etc) s�o removidas do dataset de trabalho
						DataRow[] parentRows = null;
						bool parentMissing = false;
						foreach (DataRelation rel in rowWithinLoop.Table.ParentRelations)
						{
							parentRows = rowWithinLoop.GetParentRows(rel);
							if (parentRows.Length != 0){
								string queryRows = ConcorrenciaRule.Current.getQueryForRows(parentRows, DataRowState.Unchanged, DataRowState.Modified);
								DataTable parentTable = GisaDataSetHelper.GetInstance().Tables[rel.ParentTable.TableName];
								if (!OriginalRowsDB.Tables.Contains(rel.ParentTable.TableName) ||
									parentTable.Select(queryRows).Length != OriginalRowsDB.Tables[rel.ParentTable.TableName].Select(queryRows).Length) 
                                {
									ConcorrenciaRule.Current.FillTableInGetLinhasConcorrentes(OriginalRowsDB, parentTable , string.Format("WHERE {0}", queryRows), DBAbstractDataLayer.DataAccessRules.SqlClient.SqlSyntax.DataDeletionStatus.All, tran);
								}
  								foreach (DataRow parentRow in parentRows)
								{
									DataRow[] parentRows2 = null;
									parentRows2 = OriginalRowsDB.Tables[parentRow.Table.TableName].Select(ConcorrenciaRule.Current.buildFilter(parentRow.Table, parentRow).ToString());

									// � detectado um pai em falta se:
									//  -> n�o � encontrado um na Bd 
									//  -> n�o existe um "novo" em mem�ria  (uma linha added ou uma linha modified que tenha sido originalmente added)
									// Nota: a condi��o "Not parentRow.RowState = DataRowState.Added" deve j� estar abrangida pela "Not parentRow("Versao") Is DBNull.Value" e n�o ser por isso necess�ria
									if ((parentRows2.Length == 0 || (byte)(parentRows2[0]["isDeleted"]) == 1) && (! (parentRow.RowState == DataRowState.Added) && ! (parentRow["Versao"] == DBNull.Value) && ! (((byte[])(parentRow["Versao"])).Length == 0)))
									{
                                        parentMissing = true;
										break;
									}
								}
							}
						}

						if (parentMissing)
						{
							//ToDo: criar mensagem a indicar que a informa��o n�o pode ser gravada na base de dados
							//ConcorrenciaMessagesHelper.LinhasNaoGravadas.Add(New ConcorrenciaMessagesHelper.infoNotSaved(row.Table, row))

							if (infoNaoGravada.Length > 0)
								infoNaoGravada.Append("; ");

							infoNaoGravada.Append(MetaModelHelper.getFriendlyName(rowWithinLoop.Table.TableName));

							removeChildRows(rowWithinLoop, changedRows);
							changedRows r = getChangedRowsElement(changedRows, row.Table.TableName);
							r.rowsAdd.Remove(rowWithinLoop);
							rowWithinLoop.Table.Rows.Remove(rowWithinLoop);
						}
					}
				}
			}

			//Itera��o sobre as linhas com estado "modified" e "deleted"
			foreach (changedRows tab in changedRows)
			{
				rows.Clear();
				rows.AddRange(tab.rowsMod);
				rows.AddRange(tab.rowsDel);
				dt = GisaDataSetHelper.GetInstance().Tables[tab.tab];

                //TimeSpan start = new TimeSpan();
				foreach (DataRow rowWithinLoop in rows)
				{
                    row = rowWithinLoop;
                    s = ConcorrenciaRule.Current.buildFilter(dt, rowWithinLoop);
                    DataRow originalRow = GetOriginalRow(dt, rowWithinLoop);

					//se a row actual tem o estado "deleted" e a sua correspondente na BD n�o existe 
					//ou tem o booleano isDeleted igual a true...
					if (rowWithinLoop.RowState == DataRowState.Deleted && (originalRow == null || (originalRow != null && (byte)originalRow["isDeleted"] == 1)))
					{
						//remover a row da estrutura que mant�m as linhas alteradas
						tab.rowsDel.Remove(rowWithinLoop);
						rowWithinLoop.AcceptChanges();

						//ToDo: verificar se as linhas descendentes tamb�m t�m o estado "deleted"
					}
					else if (rowWithinLoop.RowState == DataRowState.Modified && ! (rowWithinLoop["Versao"] == DBNull.Value) && ((byte[])(rowWithinLoop["Versao"])).Length > 0 && (originalRow == null || (originalRow != null && (byte)originalRow["isDeleted"] == 1)))
					{
						//a linha que se pretende editar n�o existe na BD ou a sua correspondente tem o boolano isDeleted igual a true;
						//nesta situa��o a linha � apagada bem como todas as suas filhas
						//NOTA: o teste feito sobre o valor "Versao" tem como objectivo distinguir as linhas cujo estado original � 
						//"added" daquelas com o estado "modified" (se o valor n�o for nulo estamos perante uma linha com estado 
						//original modified)



						//marcar como deleted as linhas que lhe estao associadas
						removeChildRows(rowWithinLoop, changedRows);

						//retirar a row da estrutura que mantem as linhas apagadas
						tab.rowsMod.Remove(rowWithinLoop);

						//remover a linha
						rowWithinLoop.Table.Rows.Remove(rowWithinLoop);

						//MessageBox.Show(String.Format("A informa��o relativa a {0} n�o pode ser guardada devido ao facto de o contexto ter sido apagado por outro utilizador.", MetaModelHelper.getFriendlyName(row.Table.TableName)))

						if (infoNaoGravada.Length > 0)
							infoNaoGravada.Append("; ");

						infoNaoGravada.Append(MetaModelHelper.getFriendlyName(rowWithinLoop.Table.TableName));

						//ConcorrenciaMessagesHelper.LinhasNaoGravadas.Add(New ConcorrenciaMessagesHelper.infoNotSaved(row.Table, row))

						//ToDo: verificar se ainda � necess�rio testar o valor da coluna Versao

					}
					else if (rowWithinLoop.RowState == DataRowState.Modified && originalRow != null && resurrectedRows.Contains(originalRow))
					{

						//evitar o processo de detec��o de conflitos de concorr�ncia no caso da linha existir na BD e estar 
						//marcada como deleted (situa��o onde � criada um linha com a mesma PK de uma existente na BD mas com o 
						//booleano igual a true e s� � pretendido manter o valor da PK)
						//so � feito um AcceptChanges() (esta opera��o n�o � executada antes devido � necessidade de detectar 
						//esta situa��o descrita na linha anterior)

						//ToDo: verificar a utilidade destas instru��es (o estado da row2 n�o � "unchanged"?)
						originalRow.AcceptChanges();
					}
					else
					{
						//processo de detec��o de conflitos de concorr�ncia


						// linha da BD correspondente �quela que est� a ser tratada
						//row2 = OriginalRowsDB.Ta0.bles.Item(tab.tab).Select(s.ToString())(0)

						// se a linha n�o tiver valor timestamp (nem original nem current) � indicativo que inicialmente estava maracada como
						// "added" e no ciclo anterior o seu estado foi alterado para "modified" (da� o facto de n�o ter nenhum timestamp)
						if ((rowWithinLoop["Versao", DataRowVersion.Original] == DBNull.Value || ((byte[])(rowWithinLoop["Versao", DataRowVersion.Original])).Length == 0) && (rowWithinLoop["Versao", DataRowVersion.Current] == DBNull.Value || ((byte[])(rowWithinLoop["Versao", DataRowVersion.Current])).Length == 0))
						{
							bool haColunasConcorrentes = getColunasConcorrentes(tab.tab, row, originalRow);
							if (haColunasConcorrentes)
							{
								c += 1;
								//msgConcorrencia.Append(MetaModelHelper.getFriendlyName(tab.tab) + ", " + strColConc + ", ")
								//str.Append(strColConc + ", ")
							}
							else
							{
								rowWithinLoop.AcceptChanges();
								tab.rowsMod.Remove(rowWithinLoop);
							}
							//se se tratar duma linha inicialmente marcada como "modified"
						}
						else
						{
							//pretende-se a versao original da linha pois para o caso da linha estiver marcada como deleted somente os valores originais est�o acessiveis
							byte[] v1 = (byte[])(row["Versao", DataRowVersion.Original]);
							byte[] v2 = null;
							try
							{
								v2 = (byte[])(originalRow["Versao"]);
							}
							catch (Exception ex)
							{
								Trace.WriteLine(ex.ToString());
								throw ex;
							}

							if (haConcorrencia(v1, v2))
							{
								//strColConc = getColunasConcorrentes(tab.tab, row, row2).ToString()
								bool haColunasConcorrentes = getColunasConcorrentes(tab.tab, row, originalRow);
								if (haColunasConcorrentes)
								{
									c += 1;
									//msgConcorrencia.Append(MetaModelHelper.getFriendlyName(tab.tab) + ", " + strColConc + ", ")
									//str.Append(strColConc + ", ")
								}
								else
								{
									//a row alterada � igual � correspondente em mem�ria
									rowWithinLoop.AcceptChanges();
									tab.rowsMod.Remove(rowWithinLoop);
									tab.rowsDel.Remove(rowWithinLoop);
									//garantir que no dataset com as linhas provenientes da BD so existem aquelas que realmente est�o em concorrencia
									originalRow.Table.Rows.Remove(originalRow);
								}
							}
							else
							{
								//foi detectado que n�o existe conflito de concorr�ncia, no entanto, pode ter existido uma altera��o sobre o valor 
								//de uma FK que entretanto a linha referenciada por este valor foi apagada por outro utilizador provocando um problema
								//de inconsist�ncia de dados (exemplo: � selecionado um auto de elimina��o, que n�o est� associado a qualquer 
								//n�vel entretanto apagado por outro utilizador; na mudan�a de contexto s� existe, no m�nimo, uma linha com estado 
								//modified cuja altera��o � o valor da coluna IDAutoEliminacao; como a elimina��o do auto de elimina��o n�o provocou
								//qualquer mudan�a na avalia��o em quest�o n�o � detectado conflito de concorr�ncia; no entanto, quando a avalia��o
								//vai ser gravada na base de dados vai ocorrer um erro pela aus�ncia do auto de elimina��o)
								//Por isso, se n�o for detectado qualquer conflito de concorr�ncia deve-se, ainda antes de gravar, verificar se as 
								//linhas referenciadas pelos valores das FK ainda existem na base de dados para prever este tipo de erros.

								if (rowWithinLoop.RowState == DataRowState.Modified)
								{
									DataRow[] parentRows = null;
									foreach (DataRelation rel in rowWithinLoop.Table.ParentRelations)
									{
                                        if (rel.ChildKeyConstraint.Columns[0].AllowDBNull)
                                        {
                                            parentRows = rowWithinLoop.GetParentRows(rel);
                                            if (parentRows.Length > 0)
                                            {
                                                ConcorrenciaRule.Current.FillTableInGetLinhasConcorrentes(OriginalRowsDB, rel.ParentTable, string.Format("WHERE {0}", ConcorrenciaRule.Current.getQueryForRows(parentRows, DataRowState.Unchanged, DataRowState.Modified)), DBAbstractDataLayer.DataAccessRules.SqlClient.SqlSyntax.DataDeletionStatus.All, tran);
                                                foreach (DataRow parentRow in parentRows)
                                                {
                                                    DataRow[] parentRows2 = null;
                                                    parentRows2 = OriginalRowsDB.Tables[parentRow.Table.TableName].Select(ConcorrenciaRule.Current.buildFilter(parentRow.Table, parentRow).ToString());

                                                    // � detectado um pai em falta se:
                                                    //  -> n�o � encontrado um na Bd 
                                                    //  -> n�o existe um "novo" em mem�ria  (uma linha added ou uma linha modified que tenha sido originalmente added)
                                                    // Nota: a condi��o "Not parentRow.RowState = DataRowState.Added" deve j� estar abrangida pela "Not parentRow("Versao") Is DBNull.Value" e n�o ser por isso necess�ria
                                                    if ((parentRows2.Length == 0 || (byte)parentRows2[0]["isDeleted"] == 1) && !(parentRow.RowState == DataRowState.Added) && !(parentRow["Versao"] == DBNull.Value) && !(((byte[])(parentRow["Versao"])).Length == 0))
                                                    {
                                                        //a coluna que � uma FK permite o valor NULL
                                                        if (rel.ChildKeyConstraint.Columns[0].AllowDBNull)
                                                        {
                                                            Trace.WriteLine("A colocar o valor null a uma coluna FK.");
                                                            rowWithinLoop[rel.ChildKeyConstraint.Columns[0].ColumnName] = DBNull.Value;
                                                            //FIXME: indicar ao utilizador que alguma coisa correu mal e n�o p�de ser gravada (no caso das 
                                                            //avalia��es n�o foi poss�vel associar um auto de elimina��o a um n�vel por este ter sido eliminado)
                                                        }
                                                        else
                                                        {
                                                            Trace.WriteLine("N�o � permitido colocar o valor NULL � coluna por isso vai ocorrer uma erro.");
                                                            Debug.Assert(false, "FK unexpected error occurred.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
									}
								}
							}
						}
					}
				}

                //Debug.WriteLine("<<modified/deleted row>>: " + start.ToString());
			}

			mStrConcorrenciaLinhasNaoGravadas = infoNaoGravada;

            return c > 0;
        }        

        public List<object> buildFilterPK(DataTable dt, DataRow row)
        {
            List<object> res = new List<object>();
            DataRowVersion version = DataRowVersion.Original;
            if (row.RowState == DataRowState.Added)
                version = DataRowVersion.Current;

            foreach (DataColumn col in dt.PrimaryKey)
                res.Add(row[col.ColumnName, version]);

            return res;
        }

        public DataRow GetOriginalRow(DataTable dt, DataRow row)
        {
            //obter row original tendo em considera��o s� a PK da tabela

            //long tempo = DateTime.Now.Ticks;
            DataView dv;
            DataRowView[] dr;
            DataRow originalRow = null;

            List<object> res = buildFilterPK(dt, row);
            dv = OriginalRowsDB.Tables[dt.TableName].DefaultView;
            dv.ApplyDefaultSort = true;
            dr = dv.FindRows(res.ToArray());
            if (dr.Length > 0)
                originalRow = dr[0].Row;
            //start += new TimeSpan(DateTime.Now.Ticks - tempo);


            // caso n�o se encontre a row original pela PK, procura-se novamente mas considerando
            // outras unique constraints (Ex: quando se cria uma row da FRDBase, o ID � negativo e 
            // por esse motivo n�o se encontra a row original; procura-se ent�o pelo IDNivel que 
            // faz parte de outra unique constraint)
            if (originalRow == null)
            {
                string s = ConcorrenciaRule.Current.buildFilter(dt, row);
                DataRow[] rows = OriginalRowsDB.Tables[dt.TableName].Select(s);
                if (rows.Length > 0)
                    originalRow = rows[0];
            }

            return originalRow;
        }

		private void changeRowStateToModified(DataRow dataRow, changedRows changedRow, DataTable dt)
		{
			//verificar se � possivel for�ar a mudan�a de estado (tabelas intermedias que so sejam compostas por chaves primarias n�o s�o permitidas edi��es)
			if (allowRowStateChanges(dt))
			{
				//for�ar a mudan�a de estado para modified da linha marcada como added
				dataRow.AcceptChanges();
				bool isReadOnly = false;
				isReadOnly = dataRow.Table.Columns[0].ReadOnly;
				dataRow.Table.Columns[0].ReadOnly = false;
				dataRow[0] = dataRow[0];
				dataRow.Table.Columns[0].ReadOnly = isReadOnly;
				changedRow.rowsMod.Add(dataRow);
				RowsChangedToModified.Add(dataRow, dataRow);
			}
			else
			{
				dataRow.AcceptChanges();
			}

			//como a linha j� existe na BD esta deve ser removida da lista das added
			changedRow.rowsAdd.Remove(dataRow);

		}

		//funcao que retorna a estrutura que contem as linhas modificadas da tabela passada como argumento
		internal changedRows getChangedRowsElement(ArrayList changedRows, string dt)
		{
			foreach (changedRows el in changedRows)
			{
				if (el.tab.Equals(dt))
					return el;
			}
			return (changedRows)(changedRows[changedRows.Add(new changedRows(dt, new ArrayList(), new ArrayList(), new ArrayList()))]);
		}

		// actualiza os valores de datarow1 com os existentes em datarow2 (somente as colunas da chave primaria)
		private void updateRow(DataRow dataRow1, DataRow dataRow2)
		{
			updateRow(dataRow1, dataRow2, true);
		}

		private void updateRow(DataRow dataRow1, DataRow dataRow2, bool justPK)
		{
			DataTable table = dataRow1.Table;
			bool isReadOnly = false;
			int tempFor1 = dataRow1.Table.Columns.Count;
			for (int i = 0; i < tempFor1; i++)
			{
				if (Array.IndexOf(table.PrimaryKey, table.Columns[i]) != -1)
				{
					isReadOnly = table.Columns[i].ReadOnly;
					table.Columns[i].ReadOnly = false;
					dataRow1[i] = dataRow2[i];
					table.Columns[i].ReadOnly = isReadOnly;
				}
			}
		}

		//metodo que verifica se existe concorrencia entre duas linhas comparando os seus timestamps (retorna true se houver concorrencia)
		private bool haConcorrencia(byte[] row1, byte[] row2)
		{
			int tempFor1 = row1.Length;
            for (int i = row1.Length - 1; i >= 0; i--)
			{
                if (row1[i] != row2[i])
					return true;
			}
			return false;
		}

		private bool getColunasConcorrentes(string tabName, DataRow rowDS, DataRow rowDB)
		{
			bool temColunasConcorrentes = false;

			System.Text.StringBuilder strUser = new System.Text.StringBuilder();
			System.Text.StringBuilder strBD = new System.Text.StringBuilder();

			string tableFriendlyName = null;
			string columnFriendlyName = null;

			System.Data.DataRowVersion versao = 0;
			if (rowDS.RowState == DataRowState.Deleted)
				versao = DataRowVersion.Original;
			else
				versao = DataRowVersion.Current;


			ArrayList fkColumns = getForeignKeyColumns(rowDS.Table);
			int tempFor1 = rowDS.Table.Columns.Count;
			for (int i = 0; i < tempFor1; i++)
			{

				// Verificar se os valores s�o diferentes (para cada coluna desta row)
				// Primeiro testa-se se os valores das colunas s�o dbnull
				// Para as strings � feito um trim antes da compara��o
				if (((! (rowDS[i, versao] == DBNull.Value) && ! (rowDB[i] == DBNull.Value)) || (! (rowDS[i, versao] == DBNull.Value) && rowDB[i] == DBNull.Value) || (rowDS[i, versao] == DBNull.Value && ! (rowDB[i] == DBNull.Value))) && (rowDS.Table.Columns[i].DataType == typeof(string) && ! (rowDS[i, versao].ToString().Trim().Equals(rowDB[i].ToString().Trim())) || ! (rowDS.Table.Columns[i].DataType == typeof(string)) && ! (rowDS[i, versao].Equals(rowDB[i]))) && ! (rowDS.Table.Columns[i].ReadOnly) && ! (rowDS.Table.Columns[i].ColumnName.Equals("isDeleted")))
				{
					// verificar se a coluna actual pertence �s foreign keys da sua tabela
					// Nota: n�o se espera que neste teste entrem valores "nothing" por se tratarem de FK
					if (fkColumns.Contains(rowDS.Table.Columns[i]))
					{
						DataTable foreignTable = getForeignTable(rowDS.Table.Columns[i]);
						Debug.WriteLine(foreignTable.TableName);
						tableFriendlyName = MetaModelHelper.getFriendlyName(tabName);

						strUser.Append(tableFriendlyName + ":");
						strBD.Append(tableFriendlyName + ":");

						strUser.Append(System.Environment.NewLine);
						strBD.Append(System.Environment.NewLine);

						if (foreignTable.TableName.Equals("Iso639"))
						{
							strUser.Append(foreignTable.Select(string.Format("ID={0}", rowDS[i, versao]))[0]["LanguageNameEnglish"]);
							strBD.Append(foreignTable.Select(string.Format("ID={0}", rowDB[i]))[0]["LanguageNameEnglish"]);
						}
						else if (foreignTable.TableName.Equals("Iso15924"))
						{
							strUser.Append(foreignTable.Select(string.Format("ID={0}", rowDS[i, versao]))[0]["ScriptNameEnglish"]);
							strBD.Append(foreignTable.Select(string.Format("ID={0}", rowDB[i]))[0]["ScriptNameEnglish"]);
						}
						else
						{
							strUser.Append(getReadableRowValue(rowDS, rowDS.Table.Columns[i], versao, foreignTable));
							strBD.Append(getReadableRowValue(rowDB, rowDB.Table.Columns[i], DataRowVersion.Current, foreignTable));
						}

						strUser.Append(System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine);
						strBD.Append(System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine);

						temColunasConcorrentes = true;
					}
					else
					{
						tableFriendlyName = MetaModelHelper.getFriendlyName(tabName);
						strUser.Append(tableFriendlyName);
						strBD.Append(tableFriendlyName);
						columnFriendlyName = MetaModelHelper.getFriendlyName(tabName, rowDS.Table.Columns[i].ColumnName);
						if (tableFriendlyName != null && tableFriendlyName.Length > 0 && columnFriendlyName != null && columnFriendlyName.Length > 0)
						{
							strUser.Append(", ");
							strBD.Append(", ");
						}
						strUser.Append(columnFriendlyName + ":" + System.Environment.NewLine);
						strBD.Append(columnFriendlyName + ":" + System.Environment.NewLine);
						strUser.Append(getReadableRowValue(rowDS, rowDS.Table.Columns[i], versao));
						strBD.Append(getReadableRowValue(rowDB, rowDB.Table.Columns[i]));
						temColunasConcorrentes = true;
						strUser.Append(System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine);
						strBD.Append(System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine);
					}
				}
			}

			if (temColunasConcorrentes)
			{
				mStrConcorrenciaUser.Append(strUser.ToString());
				mStrConcorrenciaBD.Append(strBD.ToString());
			}

			return temColunasConcorrentes;
		}

		private string getReadableRowValue(DataRow row, DataColumn column, DataRowVersion version)
		{
			return getReadableRowValue(row, column, version, null);
		}

		private string getReadableRowValue(DataRow row, DataColumn column)
		{
			return getReadableRowValue(row, column, DataRowVersion.Current, null);
		}

		private string getReadableRowValue(DataRow row, DataColumn column, DataRowVersion version, DataTable lookupTable)
		{
			if (row[column, version] == DBNull.Value)
				return "Valor n�o atribu�do.";
			else
			{
				if (lookupTable == null)
				{
					if (column.DataType == typeof(bool))
						return translateBoolean((bool)(row[column, version]));
					else
						return row[column, version].ToString();
				}
				else
				{
					if (lookupTable.Select(string.Format("ID={0}", row[column, version])).Length > 0)
					{
						if (lookupTable.TableName == "TrusteeUser")
							return lookupTable.Select(string.Format("ID={0}", row[column, version]))[0]["FullName"].ToString();
						else
							return lookupTable.Select(string.Format("ID={0}", row[column, version]))[0]["Designacao"].ToString();
					}
					else
						return "Valor n�o atribu�do.";
				}
			}
		}

		// devolve um arraylist de DataColumns foreign key de uma tabela
		private ArrayList getForeignKeyColumns(DataTable table)
		{
			ArrayList result = new ArrayList();
			foreach (DataRelation relation in table.ParentRelations)
				result.AddRange(relation.ChildColumns);

			return result;
		}

		//retorna a parenttable da foreignkey column passada como argumento
		private DataTable getForeignTable(DataColumn column)
		{
			foreach (DataRelation relation in column.Table.ParentRelations)
			{
				if (Array.IndexOf(relation.ChildColumns, column) != -1)
					return relation.ParentColumns[0].Table;
			}
			return null;
		}

		//retorna verdadeiro se for permitido for�ar a mudan�a de estado das linhas da tabela
		private bool allowRowStateChanges(DataTable dt)
		{
			foreach (DataColumn column in dt.Columns)
			{
				if (! column.ReadOnly && System.Array.IndexOf(dt.PrimaryKey, column) < 0)
					return true;
			}
			return false;
		}

	#region  Rollback Dataset 
		//metodo que actualiza os dados em memoria com aqueles recolhidos da base de dados para efeitos de tratamento de concorrencia
		public void mergeDataFromDataBase(DataSet ds)
		{
			GisaDataSetHelper.GetInstance().Merge(ds, false);
		}


		public void MergeDatasets(DataSet srcDs, DataSet dstDs, ArrayList dataSetTablesOrderedA)
		{
			MergeDatasets(srcDs, dstDs, dataSetTablesOrderedA, null);
		}

//INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
//ORIGINAL LINE: Public Sub MergeDatasets(ByVal srcDs As DataSet, ByVal dstDs As DataSet, ByVal dataSetTablesOrderedA As ArrayList, Optional ByVal trackNewIDs As Hashtable = null)
		public void MergeDatasets(DataSet srcDs, DataSet dstDs, ArrayList dataSetTablesOrderedA, Hashtable trackNewIDs)
		{
			//dstDs.EnforceConstraints = false

			// efectuar a primeira fase do merge entre os dois datasets: no final desta instru��o todas as linhas merged no dataset 
			// de destino ficam no estado modified (o estado actual dessas linhas no dataset de destino � unchanged) caso as linhas 
			// correspondentes no dataset de origem tenham o estado added
			dstDs.Merge(srcDs);

			// DataSet de apoio ao merge: de forma a se consegui mudar o estado das linhas, agora modified, para o estado original
			// (antes do merge) added � necess�rio remov�-las do DataSet e fazer um import da sua correspondente e com o estado original
			// guardada no DataSet de origem; ao remover essa linha do DataSet todas e quaiquer linhas que se sejam dependentes de si
			// s�o igualmente eliminadas pelo que este DataSet vai servir para as guardar e no final deste passo do merge voltar a 
			// coloc�-las no DataSet de origem.
			DataSet ds = new DataSet();
			foreach (TableDepthOrdered.tableDepth t in dataSetTablesOrderedA)
			{
				if (srcDs.Tables.Contains(t.tab.TableName))
				{
					DataTable srcTable = srcDs.Tables[t.tab.TableName];
					DataTable dstTable = dstDs.Tables[srcTable.TableName];
					foreach (DataRow srcInsRow in srcTable.Rows)
					{
						string filter = ConcorrenciaRule.Current.buildFilter(dstTable, srcInsRow, false);
						if (srcInsRow.RowState == DataRowState.Added)
						{
							// verificar se a linha actual ainda existe no DataSet de trabalho prevendo o caso de 
							// esta ter sido eliminada (marcada como deleted) por motivos de actualiza��o do estado 
							// para added de uma linha "pai"; 
							// - nessa situa��o a linha � reposta no DataSet de  trabalho (destino) e o ciclo
							//   deve prosseguir com a pr�xima linha;
							// - caso contr�rio, antes de o estado (rowstate) da linha actual ser actualizado, 
							//   todas as suas linhas filhas, netas, ..., s�o copiadas para um terceiro DataSet
							//   com o fim de as voltar a copiar para o DataSet de destino um vez que aquando a 
							//   actualiza��o do estado da linha actual, esse conjunto de linhas � apagado.
							if (dstTable.Select(filter).Length == 0)
							{
								if (dstTable.Select(filter, "", DataViewRowState.Deleted).Length > 0 && ds.Tables[dstTable.TableName].Select(filter).Length > 0)
								{

									dstTable.Select(filter, "", DataViewRowState.Deleted)[0].AcceptChanges();
									dstTable.ImportRow(srcInsRow);
								}
								else if (trackNewIDs != null && trackNewIDs.Contains(t.tab.TableName))
								{
									if (((Hashtable)(trackNewIDs[t.tab.TableName])).Contains(filter))
									{
										Hashtable ht = (Hashtable)(trackNewIDs[t.tab.TableName]);
										DataRow[] row = (DataRow[])(GisaDataSetHelper.GetInstance().Tables[t.tab.TableName].Select(((ArrayList)(ht[filter]))[1].ToString()));
										if (row.Length > 0)
										{
											dstTable.Rows.Remove(row[0]);
											dstTable.ImportRow(srcInsRow);
										}
									}
								}
								else
								{
									Debug.WriteLine("Situa��o desconhecida: " + dstTable.TableName);
								}
							}
							else
							{
								DataRow dstRow = dstTable.Select(filter)[0];
								// se tanto a linha de origem como a linha de destino tiverem o rowstate
								// added, o merge entre as duas j� foi feito com o m�todo DataSet.Merge(DataSet)
								if (! (dstRow.RowState == DataRowState.Added && srcInsRow.RowState == DataRowState.Added))
								{
									if (dstRow.RowState == DataRowState.Modified && srcInsRow.RowState == DataRowState.Added)
									{
										foreach (DataRelation rel in dstTable.ChildRelations)
										{
											if (dstRow.GetChildRows(rel).Length > 0)
											{
												ds.Tables.Add(rel.ChildTable.Clone());
												foreach (DataRow childRow in dstRow.GetChildRows(rel))
												{
													string filter2 = ConcorrenciaRule.Current.buildFilter(ds.Tables[rel.ChildTable.TableName], childRow, false);
													if (ds.Tables[rel.ChildTable.TableName].Select(filter2).Length == 0)
													{
														ds.Tables[rel.ChildTable.TableName].ImportRow(childRow);
													}
													getChildRows(childRow, dstDs, ds);
												}
											}
										}
									}
									dstTable.Rows.Remove(dstRow);
									dstTable.ImportRow(srcInsRow);
								}
							}
						}
						else if (srcInsRow.RowState == DataRowState.Unchanged)
						{
							// prever todos os casos onde as linhas s�o alteradas ap�s a execu��o do backup; uma 
							// vez que � feito um primeiro Merge no in�cio do m�todo, toda informa��o que possa ter
							// sido alterada, neste ponto de execu��o, esta j� foi reposta para o valor original
							// faltando somente voltar a definir o RowState para o valor Unchanged (tipicamente 
							// o RowState destas linhas tem o valor Modified)
							if (dstTable.Select(filter).Length > 0 && dstTable.Select(filter)[0].RowState == DataRowState.Modified)
							{
								dstTable.Select(filter)[0].AcceptChanges();
							}
						}
					}
				}
			}

			// repor no dataset de destino todas as linhas filhas daquelas que tinham o estado original added
			foreach (DataTable srcTable in ds.Tables)
			{
				DataTable dstTable = dstDs.Tables[srcTable.TableName];
				foreach (DataRow srcRow in srcTable.Rows)
				{
					string filter = ConcorrenciaRule.Current.buildFilter(dstTable, srcRow, false);
					if (dstTable.Select(filter, "", DataViewRowState.Deleted).Length > 0)
					{
						DataRow dstRow = dstTable.Select(filter, "", DataViewRowState.Deleted)[0];
						dstRow.AcceptChanges();
						dstTable.ImportRow(srcRow);
					}
				}
			}
			//dstDs.EnforceConstraints = true;
		}

		// M�todo de apoio ao MergeDatasets que tem como fun��o identificar e copiar para um DataSet as 
		// linhas "filhas" daquela passada como argumento
		private void getChildRows(DataRow row, DataSet dstDs, DataSet ds)
		{
			foreach (DataRelation rel in row.Table.ChildRelations)
			{
				if (row.GetChildRows(rel).Length > 0)
				{
					ds.Tables.Add(rel.ChildTable.Clone());
					foreach (DataRow childRow in row.GetChildRows(rel))
					{
						string filter2 = ConcorrenciaRule.Current.buildFilter(ds.Tables[rel.ChildTable.TableName], childRow, false);
						if (ds.Tables[rel.ChildTable.TableName].Select(filter2).Length == 0)
						{
							ds.Tables[rel.ChildTable.TableName].ImportRow(childRow);
						}
						getChildRows(childRow, dstDs, ds);
					}
				}
			}
		}

		// M�todo que tem como objectivo manter a informa��o referente � actualiza��o dos Ids das linhas 
		// quando estas s�o adicionadas na base de dados, isto �, saber qual o valor (negativo) do ID 
		// antes da linha ser gravada e o valor (positivo) atribu�do pela base de dados depois do save
		public void startTrackingIdsAddedRows(DataSet workDataSet, ArrayList workDataSetChangedRows, ref Hashtable trackStruture)
		{
			foreach (changedRows changedRow in workDataSetChangedRows)
			{
				if (changedRow.rowsAdd.Count > 0)
				{
					ArrayList dataRowEFiltroNovo = new ArrayList();
					Hashtable filtrosEDataRow = new Hashtable();
					foreach (DataRow addRow in changedRow.rowsAdd)
					{
						if (addRow.RowState != DataRowState.Detached) {
							string filter = ConcorrenciaRule.Current.buildFilter(addRow.Table, addRow);
							dataRowEFiltroNovo.Add(addRow);
                            filtrosEDataRow.Add(filter, dataRowEFiltroNovo);
						}					
					}
					trackStruture.Add(changedRow.tab, filtrosEDataRow);
				}
			}
		}

		// determinar o filtro para a query que ir� obter as rows added com os Ids actualizados, ou seja, 
		// com valores positivos
		public void prepareRollBackDataSet(ref Hashtable trackStruture)
		{
			foreach (Hashtable filtroAntigo in trackStruture.Values)
			{
				foreach (ArrayList rowAlterada in filtroAntigo.Values)
				{
					rowAlterada.Add(ConcorrenciaRule.Current.buildFilter(((DataRow)(rowAlterada[0])).Table, (DataRow)(rowAlterada[0])));
				}
			}
		}

		// apagar do dataset de trabalho todas as linhas (inicialmente com o estado added) que foram 
		// gravadas (acceptChanges) em mem�ria, mas que por motivos de concorr�ncia a transac��o onde 
		// estavam inclu�das foi reiniciada e por esse motivo deixaram de ser �teis pois v�o ser atribu�dos
		// novos Ids caso a transac��o seja bem sucedida
		public void deleteUnusedRows(DataSet workDataSet, ref Hashtable trackStruture)
		{
			foreach (Hashtable filtroAntigo in trackStruture.Values)
			{
				foreach (ArrayList rowAlterada in filtroAntigo.Values)
				{
					DataRow row = (DataRow)rowAlterada[0];
					if (!(row.RowState == DataRowState.Detached) && workDataSet.Tables[row.Table.TableName].Select(ConcorrenciaRule.Current.buildFilter(row.Table, row)).Length > 0) {
						workDataSet.Tables[row.Table.TableName].Rows.Remove(row);
					}
				}
			}
		}
	#endregion

		//metodo de suporte para verificar se existe concorrencia quando o utilizador decide manter as suas altera��es
		public bool wasModified(DataSet ds1, DataSet ds2, ArrayList cr)
		{
			ArrayList rows = new ArrayList();
			DataRow row1 = null;
			DataRow row2 = null;
			DataTable dt = null;
			string filter = null;

			foreach (changedRows tab in cr)
			{
				rows.Clear();
				rows.AddRange(tab.rowsMod);
				rows.AddRange(tab.rowsDel);
				dt = GisaDataSetHelper.GetInstance().Tables[tab.tab];

				foreach (DataRow r in rows)
				{
					filter = ConcorrenciaRule.Current.buildFilter(dt, r).ToString();

					if (ds1.Tables[tab.tab].Select(filter).Length > 0 && ds2.Tables[tab.tab].Select(filter).Length > 0)
					{
						row1 = ds1.Tables[tab.tab].Select(filter)[0];
						row2 = ds2.Tables[tab.tab].Select(filter)[0];

						//testar se ha concorrencia
						if (haConcorrencia((byte[])(row1["Versao"]), (byte[])(row2["Versao"])))
							return true;

					}
					else if (! (ds1.Tables[tab.tab].Select(filter).Length > 0) && ds2.Tables[tab.tab].Select(filter).Length > 0)
					{
						//linha passou a existir na BD
					}
					else if (ds1.Tables[tab.tab].Select(filter).Length > 0 && ! (ds2.Tables[tab.tab].Select(filter).Length > 0))
					{
						//linha deixou de existir na BD
					}
				}
			}

			return false;
		}

		//metodo que verifica se 1 dataset tem linhas guardadas
		public bool temLinhas(DataSet ds)
		{
			foreach (DataTable dt in ds.Tables)
			{
				if (dt.Rows.Count > 0)
					return true;
			}
			return false;
		}

		//m�todo de suporte para a constru��o da mansagem a apresentar ao utilizador numa situa��o de conflito
		//traduz os valores da variavel "val" para sim, n�o ou n�o definido consoante o valor passado
        public static string translateBoolean(bool val)
		{
			if (val == true)
				return "Sim";
			else
				return "N�o";
		}

		//apaga / marca como apagada, as linhas descendentes daquela passada como argumento e remove-as da estrutura que mantem as linhas alteradas em memoria
		private void removeChildRows(DataRow dr, ArrayList changedrows)
		{
			changedRows el = new changedRows();
			ArrayList rows = new ArrayList();
			foreach (DataRelation rel in dr.Table.ChildRelations)
			{
				foreach (DataRow row in dr.GetChildRows(rel))
				{
					if (row.Table.ChildRelations.Count > 0)
						removeChildRows(row, changedrows);

					rows.Clear();
					//verificar se a row est� no arraylist com as linhas alteradas
					if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Modified)
					{
						el = getChangedRowsElement(changedrows, row.Table.TableName);
						el.rowsAdd.Remove(row);
						el.rowsMod.Remove(row);
						el.rowsDel.Remove(row);
					}
					row.Table.Rows.Remove(row);
				}
			}
		}

		private static bool has1To1Relations(DataTable dt)
		{
			foreach (DataRelation rel in dt.ParentRelations)
			{
				DataColumn[] relationParentColumns = rel.ParentColumns;
				DataColumn[] parentTablePrimaryKey = rel.ParentTable.PrimaryKey;
				if (dt.PrimaryKey.Length == rel.ParentTable.PrimaryKey.Length && areTheSameColumns(relationParentColumns, parentTablePrimaryKey))
					return true;
			}

			foreach (DataRelation rel in dt.ChildRelations)
			{
				DataColumn[] relationChildColumns = rel.ChildColumns;
				DataColumn[] childTablePrimaryKey = rel.ChildTable.PrimaryKey;
				if (dt.PrimaryKey.Length == rel.ChildTable.PrimaryKey.Length && areTheSameColumns(relationChildColumns, childTablePrimaryKey))
					return true;
			}

			return false;
		}

		private static bool areTheSameColumns(DataColumn[] list1, DataColumn[] list2)
		{
			if (list1.Length != list2.Length)
				return false;

			int tempFor1 = list1.Length;
			for (int i = 0; i < tempFor1; i++)
			{
				if (! (list1[i] == list2[i]))
					return false;
			}
			return true;
		}

		private static bool parentsExists(DataRow row)
		{
			if (row.Table.ParentRelations.Count == 0)
				return true;

			foreach (DataRelation rel in row.Table.ParentRelations)
				return row.GetParentRows(rel).Length > 0;

			return false;
		}
	}

//#If DEBUG Then
//<TestFixture()> Public Class TestConcorrencia

//    <SetUp()> Public Sub SetUp()
//    End Sub

//    <TearDown()> Public Sub TearDown()
//    End Sub

//    <Test()> Public Sub TestHas1To1Relations()
//        Dim ds As New GISADataset
//        Dim mc As New MockConcorrencia
//        Assertion.Assert("Table RelacaoHierarquica reported unexpected relations.", Not mc.InvokeHas1To1Relations(ds.RelacaoHierarquica))
//    End Sub

//    Private Class MockConcorrencia
//        Inherits Concorrencia

//        Public Function InvokeHas1To1Relations(ByVal dt As DataTable) As Boolean
//            Return has1To1Relations(dt)
//        End Function
//    End Class
//End Class
//#End If
}