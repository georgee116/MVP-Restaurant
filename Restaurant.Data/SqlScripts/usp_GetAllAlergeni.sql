CREATE PROCEDURE dbo.usp_GetAllAlergeni
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nume
    FROM Alergeni
    ORDER BY Nume;
END
GO
