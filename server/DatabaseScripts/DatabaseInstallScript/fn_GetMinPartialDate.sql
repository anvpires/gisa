/****** Object:  UserDefinedFunction [dbo].[fn_GetMinPartialDate]    Script Date: 04/24/2009 17:12:15 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
/****************************
 * Devolve uma tabela de uma linha apenas com a menor data. NA REALIDADE DEVIAMOS DESDOBRAR ISTO EM 2 FUNCOES SEPARADAS, UMA QUE CALCULE QUAL O MAIOR E OUTRA QUE CALCULE QUAL O MAIS VAGO
 */
CREATE FUNCTION [dbo].[fn_GetMinPartialDate] (
	@AnoA varchar(4),
	@MesA varchar(2),
	@DiaA varchar(2),
	@AnoB varchar(4),
	@MesB varchar(2),
	@DiaB varchar(2))
	RETURNS @DataMinima TABLE
	(
		Ano	VARCHAR(4),
		Mes	VARCHAR(2),
		Dia	VARCHAR(2)
	)
AS  
BEGIN 

	DECLARE @ComparacaoAno INTEGER
	DECLARE @ComparacaoMes INTEGER
	DECLARE @ComparacaoDia INTEGER
	DECLARE @ResultadoFinal INTEGER

	-- Remocao das interrogacoes completas 
	IF @AnoA IS NULL OR @AnoA = '?' OR @AnoA = '??' OR @AnoA = '???' OR @AnoA = '????' SET @AnoA = ''
	IF @MesA IS NULL OR @MesA = '?' OR @MesA = '??' SET @MesA = ''
	IF @DiaA IS NULL OR @DiaA = '?' OR @AnoA = '??' SET @DiaA = ''
	IF @AnoB IS NULL OR @AnoB = '?' OR @AnoB = '??' OR @AnoB = '???' OR @AnoB = '????' SET @AnoB = ''
	IF @MesB IS NULL OR @MesB = '?' OR @MesB = '??' SET @MesB = ''
	IF @DiaB IS NULL OR @DiaB = '?' OR @DiaB = '??' SET @DiaB = ''

	-- Determinar resultado final da comparacao bem como resultados intermedios
	SET @ComparacaoAno = dbo.fn_ComparePartialNumber2(@AnoA, @AnoB)
	SET @ComparacaoMes = dbo.fn_ComparePartialNumber2(@MesA, @MesB)
	SET @ComparacaoDia = dbo.fn_ComparePartialNumber2(@DiaA, @DiaB)
	IF (@ComparacaoAno IS NULL OR @ComparacaoAno <> 0) SET @ResultadoFinal = @ComparacaoAno
	ELSE IF (@ComparacaoMes IS NULL OR @ComparacaoMes <> 0) SET @ResultadoFinal = @ComparacaoMes
	ELSE IF (@ComparacaoDia IS NULL OR @ComparacaoDia <> 0) SET @ResultadoFinal = @ComparacaoDia
	ELSE SET @ResultadoFinal = 0


	IF (@ResultadoFinal IS NULL)
	BEGIN
		INSERT @DataMinima
			SELECT CASE WHEN @ComparacaoAno IS NULL THEN
					CASE WHEN LEN(@AnoA)=0 OR LEN(@AnoB)=0 THEN '' ELSE @AnoA END
				    WHEN @ComparacaoAno <= 0 THEN @AnoA
				    WHEN @ComparacaoAno > 2 THEN @AnoB
				    ELSE 'ERRO' -- erro!!!
				END,
	
				CASE WHEN @ComparacaoMes IS NULL THEN 
					CASE WHEN @ComparacaoAno IN (0, 1, -1) THEN @MesA ELSE '' END
				     WHEN @ComparacaoMes <= 0 THEN
					CASE WHEN @ComparacaoAno IN (0, -1) THEN @MesA ELSE '' END
				     WHEN @ComparacaoMes > 0 THEN 
					CASE WHEN @ComparacaoAno IN (0, 1) THEN @MesB ELSE '' END
				     ELSE 'ERRO' -- erro!!!
				END, 
	
				CASE WHEN @ComparacaoDia IS NULL THEN 
					CASE WHEN @ComparacaoMes IN (0, 1, -1) THEN @DiaA ELSE '' END
				     WHEN @ComparacaoDia <= 0 THEN
					CASE WHEN @ComparacaoAno IN (0, -1) AND @ComparacaoMes IN (0, -1) THEN @DiaA ELSE '' END
				     WHEN @ComparacaoDia > 0 THEN 
					CASE WHEN @ComparacaoAno IN (0, 1) AND @ComparacaoMes IN (0, 1) THEN @DiaB ELSE '' END
				     ELSE 'ERRO' -- erro!!!
				END
		RETURN	
	END
	ELSE IF (@ResultadoFinal = 0)
	BEGIN
		INSERT @DataMinima
			SELECT @AnoA, @MesA, @DiaA
		RETURN
	END


	IF (@ResultadoFinal < 0)  -- Considerar a primeira data
	BEGIN
		INSERT @DataMinima
			SELECT @AnoA, 
				CASE WHEN ABS(@ComparacaoAno) IN (0, 1) THEN @MesA ELSE '' END,
				CASE WHEN ABS(@ComparacaoAno) IN (0, 1) AND ABS(@ComparacaoMes) IN (0, 1) THEN @DiaA ELSE '' END
	END
	ELSE IF (@ResultadoFinal > 0)  -- Considerar a segunda data
	BEGIN
		INSERT @DataMinima
			SELECT @AnoB, 
				CASE WHEN ABS(@ComparacaoAno) IN (0, 1) THEN @MesB ELSE '' END, 
				CASE WHEN ABS(@ComparacaoAno) IN (0, 1) AND ABS(@ComparacaoMes) IN (0, 1) THEN @DiaB ELSE '' END
	END

	RETURN
END
GO
