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
    CHECK ([dbo].userTypeIsFranchisee(UserID) = 1 )


-- create view for stock requests
CREATE VIEW StockRequestsView
AS
SELECT StockRequestID                                             As ID,
       S2.Name                                                    As Store,
       P.Name                                                     As Product,
       SR.Quantity,
       OI.StockLevel                                              As "Current Stock",
       CAST(IIF((OI.StockLevel - SR.Quantity) >= 0, 1, 0) As BIT) As "Stock Availability"
FROM StockRequest SR
       JOIN Product P on SR.ProductID = P.ProductID
       JOIN Store S2 on SR.StoreID = S2.StoreID
       JOIN OwnerInventory OI on SR.ProductID = OI.ProductID


-- all stock requests
CREATE PROCEDURE [get all stock requests] @offset integer, @fetch integer
AS
select *
from StockRequestsView
order by ID offset @offset rows
         fetch next @fetch rows only
go

-- count stock requests
CREATE PROC [count total rows] @tablename VARCHAR(30)
AS
EXEC ('select count(*) FROM [' + @tablename + ']')
go

execute [count total rows]
        @tablename = 'StockRequestsView'
go

-- action stock request
CREATE PROCEDURE [dbo].[action stock request] @RequestID AS int
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
      SELECT @UpdateTransaction = count(*)
      FROM StoreInventory
      WHERE StoreID = @StoreID
        AND ProductID = @ProductID;
      IF @UpdateTransaction > 0
        BEGIN
          declare @OldValue int;
          SELECT @OldValue = StockLevel
          FROM StoreInventory
          WHERE StoreID = @StoreID
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
create procedure dbo.[get all owner inventory] @offset integer, @fetch integer
AS
select *
from OwnerInventoryView
order by ProductID offset @offset rows
         fetch next @fetch rows only
go

-- update stock level
create procedure dbo.[reset stock level] @ProductId int, @level int
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


-- user location view
create view FranchLocationView
AS
select us.StoreID, S.Name, u.user_name
from UserStore us
       join Store S on us.StoreID = S.StoreID
       join Users U on us.UserID = U.id
GO

-- get franchise for franchisee
create proc dbo.[get location] @username varchar(25)
AS
select StoreID
from FranchLocationView
where user_name = @username;
go

-- store inventory levels view
create view StoreInventoryView
as
select si.ProductID, P.Name As "product", S.Name as "store", si.StockLevel
from StoreInventory SI
       join Product P on SI.ProductID = P.ProductID
       join Store S on SI.StoreID = S.StoreID


-- get stock level for a store
create proc dbo.[get stock for store]
  @storename varchar(25), @offset integer, @fetch integer
as
select ProductID, product, StockLevel
from StoreInventoryView
where store = @storename
order by ProductID
         offset @offset rows
         fetch next @fetch rows only;
go

-- get stock level for a store with threshold
create proc dbo.[get stock for store with threshold]
  @storename varchar(25), @threshold int, @offset int, @fetch int
as
select ProductID, product, StockLevel
from StoreInventoryView
where store = @storename
  and StockLevel < @threshold
order by ProductID
         offset @offset rows
         fetch next @fetch rows only;
go


-- count stock for store
create proc dbo.[count stocks for store]
@storename varchar(25)
as
select count(*)
from StoreInventoryView
where store = @storename
go


-- count stock for store with threshold
create proc dbo.[count stocks for store with threshold]
  @storename varchar(25), @threshold int
as
select count(*)
from StoreInventoryView
where store = @storename
  and StockLevel < @threshold
go



-- get stock level for a store
create proc dbo.[get complete stock list for store] @storeName varchar(25)
as
with SS as (select ProductID, StockLevel
            from StoreInventoryView
            where store = @storeName)
select P.*, coalesce(SS.StockLevel, 0) as "StockLevel"
from Product P
       left outer join SS on P.ProductID = SS.ProductID
go

-- create new stock request
create proc dbo.[create stock request] @storeName varchar(25), @prodId int, @qty int
as
begin
  declare @storeId int;
  select @storeId = StoreID FROM Store where Name = @storeName;
  insert into StockRequest(StoreID, ProductID, Quantity) values (@storeId, @prodId, @qty)
end
go


-- get non store stocks
create proc dbo.[get non stock for store]
  @storename varchar(25), @offset int, @fetch int
as
select *
from OwnerInventoryView
where ProductID not in (select ProductID from StoreInventoryView where store = @storename)
order by ProductID
         offset @offset rows
         fetch next @fetch rows only;
go

-- count non stock store
create proc dbo.[count non stocks for store]
@storename varchar(25)
as
select count(ProductID)
from OwnerInventoryView
where ProductID not in (select ProductID from StoreInventoryView where store = @storename)
go


-- make purchase an item in store
create proc dbo.[purchase an item in store]
  @storename varchar(25), @prodId int, @qty int
as
begin
  declare @storeId int;
  select @storeId = StoreID FROM Store where Name = @storeName;
  update StoreInventory
  set StockLevel = @qty
  where StoreID = @storeId
    and ProductID = @prodId
end