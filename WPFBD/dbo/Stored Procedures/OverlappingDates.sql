-- =============================================
-- Author:		Dzianis_Tarasevich
-- Create date: 
-- Description:	Overlapping Dates
-- =============================================
CREATE PROCEDURE [dbo].[OverlappingDates] 
	@beginningDate datetime, 
	@endingDate datetime,
	@locationId int,
	@result int OUTPUT
AS
DECLARE @test int
	SELECT @test = COUNT(AppointmentId)
	FROM Appointments

	SELECT @result = COUNT(A.AppointmentId)
	FROM Appointments AS A JOIN Appointments AS B
	ON(A.LocationId = @locationId
	AND A.BeginningDate < @endingDate
	AND @beginningDate < A.EndingDate)

	IF @test > 0
BEGIN
	SET @result = (@result / @test)
	SELECT @result
END
	ELSE
BEGIN
	SELECT @result
END