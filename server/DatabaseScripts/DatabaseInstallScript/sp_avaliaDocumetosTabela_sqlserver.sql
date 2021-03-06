IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_avaliaDocumetosTabela]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_avaliaDocumetosTabela]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE sp_avaliaDocumetosTabela
@frdID BIGINT,
@modeloAvaliacaoID BIGINT,
@avaliacaoTabela BIT,
@preservar BIT,
@prazoConservacao SMALLINT
AS
BEGIN
	UPDATE SFRDAvaliacao SET SFRDAvaliacao.IDModeloAvaliacao = @modeloAvaliacaoID, SFRDAvaliacao.AvaliacaoTabela = @avaliacaoTabela
	FROM FRDBase frdDocs
		INNER JOIN SFRDAvaliacao sfrda ON sfrda.IDFRDBase = frdDocs.ID
		INNER JOIN RelacaoHierarquica rh ON rh.ID = frdDocs.IDNivel AND IDTipoNivelRelacionado = 9
		INNER JOIN FRDBase frdSerie ON frdSerie.IDNivel = rh.IDUpper
	WHERE frdSerie.ID = @frdID 
		AND sfrda.Preservar IS NULL
	
	INSERT INTO SFRDAvaliacao (IDFRDBase, IDModeloAvaliacao, AvaliacaoTabela, Publicar)
	SELECT frdDocs.ID, @modeloAvaliacaoID, @avaliacaoTabela, 0
	FROM FRDBase frdDocs
		LEFT JOIN SFRDAvaliacao sfrda ON sfrda.IDFRDBase = frdDocs.ID
		INNER JOIN RelacaoHierarquica rh ON rh.ID = frdDocs.IDNivel AND IDTipoNivelRelacionado = 9
		INNER JOIN FRDBase frdSerie ON frdSerie.IDNivel = rh.IDUpper
	WHERE sfrda.IDFRDBase IS NULL 
		AND frdSerie.ID = @frdID
END
GO

