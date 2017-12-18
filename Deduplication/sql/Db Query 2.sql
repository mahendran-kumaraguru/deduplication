select * from tblUserDetails
select * from tblHashTable
select * from tblFileDetails
select * from tblVisitorMessages


drop table tblUserDetails
drop table tblHashTable
drop table tblFileDetails
drop table tblVisitorMessages

create database DedupDB1 
on 
(
 Name='DedupDB1',
 Filename='W:\College\project\Deduplication - 08 04 16\Deduplication\App_Data\DedupDB1.mdf'
 )
 go


create proc spRegistration
@Name nvarchar(100),
@Username nvarchar(100),
@Password nvarchar(100),
@Mail nvarchar(100),
@Mobile nvarchar(10),
@UserKey nvarchar(100)
as
begin
	if(exists(select UserID from tblUserDetails
				where Username=@Username))
	begin
		select 0 as IsRegistered
	end
	else
	begin
		insert into tblUserDetails (Name, Username, Password, Mail, Mobile, UserKey)
		values (@Name, @Username, @Password, @Mail, @Mobile, @UserKey)
		select 1 as IsRegistered
	end
end


create proc spUpdateEncHash
@Username varchar(100),
@FileToken varchar(200)
as
begin
	declare @FileID int
	declare @UserId int
	select @UserID=UserID from tblUserDetails where Username=@Username
	insert into tblHashTable (UserID, FileToken) values (@UserID, @FileToken)
end


create proc spGetFiles
@Username varchar(100)
as
begin
	declare @UserID int
	declare @FileID int
	select @UserID=UserID from tblUserDetails where Username=@Username
	select * from tblHashTable where UserID=@UserID
end




create table tblUserDetails
(
	UserID int identity primary key,
	Name nvarchar(100) not null,
	Username nvarchar(100) not null unique,
	Password nvarchar(100) not null,
	Mail nvarchar(100) not null,
	Mobile nvarchar(10) not null,
	UserKey nvarchar(200) not null
)

create table tblFileDetails
(
	FileID int identity primary key,
	FileName varchar(100),
	FileType varchar(100),
	FileLength varchar(100),
	FileUsers int,
	FileKey varchar(100),
	EncFileKey varchar(100)
)


create table tblHashTable
(
	UserID int foreign key references tblUserDetails(UserID),
	FileToken varchar(500),
)

create table tblVisitorMessages
(
	VisitorID int identity,
	VisitorName varchar(100),
	VisitorMail varchar(100),
	VisitorMobile varchar(10),
	VisitorMessage varchar(max),
	VisitorTime datetime
)