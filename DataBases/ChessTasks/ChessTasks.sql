--Запросы

--------------------------------------------------------------------------
--1. Сколько фигур стоит на доске? Вывести количество.

SELECT COUNT(*) AS 'Всего фигур на доске'
FROM chess_board

--2. Вывести id фигур, чьи названия начинаются на букву k.

SELECT f_id AS 'id фигур с названием на К'
FROM chess_figures
WHERE f_type LIKE 'K%'

--3. Какие типы фигур бывают и по сколько штук? Вывести тип и количество.

SELECT f_type AS 'Тип' , COUNT(*) AS 'Количество'
FROM chess_figures
GROUP BY f_type

--4. Вывести id белых пешек , стоящих на доске?

SELECT chess_figures.f_id
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
WHERE f_type = 'Pawn' AND f_color = 'White'

--5. Какие фигуры стоят на главной диагонали? Вывести их тип и цвет.

SELECT chess_figures.f_type AS 'Тип', chess_figures.f_color AS 'Цвет'
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
WHERE ASCII(x) - 64 = y

--6. Найдите общее количество фигур, оставшихся у каждого игрока. Вывести цвет и
--количество.

SELECT chess_figures.f_color AS 'Цвет', COUNT(*) AS 'Количество на доске'
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
GROUP BY f_color

--7. Какие фигуры черных имеются на доске? Вывести тип.

SELECT chess_figures.f_type AS 'Тип'
FROM  chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id AND chess_figures.f_color = 'Black'

--8. Какие фигуры черных имеются на доске? Вывести тип и количество.

SELECT chess_figures.f_type AS 'Тип', COUNT(*) AS 'Количество'
FROM  chess_figures JOIN chess_board ON chess_figures.f_id = chess_board.f_id AND chess_figures.f_color = 'Black'
GROUP BY f_type

--9. Найдите типы фигур (любого цвета), которых осталось, по крайней мере, не
--меньше двух на доске.

SELECT chess_figures.f_type AS 'Тип'
FROM  chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
GROUP BY f_type 
HAVING COUNT(*) >= 2

--10. Вывести цвет фигур, которых на доске больше.

SELECT TOP 1 WITH TIES chess_figures.f_color AS 'Цвет'
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
GROUP BY f_color
ORDER BY COUNT(*) DESC

/*11. Найдите фигуры, которые стоят на возможном пути движения ладьи (rook) (Любой
ладьи любого цвета). (Ладья может двигаться по горизонтали или по вертикали
относительно своего положения на доске в любом направлении.).*/

GO
CREATE VIEW [rocks] AS
SELECT chess_board.*
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id AND chess_figures.f_type = 'Rook'
GO
CREATE VIEW [current_figures] AS
SELECT chess_board.*, chess_figures.f_type, chess_figures.f_color
FROM  chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
GO
SELECT DISTINCT [current_figures].f_id, [current_figures].f_type AS 'Тип', [current_figures].f_color as 'Цвет'
FROM [rocks] JOIN [current_figures]
ON ([rocks].x = [current_figures].x OR [rocks].y = [current_figures].y) AND [rocks].f_id != [current_figures].f_id
ORDER BY [current_figures].f_id
GO
DROP VIEW [rocks]
DROP VIEW [current_figures]
GO

--12. У каких игроков (цвета) еще остались ВСЕ пешки (pawn)?

SELECT chess_figures.f_color AS 'Цвет'
FROM chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id AND chess_figures.f_type = 'pawn'
GROUP BY f_color
HAVING COUNT(*) = 8

/*13. Пусть отношения board1 и board2 представляют собой два последовательных
состояние игры (Chessboard). Какие фигуры (f_id) изменили свою позицию (за один
ход это может быть передвигаемая фигура и возможно еще фигура, которая была
“съедена”)?
В отношение Chessboard2 надо скопировать все строки отношения Chessboard. Из
Chessboard2 надо одну фигуру удалить, а другую фигуру поставить на место
удаленной («съесть фигуру»).
Задание надо выполнить двумя способами:
a) через JOIN
b) через UNION/INTERSECT/EXCEPT*/
----------------------

SELECT * 
INTO chess_board_2
FROM chess_board

UPDATE chess_board_2
SET y = 3
WHERE f_id = 1
----------------------
--a) JOIN

SELECT chess_board.f_id AS 'Id фигуры, чья позиция изменилась'
FROM chess_board FULL OUTER JOIN chess_board_2
ON chess_board.f_id = chess_board_2.f_id
WHERE chess_board.x != chess_board_2.x OR chess_board.y != chess_board_2.y OR chess_board_2.x IS NULL

