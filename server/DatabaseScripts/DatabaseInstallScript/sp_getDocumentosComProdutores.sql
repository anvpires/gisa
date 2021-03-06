IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_getDocumentosComProdutores]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_getDocumentosComProdutores]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getDocumentosComProdutores] @NivelId BIGINT = NULL
AS
BEGIN

SET NOCOUNT ON;
	
CREATE TABLE #NivelDocsTopo (ID BIGINT PRIMARY KEY)
CREATE TABLE #GetProdutores (IDStart BIGINT, IDNivel BIGINT, IDUpper BIGINT, Gen INT)
CREATE TABLE #Produtores (ID BIGINT, Termo NVARCHAR(4000), TermoAutorizado NVARCHAR(4000))
CREATE TABLE #Niveis (IDStart BIGINT, ID BIGINT)
CREATE TABLE #Result (IDDocumento BIGINT, TituloProdutor NVARCHAR(4000), TituloProdutorAutorizado NVARCHAR(4000))

CREATE INDEX niveis_idx1 ON #Niveis (IDStart)
CREATE INDEX niveis_idx2 ON #Niveis (ID)
CREATE INDEX produtores_idx ON #Produtores (ID)
CREATE INDEX GetProdutores_idx1 ON #GetProdutores (IDUpper)
CREATE INDEX GetProdutores_idx2 ON #GetProdutores (IDStart);

/* encontrar niveis documentais de topo */
IF @NivelId IS NULL
BEGIN
	INSERT INTO #NivelDocsTopo
	SELECT DISTINCT rh.ID
	FROM RelacaoHierarquica rh
		INNER JOIN Nivel n ON n.ID = rh.IDUpper AND n.IDTipoNivel = 2 AND n.isDeleted = 0
	WHERE rh.IDTipoNivelRelacionado > 6 AND rh.isDeleted = 0
END
ELSE
BEGIN
	-- produtor
	WITH GetNivelDocsTopo (ID, IDUpper, Level)
	AS
	(
		SELECT rh.ID, rh.IDUpper, 0 AS Level 
		FROM RelacaoHierarquica rh
		WHERE rh.ID = @NivelId AND rh.IDTipoNivelRelacionado < 7 AND rh.isDeleted = 0
		UNION ALL
		
		SELECT rh.ID, rh.IDUpper, Level + 1
		FROM RelacaoHierarquica rh 
			INNER JOIN GetNivelDocsTopo ON GetNivelDocsTopo.ID = rh.IDUpper AND GetNivelDocsTopo.Level = Level
			INNER JOIN Nivel n ON n.ID = rh.IDUpper AND n.IDTipoNivel = 2
		WHERE rh.isDeleted = 0
	)
	INSERT INTO #NivelDocsTopo
	SELECT DISTINCT ndt.ID
	FROM GetNivelDocsTopo ndt
		INNER JOIN Nivel n ON n.ID = ndt.ID AND n.IDTipoNivel = 3 AND n.isDeleted = 0;
	
	-- documento	
	WITH GetNivelDocsTopo (ID, IDUpper, Level)
	AS
	(
		SELECT rh.ID, rh.IDUpper, 0 AS Level 
		FROM RelacaoHierarquica rh
		WHERE rh.ID = @NivelId AND rh.IDTipoNivelRelacionado > 6 AND rh.isDeleted = 0
		UNION ALL
		
		SELECT rh.ID, rh.IDUpper, Level + 1
		FROM RelacaoHierarquica rh 
			INNER JOIN GetNivelDocsTopo ON GetNivelDocsTopo.IDUpper = rh.ID AND GetNivelDocsTopo.Level = Level
		WHERE rh.IDTipoNivelRelacionado > 6 AND rh.isDeleted = 0
	)
	INSERT INTO #NivelDocsTopo
	SELECT DISTINCT ndt.ID
	FROM GetNivelDocsTopo ndt
		INNER JOIN Nivel n ON n.ID = ndt.IDUpper AND n.IDTipoNivel = 2 AND n.isDeleted = 0
END

/* Identificar produtores directos e indirectos para cada n?vel documental de topo */
DECLARE @c_age TINYINT
SET @c_age = 0

INSERT INTO #GetProdutores
SELECT rh.ID, rh.ID, rh.IDUpper, @c_age
FROM #NivelDocsTopo nd
	inner join RelacaoHierarquica rh ON rh.ID = nd.ID AND isDeleted = 0

-- 3: EXPAND THE WORLD!
WHILE (@@ROWCOUNT>0)
BEGIN  
	SET @c_age = @c_age + 1;
	    
	INSERT INTO #GetProdutores
	SELECT Distinct pe.IDStart, rh.ID, rh.IDUpper, @c_age
	FROM RelacaoHierarquica rh
		INNER JOIN #GetProdutores pe ON rh.ID= pe.IDUpper AND pe.Gen = @c_age - 1
		LEFT JOIN #GetProdutores te_ver ON te_ver.IDUpper = rh.IDUpper AND te_ver.IDNivel = rh.ID AND te_ver.Gen = @c_age
	WHERE rh.IDTipoNivelRelacionado > 2 AND rh.isDeleted = 0
		AND te_ver.IDNivel IS NULL
END

