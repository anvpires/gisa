SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_manageModelosAvaliacao]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_manageModelosAvaliacao]
GO

/*
@Operation = 0 => Edit
@Operation = 1 => Delete
*/


CREATE PROCEDURE sp_manageModelosAvaliacao (
@Operation BIT,
@IDModeloAvaliacao BIGINT,
@Designacao NVARCHAR (768) = NULL,
@PrazoConservacao SMALLINT = NULL,
@Preservar BIT = NULL
) AS
BEGIN
	DECLARE @cont BIGINT
	
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION

		SELECT @cont = COUNT(IDFRDBase) FROM SFRDAvaliacao WHERE IDModeloAvaliacao = @IDModeloAvaliacao

		IF @cont = 0
		BEGIN
			IF @Operation = 0
				UPDATE ModelosAvaliacao SET Designacao = @Designacao, PrazoConservacao = @PrazoConservacao, Preservar = @Preservar WHERE ID = @IDModeloAvaliacao
			ELSE
				DELETE FROM ModelosAvaliacao WHERE ID = @IDModeloAvaliacao
			
			SELECT 1
		END
		ELSE
			SELECT 0

	COMMIT
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