--b) UNION/INTERSECT/EXCEPT

SELECT *
FROM chess_board
EXCEPT
SELECT * FROM chess_board_2

---------------------
DROP TABLE chess_board_2

--14. Вывести id фигуры, если она стоит в «опасной близости» от черного короля?
--«опасной близостью» будем считать квадрат 5х5 с королем в центре.

GO
CREATE VIEW [current_figures] AS
SELECT chess_board.*, chess_figures.f_type, chess_figures.f_color
FROM  chess_figures JOIN chess_board 
ON chess_figures.f_id = chess_board.f_id
GO
SELECT A.f_id
FROM [current_figures] as A JOIN [current_figures] as B
ON B.f_type = 'King' AND B.f_color = 'Black' AND ABS(ASCII(A.x) - ASCII(B.x)) <= 2 AND ABS(A.y - B.y) <= 2
WHERE A.f_type != 'King'
GO
DROP VIEW [current_figures]

--15. Найти фигуру, ближе всех стоящую к белому королю (расстояние считаем по
--метрике L1 – разница координат по X + разница координат по Y.

GO
CREATE VIEW [current_figures] AS
SELECT chess_board.*, chess_figures.f_type, chess_figures.f_color
FROM chess_figures JOIN chess_board
ON chess_figures.f_id = chess_board.f_id
GO
SELECT TOP 1 A.*
FROM current_figures as A JOIN current_figures as B
ON B.f_type = 'King' AND B.f_color = 'White'
WHERE A.f_type != 'King'
ORDER BY (ABS(ASCII(A.x) - ASCII(B.x)) + ABS(A.y - B.y))
GO
DROP VIEW [current_figures]

--------------------------------------------------------------------------
-- Процедура

--Процедура – «сделать ход». Если мы встали на клетку, где стояла фигура другого
--цвета, то «съесть» ее, если своего, то такой ход делать нельзя.
GO
CREATE PROCEDURE Turn
	@f_id INT,
	@x CHAR,
	@y int
AS
BEGIN
	DECLARE @target_id INT
	DECLARE @target_color VARCHAR
	DECLARE @f_color VARCHAR
	DECLARE @request VARCHAR

	SET @f_color =
		(SELECT chess_figures.*, chess_board.x, chess_board.y
		 FROM  chess_figures JOIN chess_board 
		 ON chess_figures.f_id = chess_board.f_id
		 WHERE chess_figures.f_id = @f_id)

	IF (@f_color IS NULL)
		PRINT 'Invalid figure id'
		RETURN

	SET @target_id = 
		(SELECT chess_figures.f_id
		 FROM  chess_figures JOIN chess_board 
		 ON chess_figures.f_id = chess_board.f_id
		 WHERE chess_board.x = @x AND chess_board.y = @y)

	IF (@target_id IS NULL)
		UPDATE chess_board
		SET x = @x, y = @y
		WHERE f_id = @f_id

		RETURN

	SET @target_color =
		(SELECT chess_figures.f_color
		 FROM chess_figures
		 WHERE f_id = @target_id)
		
		IF (@target_color = @f_color)
			PRINT 'Do not eat friends'
			RETURN

			DELETE FROM chess_board
			WHERE x = @x AND y = @y

			UPDATE chess_board
			SET x = @x, y = @y
			WHERE f_id = @f_id
END

--------------------------------------------------------------------------
--Функция
--
/*Функция-таблица, имеет параметр ID фигуры. В качестве результата выдает
фигуры противника, которого может съесть заданная фигура. Т.к. фигур много, и
правила “съедания” для каждого типа фигур свои, можно ограничиться одним
типом фигур (например, слоны, ладьи и пр.) Про фигуры, которые можем съесть,
выводим следующую информацию: ID, тип, Х, У.*/

GO
CREATE FUNCTION FiguresToEatForRook (@f_id INT)
RETURNS @figures_to_eat_for_rook TABLE (
	f_id INT,
	f_type VARCHAR(20),
	x CHAR,
	y INT)
AS
BEGIN
	DECLARE @x CHAR
	DECLARE @y INT
	DECLARE @color VARCHAR(20)

	SET @x =
		(SELECT x
		 FROM chess_board
		 WHERE f_id = @f_id)

	IF (@x IS NULL)
		RETURN

	IF ((SELECT f_type 
		FROM chess_figures
		WHERE f_id = @f_id)
		!= 'Rook')
		RETURN

	SET @y =
		(SELECT y
		 FROM chess_board
		 WHERE f_id = @f_id)

	SET @color =
		(SELECT f_color
		 FROM chess_figures
		 WHERE f_id = @f_id)

	INSERT INTO @figures_to_eat_for_rook
	SELECT closestXdown_figures.f_id, closestXdown_figures.f_type, closestXdown_figures.x, closestXdown_figures.y
	FROM
		(
		SELECT TOP 1 chess_figures.*, chess_board.x, chess_board.y
		FROM chess_board JOIN chess_figures
		ON chess_board.f_id = chess_figures.f_id
		WHERE chess_board.x = @x AND chess_board.y < @y
		ORDER BY (ABS(ASCII(chess_board.x) - ASCII(@x)) + ABS(chess_board.y - @y))
		) AS closestXdown_figures
	WHERE closestXdown_figures.f_color != @color

	INSERT INTO @figures_to_eat_for_rook
	SELECT closestXup_figures.f_id, closestXup_figures.f_type, closestXup_figures.x, closestXup_figures.y
	FROM
		(
		SELECT TOP 1 chess_figures.*, chess_board.x, chess_board.y
		FROM chess_board JOIN chess_figures
		ON chess_board.f_id = chess_figures.f_id
		WHERE chess_board.x = @x AND chess_board.y > @y
		ORDER BY (ABS(ASCII(chess_board.x) - ASCII(@x)) + ABS(chess_board.y - @y))
		) AS closestXup_figures
	WHERE closestXup_figures.f_color != @color

	INSERT INTO @figures_to_eat_for_rook
	SELECT closestYleft_figures.f_id, closestYleft_figures.f_type, closestYleft_figures.x, closestYleft_figures.y
	FROM
		(
		SELECT TOP 1 chess_figures.*, chess_board.x, chess_board.y
		FROM chess_board JOIN chess_figures
		ON chess_board.f_id = chess_figures.f_id
		WHERE chess_board.y = @y AND ASCII(chess_board.x) < ASCII(@x)
		ORDER BY (ABS(ASCII(chess_board.x) - ASCII(@x)) + ABS(chess_board.y - @y))
		) AS closestYleft_figures
	WHERE closestYleft_figures.f_color != @color

	INSERT INTO @figures_to_eat_for_rook
	SELECT closestYrignt_figures.f_id, closestYrignt_figures.f_type, closestYrignt_figures.x, closestYrignt_figures.y
	FROM
		(
		SELECT TOP 1 chess_figures.*, chess_board.x, chess_board.y
		FROM chess_board JOIN chess_figures
		ON chess_board.f_id = chess_figures.f_id
		WHERE chess_board.y = @y AND ASCII(chess_board.x) > ASCII(@x)
		ORDER BY (ABS(ASCII(chess_board.x) - ASCII(@x)) + ABS(chess_board.y - @y))
		) AS closestYrignt_figures
	WHERE closestYrignt_figures.f_color != @color

	RETURN
END

--------------------------------------------------------------------------
--Триггер
--
--Создать файл для записи истории шахматной партии. Структура таблицы:
--1) идентификатор хода,
--2) время (TIMESTAMP),
--3) ID фигуры,
--4) новая X координата,
--5) новая Y координата.
--Для тех фигур, которые были съедены, новые координаты становятся незаданным
--значением.
--При каждом ходе записывать изменения в таблице истории.

-- 
CREATE TABLE turn_history
(
	turn_id INT IDENTITY(1,1) PRIMARY KEY,
    time_stamp TIMESTAMP NOT NULL,
	f_id INT NOT NULL,
	x CHAR,
	y INT
)

GO
CREATE TRIGGER chessBoard_UPDATE_DELETE
ON chess_board
AFTER UPDATE, DELETE
AS
BEGIN
	--DECLARE @numberOfMove int
	DECLARE @f_id INT
	DECLARE @eaten_id INT
	DECLARE @x CHAR
	DECLARE @y INT

	SET @f_id = (SELECT f_id FROM inserted)
	SET @eaten_id = (SELECT f_id FROM deleted)
	SET @x = (SELECT x FROM inserted)
	SET @y = (SELECT y FROM inserted)

	IF (@f_id IS NULL)
		INSERT INTO turn_history 
		VALUES(CURRENT_TIMESTAMP, @eaten_id, null, null)
	ELSE
		INSERT INTO turn_history 
		VALUES(CURRENT_TIMESTAMP, @f_id, @x, @y)
END
--------------------------------------------------------------------------
