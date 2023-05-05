CREATE PROCEDURE [dbo].[spUser_Insert]
	@Id nvarchar(128),
	@ObjectIdentifier nvarchar(MAX),
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@DisplayName nvarchar(100),
	@EmailAddress nvarchar(255)
AS
begin
	set nocount on;

	insert into [dbo].[User] (Id, ObjectIdentifier, FirstName, LastName, DisplayName, EmailAddress)
	values (@Id, @ObjectIdentifier, @FirstName, @LastName, @DisplayName, @EmailAddress);
end