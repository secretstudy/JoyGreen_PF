use JoyGreen_PF

create table TBL_JG_STORE(	
	[STORE_IDX] [bigint] IDENTITY(1,1) NOT NULL,
	[NAME] [nvarchar] (50)  NOT NULL, --좌석번호
	[ID]   [nvarchar] (50) PRIMARY KEY NOT NULL,--아이디
	[PW]   [nvarchar] (50) NOT NULL--비밀번호
	
)
select * from dbo.TBL_JG_STORE
--insert into TBL_JG_STORE (name,id,pw) values()
--drop table dbo.TBL_JG_STORE
--drop table dbo.TBL_JG_STORE