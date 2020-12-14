--Сделайте процедуру заказа, указывая параметрами блюда и их количество и время выполнения заказа.
--
--Если время выполнения текущее (или +- 10 минут), то считайте, что заказ выполняется сразу. 
--При этом надо уменьшать количество ингредиентов, необходимое для выполнения конкретного заказа. 
--Если время выполнения заказа – в будущем, то надо резервировать продукты, необходимые для выполнения заказа, 
--и фиксировать время, когда заказ должен быть готов. 
--Количество и название (id) блюд можно брать из временной таблицы или табличной переменной. 
--Если для выполнения заказа не хватает продуктов, то надо посмотреть, 
--не прошел ли срок выполнения какого-то заказа, который так 
--и не был выполнен (клиент не пришел) – тогда надо снимать бронь со всех продуктов и отменять заказ.

GO
CREATE PROCEDURE PlaceOrder
	@dish_name VARCHAR(50),
	@dish_count INT,
	@time DATETIME
AS
BEGIN
	IF (DATEDIFF(MI, CAST(CURRENT_TIMESTAMP AS DATETIME), @time) < 0)
		BEGIN
			PRINT 'Укажите корректное время'
			RETURN
		END
	
	DECLARE @dish_id UNIQUEIDENTIFIER

	-- Получить id блюда
	SET @dish_id = (SELECT id FROM dishes WHERE name = @dish_name)

	-- Попытка добавления в очередь
	-- Тригер на INSERT проверит список заказов на наличие просроченных, отменит их
	-- Тригер на изменение is_canceled освободит зарезервированные продукты
	-- Если не хватает ингредиентов, триггер не даст завершить операцию
	-- и выкинет исключение
	-- Засчёт триггера информация о доступных ингредиентах на складе изменится
	-- Если время выполнения  +- 10 минут, сразу поставим is_completed = 1

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
		PRINT 'Не хватает ингредиентов для блюда'
	END CATCH

	RETURN
END
GO
--exec PlaceOrder 'Штрудель яблочный с мороженым', 51, '2020-09-06 21:00'
--drop procedure PlaceOrder


--Написать процедуру выполнения зарезервированного заказа

GO
CREATE PROCEDURE ProceedOrder
	@id UNIQUEIDENTIFIER
AS
BEGIN
	-- Засчёт того, что информация о зарезервированных ингредиентах
	-- изменится после того, как заказ будет добавлен, при выполнении просто пометим его выполненным
	UPDATE orders
	SET is_completed = 1
	WHERE id = @id
	RETURN
END

--Вспомогательные вещи для полноты картины

--Триггер для высвобождения зарезервированных продуктов в момент, когда заказ помечается отменённым
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

		--Высвобождение зарезервированных продуктов
		DECLARE @reserved_ingredients TABLE
		(
			ingredient_id UNIQUEIDENTIFIER,
			reserved_amount DECIMAL(18, 2)
		)

		INSERT INTO @reserved_ingredients
		SELECT dishes_ingredients.ingredient_id, dishes_ingredients.ingredient_amount
		FROM dishes_ingredients
		WHERE dish_id = @reserved_dish_id

		--Само обновление кол-ва ингредиентов
		UPDATE ingredients
		SET instock = 
		instock + 
		((SELECT reserved_amount FROM @reserved_ingredients WHERE ingredient_id = ingredients.id) * @reserved_dish_count)
		WHERE id IN (SELECT ingredient_id FROM @reserved_ingredients)

		RETURN	
	END
END
-- drop trigger orders_on_cancel 

-- Триггер
--
-- Таблицу выполненных заказов надо хранить в течение месяца,  при добавлении строк в эту таблицу старые заказы надо удалять.
-- (Помимо чистки добавил другую необходимую функциональность для обработки поступивших заказов)
GO 
CREATE TRIGGER orders_on_insert
ON orders
AFTER INSERT
AS
BEGIN
	-- На случай если накопились просроченные заказы, а новый заказ был добавлен не через PlaceOrder
	-- Срабатывает orders_on_cancel, все зарезервированные ингредиенты для просроченных заказов
	-- освобождаются
	UPDATE orders
	SET is_canceled = 1
	WHERE DATEDIFF(SS, CAST(CURRENT_TIMESTAMP AS DATETIME), execution_time) < 0

	-- Получаем блюдо и количество
	
	DECLARE @reserved_dish_id UNIQUEIDENTIFIER
	DECLARE @reserved_dish_count INT

	SET @reserved_dish_id = (SELECT dish_id FROM inserted)
	SET @reserved_dish_count = (SELECT dish_count FROM inserted)

	-- Смотрим, какие ингредиенты нужны и в каком количестве
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
		RAISERROR('Не хватает ингредиентов', 16, 1)
		ROLLBACK TRANSACTION
	END

	-- Если хватает ингредиентов, меняем данные о свободных ингредиентах
	UPDATE ingredients
	SET instock = 
	(instock - 
	CAST((SELECT reserved_amount FROM @reserved_ingredients WHERE ingredient_id = ingredients.id) AS decimal(18,2)))
	WHERE id IN (SELECT ingredient_id FROM @reserved_ingredients)

	-- Чистим информацию о заказах, которым больше месяца
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