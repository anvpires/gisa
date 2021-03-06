IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ExistsControloAutDicionario]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ExistsControloAutDicionario]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_ExistsControloAutDicionario]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_ExistsControloAutDicionario]
GO

CREATE PROCEDURE sp_ExistsControloAutDicionario  @IDDicionario BIGINT, @IDTipoControloAutForma BIGINT, @IDTipoNoticiaAut BIGINT   AS

	IF EXISTS (SELECT * FROM ControloAutDicionario cad WITH (UPDLOCK) INNER JOIN ControloAut ca ON ca.ID = cad.IDControloAut WHERE cad.IDDicionario = @IDDicionario AND cad.IDTipoControloAutForma = @IDTipoControloAutForma AND ca.IDTipoNoticiaAut = @IDTipoNoticiaAut AND cad.isDeleted = 0 AND ca.isDeleted = 0)
		SELECT 1
	ELSE
		SELECT 0
GO

