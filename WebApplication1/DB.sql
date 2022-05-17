CREATE DATABASE SistemaFinanceiro;
USE SistemaFinanceiro;

CREATE TABLE Pagamentos(
	Id int identity(1,1),
	Descricao varchar(50) NOT NULL,
	Valor decimal(10,2) NOT NULL,
	CodBarras varchar(48) NOT NULL,
	DataVencimento date NOT NULL,
	DataPagamento date,
	Pago bit NOT NULL,
	Ativo bit NOT NULL,
PRIMARY KEY 
(
	Id ASC
) 
)

select * from Pagamentos

INSERT INTO Pagamentos VALUES ('Energia', 209.45,'123456789112345678911234567891123456789112345678','2022-05-17', '2022-05-15', 1,1);
INSERT INTO Pagamentos VALUES ('�gua', 85.00 ,'123456789112345678911234567891123456789112345678','2022-05-17','',0,1);
INSERT INTO Pagamentos VALUES ('Internet', 119.20,'123456789112345678911234567891123456789112345678','2022-05-17','',0,1);
INSERT INTO Pagamentos VALUES ('Casa', 1109.20,'123456789112345678911234567891123456789112345678','2022-05-13','',0,1);

