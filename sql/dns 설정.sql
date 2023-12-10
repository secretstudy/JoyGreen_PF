USE JoyGreen_PF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/**************************************

	115.89.203.106 / 115.89.203.54
	115.89.203.30  / 115.89.203.47
	115.89.203.133 / 115.89.203.143
	
	중 랜덤으로 dns 값을 받아 온다
	
**************************************/
CREATE PROCEDURE [dbo].[USP_SET_DNS_IP]
		@p_STORE_IDX		BIGINT		=	0
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
BEGIN
	declare @DnsIp				nvarchar(100)	=	''
		,	@dnsIP2				nvarchar(100)	=	''
		
	
	SELECT	@p_STORE_IDX		=	STORE_IDX
		
	  FROM	DBO.TBL_JG_STORE
	 WHERE	STORE_IDX			=	@p_STORE_IDX
	   

	begin
		DECLARE @RandomIndex INT;
        SET @RandomIndex = CAST((RAND() * 6) AS INT);

	    SET @DnsIp = CASE @RandomIndex
            WHEN 0 THEN '115.89.203.106'
            WHEN 1 THEN '115.89.203.54'
            WHEN 2 THEN '115.89.203.30'
            WHEN 3 THEN '115.89.203.47'
			WHEN 4 THEN	'115.89.203.133'
			WHEN 5 THEN	'115.89.203.143'
        END;
	  
		set @dnsIP2 = '203.248.252.2'
	end
	
	select	@DnsIp	as	DnsIp, @dnsIP2 as DnsIp2
END

GO


