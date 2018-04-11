
CREATE PROCEDURE [dbo].[TestDates] 
	@beginningDate datetime, 
	@endingDate datetime,
	@locationId int

AS

	SELECT *
	FROM Appointments AS A 

	WHERE(A.LocationId = @locationId
	AND A.BeginningDate < @endingDate
	AND @beginningDate < A.EndingDate)