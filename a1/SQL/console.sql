-- check if user type is franchisee
ALTER FUNCTION [dbo].[userTypeIsFranchisee]
  (
    @UserID AS int
  )
  RETURNS bit
AS
  BEGIN
    DECLARE @userType VARCHAR(10)
    SELECT @userType = type from users where id = @UserID;
    IF @userType = 'Franchisee'
      RETURN 1;
    RETURN 0;
  END;

-- make sure user store associative table holds only franchisee
ALTER TABLE UserStore
  ADD CONSTRAINT chk_user
CHECK ([dbo].userTypeIsFranchisee(UserID)  =  1 )


-- create view for stock requests
CREATE VIEW StockRequestsView
  AS
    SELECT StockRequestID                                            As ID,
           S2.Name                                                   As Store,
           P.Name                                                    As Product,
           SR.Quantity,
           OI.StockLevel                                             As "Current Stock",
           CAST(IIF((OI.StockLevel - SR.Quantity) >= 0, 1, 0) As BIT) As "Stock Availability"
    FROM StockRequest SR
           JOIN Product P on SR.ProductID = P.ProductID
           JOIN Store S2 on SR.StoreID = S2.StoreID
           JOIN OwnerInventory OI on SR.ProductID = OI.ProductID


-- all stock requests
CREATE PROCEDURE [get all stock requests]
    @offset integer, @fetch integer
AS
  select *
  from StockRequestsView
  order by ID offset @offset rows
  fetch next @fetch rows only
go

execute [get all stock requests] 0, 6

-- count stock requests
CREATE PROC [count total rows]
    @tablename VARCHAR(30)
AS
  EXEC ('select count(*) FROM [' + @tablename + ']')
go

execute [count total rows]
    @tablename = 'StockRequestsView'
go

-- action stock request
CREATE PROCEDURE [dbo].[action stock request]
    @RequestID AS int
AS
  BEGIN
    DECLARE @CanUpdate BIT;
    SELECT @CanUpdate = [Stock Availability] FROM StockRequestsView WHERE ID = @RequestID;
    IF @CanUpdate = 0
      THROW 51000, 'Insufficient stock level', 1;
    ELSE
      BEGIN
        DECLARE @StoreID int;
        DECLARE @ProductID int;
        DECLARE @Quantity int;
        SELECT @StoreID = StoreID FROM StockRequest WHERE StockRequestID = @RequestID;
        SELECT @ProductID = ProductID FROM StockRequest WHERE StockRequestID = @RequestID;
        SELECT @Quantity = Quantity FROM StockRequest WHERE StockRequestID = @RequestID;
        DECLARE @UpdateTransaction BIT;
        -- Owner Inventory Stock level
        declare @InvOldLevel int;
        select @InvOldLevel = StockLevel FROM OwnerInventory where ProductID = @ProductID;

        -- this is update transaction if count greater than 0 due to constraints in StoreInventory table
        SELECT @UpdateTransaction = count(*) FROM StoreInventory WHERE StoreID = @StoreID
                                                                   AND ProductID = @ProductID;
        IF @UpdateTransaction > 0
          BEGIN
            declare @OldValue int;
            SELECT @OldValue = StockLevel FROM StoreInventory WHERE StoreID = @StoreID
                                                                AND ProductID = @ProductID;
            BEGIN TRY
            BEGIN TRANSACTION
            UPDATE StoreInventory
            SET StockLevel = (@OldValue + @Quantity)
            WHERE StoreID = @StoreID
              AND ProductID = @ProductID;
            Update OwnerInventory SET StockLevel = (@InvOldLevel - @Quantity) WHERE ProductID = @ProductID;
            DELETE FROM StockRequest WHERE StockRequestID = @RequestID;
            COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
            IF @@TRANCOUNT > 0
              ROLLBACK TRANSACTION;
            THROW;
            END CATCH
          END
        ELSE
          begin try
          begin transaction
          insert into StoreInventory values (@StoreID, @ProductID, @Quantity);
          Update OwnerInventory SET StockLevel = (@InvOldLevel - @Quantity) WHERE ProductID = @ProductID;
          DELETE FROM StockRequest WHERE StockRequestID = @RequestID;
          commit transaction;
          end try
          begin catch
          if @@trancount > 0
            rollback transaction;
          throw;
          end catch
      END
  END
GO

exec [action stock request] 4


-- get table from stock request
CREATE FUNCTION dbo.[get request structure](@RequestID as int)
  returns table as
  return
  (
  SELECT SR.StoreID, SR.ProductID, SR.Quantity
  FROM StockRequest SR
  WHERE SR.StockRequestID = @RequestID
  );


-- owner inventory view
CREATE VIEW OwnerInventoryView
  AS
    SELECT OI.ProductID,
           P.Name,
           OI.StockLevel
    FROM OwnerInventory OI
           LEFT JOIN Product P on OI.ProductID = P.ProductID
GO


-- get all Owner Inventory with stock level
create procedure dbo.[get all owner inventory]
    @offset integer, @fetch integer
AS
  select *
  from OwnerInventoryView
  order by ProductID offset @offset rows
  fetch next @fetch rows only
go

-- update stock level
create procedure dbo.[reset stock level]
    @ProductId int, @level int
AS
  begin
    declare @StockLevel int;
    select @StockLevel = StockLevel FROM OwnerInventory where ProductID = @ProductId;
    if @StockLevel < @level
      update OwnerInventory set StockLevel = @level where ProductID = @ProductId;
    else
      THROW 51000, 'enough stock', 1;
  end

go

