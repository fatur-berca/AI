ALTER PROCEDURE [dbo].[SendEmailTransportExecutionSI]
    @mail VARCHAR(1000),
	@nama VARCHAR(500),
	@reg VARCHAR(50),
	@week CHAR(2),
	@tglawal CHAR(12),
	@tglakhir CHAR(12),
	@sender VARCHAR(500),
	@files VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE
		@desc VARCHAR(MAX)

	SELECT
		@desc = 'Shipping Instruction ' + @nama + ' (Dedicated)-' + @reg + ' Week ' + @week + '<br><br>Hi,<br><br>We would like to inform you that,<br>Shipping Instruction Week ' + @week + ' (' + @tglawal + ' - ' + @tglakhir + ') have been created and send by<br><b><font color=#000><u> ' + @sender + ' </u></font></b><br>Please give feedback using the same file as attached<br><br>Thank You'

    EXEC msdb.dbo.sp_send_dbmail
		@profile_name='TOM_Mail',
        @recipients = @mail,
        @body_format = 'HTML',
        @body = @desc,
        @subject = 'TOM Email Shipping Instruction',
		@file_attachments = @files
END
