CREATE FUNCTION fn_AddPaddingToDateMember_new2 (@member NVARCHAR(4), @maxLen INTEGER) RETURNS NVARCHAR(4) AS  
BEGIN 
	IF LEN(@member) = @maxLen RETURN @member;
	IF @member IS NULL OR LEN(@member) = 0 OR @member = '?' OR @member = '??' OR @member = '???' OR @member = '????' 
		RETURN replicate('9', @maxLen)
	RETURN replicate('9', @maxLen - LEN(@member)) + replace(@member, '?','9');
END
GO

