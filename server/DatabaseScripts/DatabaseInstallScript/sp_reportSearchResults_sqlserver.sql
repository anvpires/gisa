if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_reportSearchResults]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_reportSearchResults]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE dbo.sp_reportSearchResults

 AS
BEGIN
	CREATE TABLE #SPParametersNiveis (IDNivel BIGINT)
	CREATE TABLE #SPResultsCodigos(IDNivel BIGINT, CodigoCompleto NVARCHAR(300))

	-- Parametrizar Niveis para calculo de codigos
	INSERT INTO #SPParametersNiveis (IDNivel)
	SELECT rpn.ID
	FROM #ReportParametersNiveis rpn
	  
  	-- Calcular códigos
	EXEC sp_getCodigosCompletosNiveis

	-- retornar os códigos
	SELECT IDNivel, CodigoCompleto
	FROM #SPResultsCodigos 
 
	-- retornar os documentos
	SELECT rpn.ID, nd.Designacao, tnr.Designacao, dp.InicioAno, dp.InicioMes, dp.InicioDia, dp.InicioAtribuida, dp.FimAno, dp.FimMes, dp.FimDia, dp.FimAtribuida
	FROM #ReportParametersNiveis rpn
		INNER JOIN NivelDesignado nd ON nd.ID = rpn.ID
		INNER JOIN (
			SELECT rpn.ID, min(tnr.ID) m, tnr.Designacao
			FROM #ReportParametersNiveis rpn
				INNER JOIN RelacaoHierarquica rh ON rh.ID = rpn.ID
				INNER JOIN TipoNivelRelacionado tnr ON tnr.ID = rh.IDTipoNivelRelacionado
			GROUP BY tnr.ID, rpn.ID, tnr.Designacao
		) tnr ON tnr.ID = rpn.ID
		INNER JOIN FRDBase frd on frd.IDNivel = rpn.ID
		LEFT JOIN SFRDDatasProducao dp ON dp.IDFRDBase = frd.ID
	ORDER BY rpn.seq_id

      	DROP TABLE #SPParametersNiveis
	DROP TABLE #SPResultsCodigos
END
GO

