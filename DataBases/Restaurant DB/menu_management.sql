--������� ����� �������� ���� � ����?

SELECT count(*) FROM dishes WHERE dish_type = '������'

--�������� ��������� ������� �� 
--����� ������� (�������� ��������) � ������ (�������), � ������ ������� ������ ������.

SELECT ingredients.name AS �����, dish_ingr_id.name AS ����� FROM
(SELECT dishes.name, dishes_ingredients.ingredient_id FROM 
dishes JOIN dishes_ingredients 
ON dishes.id = dishes_ingredients.dish_id) AS dish_ingr_id
JOIN ingredients ON dish_ingr_id.ingredient_id = ingredients.id
WHERE ingredients.ingredient_type = '�����'

--��������� (�������� � �����).

--���� ��� ����������� ���� � �������, �� ����������� ���������� ������, 
--������� ����� ����������. ���� ������-�� ������ ����������� ���, 
--�� �������� �������, ����� �� ���� ���������, � �������� ������ 
--� ������ ������ �������� (��� ����� ��� ��)
GO
CREATE PROCEDURE DishInfo
	@name VARCHAR(50)
AS
BEGIN
	DECLARE @res INT
	DECLARE @id UNIQUEIDENTIFIER
	DECLARE @dishes_info TABLE
	(
		dish_id UNIQUEIDENTIFIER,
		ingredient_id UNIQUEIDENTIFIER,
		ingredient_amount DECIMAL(18,2),
		instock DECIMAL(18,2)
	)
	DECLARE @needed_ingredients TABLE
	(
		needed_id UNIQUEIDENTIFIER,
		needed DECIMAL(18, 2),
		instock DECIMAL(18, 2)
	)
	DECLARE @not_enough_ingredients TABLE
	(
		not_enough_id UNIQUEIDENTIFIER,
		needed DECIMAL(18, 2)
	)
	-------------------------�������, ����� ����� ����������� � � ����� ���-��----------------------------------
	SET @id = (SELECT id FROM dishes WHERE dishes.name = @name)

	INSERT INTO @dishes_info
	SELECT dishes.id, dishes_ingredients.ingredient_id, ingredient_amount, ingredients.instock
	FROM dishes JOIN dishes_ingredients
	ON dishes.id = dishes_ingredients.dish_id
	JOIN
	ingredients ON ingredient_id = ingredients.id

	INSERT INTO @needed_ingredients
	SELECT ingredient_id, sum(ingredient_amount), instock
	FROM @dishes_info
	WHERE dish_id  = @id
	GROUP BY ingredient_id, instock

	INSERT INTO @not_enough_ingredients
	SELECT needed_id, needed
	FROM @needed_ingredients
	WHERE needed > instock

	-------------------------���� ����� �������, ������� ���������----------------------------------
	IF (NOT EXISTS(SELECT 1 FROM @not_enough_ingredients))
	BEGIN
		SET @res =
		(
			SELECT TOP 1 (instock / needed) 
			FROM @needed_ingredients
			ORDER BY (instock / needed)
		)
		PRINT FORMATMESSAGE('����� ���������� %d ��. ����� %s', @res, @name)
		RETURN
	END

	PRINT FORMATMESSAGE('��� %s �� ������� ������������', @name)

	-------------------------�������, ����� ����������� ����������� ����� ��������----------------------------------
	DECLARE @available_swaps TABLE
	(
		old_id UNIQUEIDENTIFIER,
		needed DECIMAL(18, 2),
		swap_id UNIQUEIDENTIFIER,
		swap_instock DECIMAL(18, 2)
	)

	INSERT INTO @available_swaps
	SELECT not_enough_id, needed, ingredients_swap.ingredient_id_a, ingredients.instock
	FROM @not_enough_ingredients JOIN ingredients_swap
	ON not_enough_id = ingredients_swap.ingredient_id_b
	JOIN ingredients
	ON ingredients.id = ingredients_swap.ingredient_id_a
	WHERE needed < ingredients.instock
	UNION
	SELECT not_enough_id, needed, ingredients_swap.ingredient_id_b, ingredients.instock
	FROM @not_enough_ingredients JOIN ingredients_swap
	ON not_enough_id = ingredients_swap.ingredient_id_a
	JOIN ingredients
	ON ingredients.id = ingredients_swap.ingredient_id_b
	WHERE needed < ingredients.instock

	-------------------------��������, ��� �� ��� ��������----------------------------------
	DECLARE @single_swaps TABLE
	(
		old_id UNIQUEIDENTIFIER,
		needed DECIMAL(18, 2),
		swap_id UNIQUEIDENTIFIER,
		swap_instock DECIMAL(18, 2)
	)

	INSERT INTO @single_swaps
	SELECT A.*
	FROM @available_swaps AS A
	INNER JOIN
		(SELECT old_id, MIN(swap_id) as swap_id
		FROM @available_swaps
		GROUP BY old_id) AS B
	ON A.swap_id = B.swap_id

	-------------------------���� ������ �������� ���-��, ������� �� ����----------------------------------
	IF (
		(SELECT COUNT(*) FROM @needed_ingredients)
		!=
		(SELECT COUNT(*) FROM @single_swaps)
	   )
	BEGIN
		PRINT FORMATMESSAGE('������ �������� ����������� ����������� ��� %s', @name)
		RETURN
	END

	-------------------------��������, ��� �� ��� ����������----------------------------------
	DECLARE @old_id UNIQUEIDENTIFIER,
	@swap_id UNIQUEIDENTIFIER,
	@old_name VARCHAR(50),
	@swap_name VARCHAR(50)
	DECLARE @tmp TABLE
	(
		old_id UNIQUEIDENTIFIER,
		swap_id UNIQUEIDENTIFIER
	)
	INSERT INTO @tmp
	SELECT old_id, swap_id
	FROM @single_swaps

	SELECT TOP 1 @old_id = old_id, @swap_id = swap_id FROM @tmp

	WHILE (@@ROWCOUNT > 0)
	BEGIN
		SET @old_name = (SELECT name FROM ingredients WHERE id = @old_id)
		SET @swap_name = (SELECT name FROM ingredients WHERE id = @swap_id)
		PRINT FORMATMESSAGE('���������� �������� %s �� %s', @old_name, @swap_name)
		DELETE FROM @tmp WHERE old_id = @old_id
		SELECT TOP 1 @old_id = old_id, @swap_id = swap_id FROM @tmp
	END
	-------------------------�������, ������� ����� �����������----------------------------------
	DECLARE @swap_res INT, @min INT

	SET @swap_res =
		(
			SELECT TOP 1 (swap_instock / needed) 
			FROM @single_swaps
			ORDER BY (swap_instock / needed)
		)

	SET @res = 
		(
			SELECT TOP 1 (instock / needed) 
			FROM @needed_ingredients
			WHERE needed < instock
			ORDER BY (instock / needed)
		)

	IF (@res IS NULL)
		SET @min = @swap_res
	ELSE
		SET @min = (SELECT MIN(x) FROM (VALUES (@swap_res), (@res)) AS VALUE(x))

	PRINT FORMATMESSAGE('����� ���������� %d ��. ����� %s', @min, @name)
END
GO
--������ ������ (��� ������������ ����������� � ����������� ������������� ������ �� ������� ��������� � ����������)
EXEC DishInfo '�������� �������� � ���������'
-- drop procedure DishInfo