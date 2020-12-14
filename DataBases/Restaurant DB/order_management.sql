--�������� ��������� ������, �������� ����������� ����� � �� ���������� � ����� ���������� ������.
--
--���� ����� ���������� ������� (��� +- 10 �����), �� ��������, ��� ����� ����������� �����. 
--��� ���� ���� ��������� ���������� ������������, ����������� ��� ���������� ����������� ������. 
--���� ����� ���������� ������ � � �������, �� ���� ������������� ��������, ����������� ��� ���������� ������, 
--� ����������� �����, ����� ����� ������ ���� �����. 
--���������� � �������� (id) ���� ����� ����� �� ��������� ������� ��� ��������� ����������. 
--���� ��� ���������� ������ �� ������� ���������, �� ���� ����������, 
--�� ������ �� ���� ���������� ������-�� ������, ������� ��� 
--� �� ��� �������� (������ �� ������) � ����� ���� ������� ����� �� ���� ��������� � �������� �����.

GO
CREATE PROCEDURE PlaceOrder
	@dish_name VARCHAR(50),
	@dish_count INT,
	@time DATETIME
AS
BEGIN
	IF (DATEDIFF(MI, CAST(CURRENT_TIMESTAMP AS DATETIME), @time) < 0)
		BEGIN
			PRINT '������� ���������� �����'
			RETURN
		END
	
	DECLARE @dish_id UNIQUEIDENTIFIER

	-- �������� id �����
	SET @dish_id = (SELECT id FROM dishes WHERE name = @dish_name)

	-- ������� ���������� � �������
	-- ������ �� INSERT �������� ������ ������� �� ������� ������������, ������� ��
	-- ������ �� ��������� is_canceled ��������� ����������������� ��������
	-- ���� �� ������� ������������, ������� �� ���� ��������� ��������
	-- � ������� ����������
	-- ������ �������� ���������� � ��������� ������������ �� ������ ���������
	-- ���� ����� ����������  +- 10 �����, ����� �������� is_completed = 1

	BEGIN TRY
		DECLARE @order_id UNIQUEIDENTIFIER

		SET @order_id = NEWID()

		INSERT INTO orders
		VALUES
		(@order_id, @dish_id, @dish_count, 0, 0, @time)

		IF (ABS(DATEDIFF(MI, CAST(CURRENT_TIMESTAMP AS DATETIME), @time)) <= 10)
		BEGIN
			UPDATE orders
			SET is_completed = 1
			WHERE id = @order_id
		END
	END TRY
	BEGIN CATCH
		PRINT '�� ������� ������������ ��� �����'
	END CATCH

	RETURN
END
GO
--exec PlaceOrder '�������� �������� � ���������', 51, '2020-09-06 21:00'
--drop procedure PlaceOrder


--�������� ��������� ���������� ������������������ ������

GO
CREATE PROCEDURE ProceedOrder
	@id UNIQUEIDENTIFIER
AS
BEGIN
	-- ������ ����, ��� ���������� � ����������������� ������������
	-- ��������� ����� ����, ��� ����� ����� ��������, ��� ���������� ������ ������� ��� �����������
	UPDATE orders
	SET is_completed = 1
	WHERE id = @id
	RETURN
END

--��������������� ���� ��� ������� �������

--������� ��� ������������� ����������������� ��������� � ������, ����� ����� ���������� ���������
GO 
CREATE TRIGGER orders_on_cancel
ON orders
AFTER UPDATE
AS 
BEGIN
	IF ((SELECT is_canceled FROM inserted) = 1)
	BEGIN
		DECLARE @reserved_dish_id UNIQUEIDENTIFIER
		DECLARE @reserved_dish_count INT

		SET @reserved_dish_id = (SELECT dish_id FROM inserted)
		SET @reserved_dish_count = (SELECT dish_count FROM inserted)

		--������������� ����������������� ���������
		DECLARE @reserved_ingredients TABLE
		(
			ingredient_id UNIQUEIDENTIFIER,
			reserved_amount DECIMAL(18, 2)
		)

		INSERT INTO @reserved_ingredients
		SELECT dishes_ingredients.ingredient_id, dishes_ingredients.ingredient_amount
		FROM dishes_ingredients
		WHERE dish_id = @reserved_dish_id

		--���� ���������� ���-�� ������������
		UPDATE ingredients
		SET instock = 
		instock + 
		((SELECT reserved_amount FROM @reserved_ingredients WHERE ingredient_id = ingredients.id) * @reserved_dish_count)
		WHERE id IN (SELECT ingredient_id FROM @reserved_ingredients)

		RETURN	
	END
END
-- drop trigger orders_on_cancel 

-- �������
--
-- ������� ����������� ������� ���� ������� � ������� ������,  ��� ���������� ����� � ��� ������� ������ ������ ���� �������.
-- (������ ������ ������� ������ ����������� ���������������� ��� ��������� ����������� �������)
GO 
CREATE TRIGGER orders_on_insert
ON orders
AFTER INSERT
AS
BEGIN
	-- �� ������ ���� ���������� ������������ ������, � ����� ����� ��� �������� �� ����� PlaceOrder
	-- ����������� orders_on_cancel, ��� ����������������� ����������� ��� ������������ �������
	-- �������������
	UPDATE orders
	SET is_canceled = 1
	WHERE DATEDIFF(SS, CAST(CURRENT_TIMESTAMP AS DATETIME), execution_time) < 0

	-- �������� ����� � ����������
	
	DECLARE @reserved_dish_id UNIQUEIDENTIFIER
	DECLARE @reserved_dish_count INT

	SET @reserved_dish_id = (SELECT dish_id FROM inserted)
	SET @reserved_dish_count = (SELECT dish_count FROM inserted)

	-- �������, ����� ����������� ����� � � ����� ����������
	DECLARE @reserved_ingredients TABLE
	(
		ingredient_id UNIQUEIDENTIFIER,
		reserved_amount DECIMAL(18, 2)
	)

	INSERT INTO @reserved_ingredients
	SELECT dishes_ingredients.ingredient_id, (dishes_ingredients.ingredient_amount * @reserved_dish_count)
	FROM dishes_ingredients
	WHERE dish_id = @reserved_dish_id

	DECLARE @not_enough_count INT

	SET @not_enough_count = 
		(
		SELECT COUNT(*) FROM
		@reserved_ingredients JOIN ingredients
		ON ingredient_id = ingredients.id
		WHERE reserved_amount > ingredients.instock
		)

	IF (@not_enough_count != 0)
	BEGIN
		RAISERROR('�� ������� ������������', 16, 1)
		ROLLBACK TRANSACTION
	END

	-- ���� ������� ������������, ������ ������ � ��������� ������������
	UPDATE ingredients
	SET instock = 
	(instock - 
	CAST((SELECT reserved_amount FROM @reserved_ingredients WHERE ingredient_id = ingredients.id) AS decimal(18,2)))
	WHERE id IN (SELECT ingredient_id FROM @reserved_ingredients)

	-- ������ ���������� � �������, ������� ������ ������
	DELETE FROM orders
	WHERE id IN
	(
		SELECT id
		FROM orders
		WHERE DATEDIFF(MM, CAST(CURRENT_TIMESTAMP AS DATETIME), execution_time) >= 1
		AND (is_completed = 1 OR is_canceled = 1)
	)
END
GO
--DROP TRIGGER orders_on_insert