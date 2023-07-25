-- Jsem device s ID bla bla bla, jake klice me otevrou?
SELECT DISTINCT k.Pass
FROM dbo.Devices as d
INNER JOIN dbo.DevicesRoles AS dr 
	ON (dr.DeviceFId = d.Id)
INNER JOIN dbo.RolesKeys AS rk
	ON (rk.RoleFId = dr.RoleFId)
INNER JOIN dbo.Keys AS k 
	ON (k.Id = rk.KeyFId)
WHERE d.Id = '90d42ea2-96b2-4cb2-b949-840f96d66d2a'