/* Designa??es dos produtores */
INSERT INTO #Produtores
SELECT n.ID, Termo, CASE WHEN ca.Autorizado = 1 THEN d.Termo ELSE NULL END
FROM Nivel n
	INNER JOIN (SELECT DISTINCT IDUpper FROM #GetProdutores) prod ON prod.IDUpper = n.ID
	INNER JOIN NivelControloAut nca ON nca.ID = n.ID
	INNER JOIN ControloAut ca ON ca.ID = nca.IDControloAut 
	INNER JOIN ControloAutDicionario cad ON cad.IDControloAut = ca.ID
	INNER JOIN Dicionario d ON d.ID = cad.IDDicionario;

DECLARE @IDTipoNivelRelacionado BIGINT
SET @IDTipoNivelRelacionado = -1

SELECT @IDTipoNivelRelacionado = IDTipoNivelRelacionado
FROM RelacaoHierarquica where ID = @NivelId

IF @IDTipoNivelRelacionado <= 7 OR @IDTipoNivelRelacionado = 9
BEGIN
WITH GetNiveisDoc (IDStart, ID, IDUpper, Level)
AS
(
	SELECT dt.ID, rh.ID, rh.IDUpper, 0 AS Level 
	FROM RelacaoHierarquica rh
		INNER JOIN #NivelDocsTopo dt on dt.ID = rh.IDUpper
	WHERE rh.isDeleted = 0
	UNION ALL
	
	SELECT GetNiveisDoc.IDStart, rh.ID, rh.IDUpper, Level + 1
	FROM RelacaoHierarquica rh 
		INNER JOIN GetNiveisDoc ON GetNiveisDoc.ID = rh.IDUpper AND GetNiveisDoc.Level = Level
	WHERE rh.isDeleted = 0
)
INSERT INTO #Niveis
SELECT IDStart, ID
FROM GetNiveisDoc
END
ELSE
BEGIN
WITH GetNiveisDoc (IDStart, ID, IDUpper, Level)
AS
(
	SELECT (SELECT TOP 1 ID FROM #NivelDocsTopo), rh.ID, rh.IDUpper, 0 AS Level 
	FROM RelacaoHierarquica rh
	WHERE rh.isDeleted = 0 AND rh.ID = @NivelId
	UNION ALL
	
	SELECT GetNiveisDoc.IDStart, rh.ID, rh.IDUpper, Level + 1
	FROM RelacaoHierarquica rh 
		INNER JOIN GetNiveisDoc ON GetNiveisDoc.ID = rh.IDUpper AND GetNiveisDoc.Level = Level
	WHERE rh.isDeleted = 0
)
INSERT INTO #Niveis
SELECT IDStart, ID
FROM GetNiveisDoc
WHERE IDStart <> @NivelId
END

INSERT INTO #Result
SELECT n1.ID,
	Termos = LEFT(o1.list, LEN(o1.list)),
	TermosAutorizados = LEFT(o2.list, LEN(o2.list))
FROM #Niveis n1
	CROSS APPLY (
		SELECT Termo + ' ' AS [text()]
		FROM #Niveis n2
			INNER JOIN #GetProdutores gp ON gp.IDStart = n2.IDStart
			INNER JOIN #Produtores prod ON prod.ID = gp.IDUpper
		WHERE n2.ID = n1.ID
		FOR XML PATH('')
	) o1 (list)
	CROSS APPLY (
		SELECT TermoAutorizado + ' ' AS [text()]
		FROM #Niveis n2
			INNER JOIN #GetProdutores gp ON gp.IDStart = n2.IDStart
			INNER JOIN #Produtores prod ON prod.ID = gp.IDUpper
		WHERE n2.ID = n1.ID AND TermoAutorizado IS NOT NULL
		FOR XML PATH('')
	) o2 (list)

INSERT INTO #Result
SELECT n1.ID,
	Termos = LEFT(o1.list, LEN(o1.list)),
	TermosAutorizados = LEFT(o2.list, LEN(o2.list))
FROM #NivelDocsTopo n1
	CROSS APPLY (
		SELECT Termo + ' ' AS [text()]
		FROM #NivelDocsTopo n2
			INNER JOIN #GetProdutores gp ON gp.IDStart = n2.ID
			INNER JOIN #Produtores prod ON prod.ID = gp.IDUpper
		WHERE n2.ID = n1.ID
		FOR XML PATH('')
	) o1 (list)
	CROSS APPLY (
		SELECT TermoAutorizado + ' ' AS [text()]
		FROM #NivelDocsTopo n2
			INNER JOIN #GetProdutores gp ON gp.IDStart = n2.ID
			INNER JOIN #Produtores prod ON prod.ID = gp.IDUpper
		WHERE n2.ID = n1.ID AND TermoAutorizado IS NOT NULL
		FOR XML PATH('')
	) o2 (list)
	
-- no caso de existirem documentos debaixo de niveis tem�tico-funcionais os produtores v�m a NULL
SELECT IDDocumento, COALESCE(TituloProdutor, ' ') TituloProdutor, COALESCE(TituloProdutorAutorizado, ' ') TituloProdutorAutorizado FROM #Result

DROP TABLE #GetProdutores
DROP TABLE #NivelDocsTopo
DROP TABLE #Produtores
DROP TABLE #Niveis
DROP TABLE #Result

SET NOCOUNT OFF

END
GO