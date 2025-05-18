CREATE PROCEDURE dbo.usp_GetMeniuDetails
    @MeniuId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        m.Id                         AS MeniuId,
        m.Denumire,
        c.Nume                       AS Categorie,
        pm.PreparatId,
        p.Denumire                   AS Preparat,
        pm.CantitatePortieMeniu      AS GramajPortie,
        p.Pret                       AS PretStandard,
        pm.CantitatePortieMeniu * p.Pret AS Subtotal
    FROM Meniuri AS m
    INNER JOIN Categorii       AS c  ON c.Id = m.CategorieId
    INNER JOIN PreparatMeniuri AS pm ON pm.MeniuId = m.Id
    INNER JOIN Preparate       AS p  ON p.Id = pm.PreparatId
    WHERE m.Id = @MeniuId;

    SELECT
        SUM(pm.CantitatePortieMeniu)           AS TotalGramaj,
        SUM(pm.CantitatePortieMeniu * p.Pret)  AS TotalPret
    FROM PreparatMeniuri AS pm
    INNER JOIN Preparate      AS p ON p.Id = pm.PreparatId
    WHERE pm.MeniuId = @MeniuId;
END
GO
