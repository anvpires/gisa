SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[fn_ComparePartialNumber]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[fn_ComparePartialNumber]
GO


CREATE FUNCTION dbo.fn_ComparePartialNumber (@A varchar(100), @B varchar(100)) RETURNS Integer AS  
BEGIN
declare @clip integer
set @clip = 1

while (len(@A)>0 and len(@B)>0 and (isnumeric(@A)=0 or isnumeric(@B)=0))
begin
set @A = left(@A, len(@A)-1)
set @B = left(@B, len(@B)-1)
set @clip=2
end

if (len(@A)>0 and len(@B)>0)
begin
declare @Ai integer
declare @Bi integer
set @Ai = convert(integer, @A)
set @Bi = convert(integer, @B)
if (@Ai < @Bi) return -@clip
if (@Ai > @Bi) return @clip
if (abs(@clip) > 1) return @clip * @clip
return 0
end

return null
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

