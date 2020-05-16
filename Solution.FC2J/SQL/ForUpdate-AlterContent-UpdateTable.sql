declare @subscribed as int = 0

Select * from PriceList where Id  = 38
select @subscribed = count(*) from CustomerPriceList where pricelistId = 38
update PriceList set Subscribed = @subscribed where Id  = 38
Select * from PriceList where Id  = 38

Select * from PriceList where Id  = 39
select @subscribed = count(*) from CustomerPriceList where pricelistId = 39
update PriceList set Subscribed = @subscribed where Id  = 39
Select * from PriceList where Id  = 39

Select * from PriceList where Id  = 40
select @subscribed = count(*) from CustomerPriceList where pricelistId = 40
update PriceList set Subscribed = @subscribed where Id  = 40
Select * from PriceList where Id  = 40

Select * from PriceList where Id  = 41
select @subscribed = count(*) from CustomerPriceList where pricelistId = 41
update PriceList set Subscribed = @subscribed where Id  = 41
Select * from PriceList where Id  = 41

Select * from PriceList where Id  = 43
select @subscribed = count(*) from CustomerPriceList where pricelistId = 43
update PriceList set Subscribed = @subscribed where Id  = 43
Select * from PriceList where Id  = 43