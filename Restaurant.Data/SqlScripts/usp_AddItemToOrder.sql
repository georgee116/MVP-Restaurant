CREATE PROCEDURE dbo.usp_AddItemToOrder
    @ComandaId    INT,
    @PreparatId   INT,
    @MeniuId      INT     = NULL,
    @Cantitate    INT,
    @CantPortie   FLOAT,
    @PretUnitate  DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ComandaItems
        (ComandaId, PreparatId, MeniuId, Cantitate, CantitatePortie, PretUnitate)
    VALUES
        (@ComandaId, @PreparatId, @MeniuId, @Cantitate, @CantPortie, @PretUnitate);
END
GO
