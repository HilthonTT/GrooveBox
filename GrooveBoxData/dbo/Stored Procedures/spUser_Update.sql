CREATE PROCEDURE [dbo].[spUser_Update]
	@ObjectId nvarchar(MAX),
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@DisplayName nvarchar(100),
	@EmailAddress nvarchar(255)
AS
begin
	set nocount on;

	update [dbo].[User] set
	FirstName = @FirstName,
	LastName = @LastName,
	DisplayName = @DisplayName,
	EmailAddress = @EmailAddress

	where ObjectIdentifier = @ObjectId;
end