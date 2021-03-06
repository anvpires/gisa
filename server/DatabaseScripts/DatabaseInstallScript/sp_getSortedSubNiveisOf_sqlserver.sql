SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_getSortedSubNiveisOf]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_getSortedSubNiveisOf]
GO

CREATE PROCEDURE sp_getSortedSubNiveisOf 
@nivelID BIGINT
AS
SELECT n.ID
FROM Nivel n
	INNER JOIN RelacaoHierarquica rh ON rh.ID = n.ID
	INNER JOIN Nivel nUpper ON nUpper.ID = rh.IDUpper
	LEFT JOIN NivelDesignado nd ON nd.ID = n.ID
	LEFT JOIN NivelControloAut nca ON nca.ID = n.ID
	LEFT JOIN ControloAut ca ON ca.ID = nca.IDControloAut
	LEFT JOIN ControloAutDatasExistencia cade ON cade.IDControloAut = ca.ID
	LEFT JOIN ControloAutDicionario cad ON cad.IDControloAut = ca.ID
	LEFT JOIN Dicionario d ON d.ID = cad.IDDicionario
	LEFT JOIN FRDBase frd ON frd.IDNivel = n.ID
	LEFT JOIN SFRDDatasProducao dp ON dp.IDFRDBase = frd.ID
WHERE
	rh.IDUpper = @nivelID AND
	rh.IDTipoNivelRelacionado != 11 AND
	(cad.IDTipoControloAutForma IS NULL OR cad.IDTipoControloAutForma = 1)  AND -- se se tratar de um nível controlado, obter apenas a forma autorizada
	(cad.IDDicionario IS NULL OR cad.isDeleted = 0)  AND 
	(frd.IDTipoFRDBase IS NULL OR frd.IDTipoFRDBase = 1) AND
	rh.isDeleted = 0 AND
	n.isDeleted = 0
ORDER BY 
	rh.IDTipoNivelRelacionado,
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN n.Codigo ELSE '' END,
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.InicioAno, 4) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.InicioAno else dp.InicioAno end , 4) END, 
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.InicioMes, 2) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.InicioMes else dp.InicioMes end , 2) END, 
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.InicioDia, 2) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.InicioDia else dp.InicioDia end , 2) END, 
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.FimAno, 4) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.FimAno else dp.FimAno end , 4) END, 
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.FimMes, 2) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.FimMes else dp.FimMes end , 2) END, 
	CASE WHEN rh.IDTipoNivelRelacionado = 9 THEN '' WHEN rh.IDTipoNivelRelacionado = 3 THEN dbo.fn_AddPaddingToDateMember(cade.FimDia, 2) ELSE dbo.fn_AddPaddingToDateMember(case when nUpper.IDTipoNivel = 2 then rh.FimDia else dp.FimDia end , 2) END, 
	COALESCE(nd.Designacao, CONVERT(NVARCHAR, d.Termo))
GO

