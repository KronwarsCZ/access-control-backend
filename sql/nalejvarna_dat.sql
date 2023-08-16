-- Nalejvarna dat
-- Owners
DELETE FROM dbo.Owners;
INSERT INTO dbo.Owners (Id, Name, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('fb57f173-592d-4dbd-8224-ded75c01bd54', 'Firma1', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('C6513C1A-D70C-42F4-B627-50E933638A39', 'Firma2', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Areas
DELETE FROM dbo.Areas;
INSERT INTO dbo.Areas (Id, Name, Location, OwnerId, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('800332eb-be1c-4fbc-8901-beb67d66418e', 'Zakladna1', 'Praha', 'fb57f173-592d-4dbd-8224-ded75c01bd54', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('5414f12d-267e-4464-8f24-d7ebfed2c776','Zakladna2','Brno', 'fb57f173-592d-4dbd-8224-ded75c01bd54', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('807da66d-b83a-471a-9a01-07008327313d', 'Sidlo firmy2', 'Praha', 'C6513C1A-D70C-42F4-B627-50E933638A39', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Buildings
DELETE FROM dbo.Buildings;
INSERT INTO dbo.Buildings (Id, Name, Address, AreaEntityId, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('e2e7eca0-ddf9-4bcf-82e8-0a50c9aac741', 'Budova1', 'Nam. Republiky 1', '800332eb-be1c-4fbc-8901-beb67d66418e', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('3dc23a81-ef2c-45e9-8c06-e8984f70c3d7','Budova2','Nam. Republiky 2','800332eb-be1c-4fbc-8901-beb67d66418e', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('739dda7b-8648-48bb-af5f-1b62005f989d', 'Budova 3', 'Namesti s velkym cernym curakem', '5414f12d-267e-4464-8f24-d7ebfed2c776', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('f22945c6-e6ed-4c10-a49b-2186d58163ff', 'Hlavni budova', 'Uhelne doly', '807da66d-b83a-471a-9a01-07008327313d', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Roles
DELETE FROM dbo.Roles;
INSERT INTO dbo.Roles (Id, Name, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('8746faa2-f2ee-4254-9c97-a7a2b0b49a8f', 'Budova1 samotna role', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('ec7206da-1571-4757-bb57-37d8e892b1cd', 'Budova1,Budova 2 sdilena role', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('d4f7a401-9e71-479b-ad42-584dadc06323', 'Budova 2 samotna role', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('508764ed-c652-4a22-951b-2c9eed66b82b','Budova 3 samotna role', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
		('ae6c8f45-bd95-44cd-9f1c-ab745a812ba7','Hlavni budova samotna role', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Devices
DELETE FROM dbo.Devices;
INSERT INTO dbo.Devices (Id, Name, DeviceType, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('90d42ea2-96b2-4cb2-b949-840f96d66d2a','B1 dvere1','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('b4d18bba-04a5-416a-be86-3f8eeca3c323','B1 dvere2','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('ced5cf90-2aeb-4de6-9655-dfcff48f8e9b','B1/2 dvere 3','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('d7c604f9-73d1-437b-abd4-16a8ae037ce6','B2 dvere1','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('a1a32f8a-16c8-4303-a333-848678af0376','B3 dvere1','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('ff4412dc-5bc1-4ea5-ba51-8569ac44124c','Hl.b dvere1','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('75468618-ad71-4074-8865-0eaed0880edf','Hl.b dvere2','0', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Keys
DELETE FROM dbo.Keys;
INSERT INTO dbo.Keys (Id, Name, Pass, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES  ('0e0daf21-fb9a-4cff-b3b6-08f57999f39d','K1','aaa', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('3bc73d31-c206-4d3c-836b-aff1b128af94','K2','bbb', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('1bf9b052-952c-4a29-b29f-ffd69d65f73b','K3','ccc', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('04bcbc8a-49a3-41e3-9d91-429a0351cf35','K4','ddd', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('b34f7928-947d-49ae-af1b-5fc5c91aaad5','K5','eee', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('a5ca4ddb-6abf-4f64-ada8-15ac0347ef4b','K6','fff', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('451ff60d-17e4-45fc-a99e-3ceab3490940','K7','ggg', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('7db8b76e-42b4-4437-ac3b-9833c6f86914','K8','hhh', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('85e924da-25d7-49bb-9514-08bf0fae8ee3','K9','iii', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0'),
        ('a8e72dc0-63d9-4903-b051-75fa0ce1790d','K10','jjj', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');

-- Join table BuildingRoles
DELETE FROM dbo.BuildingRoles;
INSERT INTO dbo.BuildingRoles (BuildingId, RoleId)
VALUES  ('e2e7eca0-ddf9-4bcf-82e8-0a50c9aac741','8746faa2-f2ee-4254-9c97-a7a2b0b49a8f'),
        ('e2e7eca0-ddf9-4bcf-82e8-0a50c9aac741','ec7206da-1571-4757-bb57-37d8e892b1cd'),
        ('3dc23a81-ef2c-45e9-8c06-e8984f70c3d7','ec7206da-1571-4757-bb57-37d8e892b1cd'),
        ('3dc23a81-ef2c-45e9-8c06-e8984f70c3d7','d4f7a401-9e71-479b-ad42-584dadc06323'),
		('f22945c6-e6ed-4c10-a49b-2186d58163ff','ae6c8f45-bd95-44cd-9f1c-ab745a812ba7'),
        ('739dda7b-8648-48bb-af5f-1b62005f989d','508764ed-c652-4a22-951b-2c9eed66b82b');


-- Join table DevicesRoles
DELETE FROM dbo.DevicesRoles;
INSERT INTO dbo.DevicesRoles (DeviceId, RoleId)
VALUES  ('90d42ea2-96b2-4cb2-b949-840f96d66d2a','8746faa2-f2ee-4254-9c97-a7a2b0b49a8f'),
        ('b4d18bba-04a5-416a-be86-3f8eeca3c323','8746faa2-f2ee-4254-9c97-a7a2b0b49a8f'),
        ('ced5cf90-2aeb-4de6-9655-dfcff48f8e9b','ec7206da-1571-4757-bb57-37d8e892b1cd'),
        ('d7c604f9-73d1-437b-abd4-16a8ae037ce6','d4f7a401-9e71-479b-ad42-584dadc06323'),
        ('a1a32f8a-16c8-4303-a333-848678af0376','508764ed-c652-4a22-951b-2c9eed66b82b'),
        ('ff4412dc-5bc1-4ea5-ba51-8569ac44124c','ae6c8f45-bd95-44cd-9f1c-ab745a812ba7'),
        ('75468618-ad71-4074-8865-0eaed0880edf','ae6c8f45-bd95-44cd-9f1c-ab745a812ba7');

-- Join table RolesKeys
DELETE FROM dbo.RolesKeys;
INSERT INTO dbo.RolesKeys (RoleId, KeyId)
VALUES  ('8746faa2-f2ee-4254-9c97-a7a2b0b49a8f','0e0daf21-fb9a-4cff-b3b6-08f57999f39d'),
        ('8746faa2-f2ee-4254-9c97-a7a2b0b49a8f','3bc73d31-c206-4d3c-836b-aff1b128af94'),
        ('ec7206da-1571-4757-bb57-37d8e892b1cd','1bf9b052-952c-4a29-b29f-ffd69d65f73b'),
        ('ec7206da-1571-4757-bb57-37d8e892b1cd','3bc73d31-c206-4d3c-836b-aff1b128af94'),
        ('d4f7a401-9e71-479b-ad42-584dadc06323','04bcbc8a-49a3-41e3-9d91-429a0351cf35'),
        ('508764ed-c652-4a22-951b-2c9eed66b82b','b34f7928-947d-49ae-af1b-5fc5c91aaad5'),
        ('ae6c8f45-bd95-44cd-9f1c-ab745a812ba7','a5ca4ddb-6abf-4f64-ada8-15ac0347ef4b'),
        ('ae6c8f45-bd95-44cd-9f1c-ab745a812ba7','451ff60d-17e4-45fc-a99e-3ceab3490940'),
        ('ae6c8f45-bd95-44cd-9f1c-ab745a812ba7','7db8b76e-42b4-4437-ac3b-9833c6f86914'),
        ('ec7206da-1571-4757-bb57-37d8e892b1cd','85e924da-25d7-49bb-9514-08bf0fae8ee3'),
        ('ec7206da-1571-4757-bb57-37d8e892b1cd','a8e72dc0-63d9-4903-b051-75fa0ce1790d'),
        ('8746faa2-f2ee-4254-9c97-a7a2b0b49a8f','85e924da-25d7-49bb-9514-08bf0fae8ee3'),
        ('d4f7a401-9e71-479b-ad42-584dadc06323','a8e72dc0-63d9-4903-b051-75fa0ce1790d');

-- Accounts
DELETE FROM dbo.Accounts;
INSERT INTO dbo.Accounts(Id, Email, Role, PasswordHash, TwoFactorAuthenticationEnabled, OwnerId, CreatedUtc, UpdatedUtc, IsDeleted)
VALUES	('ee539bbe-ad02-490a-bf99-ad9570f46ca6', 'test@example.com', '1', '00000000000000000000000000000', '1', 'fb57f173-592d-4dbd-8224-ded75c01bd54', '2023-07-19T19:48:10.876Z', '2023-07-19T19:48:10.876Z', '0');