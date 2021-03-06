/****** Object:  StoredProcedure [dbo].[sp_deleteNivel]    Script Date: 08/05/2013 14:39:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_deleteNivel]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_deleteNivel]
GO

/****** Object:  StoredProcedure [dbo].[sp_deleteNivel]    Script Date: 08/05/2013 14:39:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/***********************************************************************************************
 * Elimina um nível bem como toda a informação dele directamente dependente, 
 * isto inclui todas as suas FRDs.
 * Este processo de eliminação consiste apenas em mudar a flag isDeleted 
 * para true.
 ***********************************************************************************************/

CREATE PROCEDURE [dbo].[sp_deleteNivel] @IDNivel BIGINT AS
BEGIN
	BEGIN TRANSACTION

		CREATE TABLE #FRDsToDelete (IDFRDBase BIGINT);
		INSERT INTO #FRDsToDelete SELECT ID FROM FRDBase WHERE FRDBase.IDNivel = @IDNivel
	
		EXEC sp_deleteFRD NULL

		UPDATE SFRDAvaliacaoRel SET isDeleted = 1 WHERE IDNivel = @IDNivel
		UPDATE SFRDUnidadeFisica SET isDeleted = 1 WHERE IDNivel = @IDNivel
		UPDATE NivelUnidadeFisicaCodigo SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE NivelUnidadeFisica SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE NivelDocumentoSimples SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE NivelDesignado SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE NivelImagemIlustracao SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE NivelControloAut SET isDeleted = 1 WHERE ID = @IDNivel
		UPDATE RelacaoHierarquica SET isDeleted = 1 WHERE ID = @IDNivel OR IDUpper = @IDNivel
		UPDATE TrusteeNivelPrivilege SET isDeleted = 1 WHERE IDNivel = @IDNivel
		UPDATE Nivel SET isDeleted = 1 WHERE ID = @IDNivel	
	

		DROP TABLE #FRDsToDelete

	COMMIT TRANSACTION
END


GO

