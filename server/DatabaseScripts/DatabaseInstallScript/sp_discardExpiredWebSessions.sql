IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ExistsControloAutDicionario]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ExistsControloAutDicionario]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_discardExpiredWebSessions] 
AS
BEGIN
    -- Delete cached results with more than 0.8 days
	DELETE SearchCacheWeb 
		FROM SearchCacheWeb scw
			INNER JOIN WebClientActivity wca ON wca.ClientGUID=scw.ClientGUID
		WHERE wca.LastSearch < GETDATE() - 0.8;
	DELETE FROM WebClientActivity 
	WHERE LastSearch < GETDATE() - 0.8;
END
GO
