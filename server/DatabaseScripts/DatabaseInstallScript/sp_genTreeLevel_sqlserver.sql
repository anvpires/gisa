/****** Object:  StoredProcedure [dbo].[sp_genTreeLevel]    Script Date: 07/31/2013 17:39:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_genTreeLevel]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_genTreeLevel]
GO

/****** Object:  StoredProcedure [dbo].[sp_genTreeLevel]    Script Date: 07/31/2013 17:39:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_genTreeLevel]
@NivelID BIGINT, 
@TrusteeID BIGINT,
@MaxExceptIDTipoNivelRel INT --IDTipoNivelRelacionado Máximo (exclusive) que define o nível de expansão máxima na árvore de navegação
AS
BEGIN
	SELECT rh.ID, rh.IDUpper, netos.NroNetos, rh.IDTipoNivelRelacionado, n.CatCode, 
		CASE WHEN n.IDTipoNivel = 2 AND n.CatCode = 'CA' THEN d.Termo ELSE nd.Designacao END AS Designacao, rh.InicioAno, rh.FimAno
	FROM RelacaoHierarquica rh
		INNER JOIN (
			SELECT rh.ID, COUNT(rhDesc.ID) NroNetos
			FROM RelacaoHierarquica rh
				LEFT JOIN RelacaoHierarquica rhDesc ON rhDesc.IDUpper = rh.ID AND rhDesc.IDTipoNivelRelacionado < @MaxExceptIDTipoNivelRel AND rhDesc.isDeleted = 0
			WHERE rh.IDUpper = @NivelID
				AND rh.IDTipoNivelRelacionado < @MaxExceptIDTipoNivelRel
				AND rh.isDeleted = 0
			GROUP BY rh.ID
		) netos ON netos.ID = rh.ID AND rh.IDUpper = @NivelID
		INNER JOIN Nivel n ON n.ID = rh.ID AND n.isDeleted = 0
		LEFT JOIN NivelDesignado nd ON n.ID  = nd.ID AND nd.isDeleted = 0
		LEFT JOIN NivelControloAut nca ON nca.ID = n.ID AND nca.isDeleted = 0
		LEFT JOIN ControloAutDicionario cad ON cad.IDControloAut = nca.IDControloAut AND cad.IDTipoControloAutForma = 1 AND cad.isDeleted = 0
		LEFT JOIN Dicionario d ON d.ID = cad.IDDicionario AND d.isDeleted = 0
	WHERE rh.IDUpper = @NivelID 
		AND rh.isDeleted = 0
	ORDER BY dbo.fn_AddPaddingToDateMember_new2(rh.FimAno, 4) DESC, 
		--  dbo.fn_AddPaddingToDateMember_new(rh.FimMes, 2), 
		--  dbo.fn_AddPaddingToDateMember_new(rh.FimDia, 2),
		dbo.fn_AddPaddingToDateMember_new2(rh.InicioAno, 4) DESC, 
		--  dbo.fn_AddPaddingToDateMember_new(rh.InicioMes, 2), 
		--  dbo.fn_AddPaddingToDateMember_new(rh.InicioDia, 2);
		rh.IDTipoNivelRelacionado,
		Designacao
END

GO


