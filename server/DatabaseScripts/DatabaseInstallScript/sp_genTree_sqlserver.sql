/****** Object:  StoredProcedure [dbo].[sp_genTree]    Script Date: 07/31/2013 17:40:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_genTree]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_genTree]
GO

/****** Object:  StoredProcedure [dbo].[sp_genTree]    Script Date: 07/31/2013 17:40:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[sp_genTree]
@NivelID BIGINT, 
@IDTrustee BIGINT
AS
BEGIN

	-- 1: DECLARE VARS AND TABLES
	DECLARE @c_age TINYINT
	SET @c_age = 0

	CREATE TABLE #Tree_Exp (ID BIGINT, IDUpper BIGINT, gen INT, IDTipoNivelRelacionado BIGINT, TipoNivel NVARCHAR(3), Designacao NVARCHAR(768), InicioAno NVARCHAR(4), FimAno NVARCHAR(4))

	-- 2a: STARTING POINT(Users)
	INSERT into #Tree_Exp 
	SELECT rh.ID, rh.IDUpper, @c_age, tnr.GUIOrder, 'NVL', nd.Designacao, NULL, NULL
	FROM RelacaoHierarquica rh
		LEFT JOIN NivelDesignado nd ON rh.ID  = nd.ID		
		INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
	WHERE rh.isDeleted = 0
		AND rh.ID = @NivelID

	-- 3: EXPAND THE WORLD!
	WHILE EXISTS(SELECT ID FROM #Tree_Exp WHERE gen = @c_age)
	BEGIN  
		SET @c_age = @c_age + 1;
		    
		INSERT INTO #Tree_Exp 
		SELECT rh.ID, rh.IDUpper, @c_age, rh.IDTipoNivelRelacionado, 'NVL', nd.Designacao, rh.InicioAno, rh.FimAno
		FROM RelacaoHierarquica rh
			INNER JOIN #Tree_Exp pe ON rh.ID= pe.IDUpper AND pe.gen = @c_age - 1
			LEFT JOIN NivelDesignado nd ON rh.ID  = nd.ID
			LEFT JOIN #Tree_Exp te_ver ON te_ver.IDUpper = rh.IDUpper AND te_ver.ID = rh.ID AND te_ver.gen = @c_age
			INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
		WHERE rh.isDeleted = 0
			AND te_ver.ID IS NULL
	END

	-- 4: UPDATE THE TERMS!
	UPDATE #Tree_Exp
	SET Designacao = (  
		SELECT TOP 1 dic.Termo
		FROM Dicionario dic 
			INNER JOIN ControloAutDicionario cad ON cad.IDDicionario = dic.ID AND cad.IDTipoControloAutForma = 1 AND cad.isDeleted = 0
			INNER JOIN NivelControloAut nc ON nc.IDControloAut = cad.IDControloAut AND nc.isDeleted = 0
		WHERE nc.ID = #Tree_Exp.ID AND dic.isDeleted = 0
		), TipoNivel = 'CA'
	WHERE Designacao IS NULL

	-- 5: Inserir Entidades Detentoras
	INSERT INTO #Tree_Exp
	SELECT n.ID, NULL, te.gen + 1, 1, 'NVL', nd.Designacao, NULL, NULL
	FROM #Tree_Exp te
		INNER JOIN Nivel n ON n.ID = te.IDUpper
		LEFT JOIN RelacaoHierarquica rh ON rh.ID = n.ID
		INNER JOIN NivelDesignado nd ON nd.ID = n.ID
	WHERE rh.ID IS NULL

	-- 6: Inserir Controlos de Autoridade que não estejam associados a Entidades Detentoras
	-- NOTA: é passado o valor -1 para indicar que se trata de um Controlo de Autoridade desassociado de uma Entidade Detentora
	INSERT INTO #Tree_Exp
	SELECT n.ID, NULL, te.gen + 1, -1, 'CA', d.Termo, NULL, NULL
	FROM #Tree_Exp te
		INNER JOIN Nivel n ON n.ID = te.IDUpper
		LEFT JOIN RelacaoHierarquica rh ON rh.ID = n.ID
		INNER JOIN NivelControloAut nca ON nca.ID = n.ID
		INNER JOIN ControloAutDicionario cad ON cad.IDControloAut = nca.IDControloAut
		INNER JOIN Dicionario d on d.ID = cad.IDDicionario
	WHERE rh.ID IS NULL
		AND cad.IDTipoControloAutForma = 1
			
	--8: calcular permissões
	CREATE TABLE #effective (IDNivel BIGINT PRIMARY KEY, IDUpper BIGINT, Ler TINYINT)
	INSERT INTO #effective 
	SELECT DISTINCT ID, ID, NULL
	FROM #Tree_Exp
	EXEC [dbo].[sp_getEffectiveReadPermissions] @IDTrustee

	--7: ResultSet filtrar pelas permissões
	SELECT te.* 
	FROM #Tree_Exp te
		INNER JOIN #effective E ON E.IDNivel = te.ID AND E.Ler = 1
	ORDER BY te.gen DESC,
		dbo.fn_AddPaddingToDateMember_new2(te.FimAno, 4) DESC,
		dbo.fn_AddPaddingToDateMember_new2(te.InicioAno, 4) DESC

	--9: CleanUp 
	DROP TABLE #Tree_Exp
	drop table #effective
END

GO


