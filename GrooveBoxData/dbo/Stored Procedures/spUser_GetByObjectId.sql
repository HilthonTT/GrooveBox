CREATE PROCEDURE [dbo].[spUser_GetByObjectId]
	@ObjectId nvarchar(MAX)
AS
begin
	set nocount on;

	select [Id], [ObjectIdentifier], [FirstName], [LastName], [DisplayName], [EmailAddress]
	from [dbo].[User]

	where ObjectIdentifier = @ObjectId
end