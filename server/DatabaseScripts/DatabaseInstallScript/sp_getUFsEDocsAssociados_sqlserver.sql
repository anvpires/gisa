/****** Object:  StoredProcedure [dbo].[sp_getUFsEDocsAssociados]    Script Date: 07/31/2013 17:43:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_getUFsEDocsAssociados]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_getUFsEDocsAssociados]
GO

/****** Object:  StoredProcedure [dbo].[sp_getUFsEDocsAssociados]    Script Date: 07/31/2013 17:43:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[sp_getUFsEDocsAssociados] @nivelID BIGINT, @TrusteeID BIGINT AS
BEGIN
	DECLARE @now DATETIME
	SET @now = GETDATE() 

	INSERT INTO #TempRelacaoHierarquica
	SELECT rh.ID, rh.IDUpper, rh.IDTipoNivelRelacionado
	FROM RelacaoHierarquica rh 
	WHERE rh.ID = @nivelID
		AND rh.isDeleted = 0
          
	WHILE (@@ROWCOUNT>0)
		INSERT INTO #TempRelacaoHierarquica
		SELECT DISTINCT rh.ID, rh.IDUpper, rh.IDTipoNivelRelacionado
		FROM RelacaoHierarquica rh 
			LEFT JOIN #TempRelacaoHierarquica tempRh ON tempRh.ID = rh.ID AND tempRh.IDUpper = rh.IDUpper
			INNER JOIN #TempRelacaoHierarquica tempRhEncontrados ON rh.IDUpper = tempRhEncontrados.ID
		WHERE tempRh.ID IS NULL
			AND rh.isDeleted = 0
			
	-- calcular permissões
	CREATE TABLE #effective (IDNivel BIGINT PRIMARY KEY, IDUpper BIGINT, Criar TINYINT, Ler TINYINT, Escrever TINYINT, Apagar TINYINT, Expandir TINYINT)
	INSERT INTO #effective 
	SELECT DISTINCT ID, ID, NULL, NULL, NULL, NULL, NULL
	FROM #TempRelacaoHierarquica
	EXEC [dbo].[sp_getEffectivePermissions] @TrusteeID
    
	INSERT INTO #UFRelated  
	SELECT DISTINCT sfrduf.IDNivel, trh.ID, CASE WHEN trh.IDTipoNivelRelacionado < 9 THEN 1 ELSE 0 END
	FROM #TempRelacaoHierarquica trh
		INNER JOIN #effective E on E.IDNivel = trh.ID
		INNER JOIN FRDBase frd ON frd.IDNivel = trh.ID
		INNER JOIN SFRDUnidadeFisica sfrduf ON sfrduf.IDFRDBase = frd.ID
		INNER JOIN Nivel nvUF ON nvUF.ID = sfrduf.IDNivel 
		LEFT JOIN NivelUnidadeFisica nuf ON nuf.ID = nvUF.ID
	WHERE E.Ler = 1
		AND frd.isDeleted = 0
		AND sfrduf.isDeleted = 0
		AND nvUF.isDeleted = 0
		AND (nuf.isDeleted IS NULL OR nuf.isDeleted = 0)
		AND (nuf.Eliminado IS NULL OR nuf.Eliminado = 0)
    
	SELECT * FROM #UFRelated

	SELECT n.ID, n.Codigo, nd.Designacao, 
		dp.FimAno,
		dp.FimMes, 
		dp.FimDia,
		dp.InicioAno,
		dp.InicioMes, 
		dp.InicioDia,
		Ufs.IsNotDocRelated,
		Ufs.IsSerieRelated
	FROM Nivel n
		INNER JOIN (SELECT IDUF, MIN(IsNivelDoc) IsNotDocRelated, MAX(IsNivelDoc) IsSerieRelated FROM #UFRelated GROUP BY IDUF) Ufs ON Ufs.IDUF = n.ID
		INNER JOIN NivelDesignado nd ON nd.ID = n.ID
		INNER JOIN FRDBase frd on frd.IDNivel = n.ID
		LEFT JOIN SFRDDatasProducao dp ON dp.IDFRDBase = frd.ID
	WHERE n.isDeleted = 0
		AND nd.isDeleted = 0
		AND frd.isDeleted = 0
		AND (dp.isDeleted IS NULL OR dp.isDeleted = 0)
	ORDER BY dbo.fn_AddPaddingToDateMember_new(dp.FimAno, 4), 
		dbo.fn_AddPaddingToDateMember_new(dp.FimMes, 2), 
		dbo.fn_AddPaddingToDateMember_new(dp.FimDia, 2),
		dbo.fn_AddPaddingToDateMember_new(dp.InicioAno, 4), 
		dbo.fn_AddPaddingToDateMember_new(dp.InicioMes, 2), 
		dbo.fn_AddPaddingToDateMember_new(dp.InicioDia, 2)

	SELECT rh.ID, rh.IDUpper, rh.IDTipoNivelRelacionado, frd.ID,
		dp.FimAno,
		dp.FimMes, 
		dp.FimDia,
		dp.InicioAno,
		dp.InicioMes, 
		dp.InicioDia,
		n.Codigo, 
		ndUpper.Designacao DesignacaoUpper,
		nd.Designacao,
		sfrda.Preservar,
		sfrda.IDAutoEliminacao,
		CASE WHEN rh.IDTipoNivelRelacionado = 9 AND sfrda.Preservar IS NULL AND sfrdaUpper.Preservar = 0 THEN dbo.fn_IsPrazoElimExp(frd.ID, @now) ELSE NULL END,
		E.Escrever
	FROM #TempRelacaoHierarquica rh
		INNER JOIN #effective E ON E.IDNivel = rh.ID
		INNER JOIN Nivel nUpper ON nUpper.ID = rh.IDUpper AND nUpper.isDeleted = 0
		LEFT JOIN FRDBase frdUpper ON frdUpper.IDNivel = nUpper.ID AND frdUpper.isDeleted = 0
		LEFT JOIN SFRDAvaliacao sfrdaUpper ON sfrdaUpper.IDFRDBase = frdUpper.ID AND sfrdaUpper.isDeleted = 0
		INNER JOIN Nivel n ON n.ID = rh.ID AND n.isDeleted = 0
		INNER JOIN NivelDesignado nd ON nd.ID = n.ID AND nd.isDeleted = 0
		INNER JOIN FRDBase frd ON frd.IDNivel = n.ID AND frd.isDeleted = 0
		LEFT JOIN NivelDesignado ndUpper ON ndUpper.ID = rh.IDUpper AND ndUpper.isDeleted = 0 -- o nivel acima pode ser orgânico
		LEFT JOIN SFRDDatasProducao dp ON dp.IDFRDBase = frd.ID AND dp.isDeleted = 0
		LEFT JOIN SFRDAvaliacao sfrda ON sfrda.IDFRDBase = frd.ID AND sfrda.isDeleted = 0
	WHERE E.Ler = 1
	ORDER BY dbo.fn_AddPaddingToDateMember_new(dp.FimAno, 4), 
		dbo.fn_AddPaddingToDateMember_new(dp.InicioAno, 4), 
		n.Codigo, 
		DesignacaoUpper, 
		nd.Designacao
		
	DROP TABLE #effective
END

GO


