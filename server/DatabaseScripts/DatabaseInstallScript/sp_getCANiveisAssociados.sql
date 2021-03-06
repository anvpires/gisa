SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_getCANiveisAssociados]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_getCANiveisAssociados]
GO

CREATE PROCEDURE dbo.sp_getCANiveisAssociados 
AS
	CREATE TABLE #CANiveis (IDNivel BIGINT, TipoNivel Varchar(256), Designacao VARCHAR(768), AllowDelete BIT)

	-- Obter os níveis documentais associados a niveis controlodos por Entidades Produtoras
	INSERT INTO #CANiveis 
	SELECT rh.ID, tnr.Designacao, nd.Designacao, 1
	FROM NivelControloAut nca
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = nca.IDControloAut	
		INNER Join RelacaoHierarquica rh ON rh.IDUpper = nca.ID
		INNER JOIN NivelDesignado nd ON nd.ID = rh.ID
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		LEFT JOIN #CANiveis can ON can.IDNivel = nca.ID
	WHERE can.IDNivel IS NULL
		AND nca.isDeleted=0
		AND rh.isDeleted=0
		AND nd.isDeleted=0
		AND tnr.isDeleted=0

	-- Obter Conteúdos ou Tipologias associados a niveis controlodos por Entidades Produtoras
	INSERT INTO #CANiveis 
	SELECT -1, 'Noticia de Autoridade', d.Termo, 0
	FROM NivelControloAut nca
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = nca.IDControloAut
		INNER JOIN FRDBase frd on frd.IDNivel = nca.ID
		INNER JOIN IndexFRDCA idx ON frd.ID = idx.IDFRDBase 
		INNER JOIN ControloAutDicionario cad ON cad.IDControloAut = idx.IDControloAut AND cad.IDTipoControloAutForma = 1
		INNER JOIN Dicionario d ON d.ID = cad.IDDicionario
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND nca.isDeleted=0
		AND frd.isDeleted=0
		AND idx.isDeleted=0
	 	AND cad.isDeleted=0
		AND d.isDeleted=0

	-- Obter niveis controlados por Entidades Produtoras associados a Conteúdos ou Tipologias
	INSERT INTO #CANiveis
	SELECT -1, 'Entidade Produtora', d.Termo, 0
	FROM IndexFRDCA idx
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = idx.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = idx.IDFRDBase
		INNER JOIN NivelControloAut nca ON nca.ID = frd.IDNivel
		INNER JOIN ControloAutDicionario cad ON cad.IDControloAut = nca.IDControloAut AND cad.IDTipoControloAutForma = 1
		INNER JOIN Dicionario d ON d.ID = cad.IDDicionario
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND idx.isDeleted=0
		AND frd.isDeleted=0
		AND nca.isDeleted=0
	 	AND cad.isDeleted=0
		AND d.isDeleted=0

	-- Obter niveis documentais associados a Conteúdos ou Tipologias
	INSERT INTO #CANiveis
	SELECT rh.ID, tnr.Designacao, nd.Designacao, 0
	FROM IndexFRDCA idx
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = idx.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = idx.IDFRDBase
		INNER Join RelacaoHierarquica rh ON rh.ID = frd.IDNivel
		INNER JOIN NivelDesignado nd ON nd.ID = rh.ID
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND idx.isDeleted=0
		AND frd.isDeleted=0
		AND rh.isDeleted=0
		AND nd.isDeleted=0
		AND tnr.isDeleted=0

	-- Tecnicos de obra:
	INSERT INTO #CANiveis
	SELECT rh.ID, tnr.Designacao, nd.Designacao, 0
	FROM LicencaObraTecnicoObra lto
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = lto.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = lto.IDFRDBase
		INNER Join RelacaoHierarquica rh ON rh.ID = frd.IDNivel
		INNER JOIN NivelDesignado nd ON nd.ID = rh.ID
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND lto.isDeleted=0
		AND frd.isDeleted=0
		AND rh.isDeleted=0
		AND nd.isDeleted=0
		AND tnr.isDeleted=0
		
	-- Localizacao de obra actual:
	INSERT INTO #CANiveis
	SELECT rh.ID, tnr.Designacao, nd.Designacao, 0
	FROM LicencaObraLocalizacaoObraActual lol
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = lol.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = lol.IDFRDBase
		INNER Join RelacaoHierarquica rh ON rh.ID = frd.IDNivel
		INNER JOIN NivelDesignado nd ON nd.ID = rh.ID
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND lol.isDeleted=0
		AND frd.isDeleted=0
		AND rh.isDeleted=0
		AND nd.isDeleted=0
		AND tnr.isDeleted=0
		
	-- Autores:
	INSERT INTO #CANiveis
	SELECT rh.ID, tnr.Designacao, nd.Designacao, 0
	FROM SFRDAutor autor
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = autor.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = autor.IDFRDBase AND frd.isDeleted = 0
		INNER JOIN RelacaoHierarquica rh ON rh.ID = frd.IDNivel AND rh.isDeleted = 0
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		INNER JOIN NivelDesignado nd ON nd.ID = frd.IDNivel AND nd.isDeleted = 0
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND autor.isDeleted = 0
		
	SELECT distinct * FROM #CANiveis
	DROP TABLE #CANiveis
	DROP TABLE #NiveisTemp
GO



