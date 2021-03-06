IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Search_Estrutura]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Search_Estrutura]
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE sp_clearAvaliacaoTabela (@data DATETIME) AS
BEGIN
	DECLARE @cont BIGINT
	
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION
	
		SELECT @cont = COUNT(ID) 
		FROM ListaModelosAvaliacao
		WHERE DataInicio > @data
	
		IF @cont = 0	
			UPDATE SFRDAvaliacao SET IDModeloAvaliacao = NULL, Preservar = NULL, PrazoConservacao = NULL, IDAutoEliminacao = NULL
			FROM SFRDAvaliacao
				INNER JOIN FRDBase frd ON frd.ID = SFRDAvaliacao.IDFRDBase
				INNER JOIN RelacaoHierarquica rh ON rh.ID = frd.IDNivel
			WHERE SFRDAvaliacao.isDeleted = 0	
				AND rh.IDTipoNivelRelacionado BETWEEN 7 AND 8
				AND SFRDAvaliacao.AvaliacaoTabela = 1
	COMMIT
END
GO
