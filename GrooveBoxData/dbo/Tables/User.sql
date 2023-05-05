CREATE TABLE [dbo].[User]
(
	[Id] NVARCHAR(128) NOT NULL PRIMARY KEY, 
    [ObjectIdentifier] NVARCHAR(MAX) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
    [DisplayName] NVARCHAR(100) NOT NULL, 
    [EmailAddress] NVARCHAR(255) NOT NULL,
)
