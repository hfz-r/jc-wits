--Create Transactions table

CREATE TABLE Transactions (
	ID BIGINT PRIMARY KEY IDENTITY(1,1),
	ContainerID BIGINT NOT NULL,
	Quantity INT NOT NULL,
	TxnType NVARCHAR(10) NOT NULL, --INDEX
	IsOffline NVARCHAR(1) NOT NULL Default 'N',
	IsSync NVARCHAR(1) NOT NULL Default 'N',
	Enabled NVARCHAR(1) NOT NULL Default 'Y', --For Handheld device purposes
	CreatedOn DATETIME NOT NULL,
	CreatedBy NVARCHAR(20) NOT NULL,
	ModifiedOn DATETIME NOT NULL,
	ModifiedBy NVARCHAR(20) NOT NULL)