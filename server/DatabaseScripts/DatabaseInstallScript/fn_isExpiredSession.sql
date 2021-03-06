SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[fn_isExpiredSession]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[fn_isExpiredSession]
GO

CREATE FUNCTION [dbo].[fn_isExpiredSession] (@now AS DATETIME, @AccessDate AS DATETIME)  
RETURNS BIT AS  
BEGIN 
	DECLARE @result BIT

	IF datediff(ss, @AccessDate, @now) > 120 OR datediff(ss, @AccessDate, @now) < -120 
		SET @result = 1
	ELSE
		SET @result = 0

	RETURN @result	
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

