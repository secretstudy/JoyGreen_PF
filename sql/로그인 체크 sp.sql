USE JoyGreen_PF
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[USP_PC_Check_LogIn]
	@p_ID				NVARCHAR(1000)
,	@p_Pw				NVARCHAR(1000)
,	@p_ComNo			NVARCHAR(1000)
AS      

SET NOCOUNT ON      
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED     

BEGIN 

	DECLARE @loginSuccessYN						NCHAR(1)		=	'N' --�α��� ���� ����
		,	@duplicateComputerNumberYN			NCHAR(1)		=	'N' --�¼���ȣ �ߺ�
		,	@STORE_IDX							NVARCHAR(100)	=	'0' --�����ȣ
		,	@IDExistsYN							NCHAR(1)		=	'N' --���̵� ���� ����

	if exists(	select * 
				  from dbo.TBL_JG_STORE
				 where id = @p_ID
				   and pw = @p_Pw
			)
	begin
		set @loginSuccessYN = 'Y'--�α��� ����ó��
		
		select	@STORE_IDX	=	STORE_IDX 
		  from	dbo.TBL_JG_STORE
		 where	id			=	@p_ID
		   and	pw			=	@p_Pw
				
		begin
			select	'Y'				as loginSuccessYN
				,	'N'				as duplicateComputerNumberYN
				,	@STORE_IDX		as STORE_IDX -- �����ȣ
				,	@IDExistsYN		as IDExistsYN -- ���̵� ���� ����
			return;
		end

		set @IDExistsYN = 'Y'--���̵� ���� Ȯ��

		end

				
		else begin
		
		set @loginSuccessYN				= 'N'		
		set @duplicateComputerNumberYN  = 'N'		
		
	end

	select	@loginSuccessYN					AS loginSuccessYN
		,	@duplicateComputerNumberYN		AS duplicateComputerNumberYN
		,	@STORE_IDX						AS STORE_IDX
		,	@IDExistsYN						AS IDExistsYN

end 

































GO


