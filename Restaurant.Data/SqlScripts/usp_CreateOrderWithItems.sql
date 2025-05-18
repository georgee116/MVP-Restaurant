-- 1) Definirea tipului TVP, dacă nu există deja
IF TYPE_ID(N'dbo.TVP_ComandaItem') IS NULL
BEGIN
    CREATE TYPE dbo.TVP_ComandaItem AS TABLE
    (
        PreparatId    INT              NULL,
        MeniuId       INT              NULL,
        Cantitate     INT              NOT NULL,
        CantPortie    FLOAT            NOT NULL,
        PretUnitate   DECIMAL(18,2)    NOT NULL
    );
END
GO

-- 2) Procedura care creează comanda și item-urile într-o tranzacție
CREATE PROCEDURE dbo.usp_CreateOrderWithItems
    @UtilizatorId  INT,
    @Discount      DECIMAL(18,2),
    @CostTransport DECIMAL(18,2),
    @TVP_Items     dbo.TVP_ComandaItem READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRAN;

    DECLARE @NewOrderId INT;
    DECLARE @CodUnic   NVARCHAR(50) = CONCAT('CMD', FORMAT(GETDATE(), 'yyyyMMddHHmmss'));

    INSERT INTO Comenzi
        (CodUnic, DataComenzii, Status, DiscountAplicat, CostTransport, UtilizatorId)
    VALUES
        (@CodUnic, GETDATE(), 0, @Discount, @CostTransport, @UtilizatorId);

    SET @NewOrderId = SCOPE_IDENTITY();

    INSERT INTO ComandaItems
        (ComandaId, PreparatId, MeniuId, Cantitate, CantitatePortie, PretUnitate)
    SELECT
        @NewOrderId,
        PreparatId,
        MeniuId,
        Cantitate,
        CantPortie,
        PretUnitate
    FROM @TVP_Items;

    COMMIT TRAN;

    SELECT @NewOrderId AS ComandaId, @CodUnic AS CodUnic;
END
GO
