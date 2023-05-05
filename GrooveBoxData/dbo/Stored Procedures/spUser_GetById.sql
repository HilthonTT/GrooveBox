CREATE PROCEDURE [dbo].[spUser_GetById]
	@Id nvarchar(128)
AS
begin
	set nocount on;

	select [Id], [ObjectIdentifier], [FirstName], [LastName], [DisplayName], [EmailAddress]
	from [dbo].[User]
	
	where Id = @Id;
end
