--Create Supplier table

CREATE TABLE Supplier(
	ID BIGINT PRIMARY KEY,
	SupplierName NVARCHAR(500) NOT NULL,
	ContactPerson NVARCHAR(200) NULL,
	ContactTelNo NVARCHAR(100) NULL,	
	ContactFaxNo NVARCHAR(100) NULL,	
	Email NVARCHAR(100) NULL,	
	Address NVARCHAR(500) NULL,
	Enabled NVARCHAR(1) NOT NULL Default 'Y',
	CreatedOn DATETIME NOT NULL,
	CreatedBy NVARCHAR(20) NOT NULL,
	ModifiedOn DATETIME NOT NULL,
	ModifiedBy NVARCHAR(20) NOT NULL)