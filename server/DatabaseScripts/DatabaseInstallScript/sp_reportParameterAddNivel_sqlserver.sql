IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_reportParameterAddNivel]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_reportParameterAddNivel]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE sp_reportParameterAddNivel @IDNivel BIGINT AS
BEGIN
	INSERT INTO #ReportParametersNiveis VALUES(@IDNivel, NULL)
END
GO
