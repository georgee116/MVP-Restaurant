CREATE PROCEDURE dbo.usp_RemovePreparatFromMeniu
    @MeniuId    INT,
    @PreparatId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PreparatMeniuri
    WHERE MeniuId    = @MeniuId
      AND PreparatId = @PreparatId;
END
GO
