CREATE PROCEDURE [dbo].[spSendEmail]
    @profile_name VARCHAR(100),
    @recipients VARCHAR(1000),
    @copy_recipients VARCHAR(1000),
    @body NVARCHAR(MAX),
    @body_format VARCHAR(100),
    @subject NVARCHAR(1000),
    @file_attachments VARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;

    EXEC msdb.dbo.sp_send_dbmail
        @profile_name = @profile_name,
        @recipients = @recipients,
        @copy_recipients = @copy_recipients,
        @body = @body,
        @body_format = @body_format,
        @subject = @subject,
        @file_attachments = @file_attachments
END
GO