/****** Object:  StoredProcedure [dbo].[sp_deleteFRD]    Script Date: 08/05/2013 14:37:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_deleteFRD]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_deleteFRD]
GO

/****** Object:  StoredProcedure [dbo].[sp_deleteFRD]    Script Date: 08/05/2013 14:37:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/*******************************************************************************************
 * Elimina vários FRDs ou apenas um dependendo se for ou não previamente 
 * criada a tabela temporária #FRDsToDelete. Se esta tabela existir assume-se 
 * que contem os IDs dos FRDs a eliminar. 
 * Este processo de eliminação consiste apenas em mudar a flag isDeleted 
 * para true
*******************************************************************************************/

CREATE PROCEDURE [dbo].[sp_deleteFRD] @IDFRDBase BIGINT AS
BEGIN
	BEGIN TRANSACTION

		/* Determinar se o objectivo é a eliminação de um ou de vários FRDs*/
		DECLARE @multiFRDs BIT
		IF OBJECT_ID('tempdb..#FRDsToDelete') IS NOT NULL
			SET @multiFRDs = 1
		ELSE
			SET @multiFRDs = 0

		IF @multiFRDs = 0
		BEGIN
			CREATE TABLE #FRDsToDelete (IDFRDBase BIGINT);
			INSERT INTO #FRDsToDelete VALUES(@IDFRDBase)
		END

		UPDATE SFRDImagemObjetoDigital SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDImagem SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE IndexFRDCA SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDMaterialDeSuporte SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDEstadoDeConservacao SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDFormaSuporteAcond SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDLingua SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDAlfabeto SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDTecnicasDeRegisto SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDCondicaoDeAcesso SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDTradicaoDocumental SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDOrdenacao SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDUFMateriaisComponente SET isDeleted = 1 WHERE IDComponente IN (SELECT ID FROM SFRDUFComponente INNER JOIN #FRDsToDelete ON #FRDsToDelete.IDFRDBase  = SFRDUFComponente.IDFRDBase )
		UPDATE SFRDUFTecnicasRegComponente SET isDeleted = 1 WHERE IDComponente IN (SELECT ID FROM SFRDUFComponente INNER JOIN #FRDsToDelete ON #FRDsToDelete.IDFRDBase  = SFRDUFComponente.IDFRDBase )
		UPDATE SFRDUFComponente SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDUFDescricaoFisica SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDUnidadeFisica SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDDatasProducao SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDUFCota SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDAvaliacaoRel SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDAvaliacao SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDConteudoEEstrutura SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDContexto SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDDocumentacaoAssociada SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDNotaGeral SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDDimensaoSuporte SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE SFRDAgrupador SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE Codigo SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		
		UPDATE LicencaObraAtestadoHabitabilidade SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObraDataLicencaConstrucao SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObraLocalizacaoObraActual SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObraLocalizacaoObraAntiga SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObraRequerentes SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObraTecnicoObra SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		UPDATE LicencaObra SET isDeleted = 1 WHERE IDFRDBase IN (SELECT IDFRDBase FROM #FRDsToDelete)
		
		UPDATE FRDBase SET isDeleted = 1 WHERE ID IN (SELECT IDFRDBase FROM #FRDsToDelete)

		 /* Se a tabela temporária foi criada aqui é também aqui destruída */
		IF @multiFRDs = 0
			DROP TABLE #FRDsToDelete

	COMMIT TRANSACTION
END


GO


