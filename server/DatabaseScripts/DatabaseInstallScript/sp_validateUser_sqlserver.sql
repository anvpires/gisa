SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_validateUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_validateUser]
GO

CREATE PROCEDURE sp_validateUser @username NVARCHAR(50), @password NVARCHAR(50) AS
BEGIN
	IF NOT EXISTS (SELECT * FROM Trustee WHERE Name = @username AND CatCode = 'USR' AND EXISTS (SELECT * FROM TrusteeUser WHERE IsActive = 1 AND Password = @password AND TrusteeUser.ID = Trustee.ID))
	BEGIN
		SELECT 7
		RETURN
	END

	IF NOT EXISTS (SELECT * FROM Trustee WHERE Name = @username AND IsActive = 1)
	BEGIN
		SELECT 8
		RETURN
	END

	IF NOT EXISTS (SELECT * FROM Trustee t INNER JOIN TrusteePrivilege tp ON t.ID = tp.IDTrustee)
	BEGIN
		SELECT 9
		RETURN
	END
	
	SELECT 0
END
GO

