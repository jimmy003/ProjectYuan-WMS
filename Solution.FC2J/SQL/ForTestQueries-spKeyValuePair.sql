exec GetSales @UserName=N'admin'

exec [spKeyValuePair_GetList]

exec [spKeyValuePair_Update] @Key = 'Period', @Value = 'Week'

exec [spKeyValuePair_Update] @Key = 'PeriodNumber', @Value = '1'

exec [spKeyValuePair_GetList]

