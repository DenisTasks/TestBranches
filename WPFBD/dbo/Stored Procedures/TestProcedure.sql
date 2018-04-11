
CREATE PROCEDURE [dbo].[TestProcedure] 
		@result int OUTPUT
AS
		SELECT @result = COUNT(LocationId)
		FROM Appointments
		SELECT @result