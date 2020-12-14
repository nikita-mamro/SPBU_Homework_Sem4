--��������� - ������� � ������������� (����� ��������� ����� ���������� �� ������������� �������)
--� ��� ������ ����� �� ����������� ���� ��� 50 �������. 
--���������� �� ����� (���� ��� ����) ��� ���� ����������� ������, ��� ���������� ������� ���� 
--��� ����������� � ����� ������� � ����� �������.
GO
CREATE VIEW dishes_info AS
(
	SELECT dishes.id as dish_id, dishes.name AS dish_name, dishes.dish_type, dishes_ingredients.ingredient_id, ingredient_amount, ingredients.instock
	FROM dishes JOIN dishes_ingredients
	ON dishes.id = dishes_ingredients.dish_id
	JOIN
	ingredients ON ingredient_id = ingredients.id
)
GO
CREATE VIEW dishes_prices AS
(
	SELECT dish_id, dish_name, dish_type, sum(price) AS price FROM
	(
	SELECT dishes.id as dish_id, dishes.name AS dish_name, dishes.dish_type, (ingredient_amount * price) * (100 + profit_percents) / 100 AS price
	FROM dishes JOIN dishes_ingredients
	ON dishes.id = dishes_ingredients.dish_id
	JOIN
	ingredients ON ingredient_id = ingredients.id
	) AS tmp
	GROUP BY dish_id, dish_name, dish_type
)
GO
--SELECT dish_name, price FROM dishes_prices
GO
CREATE FUNCTION CheckLunch(
	@p_id UNIQUEIDENTIFIER,
	@v_id UNIQUEIDENTIFIER,
	@z_id UNIQUEIDENTIFIER,
	@n_id UNIQUEIDENTIFIER,
	@d_id UNIQUEIDENTIFIER
)
RETURNS BIT 
BEGIN
	DECLARE @total_ingredients INT
	DECLARE @enough_ingredients INT
	DECLARE @needed_ingredients TABLE
	(
		id UNIQUEIDENTIFIER,
		needed DECIMAL(18, 2),
		instock DECIMAL(18, 2)
	)

	INSERT INTO @needed_ingredients
	SELECT ingredient_id, sum(ingredient_amount), instock
	FROM dishes_info
	WHERE dish_id IN (@p_id, @v_id, @z_id, @n_id, @d_id) 
	GROUP BY ingredient_id, instock

	SET @total_ingredients = (SELECT count(*) 
	FROM @needed_ingredients)

	SET @enough_ingredients = (SELECT count(*) 
	FROM @needed_ingredients
	WHERE needed * 50 < instock) 
	-- �������� ��� ������������ 50 �� ���-�� ��������, ����� ������� ������������ ��� ����, ����� �������� ����� =)

	IF (@total_ingredients = @enough_ingredients)
		RETURN CAST(1 AS BIT)

	RETURN CAST(0 AS BIT)
END
GO
CREATE FUNCTION GetLunch()
RETURNS @lunch TABLE (
	������ NVARCHAR(50),
	������ NVARCHAR(50),
	������� NVARCHAR(50),
	������� NVARCHAR(50),
	������ NVARCHAR(50),
	���� DECIMAL(18, 2)
	)
AS
BEGIN
	DECLARE @combinations TABLE
	(
		p_name VARCHAR(50),
		v_name VARCHAR(50),
		z_name VARCHAR(50),
		n_name VARCHAR(50),
		d_name VARCHAR(50),
		total_price DECIMAL(18, 2)
	)

	INSERT INTO @combinations
	SELECT P.dish_name, V.dish_name, Z.dish_name, N.dish_name, D.dish_name, (P.price + V.price + Z.price + N.price + D.price) FROM 
	dishes_prices AS P CROSS JOIN dishes_prices AS V CROSS JOIN dishes_prices AS Z CROSS JOIN dishes_prices AS N CROSS JOIN dishes_prices AS D
	WHERE
	P.dish_type = '������' AND V.dish_type = '������' AND Z.dish_type = '�������' AND N.dish_type = '�������' AND D.dish_type = '������'
	AND dbo.CheckLunch(P.dish_id, V.dish_id, Z.dish_id, N.dish_id, D.dish_id) = 1

	INSERT INTO @lunch
	SELECT TOP 1 *
	FROM @combinations
	ORDER BY total_price DESC

	INSERT INTO @lunch
	SELECT TOP 1 *
	FROM @combinations
	ORDER BY total_price ASC

	RETURN
END
GO
--������
SELECT * FROM GetLunch()
