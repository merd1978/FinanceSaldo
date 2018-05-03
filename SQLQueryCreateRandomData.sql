DECLARE @RowCount INT
DECLARE @RowString VARCHAR(10)
DECLARE @Random INT
DECLARE @Upper INT
DECLARE @Lower INT
DECLARE @InsertDate DATETIME

SET @Lower = -730
SET @Upper = -1
SET @RowCount = 0

WHILE @RowCount < 10000
BEGIN
	SET @RowString = CAST(@RowCount AS nvarchar(10))
	SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)
	SET @InsertDate = DATEADD(dd, @Random, GETDATE())
	
	INSERT INTO Invoices
		(Name, Date, Debit, Credit, CompanyId, ExpiryDays)
	VALUES
		(REPLICATE('0', 10 - DATALENGTH(@RowString)) + @RowString
		,DATEADD(dd, 1, @InsertDate)
		, 12
		, 0
		, 1
		, 40)

	SET @RowCount = @RowCount + 1
END