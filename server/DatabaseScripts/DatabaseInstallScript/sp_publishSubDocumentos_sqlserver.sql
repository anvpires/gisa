/****** Object:  StoredProcedure [dbo].[sp_publishSubDocumentos]    Script Date: 07/31/2013 17:45:59 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_publishSubDocumentos]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_publishSubDocumentos]
GO

/****** Object:  StoredProcedure [dbo].[sp_publishSubDocumentos]    Script Date: 07/31/2013 17:45:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_publishSubDocumentos] @IDTrustee BIGINT AS
BEGIN	
	UPDATE SFRDAvaliacao SET SFRDAvaliacao.Publicar = nd.Publicar
	FROM #NiveisDoc nd
		INNER JOIN Nivel n ON n.ID = nd.ID AND n.isDeleted = 0
		INNER JOIN RelacaoHierarquica rh ON rh.IDUpper = n.ID AND rh.isDeleted = 0
		INNER JOIN FRDBase frd ON frd.IDNivel = rh.ID AND frd.isDeleted = 0
	WHERE SFRDAvaliacao.IDFRDBase = frd.ID AND SFRDAvaliacao.isDeleted = 0

	INSERT INTO SFRDAvaliacao (IDFRDBase, IDPertinencia, IDDensidade, Publicar)
	SELECT frd.ID, 1, 1, nd.Publicar
	FROM #NiveisDoc nd
		INNER JOIN RelacaoHierarquica rh ON rh.IDUpper = nd.ID AND rh.IDTipoNivelRelacionado = 10 AND rh.isDeleted = 0
		INNER JOIN FRDBase frd ON frd.IDNivel = rh.ID AND frd.isDeleted = 0
		LEFT JOIN SFRDAvaliacao sfrda ON sfrda.IDFRDBase = frd.ID AND sfrda.isDeleted = 0
	WHERE sfrda.IDFRDBase IS NULL
	
	UPDATE TrusteeNivelPrivilege SET Ler = CASE WHEN a.Publicar = 1 THEN 1 ELSE NULL END
	FROM #NiveisDoc nd
		INNER JOIN Nivel n ON n.ID = nd.ID AND n.isDeleted = 0
		INNER JOIN RelacaoHierarquica rh ON rh.IDUpper = n.ID AND rh.isDeleted = 0
		INNER JOIN FRDBase frd ON frd.IDNivel = rh.ID AND frd.isDeleted = 0
		INNER JOIN SFRDAvaliacao a ON a.IDFRDBase = frd.ID AND a.isDeleted = 0
	WHERE TrusteeNivelPrivilege.IDNivel = rh.ID AND TrusteeNivelPrivilege.IDTrustee = @IDTrustee AND TrusteeNivelPrivilege.isDeleted = 0
	
	INSERT INTO TrusteeNivelPrivilege (IDNivel, IDTrustee, Ler)
	SELECT rh.ID, @IDTrustee, CASE WHEN a.Publicar = 1 THEN 1 ELSE NULL END
	FROM #NiveisDoc nd
		INNER JOIN Nivel n ON n.ID = nd.ID AND n.isDeleted = 0
		INNER JOIN RelacaoHierarquica rh ON rh.IDUpper = n.ID AND rh.isDeleted = 0
		INNER JOIN FRDBase frd ON frd.IDNivel = rh.ID AND frd.isDeleted = 0
		INNER JOIN SFRDAvaliacao a ON a.IDFRDBase = frd.ID AND a.isDeleted = 0
		LEFT JOIN TrusteeNivelPrivilege tnp ON tnp.IDNivel = rh.ID AND tnp.IDTrustee = @IDTrustee AND tnp.isDeleted = 0
	WHERE tnp.IDNivel IS NULL
END

GO


