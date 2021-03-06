SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_getCANiveisDocAssociados]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_getCANiveisDocAssociados]
GO

CREATE PROCEDURE dbo.sp_getCANiveisDocAssociados
AS
	CREATE TABLE #CANiveis (IDNivel BIGINT, TipoNivel Varchar(256), Designacao VARCHAR(768))

	-- Obter os níveis documentais associados a niveis controlodos por Entidades Produtoras
	INSERT INTO #CANiveis 
	SELECT nDoc.ID, NULL, NULL
	FROM NivelControloAut nca
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = nca.IDControloAut	
		INNER Join RelacaoHierarquica rh ON rh.IDUpper = nca.ID
		INNER JOIN Nivel nDoc ON nDoc.ID = rh.ID
	WHERE nDoc.IDTipoNivel = 3
		AND nDoc.isDeleted=0
		AND nca.isDeleted=0
		AND rh.isDeleted=0

	-- Obter niveis documentais associados a Conteúdos ou Tipologias
	INSERT INTO #CANiveis
	SELECT n.ID, NULL, NULL
	FROM IndexFRDCA idx
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = idx.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = idx.IDFRDBase
		INNER JOIN Nivel n ON n.ID = frd.IDNivel
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND n.IDTipoNivel = 3
		AND idx.isDeleted=0
		AND frd.isDeleted=0
		AND n.isDeleted=0

	-- Tecnicos de obra:
	INSERT INTO #CANiveis
	SELECT n.ID, NULL, NULL
	FROM LicencaObraTecnicoObra lto
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = lto.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = lto.IDFRDBase
		INNER JOIN Nivel n ON n.ID = frd.IDNivel
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND n.IDTipoNivel = 3
		AND lto.isDeleted=0
		AND frd.isDeleted=0
		AND n.isDeleted=0
		
	-- Localizacao de obra actual:
	INSERT INTO #CANiveis
	SELECT n.ID, NULL, NULL
	FROM LicencaObraLocalizacaoObraActual lol
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = lol.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = lol.IDFRDBase
		INNER JOIN Nivel n ON n.ID = frd.IDNivel
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND n.IDTipoNivel = 3
		AND lol.isDeleted=0
		AND frd.isDeleted=0
		AND n.isDeleted=0
		
	-- Autores:
	INSERT INTO #CANiveis
	SELECT n.ID, NULL, NULL
	FROM SFRDAutor autor
		INNER JOIN #NiveisTemp nt ON nt.IDNivel = autor.IDControloAut
		INNER JOIN FRDBase frd ON frd.ID = autor.IDFRDBase AND frd.isDeleted = 0
		INNER JOIN Nivel n ON n.ID = frd.IDNivel
		LEFT JOIN #CANiveis can ON can.IDNivel = frd.IDNivel
	WHERE can.IDNivel IS NULL
		AND n.IDTipoNivel = 3
		AND autor.isDeleted=0
		AND frd.isDeleted=0
		AND n.isDeleted=0
		
	SELECT DISTINCT * FROM #CANiveis
	DROP TABLE #CANiveis
GO
